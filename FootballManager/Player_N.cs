using System;

namespace FootballManager
{
    public class Player_N
    {
        public string Name { get; set; }
        public PlayerPosition Position { get; set; }
        public int Strength { get; set; }
        public int Fitness { get; private set; }
        public int InjuryRounds { get; private set; }
        public int Goals { get; private set; }
        public bool IsJunior { get; private set; }

        public bool IsInjured { get { return InjuryRounds > 0; } }
        public int EffectiveStrength { get { return Strength * (100 + Fitness) / 200; } }

        public int AttackValue { get { return Position.GetAttack(EffectiveStrength); } }
        public int DefenseValue { get { return Position.GetDefense(EffectiveStrength); } }

        public int Price{ get { int calc = (Strength - 40) * 5; return calc < 0 ? 0 : calc; } }

        public Player_N(string name, PlayerPosition position, int strength, bool isJunior = false)
        {
            Name = name;
            Position = position;
            Strength = strength;
            Fitness = 100;
            IsJunior = isJunior;
        }

        public void UpdateFitnessAfterRound(bool wasStarter, int starterLoss, int extraRecovery)
        {
            Fitness += wasStarter ? -starterLoss : 15;
            Fitness += extraRecovery;
            if (Fitness > 100) Fitness = 100;
            if (Fitness < 0) Fitness = 0;
        }

        public void Injure(int rounds) { InjuryRounds = rounds; }
        public void DecreaseInjury() { if (InjuryRounds > 0) InjuryRounds--; }
        public void ScoreGoal() { Goals++; }
    }
}
