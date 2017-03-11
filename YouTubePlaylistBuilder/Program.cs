using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using System.Configuration;

namespace YouTubePlaylistBuilder
{
    class Program
    {
        const string MuzTvRuDomain = "http://muz-tv.ru";

        const string RussianChartArchiveLink = "/charts/results/6/";
        const string RnbChartArchiveLink = "/charts/results/14/";

        private const string YouTubeApiKeyConfigKey = "YouTubeApiKey";

        [STAThread]
        static void Main()
        {
/*
            ChartHelper chartHelper = new ChartHelper();

            // Get songs from the latest Russian chart
            chartHelper.GetLatestChartSongs(MuzTvRuDomain, RussianChartArchiveLink);

            // Get songs from the latest RnB chart
            chartHelper.GetLatestChartSongs(MuzTvRuDomain, RnbChartArchiveLink);
*/
            Song song = new Song { Artist = "ЕГОР КРИД", Title = "Мало Так Мало" };
            try
            {
                Task<string> getYouTubeVideoIdTask = new Program().GetYouTubeVideoId(song);
                string videoId = getYouTubeVideoIdTask.Result;
            }
            catch (AggregateException ex)
            {
                foreach (var e in ex.InnerExceptions)
                {
                    Console.WriteLine("Error: " + e.Message);
                }
            }
        }

        private async Task<string> GetYouTubeVideoId(Song song)
        {
            string videoId = null;

            string youTubeApiKey = null;
            try
            {
                youTubeApiKey = ConfigurationManager.AppSettings[YouTubeApiKeyConfigKey];
            }
            catch (ConfigurationErrorsException e)
            {
                Console.WriteLine("Error: " + e.Message);
                return null;
            }

            // Create the service
            var youTubeService = new YouTubeService(new BaseClientService.Initializer
            {
                ApiKey = youTubeApiKey,
                ApplicationName = GetType().ToString(),
            });

            var searchListRequest = youTubeService.Search.List("snippet");
            searchListRequest.Q = string.Format("{0} {1}", song.Artist, song.Title);
            searchListRequest.MaxResults = 10;

            // Call the search.list method to retrieve results matching the specified query term
            var searchListResponse = await searchListRequest.ExecuteAsync();

            foreach (var searchResult in searchListResponse.Items)
            {
                if (searchResult.Id.Kind == "youtube#video")
                {
                    videoId = searchResult.Id.VideoId;
                    break;
                }
            }

            return videoId;
        }
    }
}
