using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using YouTubePlaylistBuilder.Data;

namespace YouTubePlaylistBuilder.Services
{
    public class YouTubeApiHelper
    {
        private const string YouTubeApiKeyConfigKey = "YouTubeApiKey";

        public async Task<Chart> GetYouTubeVideoIds(Chart chart)
        {
            string youTubeApiKey;
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

            foreach (Song song in chart.Songs)
            {
                var searchListRequest = youTubeService.Search.List("snippet");
                searchListRequest.Q = song.Keyword;
                searchListRequest.MaxResults = 10;

                // Call the search.list method to retrieve results matching the specified query term
                var searchListResponse = await searchListRequest.ExecuteAsync();

                foreach (var searchResult in searchListResponse.Items)
                {
                    if (searchResult.Id.Kind == "youtube#video")
                    {
                        song.VideoId = searchResult.Id.VideoId;
                        Debug.WriteLine("Video id for {0} - {1} song is {2}", song.Artist, song.Title, song.VideoId);
                        break;
                    }
                }
            }

            return chart;
        }

        private async Task<Chart> GetOrCreatePlaylist(Chart chart)
        {
            int chartNameWithPrefixLen = chart.NameWithPrefix.Length; 

            UserCredential credential;
            try
            {
                using (FileStream stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read))
                {
                    credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        // This OAuth 2.0 access scope allows for full read/write access to the
                        // authenticated user's account.
                        new[] { YouTubeService.Scope.Youtube },
                        "user",
                        CancellationToken.None,
                        new FileDataStore(GetType().ToString())
                    );
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                return null;
            }

            var youTubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = GetType().ToString()
            });

            // Find playlist by chart name
            PlaylistsResource.ListRequest playlistReq = youTubeService.Playlists.List("snippet");
            playlistReq.Mine = true;
            playlistReq.MaxResults = 10;
            PlaylistListResponse playlistResp = await playlistReq.ExecuteAsync();

            Playlist playlist = null;
            foreach (Playlist playlistCandidate in playlistResp.Items)
            {
                string playlistTitle = playlistCandidate.Snippet.Title;
                if (playlistTitle.Length > chartNameWithPrefixLen && playlistTitle.Substring(0, chartNameWithPrefixLen) == chart.NameWithPrefix)
                {
                    playlist = playlistCandidate;
                    Debug.WriteLine("Playlist for {0} already exists as {1}", chart.Name, playlist.Snippet.Title);
                    break;
                }
            }

            if (playlist == null)
            {
                // Create a new, private playlist in the authorized user's channel
                playlist = new Playlist
                {
                    Snippet = new PlaylistSnippet
                    {
                        Title = chart.PlaylistName,
                        Description = "Playlist created with the YouTube Playlist Builder https://github.com/RDultsin/YouTubePlaylistBuilder"
                    },
                    Status = new PlaylistStatus
                    {
                        PrivacyStatus = "public"
                    }
                };
                playlist = await youTubeService.Playlists.Insert(playlist, "snippet,status").ExecuteAsync();

                Debug.WriteLine("Created playlist {0}", (object)playlist.Snippet.Title);
            }
            else
            {
                // Update title of playlist
                playlist.Snippet.Title = chart.PlaylistName;
                playlist.Snippet.Description = "Playlist updated with the YouTube Playlist Builder https://github.com/RDultsin/YouTubePlaylistBuilder";
                playlist = await youTubeService.Playlists.Update(playlist, "snippet").ExecuteAsync();

                Debug.WriteLine("Updated playlist {0}", (object)playlist.Snippet.Title);

                // Remove all videos from playlist
                PlaylistItemsResource.ListRequest playlistItemReq = youTubeService.PlaylistItems.List("snippet");
                playlistItemReq.PlaylistId = playlist.Id;
                playlistItemReq.MaxResults = 50;
                PlaylistItemListResponse playlistItemResp = await playlistItemReq.ExecuteAsync();
                foreach (PlaylistItem playlistItem in playlistItemResp.Items)
                {
                    await youTubeService.PlaylistItems.Delete(playlistItem.Id).ExecuteAsync();

                    Debug.WriteLine("Removed playlist item {0} from playlist {1}", playlistItem.Snippet.Title, playlist.Snippet.Title);
                }
            }

            // Add a videos to the playlist
            foreach (Song song in chart.Songs)
            {
                PlaylistItem playlistItem = new PlaylistItem
                {
                    Snippet = new PlaylistItemSnippet
                    {
                        PlaylistId = playlist.Id,
                        ResourceId = new ResourceId
                        {
                            Kind = "youtube#video",
                            VideoId = song.VideoId
                        }
                    }
                };
                playlistItem = await youTubeService.PlaylistItems.Insert(playlistItem, "snippet").ExecuteAsync();

                Debug.WriteLine("Added playlist item {0} to playlist {1}", playlistItem.Snippet.Title, playlist.Snippet.Title);
            }

            return chart;
        }

        public async Task<Chart> BuildPlaylist(Chart chart)
        {
            chart = await GetYouTubeVideoIds(chart);

            if (chart != null)
                chart = await GetOrCreatePlaylist(chart);

            return chart;
        }
    }
}
