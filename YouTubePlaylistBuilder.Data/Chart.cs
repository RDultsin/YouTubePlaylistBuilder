using System.Collections.Generic;

namespace YouTubePlaylistBuilder.Data
{
    public class Chart
    {
        public string Name { get; set; }
        public string ReleaseFromDate { get; set; }
        public string Domain { get; set; }
        public string Link { get; set; }
        public List<Song> Songs { get; set; }

        public string NameWithPrefix => $"МУЗ-ТВ {Name}";
        public string PlaylistName => $"МУЗ-ТВ {Name} ({ReleaseFromDate.ToLower()})";
        public string Url => $"{Domain}{Link}";
    }
}
