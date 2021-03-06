﻿using System.Collections.Generic;
using System.Linq;
using Fantasizer.Domain;
using Tools.Analysis.Domain;

namespace Tools.Analysis.Logic
{
    // TODO: There is some duplicate code in here that can be refactored.
    // TODO: The double constructors smells funny, probably indicates that this should be separated into two separate analyzers
    public class KeeperAnalyzer
    {
        private readonly LeagueTeamPlayerCollection<Player> _leagueTeamPlayers;
        private readonly LeagueDraftResultCollection _draftResults;
        private readonly TeamPlayerCollection<PlayerWithStats> _teamPlayers;
        private readonly bool _allowKeepersFromPriorSeason;

        public KeeperAnalyzer(LeagueTeamPlayerCollection<Player> leagueTeamPlayers, LeagueDraftResultCollection draftResults, bool allowKeepersFromPriorSeason)
        {
            _leagueTeamPlayers = leagueTeamPlayers;
            _draftResults = draftResults;
            _allowKeepersFromPriorSeason = allowKeepersFromPriorSeason;
        }

        public KeeperAnalyzer(TeamPlayerCollection<PlayerWithStats> teamPlayers,
                              LeagueDraftResultCollection draftResults,
                              bool allowKeepersFromPriorSeason)
        {
            _teamPlayers = teamPlayers;
            _draftResults = draftResults;
            _allowKeepersFromPriorSeason = allowKeepersFromPriorSeason;
        }

        public List<EligibleKeeper> GetEligibleKeepersForLeague(string leagueKey)
        {
            var keepers = new List<EligibleKeeper>();

            foreach (var team in _leagueTeamPlayers)
            {
                foreach (var player in team.Players)
                {
                    var keeper = new EligibleKeeper()
                    {
                        TeamName = team.Team.Name,
                        PlayerName = player.Name,
                        PlayerKey = player.Key,
                        IsEligible = true // eligible by default
                    };

                    if (!_allowKeepersFromPriorSeason && EligibleKeeper.KeptByTeamInPriorSeason(team.Team.Key, player.Key))
                    {
                        keeper.IsEligible = false;
                        keeper.IneligibilityReason = "This player was kept last season";
                    }
                    else
                    {
                        var playerDraftResult = _draftResults.DraftResults.FirstOrDefault(d => d.PlayerKey == player.Key);
                        keeper.DraftRound = playerDraftResult != null ? playerDraftResult.Round : 15;
                        keeper.Cost = playerDraftResult != null ? playerDraftResult.Cost.GetValueOrDefault(0) : 0;
                    }

                    keepers.Add(keeper);
                }
            }

            return keepers;
        }

        public List<EligibleKeeper> GetEligibleKeepersForTeam(string teamKey)
        {
            var keepers = new List<EligibleKeeper>();

            foreach (var player in _teamPlayers.Players)
            {
                var keeper = new EligibleKeeper()
                {                    
                    TeamName = _teamPlayers.Team.Name,
                    PlayerName = player.Name,
                    PlayerKey = player.Key,
                    LastSeasonPoints = player.Points.Total,
                    IsEligible = true // eligible by default
                };

                if (!_allowKeepersFromPriorSeason && EligibleKeeper.KeptByTeamInPriorSeason(_teamPlayers.Team.Key, player.Key))
                {
                    keeper.IsEligible = false;
                    keeper.IneligibilityReason = "This player was kept last season";
                }
                else
                {
                    var playerDraftResult = _draftResults.DraftResults.FirstOrDefault(d => d.PlayerKey == player.Key);
                    keeper.DraftRound = playerDraftResult != null ? playerDraftResult.Round : 15;
                    keeper.Cost = playerDraftResult != null ? playerDraftResult.Cost.GetValueOrDefault(0) : 0;
                }

                keepers.Add(keeper);
            }

            return keepers;
        }
    }
}
