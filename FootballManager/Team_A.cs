
using System;

namespace FootballManager
{
    public class Player2
    {
        public string Name { get; set; }
        public Position Position { get; set; }
        public int Strength { get; set; }
        public int Fitness { get; private set; }
        public int InjuryRounds { get; private set; }
        public int Goals { get; private set; }
        public bool IsJunior { get; private set; }

        public bool IsInjured  { get {  return InjuryRounds > 0; } }

        public int EffectiveStrength{ get  {  return Strength * (100 + Fitness) / 200; } }

        public int Price { get { int calculated = (Strength - 40) * 5;
                if (calculated < 0)
                {
                    return 0;
                }
                return calculated;
            }
        }

        public Player2(string name, Position position, int strength, bool isJunior = false)
        {
            Name = name;
            Position = position;
            Strength = strength;
            Fitness = 100;
            InjuryRounds = 0;
            Goals = 0;
            IsJunior = isJunior;
        }

        public void UpdateFitnessAfterRound(bool wasStarter, int starterLoss, int extraRecovery)
        {
            if (wasStarter)
            {
                Fitness -= starterLoss;
            }
            else
            {
                Fitness += 15;
            }

            Fitness += extraRecovery;

            if (Fitness > 100)
            {
                Fitness = 100;
            }

            if (Fitness < 0)
            {
                Fitness = 0;
            }
        }

        public void Injure(int rounds)
        {
            InjuryRounds = rounds;
        }

        public void DecreaseInjury()
        {
            if (InjuryRounds > 0)
            {
                InjuryRounds--;
            }
        }

        public void ScoreGoal()
        {
            Goals++;
        }
    }
}

