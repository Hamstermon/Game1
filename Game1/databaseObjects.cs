using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1
{
    public class Attack
    {
        int attackID = 0;
        public int AttackID
        {
            set { attackID = value; }
            get { return attackID; }
        }

        string name = "";
        public string Name
        {
            set { name = value; }
            get { return name; }
        }

        string type = "";
        public string Type
        {
            set { type = value; }
            get { return type; }
        }

        string category = "";
        public string Category
        {
            set { category = value; }
            get { return category; }
        }

        string zones = "OOXOO";
        public string Zones
        {
            set { zones = value; }
            get { return zones; }
        }

        bool fix = false;
        public bool Fixed
        {
            set { fix = value; }
            get { return fix; }
        }

        int power = 0;
        public int Power
        {
            set { power = value; }
            get { return power; }
        }

        int accuracy = 0;
        public int Accuracy
        {
            set { accuracy = value; }
            get { return accuracy; }
        }

        int priority = 0;
        public int Priority
        {
            set { priority = value; }
            get { return priority; }
        }

        int mp = 0;
        public int MP
        {
            set { mp = value; }
            get { return mp; }
        }

        string effect1Name = "";
        public string Effect1Name
        {
            set { effect1Name = value; }
            get { return effect1Name; }
        }

        int effect1Chance = 0;
        public int Effect1Chance
        {
            set { effect1Chance = value; }
            get { return effect1Chance; }
        }

        string effect2Name = "";
        public string Effect2Name
        {
            set { effect2Name = value; }
            get { return effect2Name; }
        }

        int effect2Chance = 0;
        public int Effect2Chance
        {
            set { effect2Chance = value; }
            get { return effect2Chance; }
        }

        string description = "";
        public string Description
        {
            set { description = value; }
            get { return description; }
        }
    }
    public class CharData
    {
        int charID = 0;
        public int CharID
        {
            set { charID = value; }
            get { return charID; }
        }
        string name = "";
        public string Name
        {
            set { name = value; }
            get { return name; }
        }
        string spriteSheet = "";
        public string SpriteSheet
        {
            set { spriteSheet = value; }
            get { return spriteSheet; }
        }
        int spriteIndex = 0;
        public int SpriteIndex
        {
            set { spriteIndex = value; }
            get { return spriteIndex; }
        }
        int xp = 0;
        public int XP
        {
            set { xp = value; }
            get { return xp; }
        }
        string type1 = "";
        public string Type1
        {
            set { type1 = value; }
            get { return type1; }
        }
        string type2 = "";
        public string Type2
        {
            set { type2 = value; }
            get { return type2; }
        }
        string type3 = "";
        public string Type3
        {
            set { type3 = value; }
            get { return type3; }
        }
        int hp = 0;
        public int HP
        {
            set { hp = value; }
            get { return hp; }
        }
        int mp = 0;
        public int MP
        {
            set { mp = value; }
            get { return mp; }
        }
        int atk = 0;
        public int ATK
        {
            set { atk = value; }
            get { return atk; }
        }
        int def = 0;
        public int DEF
        {
            set { def = value; }
            get { return def; }
        }
        int mag = 0;
        public int MAG
        {
            set { mag = value; }
            get { return mag; }
        }
        int res = 0;
        public int RES
        {
            set { res = value; }
            get { return res; }
        }
        int spd = 0;
        public int SPD
        {
            set { spd = value; }
            get { return spd; }
        }
        string overworldAI = "";
        public string OverworldAI
        {
            set { overworldAI = value; }
            get { return overworldAI; }
        }
        int passiveSpeed = 0;
        public int PassiveSpeed
        {
            set { passiveSpeed = value; }
            get { return passiveSpeed; }
        }
        int agressiveSpeed = 0;
        public int AgressiveSpeed
        {
            set { agressiveSpeed = value; }
            get { return agressiveSpeed; }
        }
        string battleAI = "";
        public string BattleAI
        {
            set { battleAI = value; }
            get { return battleAI; }
        }
    }
    public class MapData
    {
        int mapID = 0;
        public int MapID
        {
            set { mapID = value; }
            get { return mapID; }
        }
        string name = "";
        public string Name
        {
            set { name = value; }
            get { return name; }
        }
        string mapFileName = "";
        public string MapFileName
        {
            set { mapFileName = value; }
            get { return mapFileName; }
        }
        string battleFileName = "";
        public string BattleFileName
        {
            set { battleFileName = value; }
            get { return battleFileName; }
        }
        int spawnCap = 0;
        public int SpawnCap
        {
            set { spawnCap = value; }
            get { return spawnCap; }
        }
    }
    public class MapChar
    {
        int id = 0;
        public int ID
        {
            set { id = value; }
            get { return id; }
        }
        int mapID = 0;
        public int MapID
        {
            set { mapID = value; }
            get { return mapID; }
        }
        int charID = 0;
        public int CharID
        {
            set { charID = value; }
            get { return charID; }
        }
        int weight = 0;
        public int Weight
        {
            set { weight = value; }
            get { return weight; }
        }
        int minLevel = 0;
        public int MinLevel
        {
            set { minLevel = value; }
            get { return minLevel; }
        }
        int maxLevel = 0;
        public int MaxLevel
        {
            set { maxLevel = value; }
            get { return maxLevel; }
        }
    }
    public class CharAttack
    {
        int id = 0;
        public int ID
        {
            set { id = value; }
            get { return id; }
        }
        int charID = 0;
        public int CharID
        {
            set { charID = value; }
            get { return charID; }
        }
        int attackID = 0;
        public int AttackID
        {
            set { attackID = value; }
            get { return attackID; }
        }
        int level = 0;
        public int Level
        {
            set { level = value; }
            get { return level; }
        }
    }
    public class Dialog
    {
        string name, chrName, text, oName1, oName2, oName3, oNext1, oNext2, oNext3, oEvent1, oEvent2, oEvent3;
        public string Name
        {
            set { name = value; }
            get { return name; }
        }
        public string CharName
        {
            set { chrName = value; }
            get { return chrName; }
        }
        public string Text
        {
            set { text = value; }
            get { return text; }
        }

        public string OptionName1
        {
            set { oName1 = value; }
            get { return oName1; }
        }
        public string OptionNext1
        {
            set { oNext1 = value; }
            get { return oNext1; }
        }
        public string OptionEvent1
        {
            set { oEvent1 = value; }
            get { return oEvent1; }
        }

        public string OptionName2
        {
            set { oName2 = value; }
            get { return oName2; }
        }
        public string OptionNext2
        {
            set { oNext2 = value; }
            get { return oNext2; }
        }
        public string OptionEvent2
        {
            set { oEvent2 = value; }
            get { return oEvent2; }
        }

        public string OptionName3
        {
            set { oName3 = value; }
            get { return oName3; }
        }
        public string OptionNext3
        {
            set { oNext3 = value; }
            get { return oNext3; }
        }
        public string OptionEvent3
        {
            set { oEvent3 = value; }
            get { return oEvent3; }
        }
    }
}
