using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum Difficulty { Easy, Normal, Hard }
public enum TrainingType { Attack, Defense, Recovery }
public enum Position { Goalkeeper, Defender, Midfielder, Forward }

public class Settings
{
    public Difficulty CurrentDifficulty { get; private set; }
    public int TeamStrengthBonus { get; private set; }
    public int StartingBudget { get; private set; }
    public int OpponentStrengthBonus { get; private set; }
    public int FitnessLossPerMatch { get; private set; }
    public double ScoreMultiplier { get; private set; }

    public void ApplyDifficulty(Difficulty diff)
    {
        CurrentDifficulty = diff;
        switch (diff)
        {
            case Difficulty.Easy:
                TeamStrengthBonus = 5;
                StartingBudget = 300;
                OpponentStrengthBonus = -3;
                FitnessLossPerMatch = 4;
                ScoreMultiplier = 0.5;
                break;
            case Difficulty.Normal:
                TeamStrengthBonus = 0;
                StartingBudget = 200;
                OpponentStrengthBonus = 0;
                FitnessLossPerMatch = 6;
                ScoreMultiplier = 1.0;
                break;
            case Difficulty.Hard:
                TeamStrengthBonus = -5;
                StartingBudget = 100;
                OpponentStrengthBonus = 3;
                FitnessLossPerMatch = 10;
                ScoreMultiplier = 1.5;
                break;
        }
    }
}