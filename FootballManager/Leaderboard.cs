using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class LeaderboardEntry
{
    public string PlayerName { get; set; }
    public string TeamName { get; set; }
    public Difficulty Difficulty { get; set; }
    public int Score { get; set; }
}

public class Leaderboard
{
    private const string FilePath = "leaderboard.txt";
    private List<LeaderboardEntry> entries = new List<LeaderboardEntry>();

    public Leaderboard()
    {
        Load();
    }

    private void Load()
    {
        if (!File.Exists(FilePath)) return;

        foreach (var line in File.ReadAllLines(FilePath))
        {
            var parts = line.Split(';');
            if (parts.Length == 4)
            {
                int score;
                bool isValidScore = int.TryParse(parts[3], out score);

                Difficulty diff = Difficulty.Normal;
                bool isValidDiff = true;

                if (parts[2] == "Easy")
                {
                    diff = Difficulty.Easy;
                }
                else if (parts[2] == "Normal")
                {
                    diff = Difficulty.Normal;
                }
                else if (parts[2] == "Hard")
                {
                    diff = Difficulty.Hard;
                }
                else
                {
                    isValidDiff = false;
                }

                if (isValidScore && isValidDiff)
                {
                    entries.Add(new LeaderboardEntry
                    {
                        PlayerName = parts[0],
                        TeamName = parts[1],
                        Difficulty = diff,
                        Score = score
                    });
                }
            }
        }
        SortAndTrim();
    }


    public void AddResult(string playerName, string teamName, int leaguePoints, Settings settings)
    {
        string cleanPlayer = playerName.Replace(";", "");
        string cleanTeam = teamName.Replace(";", "");
        int finalScore = (int)Math.Floor(leaguePoints * settings.ScoreMultiplier);

        entries.Add(new LeaderboardEntry
        {
            PlayerName = cleanPlayer,
            TeamName = cleanTeam,
            Difficulty = settings.CurrentDifficulty,
            Score = finalScore
        });

        SortAndTrim();
        Save();
    }

    private void SortAndTrim()
    {
        entries = entries.OrderByDescending(e => e.Score).Take(10).ToList();
    }

    private void Save()
    {
        var lines = entries.Select(e => $"{e.PlayerName};{e.TeamName};{e.Difficulty};{e.Score}");
        File.WriteAllLines(FilePath, lines);
    }

    public void Print()
    {
        Console.WriteLine("=== LEADERBOARD (TOP 10) ===");
        if (entries.Count == 0)
        {
            Console.WriteLine("No records yet.");
            return;
        }

        for (int i = 0; i < entries.Count; i++)
        {
            var e = entries[i];
            Console.WriteLine($"{i + 1,2}. {e.PlayerName,-15} | {e.TeamName,-15} | {e.Difficulty,-6} | {e.Score} pts");
        }
    }
}
