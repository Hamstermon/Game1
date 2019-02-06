using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1
{
    public class PlayerSaveData
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

        int gold = 0;
        public int Gold
        {
            set { gold = value; }
            get { return gold; }
        }

        int mapid = 0;
        public int MapID
        {
            set { mapid = value; }
            get { return mapid; }
        }

        List<Character> characters = new List<Character>();
        public List<Character> CharacterList
        {
            set { characters = value; }
            get { return characters; }
        }

        int[] party = new int[5];
        public int[] Party
        {
            set { party = value; }
            get { return party; }
        }

        List<Item> inventory = new List<Item>();
        public List<Item> Inventory
        {
            set { inventory = value; }
            get { return inventory; }
        }

        List<Event> events = new List<Event>();
        public List<Event> Events
        {
            set { events = value; }
            get { return events; }
        }
    }
}
