using HtmlAgilityPack;
using System.Collections.Generic;
using System.Diagnostics;

namespace YouTubePlaylistBuilder
{
    class ChartHelper
    {
        readonly HtmlWeb _htmlWeb = new HtmlWeb();

        /// <summary>
        /// Get relative URL to latest chart
        /// </summary>
        /// <param name="domain">Domain URL</param>
        /// <param name="chartArchiveLink">Relative URL to chart archive</param>
        /// <returns>Relative URL to latest chart</returns>
        private string GetChartLink(string domain, string chartArchiveLink)
        {
            string chartLink = null;

            int chartArchiveUrlLen = chartArchiveLink.Length;

            HtmlDocument doc = _htmlWeb.Load(domain + chartArchiveLink);
            if (doc != null)
            {
                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//a[@href][@class='b-archive_show x-results-toggle']"))
                {
                    string chartLinkCandidate = link.GetAttributeValue("href", null);
                    if (!string.IsNullOrEmpty(chartLinkCandidate) && chartLinkCandidate.Length > chartArchiveUrlLen &&
                        chartLinkCandidate.Substring(0, chartArchiveUrlLen) == chartArchiveLink)
                    {
                        chartLink = chartLinkCandidate;
                        Debug.WriteLine("Chart link: {0}", (object)chartLink);
                        break;
                    }
                }
            }

            return chartLink;
        }

        /// <summary>
        /// Get songs from chart
        /// </summary>
        /// <param name="domain">Domain URL</param>
        /// <param name="chartLink">Relateive URL to chart</param>
        /// <returns>List of songs from chart</returns>
        private List<Song> GetChartSongs(string domain, string chartLink)
        {
            List<Song> songs = new List<Song>();

            HtmlDocument doc = _htmlWeb.Load(domain + chartLink);
            if (doc != null)
            {
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
                        songs.Add(new Song() { Artist = artist, Title = title });
                        Debug.WriteLine("Song {0}: {1} - {2}", ++i, title, artist);
                    }
                }
            }

            return songs;
        }

        /// <summary>
        /// Get songs from the latests chart
        /// </summary>
        /// <param name="domain">Domain URL</param>
        /// <param name="chartArchiveLink">Relative URL to chart archive</param>
        /// <returns>List of songs from the latest chart</returns>
        public List<Song> GetLatestChartSongs(string domain, string chartArchiveLink)
        {
            // Get URL to the latest chart
            string latestChartLink = GetChartLink(domain, chartArchiveLink);

            // Get songs from the latest chart
            if (string.IsNullOrWhiteSpace(latestChartLink))
                return null;
            else
                return GetChartSongs(domain, latestChartLink);
        }
    }
}
