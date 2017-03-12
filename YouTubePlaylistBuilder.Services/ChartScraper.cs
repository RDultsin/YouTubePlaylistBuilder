using HtmlAgilityPack;
using System.Collections.Generic;
using System.Diagnostics;
using YouTubePlaylistBuilder.Data;

namespace YouTubePlaylistBuilder.Services
{
    public class ChartScraper
    {
        private readonly HtmlWeb _htmlWeb = new HtmlWeb();

        private readonly char[] _separators = { '.' };

        /// <summary>
        /// Get relative URL to latest chart
        /// </summary>
        /// <param name="domain">Domain URL, e.g. http://muz-tv.ru</param>
        /// <param name="chartArchiveLink">Relative URL to chart archive, e.g. /charts/results/6/</param>
        /// <returns>Relative URL to latest chart, e.g. /charts/results/6/1057/</returns>
        private string GetChartLink(string domain, string chartArchiveLink)
        {
            string chartLink = null;

            int chartArchiveUrlLen = chartArchiveLink.Length;

            HtmlDocument doc = _htmlWeb.Load(domain + chartArchiveLink);
            if (doc == null)
                return null;

            foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//a[@href][@class='b-chartvoting_archive_subitem_url x-results-toggle']"))
            {
                string chartLinkCandidate = link.GetAttributeValue("href", null);
                if (string.IsNullOrEmpty(chartLinkCandidate) || chartLinkCandidate.Length <= chartArchiveUrlLen ||
                    chartLinkCandidate.Substring(0, chartArchiveUrlLen) != chartArchiveLink)
                    continue;

                chartLink = chartLinkCandidate;
                Debug.WriteLine("Chart link: {0}", (object)chartLink);
                break;
            }

            return chartLink;
        }

        /// <summary>
        /// Get songs from chart
        /// </summary>
        /// <param name="domain">Domain URL, e.g. http://muz-tv.ru</param>
        /// <param name="chartLink">Relateive URL to chart, e.g. /charts/results/6/1057</param>
        /// <returns>Chart populated with name, release from date and list of songs</returns>
        private Chart GetChartNameReleaseFromDateAndSongs(string domain, string chartLink)
        {
            Chart chart = new Chart { Link = chartLink, Songs = new List<Song>() };

            HtmlDocument doc = _htmlWeb.Load(domain + chartLink);
            if (doc == null)
                return chart;

            HtmlNode docTitle = doc.DocumentNode.SelectSingleNode("/html/head/title"); // Eg. Русский Чарт. Выпуск от 10.03.2017. Результаты.
            string[] chartNameAndReleaseFromDate = docTitle.InnerText.Split(_separators, System.StringSplitOptions.RemoveEmptyEntries);
            if (chartNameAndReleaseFromDate.Length >= 4)
            {
                chart.Name = chartNameAndReleaseFromDate[0].Trim().Replace("&#39;", "'");
                chart.ReleaseFromDate = $"{chartNameAndReleaseFromDate[1].Trim()}.{chartNameAndReleaseFromDate[2].Trim()}.{chartNameAndReleaseFromDate[3].Trim()}";
                Debug.WriteLine("Chart {0} ({1})", chart.Name, chart.ReleaseFromDate);
            }

            int i = 0;
            foreach (HtmlNode div in doc.DocumentNode.SelectNodes("//div[@class='b-ovhidden']"))
            {
                string artist = null, title = null;

                foreach (HtmlNode child in div.ChildNodes)
                {
                    if (child.Name == "h2" && child.GetAttributeValue("class", null) == "b-title")
                        artist = child.InnerText;
                    else if (child.Name == "div" && child.GetAttributeValue("class", null) == "b-text")
                        title = child.InnerText;
                }

                if (!string.IsNullOrWhiteSpace(artist) & !string.IsNullOrWhiteSpace(title))
                {
                    chart.Songs.Add(new Song { Artist = artist, Title = title });
                    Debug.WriteLine("Song {0}: {1} - {2}", ++i, artist, title);
                }
            }

            return chart;
        }

        /// <summary>
        /// Get the latests chart
        /// </summary>
        /// <param name="domain">Domain URL, e.g. http://muz-tv.ru</param>
        /// <param name="chartArchiveLink">Relative URL to chart archive, e.g. /charts/results/6/</param>
        /// <returns>Latest chart containing with name, release from date and list of songs</returns>
        public Chart GetLatestChart(string domain, string chartArchiveLink)
        {
            // Get URL to the latest chart
            string latestChartLink = GetChartLink(domain, chartArchiveLink);

            // Get songs from the latest chart
            return string.IsNullOrWhiteSpace(latestChartLink) ? null : GetChartNameReleaseFromDateAndSongs(domain, latestChartLink);
        }
    }
}
