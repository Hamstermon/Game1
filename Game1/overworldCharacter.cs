using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1
{
    public class OverworldChar
    {
        int direction = 2;
        public int Direction
        {
            set
            {
                if (direction >= 0 && direction <= 3)
                    direction = value;
            }
            get { return direction; }
        }

        int[] position = { 0, 0 };
        public int[] Position
        {
            set { position = value; }
            get { return position; }
        }

        Character[] party = new Character[5];
        public Character[] Party
        {
            set { party = value; }
            get { return party; }
        }

        int walkSpeed = 4;
        public int WalkSpeed
        {
            set { walkSpeed = value; }
            get { return walkSpeed; }
        }
    }

    public class OverworldEnemy : OverworldChar
    {
        string aiType = "Default";
        public string AIType
        {
            set { aiType = value; }
            get { return aiType; }
        }
        Squared.Tiled.Object character;
        public Squared.Tiled.Object Character
        {
            set { character = value; }
            get { return character; }
        }
    }

    public class OverworldPlayer : OverworldChar
    {
        int gold = 0;
        public int Gold
        {
            set { gold = value; }
            get { return gold; }
        }
        
    }
}
