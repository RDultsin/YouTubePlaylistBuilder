namespace YouTubePlaylistBuilder.Data
{
    public class Song
    {
        public string Artist { get; set; }
        public string Title { get; set; }
        public string VideoId { get; set; }

        public string Keyword => $"{Artist} {Title}";
    }
}
