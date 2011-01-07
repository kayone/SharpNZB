using System;
using System.Text.RegularExpressions;
using SharpNzb.Core.Model;

namespace SharpNzb.Core.Providers
{
    public class FilenameParseProvider : IFilenameParseProvider
    {
        private readonly string _singlePattern =
            @"(?<ShowName>(?:.*?))(?:[Ss](?<Season>(?:\d{1,2}))[Ee](?<Episode>(?:\d{1,2}))|(?<Season>(?:\d{1,2}))[Xx](?<Episode>(?:\d{1,2})))(?<EpisodeName>(?:.*))";
        private readonly string _multiPattern = @"[Ss(?<ShowName>(?:.*?))Season>(?:\d{1,2}))[Ee](?<EpisodeOne>(?:\d{1,2}))[Ee](?<EpisodeTwo>(?:\d{1,2}))(?<EpisodeName>(?:.*))";
        private readonly string _dailyPattern = @"(?<ShowName>(?:.*?))(?<Year>\d{4}).{1}(?<Month>\d{2}).{1}(?<Day>\d{2})(?<EpisodeName>(?:.*))";

        #region IFilenameParseProvider Members

        public TvShowParseModel ParseTv(string title)
        {
            //Try to parse out the Series return MatchSeasonEpisodeMulti(feedItem) ??
            return MatchSeasonEpisodeMulti(title) ??
                   MatchSeasonEpisode(title) ??
                   MatchFirstAiredEpisode(title);
        }

        #endregion

        private TvShowParseModel MatchSeasonEpisodeMulti(string title)
        {
            Match match = Regex.Match(title, _multiPattern);
            if (!match.Success) return null;

            return new TvShowParseModel
            {
                ShowName = match.Groups["ShowName"].Value.Replace('.', ' ').Trim(' ', '-'),
                SeasonNumber = int.Parse(match.Groups["Season"].Value),
                EpisodeNumber = int.Parse(match.Groups["EpisodeOne"].Value),
                EpisodeNumber2 = int.Parse(match.Groups["EpisodeTwo"].Value),
                EpisodeName =match.Groups["EpisodeName"].Value.Replace('.', ' ').Trim(' ', '-')
            };
        }

        private TvShowParseModel MatchSeasonEpisode(string title)
        {
            Match match = Regex.Match(title, _singlePattern);
            if (!match.Success)
            {
                return null;
            }

            return new TvShowParseModel()
            {
                ShowName = match.Groups["ShowName"].Value.Replace('.', ' ').Trim(' ', '-'),
                SeasonNumber = int.Parse(match.Groups["Season"].Value),
                EpisodeNumber = int.Parse(match.Groups["Episode"].Value),
                EpisodeName = match.Groups["EpisodeName"].Value.Replace('.', ' ').Trim(' ', '-')
            };
        }

        private TvShowParseModel MatchFirstAiredEpisode(string title)
        {
            Match match = Regex.Match(title, _dailyPattern);
            if (!match.Success) return null;

            DateTime date = DateTime.Parse(string.Format("{0}-{1}-{2}",
                                                         match.Groups["Year"].Value,
                                                         match.Groups["Month"].Value,
                                                         match.Groups["Day"].Value));

            return new TvShowParseModel
            {
                ShowName = match.Groups["ShowName"].Value.Replace('.', ' ').Trim(' ', '-'),
                Year = date.Year,
                Month = date.Month,
                Day = date.Day,
                EpisodeName = match.Groups["EpisodeName"].Value.Replace('.', ' ').Trim(' ', '-')
            };
        }
    }
}
