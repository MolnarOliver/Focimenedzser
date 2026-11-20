using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballManager
{
    public class Player
    {
        private string _name;
        private Position _position;
        private int _strength;
        private int _fitness;

        public string Name { get { return _name; } set { _name = value; } }

        public Position Position { get { return _position; } set { _position = value; } }

        public int Strength { get { return _strength; } set { _strength = value; } }

        public int Fitness { get { return _fitness; } }

        public int EffectiveStrength { get { return _strength * (100 + _fitness) / 200; } }

        public Player(string name, Position position, int strength)
        {
            _name = name;
            _position = position;
            _strength = strength;
            _fitness = 100;
        }

        public void UpdateFitnessAfterRound(bool wasStarter)
        {
            if (wasStarter)
            {
                _fitness -= 6;
                if (_fitness < 0)
                {
                    _fitness = 0;
                }
            }
            else
            {
                _fitness += 15;
                if (_fitness > 100)
                {
                    _fitness = 100;
                }
            }
        }
    }
}