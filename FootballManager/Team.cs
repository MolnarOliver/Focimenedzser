using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballManager
{
        public class Team
        {
            private string _name;
            private List<Player> _players;
            private List<Player> _starters;
            private int _wins;
            private int _draws;
            private int _losses;
            private int _goalsFor;
            private int _goalsAgainst;

            public string Name { get { return _name; } set { _name = value; }}

            public List<Player> Players {  get { return _players; }}

            public List<Player> Starters{  get { return _starters; } }

            public int Wins { get { return _wins; } }
            public int Draws { get { return _draws; } }
            public int Losses { get { return _losses; } }
            public int GoalsFor { get { return _goalsFor; } }
            public int GoalsAgainst { get { return _goalsAgainst; } }

            public int MatchesPlayed { get { return _wins + _draws + _losses; } }

            public int GoalDifference { get { return _goalsFor - _goalsAgainst; } }

            public int Points { get { return (_wins * 3) + _draws; }}

            public int TeamStrength
            {
                get
                {
                    if (_starters.Count == 0) return 0;
                    int sum = 0;
                    foreach (Player i in _starters)
                    {
                        sum += i.EffectiveStrength;
                    }
                    return sum / _starters.Count;
                }
            }

            public Team(string name)
            {
                _name = name;
                _players = new List<Player>();
                _starters = new List<Player>();
                _wins = 0;
                _draws = 0;
                _losses = 0;
                _goalsFor = 0;
                _goalsAgainst = 0;
            }

            public void SelectBestStartersByBaseStrength()
            {
                _starters = _players .OrderByDescending(i => i.Strength).Take(11).ToList();
            }

            public void SelectBestStartersByEffectiveStrength()
            {
                _starters = _players .OrderByDescending(i => i.EffectiveStrength).Take(11).ToList();
            }

            public void SwapPlayers(Player playerOut, Player playerIn)
            {
                int index = _starters.IndexOf(playerOut);
                if (index != -1 && !_starters.Contains(playerIn))
                {
                    _starters[index] = playerIn;
                }
            }

            public void RecordMatchResult(int scored, int conceded)
            {
                _goalsFor += scored;
                _goalsAgainst += conceded;

                if (scored > conceded)
                {
                    _wins++;
                }
                else if (scored == conceded)
                {
                    _draws++;
                }
                else
                {
                    _losses++;
                }
            }

            public void UpdateAllPlayersFitness()
            {
                foreach (Player i in _players)
                {
                    bool isStarter = _starters.Contains(i);
                    i.UpdateFitnessAfterRound(isStarter);
                }
            }
        }
    }
