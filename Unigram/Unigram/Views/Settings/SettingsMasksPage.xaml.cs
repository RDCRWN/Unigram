﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unigram.Views.Popups;
using Unigram.Views;
using Unigram.ViewModels.Settings;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Telegram.Td.Api;
using Unigram.Common;
using Windows.Storage;
using Unigram.Native;
using System.Windows.Input;
using Unigram.Converters;

namespace Unigram.Views.Settings
{
    public sealed partial class SettingsMasksPage : HostedPage
    {
        public SettingsMasksViewModel ViewModel => DataContext as SettingsMasksViewModel;

        public SettingsMasksPage()
        {
            InitializeComponent();
            DataContext = TLContainer.Current.Resolve<SettingsMasksViewModel>();
        }

        private void ArchivedStickers_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingsMasksArchivedPage));
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ViewModel.StickerSetOpenCommand.Execute(e.ClickedItem);
        }

        #region Recycle

        private void OnContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (args.InRecycleQueue)
            {
                return;
            }

            var content = args.ItemContainer.ContentTemplateRoot as Grid;
            var stickerSet = args.Item as StickerSetInfo;

            if (args.Phase == 0)
            {
                var title = content.Children[1] as TextBlock;
                title.Text = stickerSet.Title;
            }
            else if (args.Phase == 1)
            {
                var subtitle = content.Children[2] as TextBlock;
                subtitle.Text = Locale.Declension("Stickers", stickerSet.Size);
            }
            else if (args.Phase == 2)
            {
                var photo = content.Children[0] as Image;

                var cover = stickerSet.Thumbnail ?? stickerSet.Covers.FirstOrDefault()?.Thumbnail;
                if (cover == null)
                {
                    return;
                }

                var file = cover.Photo;
                if (file.Local.IsDownloadingCompleted)
                {
                    if (stickerSet.IsAnimated)
                    {
                        photo.Source = PlaceholderHelper.GetLottieFrame(file.Local.Path, 0, 48, 48);
                    }
                    else
                    {
                        photo.Source = PlaceholderHelper.GetWebPFrame(file.Local.Path);
                    }
                }
                else if (file.Local.CanBeDownloaded && !file.Local.IsDownloadingActive)
                {
                    photo.Source = null;
                    ViewModel.ProtoService.DownloadFile(file.Id, 1);
                }
            }

            if (args.Phase < 2)
            {
                args.RegisterUpdateCallback(OnContainerContentChanging);
            }

            args.Handled = true;
        }

        #endregion

        #region Context menu

        private void StickerSet_ContextRequested(UIElement sender, ContextRequestedEventArgs args)
        {
            var flyout = new MenuFlyout();

            var element = sender as FrameworkElement;
            var stickerSet = element.Tag as StickerSetInfo;

            if (stickerSet == null || stickerSet.Id == 0)
            {
                return;
            }

            if (stickerSet.IsOfficial)
            {
                flyout.CreateFlyoutItem(ViewModel.StickerSetHideCommand, stickerSet, Strings.Resources.StickersHide, new FontIcon { Glyph = Icons.Archive });
            }
            else
            {
                flyout.CreateFlyoutItem(ViewModel.StickerSetHideCommand, stickerSet, Strings.Resources.StickersHide, new FontIcon { Glyph = Icons.Archive });
                flyout.CreateFlyoutItem(ViewModel.StickerSetRemoveCommand, stickerSet, Strings.Resources.StickersRemove, new FontIcon { Glyph = Icons.Delete });
                //CreateFlyoutItem(ref flyout, ViewModel.StickerSetShareCommand, stickerSet, Strings.Resources.StickersShare);
                //CreateFlyoutItem(ref flyout, ViewModel.StickerSetCopyCommand, stickerSet, Strings.Resources.StickersCopy);
            }

            args.ShowAt(flyout, element);
        }

        #endregion

    }
}
