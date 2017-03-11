# YouTubePlaylistBuilder
Application to scrape online music charts and build YouTube playlist out of them

## Purpose
Because someone wants to be able to play their favorite music charts anytime knowing their respective YouTube playlist is always up-to-date (check out the two sample playlists [here](https://www.youtube.com/channel/UCIjhNqtW_oWr2ZGSdxJunxA/playlists))

## Implementation
The .NET console application coded in C#, but could be easily implemented in Python/Perl/etc.  I picked C# because someone asked me for that language specifically (and given that I haven't coded in C# for a couple of years this small project would be a good refresher).

The high-level design of the application consists of three parts:

1. Scraping chart's song information from a web site
2. Looking up videos on YouTube for chart's songs
3. Creating or updating corresponding YouTube playlist with latest videos

### Scraping Web Page
Html Agility Pack (HAP) is the most predominant NuGet package used by .NET applications for web page scraping.  YouTubePlaylistBuilder uses HAP for two purposes in [ChartScraper.cs](https://github.com/RDultsin/YouTubePlaylistBuilder/blob/master/YouTubePlaylistBuilder/ChartScraper.cs):

1. Given chart archive page to determine latest chart page
2. Given latest chart page to determine artist and titles of chart's songs

### Looking Up YouTube Videos
Keyword searching for YouTube videos is implemented in [YouTubeApiHelper.cs](https://github.com/RDultsin/YouTubePlaylistBuilder/blob/master/YouTubePlaylistBuilder/YouTubeApiHelper.cs) and uses Simple API access (API Leys): [link](https://developers.google.com/api-client-library/dotnet/guide/aaa_apikeys).  Once acquired you need to save your YouTube API key in [App.config](https://github.com/RDultsin/YouTubePlaylistBuilder/blob/master/YouTubePlaylistBuilder/App.config) file.

### Creating / Updating YouTube Playlists
Creation and updates to YouTube playlists details and playlist items is also implemented in [YouTubeApiHelper.cs](https://github.com/RDultsin/YouTubePlaylistBuilder/blob/master/YouTubePlaylistBuilder/YouTubeApiHelper.cs), but uses Authorized API access (OAuth 2.0): [link](https://developers.google.com/api-client-library/dotnet/guide/aaa_oauth).  Once acquired you need to save your client secrets in [client_secrets.json](https://github.com/RDultsin/YouTubePlaylistBuilder/blob/master/YouTubePlaylistBuilder/client_secrets.json) file.

## Tools
* Visual Studio 2017 Community Edition: https://www.visualstudio.com/
* ReSharper 2016.03: https://www.jetbrains.com/resharper/
* Html Agility Pack: http://htmlagilitypack.codeplex.com/
* YouTube Data API v3: https://developers.google.com/youtube/v3/

## What's Next
Given time I'd like to move this console app to run as a job in the cloud
