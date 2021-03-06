﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Telegram.Td.Api;
using Unigram.Common;
using Unigram.Services.Updates;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace Unigram.Services
{
    public interface ICloudUpdateService
    {
        CloudUpdate NextUpdate { get; }

        Task UpdateAsync();
        Task<CloudUpdate> GetNextUpdateAsync();
    }

    public class CloudUpdateService : ICloudUpdateService, IHandle<UpdateFile>
    {
        private readonly FileContext<CloudUpdate> _mapping = new FileContext<CloudUpdate>();

        private readonly IProtoService _protoService;
        private readonly IEventAggregator _aggregator;

        private CloudUpdate _nextUpdate;

        private long _lastCheck;
        private bool _checking;

        public CloudUpdateService(IProtoService protoService, IEventAggregator aggregator)
        {
            _protoService = protoService;
            _aggregator = aggregator;

            _aggregator.Subscribe(this);
        }

        public CloudUpdate NextUpdate => _nextUpdate;

        public async void Update()
        {
            await UpdateAsync();
        }

        public async Task UpdateAsync()
        {
            if (Package.Current.SignatureKind == PackageSignatureKind.Store)
            {
                return;
            }

            var diff = Environment.TickCount - _lastCheck;
            if (diff < 5 * 60 * 1000 || _checking)
            {
                return;
            }

            _checking = true;
            _lastCheck = diff;

            var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("updates", CreationCollisionOption.OpenIfExists);
            var files = await folder.GetFilesAsync();

            var current = Package.Current.Id.Version.ToVersion();
            var sets = new List<Version>();

            foreach (var file in files)
            {
                var split = System.IO.Path.GetFileNameWithoutExtension(file.Name);
                if (Version.TryParse(split, out Version version))
                {
                    sets.Add(version);
                }
            }

            foreach (var version in sets)
            {
                // If this isn't the most recent version and it isn't in use, just delete it
                if (version <= current)
                {
                    var file = await folder.TryGetItemAsync($"{version}.appxbundle") as StorageFile;
                    if (file != null)
                    {
                        await file.DeleteAsync();
                    }
                }

                // We can safely ignore any other version as next steps will take care of them
            }

            var cloud = await GetNextUpdateAsync();
            if (cloud != null && cloud.Version > current)
            {
                _nextUpdate = cloud;

                // There's a new version for the current font
                if (cloud.Document.Local.IsDownloadingCompleted || cloud.File != null)
                {
                    _aggregator.Publish(new UpdateAppVersion(cloud));
                }
                else if (cloud.Document.Local.CanBeDownloaded && !cloud.Document.Local.IsDownloadingActive)
                {
                    _protoService.DownloadFile(cloud.Document.Id, 16);
                }
            }

            _checking = false;
        }

        public async void Handle(UpdateFile update)
        {
            var file = update.File;
            if (_mapping.TryGetValue(file.Id, out List<CloudUpdate> sets))
            {
                foreach (var set in sets)
                {
                    set.UpdateFile(file);
                }

                var cloud = sets.FirstOrDefault();
                if (cloud == null)
                {
                    return;
                }

                if (file.Local.IsDownloadingCompleted)
                {
                    var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("updates", CreationCollisionOption.OpenIfExists);
                    var result = await TryCopyPartLocally(folder, file.Local.Path, cloud.Version, cloud.MessageId);

                    var current = Package.Current.Id.Version.ToVersion();
                    if (current < cloud.Version)
                    {
                        _nextUpdate = cloud;
                        _nextUpdate.File = result;

                        _aggregator.Publish(new UpdateAppVersion(cloud));
                    }
                }
            }
        }

        public async Task<CloudUpdate> GetNextUpdateAsync()
        {
            if (Package.Current.SignatureKind == PackageSignatureKind.Store)
            {
                return null;
            }

            var chat = await _protoService.SendAsync(new SearchPublicChat("cGFnbGlhY2Npb19kaV9naGlhY2Npbw")) as Chat;
            if (chat == null)
            {
                return null;
            }

            await _protoService.SendAsync(new OpenChat(chat.Id));

            var response = await _protoService.SendAsync(new SearchChatMessages(chat.Id, "#update", 0, 0, 0, 100, new SearchMessagesFilterDocument())) as Messages;
            if (response == null)
            {
                _protoService.Send(new CloseChat(chat.Id));
                return null;
            }

            _protoService.Send(new CloseChat(chat.Id));

            var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("updates", CreationCollisionOption.OpenIfExists);

            var dict = new List<CloudUpdate>();
            var thumbnails = new Dictionary<string, File>();

            var results = new List<CloudUpdate>();

            foreach (var message in response.MessagesValue)
            {
                var document = message.Content as MessageDocument;
                if (document == null)
                {
                    continue;
                }

                var hashtags = new List<string>();
                var changelog = string.Empty;

                foreach (var entity in document.Caption.Entities)
                {
                    if (entity.Type is TextEntityTypeHashtag)
                    {
                        hashtags.Add(document.Caption.Text.Substring(entity.Offset, entity.Length));
                    }
                    else if (entity.Type is TextEntityTypeCode)
                    {
                        changelog = document.Caption.Text.Substring(entity.Offset, entity.Length);
                    }
                }

                if (!hashtags.Contains("#update") || !document.Document.FileName.Contains("x64") || !document.Document.FileName.EndsWith(".appxbundle"))
                {
                    continue;
                }

                var split = document.Document.FileName.Split('_');
                if (split.Length >= 3 && Version.TryParse(split[1], out Version version))
                {
                    var set = new CloudUpdate
                    {
                        MessageId = message.Id,
                        Changelog = changelog,
                        Version = version,
                        Document = document.Document.DocumentValue
                    };

                    _mapping[document.Document.DocumentValue.Id].Add(set);
                    dict.Add(set);
                }
            }

            var current = Package.Current.Id.Version.ToVersion();
            var latest = dict.Count > 0 ? dict.Max(x => x.Version) : null;

            foreach (var set in dict)
            {
                if (set.Version == latest && set.Version > current)
                {
                    if (set.Document.Local.IsDownloadingCompleted)
                    {
                        set.File = await TryCopyPartLocally(folder, set.Document.Local.Path, set.Version, set.MessageId);
                    }
                    else
                    {
                        set.File = await folder.TryGetItemAsync($"{set.Version}.appxbundle") as StorageFile;
                    }

                    results.Add(set);
                }
                else
                {
                    // Delete the file from chat cache as it isn't needed anymore
                    _protoService.Send(new DeleteFileW(set.Document.Id));
                }
            }

            return results.FirstOrDefault();
        }

        public static async Task<StorageFile> TryCopyPartLocally(StorageFolder folder, string path, Version version, long messageId)
        {
            var fileName = $"{version}.appxbundle";

            var cache = await folder.TryGetItemAsync(fileName) as StorageFile;
            if (cache == null)
            {
                try
                {
                    var result = await StorageFile.GetFileFromPathAsync(path);
                    return await result.CopyAsync(folder, fileName, NameCollisionOption.FailIfExists);
                }
                catch { }
            }

            return cache;
        }
    }

    public class CloudUpdate
    {
        public long MessageId { get; set; }

        public Version Version { get; set; }
        public string Changelog { get; set; }

        public File Document { get; set; }
        public StorageFile File { get; set; }

        public bool UpdateFile(File file)
        {
            if (Document.Id == file.Id)
            {
                Document = file;
                return true;
            }

            return false;
        }
    }
}
