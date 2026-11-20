
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FootballManager
{
    internal class Game
    {
        private Random _random = new Random();
        private List<Team> _teams = new List<Team>();
        private Team _playerTeam;
        private string _managerName;
        private int _currentRound = 1;
        private const int TotalRounds = 14;

        public void Run()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine(Texts.Title);
            Console.WriteLine();

            if (!File.Exists("players.txt"))
            {
                Console.WriteLine("Hiba: A players.txt fájl nem található!");
                return;
            }

            _teams = LoadTeamsFromFile("players.txt");

            if (_teams.Count < 8 || _teams.Count % 2 != 0)
            {
                Console.WriteLine("Hiba: Legalább 8, páros számú csapat szükséges.");
                return;
            }

            foreach (Team i in _teams)
            {
                i.SelectBestStartersByBaseStrength();
            }

            _managerName = ReadNonEmptyText("Add meg a neved, Menedzser: ");
            SelectPlayerTeam();

            bool exitRequested = false;

            while (_currentRound <= TotalRounds && !exitRequested)
            {
                PrintStatusBar();

                Console.WriteLine("[1] Következő forduló");
                Console.WriteLine("[2] Kezdőcsapat");
                Console.WriteLine("[3] Tabella");
                Console.WriteLine("[4] Keret");
                Console.WriteLine("[5] Kilépés");

                int choice = ReadValidInt("Válassz menüpontot (1-5): ", 1, 5);

                switch (choice)
                {
                    case 1:
                        PlayNextRound();
                        break;
                    case 2:
                        ManageStartingLineup();
                        break;
                    case 3:
                        PrintStandings();
                        break;
                    case 4:
                        PrintSquad();
                        break;
                    case 5:
                        exitRequested = true;
                        break;
                }

                Console.WriteLine();
            }

            if (_currentRound > TotalRounds)
            {
                EndSeason();
            }

            Console.WriteLine("Nyomj meg egy billentyűt a kilépéshez.");
            Console.ReadKey(true);
        }

        private List<Team> LoadTeamsFromFile(string filePath)
        {
            Dictionary<string, Team> teamDict = new Dictionary<string, Team>();

            string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                string[] parts = line.Split(';');

                if (parts.Length != 4)
                {
                    continue;
                }

                string teamName = parts[0].Trim();
                string playerName = parts[1].Trim();
                string posText = parts[2].Trim();
                string strengthText = parts[3].Trim();

                if (teamName == "Szabad" || teamName == "" || playerName == "")
                {
                    continue;
                }

                Position position;
                int strength;

                if (!Enum.TryParse(posText, true, out position))
                {
                    continue;
                }

                if (!int.TryParse(strengthText, out strength))
                {
                    continue;
                }

                if (!teamDict.ContainsKey(teamName))
                {
                    teamDict.Add(teamName, new Team(teamName));
                }

                teamDict[teamName].Players.Add(new Player(playerName, position, strength));
            }

            return teamDict.Values.ToList();
        }

        private int ReadValidInt(string prompt, int min, int max)
        {
            int result;

            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();

                if (int.TryParse(input, out result))
                {
                    if (result >= min && result <= max)
                    {
                        return result;
                    }
                }

                Console.WriteLine("Érvénytelen bevitel!");
            }
        }

        private string ReadNonEmptyText(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(input))
                {
                    return input.Trim();
                }

                Console.WriteLine("A név nem lehet üres!");
            }
        }

        private void SelectPlayerTeam()
        {
            Console.WriteLine("\nÜdvözöllek, " + _managerName + "! Válassz csapatot:");

            for (int i = 0; i < _teams.Count; i++)
            {
                Console.WriteLine("[" + (i + 1) + "] " + _teams[i].Name +
                    " (Kezdő erő: " + _teams[i].TeamStrength + ")");
            }

            int teamChoice = ReadValidInt("Csapat sorszáma: ", 1, _teams.Count);

            _playerTeam = _teams[teamChoice - 1];

            Console.WriteLine("Kiválasztott csapat: " + _playerTeam.Name);
        }

        private List<Team> GetSortedStandings()
        {
            List<Team> standings = new List<Team>(_teams);

            for (int i = 0; i < standings.Count; i++)
            {
                for (int j = i + 1; j < standings.Count; j++)
                {
                    if (standings[j].Points > standings[i].Points ||
                        (standings[j].Points == standings[i].Points &&
                        standings[j].GoalDifference > standings[i].GoalDifference))
                    {
                        Team temp = standings[i];
                        standings[i] = standings[j];
                        standings[j] = temp;
                    }
                }
            }

            return standings;
        }

        private void PrintStatusBar()
        {
            List<Team> standings = GetSortedStandings();

            int rank = standings.IndexOf(_playerTeam) + 1;

            Console.WriteLine("==========================================");
            Console.WriteLine(_playerTeam.Name + " | Forduló " +
                _currentRound + "/" + TotalRounds +
                " | " + rank + ". hely | " + _playerTeam.Points + " pont");
            Console.WriteLine("==========================================");
        }

        private void PrintSquad()
        {
            Console.WriteLine("--- " + _playerTeam.Name + " TELJES KERETE ---");

            for (int i = 0; i < _playerTeam.Players.Count; i++)
            {
                Player p = _playerTeam.Players[i];

                string status = "Kispad";

                if (_playerTeam.Starters.Contains(p))
                {
                    status = "KEZDŐ";
                }

                Console.WriteLine((i + 1) + ". " + p.Name +
                    " | " + p.Position +
                    " | Erő: " + p.Strength +
                    " | Tényleges erő: " + p.EffectiveStrength +
                    " | Fittség: " + p.Fitness + "% | " + status);
            }
        }

        private void ManageStartingLineup()
        {
            while (true)
            {
                Console.WriteLine("--- KEZDŐCSAPAT ---");

                for (int i = 0; i < _playerTeam.Starters.Count; i++)
                {
                    Player p = _playerTeam.Starters[i];

                    Console.WriteLine("[" + (i + 1) + "] " + p.Name +
                        " | " + p.Position + " | Erő: " + p.Strength);
                }

                List<Player> bench = new List<Player>();

                foreach (Player p in _playerTeam.Players)
                {
                    if (!_playerTeam.Starters.Contains(p))
                    {
                        bench.Add(p);
                    }
                }

                Console.WriteLine("\n--- KISPAD ---");

                for (int i = 0; i < bench.Count; i++)
                {
                    Console.WriteLine("[" + (i + 1) + "] " + bench[i].Name +
                        " | " + bench[i].Position + " | Erő: " + bench[i].Strength);
                }

                Console.WriteLine("[0] Vissza");

                int outChoice = ReadValidInt("Ki menjen le? ", 0, _playerTeam.Starters.Count);

                if (outChoice == 0)
                {
                    break;
                }

                if (bench.Count == 0)
                {
                    Console.WriteLine("Nincs játékos a kispadon!");
                    break;
                }

                int inChoice = ReadValidInt("Ki jöjjön be? ", 1, bench.Count);

                Player playerOut = _playerTeam.Starters[outChoice - 1];
                Player playerIn = bench[inChoice - 1];

                _playerTeam.SwapPlayers(playerOut, playerIn);

                Console.WriteLine("Csere megtörtént!");
            }
        }

        private void PlayNextRound()
        {
            foreach (Team i in _teams)
            {
                if (i != _playerTeam)
                {
                    i.SelectBestStartersByEffectiveStrength();
                }
            }

            List<Team> shuffledTeams = new List<Team>(_teams);

            for (int i = 0; i < shuffledTeams.Count; i++)
            {
                int j = _random.Next(shuffledTeams.Count);

                Team temp = shuffledTeams[i];
                shuffledTeams[i] = shuffledTeams[j];
                shuffledTeams[j] = temp;
            }

            List<MatchResult> results = new List<MatchResult>();

            for (int i = 0; i < shuffledTeams.Count; i += 2)
            {
                Team home = shuffledTeams[i];
                Team away = shuffledTeams[i + 1];

                results.Add(PlayMatch(home, away));
            }

            Console.WriteLine("--- " + _currentRound + ". FORDULÓ EREDMÉNYEI ---");

            foreach (MatchResult match in results)
            {
                string prefix = "";

                if (match.HomeTeam == _playerTeam || match.AwayTeam == _playerTeam)
                {
                    prefix = ">> ";
                }

                Console.WriteLine(prefix + match.HomeTeam.Name + " - " +
                    match.AwayTeam.Name + ": " +
                    match.HomeGoals + " - " + match.AwayGoals);
            }

            foreach (Team i in _teams)
            {
                i.UpdateAllPlayersFitness();
            }

            _currentRound++;
        }

        private MatchResult PlayMatch(Team home, Team away)
        {
            int homeGoals = _random.Next(0, 4);
            int awayGoals = _random.Next(0, 4);

            homeGoals += home.TeamStrength / 35;
            awayGoals += away.TeamStrength / 35;

            home.RecordMatchResult(homeGoals, awayGoals);
            away.RecordMatchResult(awayGoals, homeGoals);

            return new MatchResult(home, away, homeGoals, awayGoals);
        }

        private void PrintStandings()
        {
            List<Team> standings = GetSortedStandings();

            Console.WriteLine("--- BAJNOKI TABELLA ---");

            for (int i = 0; i < standings.Count; i++)
            {
                Team t = standings[i];

                Console.WriteLine((i + 1) + ". " + t.Name +
                    " | Meccs: " + t.MatchesPlayed +
                    " | Győzelem: " + t.Wins +
                    " | Döntetlen: " + t.Draws +
                    " | Vereség: " + t.Losses +
                    " | Gólkülönbség: " + t.GoalDifference +
                    " | Pont: " + t.Points);
            }
        }

        private void EndSeason()
        {
            List<Team> standings = GetSortedStandings();

            Console.WriteLine("\n========== SZEZON VÉGE ==========");

            Console.WriteLine("Bajnok: " + standings[0].Name);
            Console.WriteLine("Pontszám: " + standings[0].Points);

            int playerRank = standings.IndexOf(_playerTeam) + 1;

            Console.WriteLine("A te csapatod: " + _playerTeam.Name);
            Console.WriteLine("Végeredmény: " + playerRank + ". hely");
            Console.WriteLine("Pontok: " + _playerTeam.Points);

            Console.WriteLine("=================================");
        }
    }
}
