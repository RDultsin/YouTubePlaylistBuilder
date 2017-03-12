using System;
using System.Threading.Tasks;
using YouTubePlaylistBuilder.Data;
using YouTubePlaylistBuilder.Services;

namespace YouTubePlaylistBuilder.App
{
    class Program
    {
        const string MuzTvRuDomain = "http://muz-tv.ru";

        const string RussianChartArchiveLink = "/charts/results/6/";
        const string RnbChartArchiveLink = "/charts/results/14/";

        [STAThread]
        static void Main()
        {
            // Russian chart
            Chart russianChart = new Program().ScrapeChartAndBuildYouTubePlaylist(RussianChartArchiveLink).Result;

            // RnB chart
            Chart rnbChart = new Program().ScrapeChartAndBuildYouTubePlaylist(RnbChartArchiveLink).Result;
         }

        private async Task<Chart> ScrapeChartAndBuildYouTubePlaylist(string chartArchiveLink)
        {
            ChartScraper chartScraper = new ChartScraper();

            // Get the latest chart
            Chart chart = chartScraper.GetLatestChart(MuzTvRuDomain, chartArchiveLink);

            YouTubeApiHelper youTubeApiHelp = new YouTubeApiHelper();

            // Build YouTube playlist
            chart = await youTubeApiHelp.BuildPlaylist(chart);

            return chart;
        }
    }
}
