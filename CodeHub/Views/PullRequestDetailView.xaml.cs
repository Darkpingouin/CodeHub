﻿using CodeHub.Helpers;
using CodeHub.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using Octokit;
using System;
using System.Threading.Tasks;
using UICompositionAnimations;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml;
using UICompositionAnimations.Enums;

namespace CodeHub.Views
{

    public sealed partial class PullRequestDetailView : Windows.UI.Xaml.Controls.Page
    {
        public PullRequestDetailViewmodel ViewModel;
        public PullRequestDetailView()
        {
            this.InitializeComponent();
            ViewModel = new PullRequestDetailViewmodel();

            this.DataContext = ViewModel;

            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ViewModel.CommentText = string.Empty;

            if (e.NavigationMode != NavigationMode.Back)
            {
                if (ViewModel.Comments != null)
                {
                    ViewModel.Comments.Clear();
                }

                ConfigureStateSymbol((e.Parameter as Tuple<Repository, PullRequest>).Item2);
                await ViewModel.Load((e.Parameter as Tuple<Repository, PullRequest>));
                CommentsPivot.SelectedItem = CommentsPivot.Items[0];
            }
        }

        protected async override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (e.NavigationMode == NavigationMode.Back)
            {
                NavigationCacheMode = NavigationCacheMode.Disabled;
            }

            await ToggleCommentDialogVisibility(false);
        }

        public void ConfigureStateSymbol(PullRequest pr)
        {
            if (pr.State == ItemState.Open)
            {
                statePanel.Background = GlobalHelper.GetSolidColorBrush("2CBE4EFF");

                //open PR
                stateSymbol.Data = GlobalHelper.GetGeomtery("M11 11.28V5c-.03-.78-.34-1.47-.94-2.06C9.46 2.35 8.78 2.03 8 2H7V0L4 3l3 3V4h1c.27.02.48.11.69.31.21.2.3.42.31.69v6.28A1.993 1.993 0 0 0 10 15a1.993 1.993 0 0 0 1-3.72zm-1 2.92c-.66 0-1.2-.55-1.2-1.2 0-.65.55-1.2 1.2-1.2.65 0 1.2.55 1.2 1.2 0 .65-.55 1.2-1.2 1.2zM4 3c0-1.11-.89-2-2-2a1.993 1.993 0 0 0-1 3.72v6.56A1.993 1.993 0 0 0 2 15a1.993 1.993 0 0 0 1-3.72V4.72c.59-.34 1-.98 1-1.72zm-.8 10c0 .66-.55 1.2-1.2 1.2-.65 0-1.2-.55-1.2-1.2 0-.65.55-1.2 1.2-1.2.65 0 1.2.55 1.2 1.2zM2 4.2C1.34 4.2.8 3.65.8 3c0-.65.55-1.2 1.2-1.2.65 0 1.2.55 1.2 1.2 0 .65-.55 1.2-1.2 1.2z");
            }
            else
            {
                statePanel.Background = GlobalHelper.GetSolidColorBrush("CB2431FF");

                //closed PR
                stateSymbol.Data = GlobalHelper.GetGeomtery("M11 11.28V5c-.03-.78-.34-1.47-.94-2.06C9.46 2.35 8.78 2.03 8 2H7V0L4 3l3 3V4h1c.27.02.48.11.69.31.21.2.3.42.31.69v6.28A1.993 1.993 0 0 0 10 15a1.993 1.993 0 0 0 1-3.72zm-1 2.92c-.66 0-1.2-.55-1.2-1.2 0-.65.55-1.2 1.2-1.2.65 0 1.2.55 1.2 1.2 0 .65-.55 1.2-1.2 1.2zM4 3c0-1.11-.89-2-2-2a1.993 1.993 0 0 0-1 3.72v6.56A1.993 1.993 0 0 0 2 15a1.993 1.993 0 0 0 1-3.72V4.72c.59-.34 1-.98 1-1.72zm-.8 10c0 .66-.55 1.2-1.2 1.2-.65 0-1.2-.55-1.2-1.2 0-.65.55-1.2 1.2-1.2.65 0 1.2.55 1.2 1.2zM2 4.2C1.34 4.2.8 3.65.8 3c0-.65.55-1.2 1.2-1.2.65 0 1.2.55 1.2 1.2 0 .65-.55 1.2-1.2 1.2z");
            }
        }

        private async void CommentDialogOpen_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await ToggleCommentDialogVisibility(true);
        }

        private async void CancelComment_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await ToggleCommentDialogVisibility(false);
        }

        private async void Comment_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(ViewModel.CommentText))
            {
                await ToggleCommentDialogVisibility(false);
                ViewModel.CommentCommand.Execute(null);
            }
        }

        private async Task ToggleCommentDialogVisibility(bool visible)
        {
            if (visible)
            {
                CommentDialog.SetVisualOpacity(0);
                CommentDialog.Visibility = Visibility.Visible;
                await CommentDialog.StartCompositionFadeScaleAnimationAsync(0, 1, 1.1f, 1, 150, null, 0, EasingFunctionNames.SineEaseInOut);
            }
            else
            {
                await CommentDialog.StartCompositionFadeScaleAnimationAsync(1, 0, 1, 1.1f, 150, null, 0, EasingFunctionNames.SineEaseInOut);
                CommentDialog.Visibility = Visibility.Collapsed;
            }
        }
    }
}
