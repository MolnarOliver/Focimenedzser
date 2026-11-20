using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballManager
{
    public abstract class PlayerPosition
    {
        public abstract string Name { get; }
        public abstract int GoalWeight { get; }

        public abstract int GetAttack(int effectiveStrength);
        public abstract int GetDefense(int effectiveStrength);

        public virtual bool IsGoalkeeper { get { return false; } }

        public static PlayerPosition FromString(string text)
        {
            switch (text.Trim().ToLower())
            {
                case "kapus":
                    return new GoalkeeperPosition();
                case "védő":
                    return new DefenderPosition();
                case "középpályás":
                    return new MidfielderPosition();
                case "csatár":
                    return new ForwardPosition();
                default:
                    return null;
            }
        }
    }

    public class GoalkeeperPosition : PlayerPosition
    {
        public override string Name { get { return "Kapus"; } }
        public override int GoalWeight { get { return 1; } }
        public override bool IsGoalkeeper { get { return true; } }

        public override int GetAttack(int effectiveStrength)
        {
            return 0;
        }

        public override int GetDefense(int effectiveStrength)
        {
            return effectiveStrength * 2;
        }
    }

    public class DefenderPosition : PlayerPosition
    {
        public override string Name { get { return "Védő"; } }
        public override int GoalWeight { get { return 1; } }

        public override int GetAttack(int effectiveStrength)
        {
            return effectiveStrength / 2;
        }

        public override int GetDefense(int effectiveStrength)
        {
            return (effectiveStrength * 3) / 2;
        }
    }

    public class MidfielderPosition : PlayerPosition
    {
        public override string Name { get { return "Középpályás"; } }
        public override int GoalWeight { get { return 2; } }

        public override int GetAttack(int effectiveStrength)
        {
            return effectiveStrength;
        }

        public override int GetDefense(int effectiveStrength)
        {
            return effectiveStrength;
        }
    }

    public class ForwardPosition : PlayerPosition
    {
        public override string Name { get { return "Csatár"; } }
        public override int GoalWeight { get { return 3; } }

        public override int GetAttack(int effectiveStrength)
        {
            return (effectiveStrength * 3) / 2;
        }

        public override int GetDefense(int effectiveStrength)
        {
            return effectiveStrength / 2;
        }
    }
}