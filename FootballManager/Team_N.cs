using System;
using System.Collections.Generic;
using System.Linq;

namespace FootballManager
{
    public class Team_N
    {
        public string Name { get; set; }
        public List<Player_N> Players { get; } = new List<Player_N>();
        public List<Player_N> Starters { get; private set; } = new List<Player_N>();
        public ITactic Tactic { get; set; } = new BalancedTactic();

        public int Wins { get; private set; }
        public int Draws { get; private set; }
        public int Losses { get; private set; }
        public int GoalsFor { get; private set; }
        public int GoalsAgainst { get; private set; }
        public int Budget { get; set; } = 200;

        public int MatchesPlayed { get { return Wins + Draws + Losses; } }
        public int GoalDifference { get { return GoalsFor - GoalsAgainst; } }
        public int Points { get { return (Wins * 3) + Draws; } }

        public int TeamAttack
        {
            get { return Starters.Sum(p => p.AttackValue) / 11; }
        }

        public int TeamDefense
        {
            get { return Starters.Sum(p => p.DefenseValue) / 11; }
        }

        public Team_N(string name)
        {
            Name = name;
        }

        public void SelectBestStartersByBaseStrength()
        {
            BuildLineup(p => p.Strength);
        }

        public void SelectBestStartersByEffectiveStrength()
        {
            BuildLineup(p => p.EffectiveStrength);
        }

        private void BuildLineup(Func<Player_N, int> strengthSelector)
        {
            Starters.Clear();
            var healthy = Players.Where(p => !p.IsInjured).ToList();

            Player_N bestGk = healthy.Where(p => p.Position.IsGoalkeeper) .OrderByDescending(strengthSelector) .FirstOrDefault();

            if (bestGk != null)
            {
                Starters.Add(bestGk);
            }

            var fieldPlayers = healthy.Where(p => !p.Position.IsGoalkeeper).OrderByDescending(strengthSelector).Take(11 - Starters.Count);
            Starters.AddRange(fieldPlayers);
        }

        public bool TrySwapPlayers(Player_N playerOut, Player_N playerIn, out string errorMessage)
        {
            errorMessage = "";
            if (playerIn.IsInjured)
            {
                errorMessage = "Sérült játékos nem állítható be!";
                return false;
            }

            int currentGkCount = Starters.Count(p => p.Position.IsGoalkeeper);
            int nextGkCount = currentGkCount - (playerOut.Position.IsGoalkeeper ? 1 : 0) + (playerIn.Position.IsGoalkeeper ? 1 : 0);

            if (nextGkCount != 1)
            {
                errorMessage = "A kezdőcsapatban pontosan 1 kapusnak kell lennie!";
                return false;
            }

            int idx = Starters.IndexOf(playerOut);
            if (idx != -1 && !Starters.Contains(playerIn))
            {
                Starters.RemoveAt(idx);
                Starters.Insert(idx, playerIn);
                return true;
            }

            return false;
        }

        public void EnsureValidStartingLineup()
        {
            var injuredStarters = Starters.Where(p => p.IsInjured).ToList();
            PlayerPosition missingPosition = new MidfielderPosition();

            foreach (Player_N injured in injuredStarters)
            {
                missingPosition = injured.Position;
                Starters.Remove(injured);

                Player_N replacement = null;
                if (injured.Position.IsGoalkeeper)
                {
                    replacement = Players
                        .Where(p => !Starters.Contains(p) && !p.IsInjured && p.Position.IsGoalkeeper)
                        .OrderByDescending(p => p.EffectiveStrength)
                        .FirstOrDefault();
                }

                if (replacement == null)
                {
                    replacement = Players
                        .Where(p => !Starters.Contains(p) && !p.IsInjured && !p.Position.IsGoalkeeper)
                        .OrderByDescending(p => p.EffectiveStrength)
                        .FirstOrDefault();
                }

                if (replacement != null)
                {
                    Starters.Add(replacement);
                }
            }

            while (Starters.Count < 11)
            {
                bool needGk = !Starters.Any(p => p.Position.IsGoalkeeper);
                PlayerPosition juniorPos = needGk ? new GoalkeeperPosition() : missingPosition;
                Player_N junior = new Player_N("Ifjúsági játékos", juniorPos, 40, true);
                Players.Add(junior);
                Starters.Add(junior);
            }
        }

        public void RemoveTemporaryJuniors()
        {
            Starters.RemoveAll(p => p.IsJunior);
            Players.RemoveAll(p => p.IsJunior);
            EnsureValidStartingLineup();
        }

        public void RecordMatchResult(int scored, int conceded, bool isPlayerTeam)
        {
            GoalsFor += scored;
            GoalsAgainst += conceded;
            if (scored > conceded) { Wins++; if (isPlayerTeam) Budget += 20; }
            else if (scored == conceded) { Draws++; if (isPlayerTeam) Budget += 10; }
            else { Losses++; if (isPlayerTeam) Budget += 5; }
        }

        public void UpdateAllPlayersAfterRound(int starterLoss, int extraRecovery)
        {
            foreach (Player_N p in Players)
            {
                p.UpdateFitnessAfterRound(Starters.Contains(p), starterLoss, extraRecovery);
                p.DecreaseInjury();
            }
        }
    }
}
