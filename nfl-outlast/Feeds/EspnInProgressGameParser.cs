using System;
using System.Linq;
using nfl_outlast.Shared;

namespace nfl_outlast.Feeds
{
    public class EspnInProgressGameParser : EspnGameParser
    {
        protected override bool CanParseGameDetails(string gameDetails)
        {
            return gameDetails.Contains(" IN ") || gameDetails.Contains("HALFTIME");
        }

        protected override EspnFeedGame ParseGameDetails(string gameDetails)
        {
            var lastClosedParen = gameDetails.LastIndexOf(")");
            var lastOpenParen = gameDetails.LastIndexOf("(");

            var timeLeftOnClock = gameDetails.Substring(lastOpenParen, lastClosedParen - lastOpenParen + 1);
            var scoreDetails = gameDetails.Replace(timeLeftOnClock, "").Trim();
            var teamScoreDetails = scoreDetails.Split("   ");

            var lastSpace = teamScoreDetails[0].LastIndexOf(' ');
            var awayTeam = new EspnFeedTeam
                {
                    Name = teamScoreDetails[0].Substring(0, lastSpace),
                    Score = int.Parse(teamScoreDetails[0].Substring(lastSpace))
                };

            lastSpace = teamScoreDetails[1].LastIndexOf(' ');
            var homeTeam = new EspnFeedTeam
                {
                    Name = teamScoreDetails[1].Substring(0, lastSpace),
                    Score = int.Parse(teamScoreDetails[1].Substring(lastSpace))
                };

            var game = new EspnFeedGame(awayTeam, homeTeam, GameStatus.InProgress);

            if (timeLeftOnClock.Contains(" IN "))
            {
                var timeAndQuarter = timeLeftOnClock.Split(" IN ");
                var timeLeft = new string(timeAndQuarter[0].Where(char.IsNumber).ToArray());
                var minutesLeft = int.Parse(timeLeft.Substring(0, 2));
                var secondsLeft = int.Parse(timeLeft.Substring(2, 2));
                var quarter = int.Parse(new string(timeAndQuarter[1].Where(char.IsNumber).ToArray()));
                game.TimeLeftInQuarter = new TimeSpan(0, minutesLeft, secondsLeft);
                game.CurrentQuarter = quarter;
            }
            else if(timeLeftOnClock.Contains("HALFTIME"))
            {
                game.TimeLeftInQuarter = new TimeSpan(0, 15, 0);
                game.CurrentQuarter = 3;
            }

            return game;
        }
    }
}