using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using MahApps.Metro.Controls.Dialogs;
using MovieFinder2025.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace MovieFinder2025.ViewModels
{
    public partial class TrailerViewModel : ObservableObject
    {

        /* ================ Constructor ================ */

        private readonly IDialogCoordinator dialogCoordinator;
        public TrailerViewModel(IDialogCoordinator coordinator, string movieTitle)
        {
            this.dialogCoordinator = coordinator;
            MovieTitle = movieTitle;

            YoutubeUri = "https://www.youtube.com";

            SearchYoutubeApi();
        }


        /* ================ MovieTitle ================ */

        private string _movieTitle;
        public string MovieTitle
        {
            get => _movieTitle;
            set => SetProperty(ref _movieTitle, value);
        }


        /* ================ YoutubeItems ================ */

        private ObservableCollection<YoutubeItem> _youtubeItems;
        public ObservableCollection<YoutubeItem> YoutubeItems
        {
            get => _youtubeItems;
            set => SetProperty(ref _youtubeItems, value);
        }


        /* ================ SelectedYoutube ================ */

        private YoutubeItem _selectedYoutube;
        public YoutubeItem SelectedYoutube
        {
            get => _selectedYoutube;
            set => SetProperty(ref _selectedYoutube, value);

        }


        /* ================ YoutubeUri ================ */

        private string _youtubeUri;
        public string YoutubeUri
        {
            get => _youtubeUri;
            set => SetProperty(ref _youtubeUri, value);
        }

        /* ================ SearchYoutubeApi ================ */

        /// <summary>
        /// 핵심 Youtube Data Api v3 호출
        /// </summary>
        private async void SearchYoutubeApi()
        {
            await LoadDataCollection();
        }


        /* ================ LoadDataCollection ================ */

        private async Task LoadDataCollection()
        {
            var service = new YouTubeService(
                new BaseClientService.Initializer()
                {
                    ApiKey = "AIzaSyDZ8XLZZkPRZ8lmJWMkB44l0-Y_1R0QCCE",
                    ApplicationName = this.GetType().ToString()
                });

            var req = service.Search.List("snippet");
            req.Q = $"{MovieTitle} 공식 예고편"; // 영화 이름만 사용해서 API를 검색
            req.Order = SearchResource.ListRequest.OrderEnum.Relevance;
            req.Type = "video";
            req.MaxResults = 10;

            ObservableCollection<YoutubeItem> youtubeItems = new ObservableCollection<YoutubeItem>();

            var res = await req.ExecuteAsync(); // Youtube API서버에 요청된 값을 실행하고 결과를 리턴(비동기)
            foreach(var item in res.Items)
            {
                youtubeItems.Add(new YoutubeItem
                {
                    Title = item.Snippet.Title,
                    ChannelTitle = item.Snippet.ChannelTitle,
                    URL = $"https://www.youtube.com/watch?v={item.Id.VideoId}",
                    Author = item.Snippet.ChannelId,
                    Thumbnail = new BitmapImage(new Uri(item.Snippet.Thumbnails.Default__.Url, UriKind.RelativeOrAbsolute))
                });
            }
            YoutubeItems = youtubeItems;
        }


        /* ================ YoutubeDoubleClick ================ */

        [RelayCommand]
        public async Task YoutubeDoubleClick()
        {
            YoutubeUri = SelectedYoutube.URL;
        }

    }
}
