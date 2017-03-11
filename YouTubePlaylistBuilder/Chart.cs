using System.Collections.Generic;

namespace YouTubePlaylistBuilder
{
    class Chart
    {
        public string Name { get; set; }
        public string ReleaseFromDate { get; set; }
        public string Link { get; set; }
        public List<Song> Songs { get; set; }

        public string NameWithPrefix => $"МУЗ-ТВ {Name}";
        public string PlaylistName => $"МУЗ-ТВ {Name} ({ReleaseFromDate.ToLower()})";
    }
}
