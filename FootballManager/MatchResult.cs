using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballManager
{
    public class MatchResult
    {
        private Team _homeTeam;
        private Team _awayTeam;
        private int _homeGoals;
        private int _awayGoals;

        public Team HomeTeam { get { return _homeTeam; } }
        public Team AwayTeam { get { return _awayTeam; } }
        public int HomeGoals { get { return _homeGoals; } }
        public int AwayGoals { get { return _awayGoals; } }

        public MatchResult(Team homeTeam, Team awayTeam, int homeGoals, int awayGoals)
        {
            _homeTeam = homeTeam;
            _awayTeam = awayTeam;
            _homeGoals = homeGoals;
            _awayGoals = awayGoals;
        }
    }
}