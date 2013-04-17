﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fantasizer.Domain;

namespace YahooFantasyFootballTools.Tests.Utilities
{
    internal class TestObjectFactory
    {
        public ICollection<Game> CreateGames()
        {
            var games = new List<Game>();

            int season = 2012;
            for (int i = 0; i < 10; i++)
            {
                games.Add(new Game(GameCode.nfl, i, season--));
            }

            return games;
        }

        public LeagueCollection CreateLeagueCollection()
        {
            var leagues = new LeagueCollection();
            
            for (int i = 0; i < 3; i++)
            {
                leagues.Add(new League(i, "League_" + i, "LeagueKey_" + i, 1, 16));
            }

            return leagues;
        }

        public LeagueTeamCollection CreateLeagueTeamCollection(string leagueName, string leagueKey)
        {
            var league = new League(1, leagueName, leagueKey, 1, 16);
            var teamCollection = new TeamCollection();
            teamCollection.Add(new Team(1, "team_key", "team_name"));
            return new LeagueTeamCollection(league, teamCollection);
        }
    }
}
