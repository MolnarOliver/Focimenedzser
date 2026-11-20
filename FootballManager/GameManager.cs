using System;
using System.Collections.Generic;
using System.Linq;

public class GoalEvent
{
    public int Minute { get; set; }
    public string PlayerName { get; set; }
}

public class GameManager
{
    private Random rndm = new Random();

    public void HandleInjuries(List<Player> squad)
    {
        squad.RemoveAll(p => p.IsJunior);

        foreach (var p in squad.Where(p => p.IsInjured))
        {
            p.InjuryRounds--;
        }

        foreach (var p in squad.Where(p => p.IsStarter && !p.IsInjured))
        {
            if (rndm.Next(100) < 5)
            {
                p.InjuryRounds = rndm.Next(1, 4);
                p.IsStarter = false;
                Console.WriteLine($"INJURY: {p.Name} is injured for {p.InjuryRounds} rounds!");
            }
        }

        while (squad.Count(p => p.IsStarter) < 11)
        {
            var bestSub = squad .Where(p => !p.IsStarter && !p.IsInjured) .OrderByDescending(p => p.EffectiveStrength) .FirstOrDefault();

            if (bestSub != null)
            {
                bestSub.IsStarter = true;
                Console.WriteLine($"SUB: {bestSub.Name} moved to the starting lineup.");
            }
            else
            {
                squad.Add(new Player
                {
                    Name = "Youth Player (Junior)",
                    Strength = 40,
                    Fitness = 100,
                    IsStarter = true,
                    IsJunior = true
                });
                Console.WriteLine("YOUTH: No healthy bench players available, a junior joined for the next match.");
            }
        }
    }

    public void PrintSquad(List<Player> squad)
    {
        foreach (var p in squad)
        {
            string status = p.IsInjured ? $"[INJURED: {p.InjuryRounds} rds]" : (p.IsStarter ? "[STARTER]" : "[BENCH]");
            Console.WriteLine($"{p.Name,-20} | Str: {p.Strength} | Fit: {p.Fitness}% | {status}");
        }
    }

    public TrainingType SelectTraining()
    {
        Console.WriteLine("Choose training: 1 - Attack (+5 chance), 2 - Defense (-5 opponent chance), 3 - Recovery (+5 fitness)");
        return Console.ReadLine() switch
        {
            "1" => TrainingType.Attack,
            "2" => TrainingType.Defense,
            _ => TrainingType.Recovery
        };
    }

    public void ApplyTrainingEffects(TrainingType training, ref int ownChance, ref int oppChance, List<Player> squad)
    {
        if (training == TrainingType.Attack) ownChance += 5;
        if (training == TrainingType.Defense) oppChance -= 5;

        ownChance = Math.Clamp(ownChance, 5, 40);
        oppChance = Math.Clamp(oppChance, 5, 40);

        if (training == TrainingType.Recovery)
        {
            foreach (var p in squad)
            {
                p.Fitness = Math.Min(100, p.Fitness + 5);
            }
        }
    }

    public void GenerateMatchGoals(int goalsScored, List<Player> squad)
    {
        var starters = squad.Where(p => p.IsStarter).ToList();
        var events = new List<GoalEvent>();

        var weightedPool = new List<Player>();
        foreach (var p in starters)
        {
            int weight = p.Position switch
            {
                Position.Forward => 3,
                Position.Midfielder => 2,
                _ => 1
            };
            for (int i = 0; i < weight; i++) weightedPool.Add(p);
        }

        for (int i = 0; i < goalsScored; i++)
        {
            var scorer = weightedPool[rndm.Next(weightedPool.Count)];
            scorer.Goals++;
            events.Add(new GoalEvent
            {
                Minute = rndm.Next(1, 91),
                PlayerName = scorer.Name
            });
        }

        foreach (var ev in events.OrderBy(e => e.Minute))
        {
            Console.WriteLine($"  {ev.Minute}' {ev.PlayerName}");
        }
    }

    public void PrintTopScorers(List<Player> squad)
    {
        Console.WriteLine("--- TOP SCORERS (TOP 10) ---");
        var top10 = squad.Where(p => !p.IsJunior).OrderByDescending(p => p.Goals) .ThenBy(p => p.Name) .Take(10);

        int rank = 1;
        foreach (var p in top10)
        {
            Console.WriteLine($"{rank++}. {p.Name,-20} - {p.Goals} goals");
        }
    }

    public void AddMatchRevenue(int ownGoals, int oppGoals, ref int budget)
    {
        if (ownGoals > oppGoals) budget += 20;
        else if (ownGoals == oppGoals) budget += 10;
        else budget += 5;
    }

    public void OpenTransferMarket(List<Player> squad, List<Player> freeAgents, ref int budget)
    {
        Console.WriteLine($"--- TRANSFER MARKET (Budget: {budget}M | Squad: {squad.Count}/18) ---");
        for (int i = 0; i < freeAgents.Count; i++)
        {
            var p = freeAgents[i];
            Console.WriteLine($"{i + 1}. {p.Name,-20} | Str: {p.Strength} | Price: {p.Price}M");
        }
        Console.WriteLine("0. Back");

        if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= freeAgents.Count)
        {
            BuyPlayer(freeAgents[choice - 1], squad, freeAgents, ref budget);
        }
    }

    private void BuyPlayer(Player target, List<Player> squad, List<Player> freeAgents, ref int budget)
    {
        if (squad.Count >= 18)
        {
            Console.WriteLine("ERROR: Squad is full (max 18 players)!");
            return;
        }
        if (budget < target.Price)
        {
            Console.WriteLine("ERROR: Insufficient funds!");
            return;
        }

        budget -= target.Price;
        target.IsStarter = false; 
        freeAgents.Remove(target);
        squad.Add(target);

        Console.WriteLine($"SUCCESS: {target.Name} joined your squad!");
    }

    public void SelectDifficulty(Settings settings)
    {
        Console.WriteLine("Select Difficulty (1 - Easy, 2 - Normal, 3 - Hard):");
        Difficulty chosen = Console.ReadLine() switch
        {
            "1" => Difficulty.Easy,
            "3" => Difficulty.Hard,
            _ => Difficulty.Normal
        };
        settings.ApplyDifficulty(chosen);
    }
}
