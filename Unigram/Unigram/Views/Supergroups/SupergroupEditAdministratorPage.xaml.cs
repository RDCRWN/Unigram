﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Telegram.Td.Api;
using Unigram.Common;
using Unigram.Views.Popups;
using Unigram.Converters;
using Unigram.ViewModels.Channels;
using Unigram.ViewModels.Delegates;
using Unigram.ViewModels.Supergroups;
using Unigram.ViewModels.Users;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Unigram.Views.Supergroups
{
    public sealed partial class SupergroupEditAdministratorPage : HostedPage, IMemberDelegate
    {
        public SupergroupEditAdministratorViewModel ViewModel => DataContext as SupergroupEditAdministratorViewModel;

        public SupergroupEditAdministratorPage()
        {
            InitializeComponent();
            DataContext = TLContainer.Current.Resolve<SupergroupEditAdministratorViewModel, IMemberDelegate>(this);
        }

        public void UpdateChat(Chat chat)
        {
        }

        public void UpdateChatTitle(Chat chat)
        {
        }

        public void UpdateChatPhoto(Chat chat)
        {
        }

        public void UpdateUser(Chat chat, User user, bool secret)
        {
            Title.Text = user.GetFullName();
            Subtitle.Text = LastSeenConverter.GetLabel(user, true);
            Photo.Source = PlaceholderHelper.GetUser(ViewModel.ProtoService, user, 64);

            Verified.Visibility = user.IsVerified ? Visibility.Visible : Visibility.Collapsed;
        }

        public void UpdateUserFullInfo(Chat chat, User user, UserFullInfo fullInfo, bool secret, bool accessToken)
        {
        }

        public void UpdateUserStatus(Chat chat, User user)
        {
            Subtitle.Text = LastSeenConverter.GetLabel(user, true);
        }

        public void UpdateMember(Chat chat, Supergroup group, User user, ChatMember member)
        {
            if (member.Status is ChatMemberStatusCreator || member.Status is ChatMemberStatusAdministrator)
            {
                var canBeEdited = (member.Status is ChatMemberStatusCreator && member.UserId == ViewModel.CacheService.Options.MyId)  || (member.Status is ChatMemberStatusAdministrator administrator && administrator.CanBeEdited);

                Header.CommandVisibility = canBeEdited ? Visibility.Visible : Visibility.Collapsed;
                DismissPanel.Visibility = canBeEdited ? Visibility.Visible : Visibility.Collapsed;
                PermissionsRoot.Footer = canBeEdited ? null : Strings.Resources.EditAdminCantEdit;
                PermissionsPanel.IsEnabled = canBeEdited;
                EditRankField.PlaceholderText = member.Status is ChatMemberStatusCreator ? Strings.Resources.ChannelCreator : Strings.Resources.ChannelAdmin;
                EditRankPanel.Footer = string.Format(Strings.Resources.EditAdminRankInfo, member.Status is ChatMemberStatusCreator ? Strings.Resources.ChannelCreator : Strings.Resources.ChannelAdmin);
            }
            else
            {
                Header.CommandVisibility = Visibility.Visible;
                DismissPanel.Visibility = Visibility.Collapsed;
                PermissionsRoot.Footer = null;
                PermissionsPanel.IsEnabled = true;
                EditRankField.PlaceholderText = Strings.Resources.ChannelAdmin;
                EditRankPanel.Footer = string.Format(Strings.Resources.EditAdminRankInfo, Strings.Resources.ChannelAdmin);
            }

            PermissionsPanel.Visibility = Visibility.Visible;

            ChangeInfo.Header = group.IsChannel ? Strings.Resources.EditAdminChangeChannelInfo : Strings.Resources.EditAdminChangeGroupInfo;
            PostMessages.Visibility = group.IsChannel ? Visibility.Visible : Visibility.Collapsed;
            EditMessages.Visibility = group.IsChannel ? Visibility.Visible : Visibility.Collapsed;
            DeleteMessages.Header = group.IsChannel ? Strings.Resources.EditAdminDeleteMessages : Strings.Resources.EditAdminGroupDeleteMessages;
            BanUsers.Visibility = group.IsChannel ? Visibility.Collapsed : Visibility.Visible;
            PinMessages.Visibility = group.IsChannel ? Visibility.Collapsed : Visibility.Visible;
            AddUsers.Header = chat.Permissions.CanInviteUsers ? Strings.Resources.EditAdminAddUsersViaLink : Strings.Resources.EditAdminAddUsers;

            //TransferOwnership.Content = group.IsChannel ? Strings.Resources.EditAdminChannelTransfer : Strings.Resources.EditAdminGroupTransfer;
        }
    }
}
