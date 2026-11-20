public class Player
{
    public string Name { get; set; }
    public int Strength { get; set; }
    public int Fitness { get; set; } = 100;
    public Position Position { get; set; }
    public bool IsStarter { get; set; }
    public bool IsJunior { get; set; } = false;

    public int InjuryRounds { get; set; } = 0;
    public bool IsInjured => InjuryRounds > 0;

    public int Goals { get; set; } = 0;

    public double EffectiveStrength => Strength * (Fitness / 100.0);

    public int Price => (Strength - 40) * 5;
}