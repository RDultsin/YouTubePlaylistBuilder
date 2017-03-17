using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using YouTubePlaylistBuilder.Data;

namespace YouTubePlaylistBuilder.Services
{
    internal class SongCache : Dictionary<string, Song>
    {
        internal SongCache(string songsJsonFile)
        {
            try
            {
                string json = File.ReadAllText(songsJsonFile);
                List<Song> songs = JsonConvert.DeserializeObject<List<Song>>(json);
                foreach (Song song in songs)
                {
                    Add(song.Keyword, song);
                }
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("Warning: " + e.Message);
            }
        }

        internal void SaveToFile(string songsJsonFile)
        {
            List<Song> songs = new List<Song>();
            foreach (Song song in Values)
            {
                songs.Add(song);
            }
            string json = JsonConvert.SerializeObject(songs, Formatting.Indented);
            File.WriteAllText(songsJsonFile, json);
        }
    }
}
