﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Telegram.Td.Api;
using Unigram.Common;
using Unigram.Controls;
using Unigram.Services;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Unigram.Views.Popups
{
    public sealed partial class LoginUrlInfoPopup : TLContentDialog
    {
        public LoginUrlInfoPopup(ICacheService cacheService, LoginUrlInfoRequestConfirmation requestConfirmation)
        {
            InitializeComponent();

            Title = Strings.Resources.OpenUrlTitle;
            Message = string.Format(Strings.Resources.OpenUrlAlert2, requestConfirmation.Url);
            PrimaryButtonText = Strings.Resources.Open;
            SecondaryButtonText = Strings.Resources.Cancel;

            var self = cacheService.GetUser(cacheService.Options.MyId);
            if (self == null)
            {
                // ??
            }

            TextBlockHelper.SetMarkdown(CheckLabel1, string.Format(Strings.Resources.OpenUrlOption1, requestConfirmation.Domain, self.GetFullName()));

            if (requestConfirmation.RequestWriteAccess)
            {
                var bot = cacheService.GetUser(requestConfirmation.BotUserId);
                if (bot == null)
                {
                    // ??
                }

                CheckBox2.Visibility = Visibility.Visible;
                TextBlockHelper.SetMarkdown(CheckLabel2, string.Format(Strings.Resources.OpenUrlOption2, bot.GetFullName()));
            }
            else
            {
                CheckBox2.Visibility = Visibility.Collapsed;
            }
        }

        public string Message
        {
            get
            {
                return TextBlockHelper.GetMarkdown(MessageLabel);
            }
            set
            {
                TextBlockHelper.SetMarkdown(MessageLabel, value);
            }
        }

        public FormattedText FormattedMessage
        {
            get
            {
                return TextBlockHelper.GetFormattedText(MessageLabel);
            }
            set
            {
                TextBlockHelper.SetFormattedText(MessageLabel, value);
            }
        }

        public bool HasAccepted
        {
            get
            {
                return CheckBox1.IsChecked == true;
            }
        }

        public bool HasWriteAccess
        {
            get
            {
                return CheckBox2.IsChecked == true;
            }
        }
    }
}
