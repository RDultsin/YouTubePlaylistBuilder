using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YouTubePlaylistBuilder.Data;
using YouTubePlaylistBuilder.Services;

namespace YouTubePlaylistBuilder.App
{
    class Program
    {
        const string MuzTvRuDomain = "http://muz-tv.ru";

        private static readonly List<string> ChartArchiveLinks = new List<string>()
        {
            "/charts/results/5/", // МУЗ-ТВ Чарт
            "/charts/results/6/", // Русский Чарт
            "/charts/results/13/", // ClipYou Чарт
            "/charts/results/14/", // R'n'B Чарт
            "/charts/results/15/", // ТОП 30. Крутяк недели
            "/charts/results/16/" // Русский крутяк недели
        };

        [STAThread]
        static void Main()
        {
            Chart chart;
            foreach (string chartArchiveLink in ChartArchiveLinks)
            {
                chart = new Program().ScrapeChartAndBuildYouTubePlaylist(chartArchiveLink).Result;
            }
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
