using Newtonsoft.Json;

namespace YouTubePlaylistBuilder.Data
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Song
    {
        // "A'STUDIO"
        [JsonProperty]
        public string Artist { get; set; }

	    // "Только С Тобой"
        [JsonProperty]
        public string Title { get; set; }

	    // "cJmsrnwMIZU"
	    [JsonProperty]
	    public string VideoId { get; set; }

        public string Keyword => $"{Artist} {Title}";
    }
}
