using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballManager
{
    public interface ITactic
    {
        string Name { get; }
        int OwnChanceModifier { get; }
        int OpponentChanceModifier { get; }
    }

    public class AttackingTactic : ITactic
    {
        public string Name { get { return "Támadó"; } }
        public int OwnChanceModifier { get { return 5; } }
        public int OpponentChanceModifier { get { return 5; } }
    }

    public class BalancedTactic : ITactic
    {
        public string Name { get { return "Kiegyensúlyozott"; } }
        public int OwnChanceModifier { get { return 0; } }
        public int OpponentChanceModifier { get { return 0; } }
    }

    public class DefensiveTactic : ITactic
    {
        public string Name { get { return "Védekező"; } }
        public int OwnChanceModifier { get { return -3; } }
        public int OpponentChanceModifier { get { return -6; } }
    }
}