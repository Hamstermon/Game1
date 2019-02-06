using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Squared.Tiled;

namespace Game1
{
    public enum Status
    {
        None,
        Poison,
        Burn,
        Paralyzed,
        Sleep,
        Frozen,
        Confused
    }
    class Fighter
    {
        Character character;
        public Character Character
        {
            set { character = value; }
            get { return character; }
        }
        Status status = Status.None;
        public Status Status
        {
            set { status = value; }
            get { return status; }
        }
        Squared.Tiled.Object chr;
        public Squared.Tiled.Object Char
        {
            set { chr = value; }
            get { return chr; }
        }
        int time;
        public int StatusDuration
        {
            set { time = value; }
            get { return time; }
        }
        int lvl;
        public int Level
        {
            set { lvl = value; }
            get { return lvl; }
        }
        int currentHP;
        public int CurrentHP
        {
            set { currentHP = value; }
            get { return currentHP; }
        }
        int currentMP;
        public int CurrentMP
        {
            set { currentMP = value; }
            get { return currentMP; }
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
            get { return Convert.ToInt32(atk * Math.Pow(1.5, atkStage)); }
        }
        int def = 0;
        public int DEF
        {
            set { def = value; }
            get { return Convert.ToInt32(def * Math.Pow(1.5, defStage)); }
        }
        int mag = 0;
        public int MAG
        {
            set { mag = value; }
            get { return Convert.ToInt32(mag * Math.Pow(1.5, magStage)); }
        }
        int res = 0;
        public int RES
        {
            set { res = value; }
            get { return Convert.ToInt32(res * Math.Pow(1.5, resStage)); }
        }
        int spd = 0;
        public int SPD
        {
            set { spd = value; }
            get { return Convert.ToInt32(spd * Math.Pow(1.5, spdStage)); }
        }
        int atkStage = 0;
        public int ATKSTAGE
        {
            set
            {
                atkStage = value;
                if (atkStage > 3)
                    atkStage = 3;
                else if (atkStage < -3)
                    atkStage = -3;
            }
            get { return atkStage; }
        }
        int defStage = 0;
        public int DEFSTAGE
        {
            set
            {
                defStage = value;
                if (defStage > 3)
                    defStage = 3;
                else if (defStage < -3)
                    defStage = -3;
            }
            get { return defStage; }
        }
        int magStage = 0;
        public int MAGSTAGE
        {
            set
            {
                magStage = value;
                if (magStage > 3)
                    magStage = 3;
                else if (magStage < -3)
                    magStage = -3;
            }
            get { return magStage; }
        }
        int resStage = 0;
        public int RESSTAGE
        {
            set
            {
                resStage = value;
                if (resStage > 3)
                    resStage = 3;
                else if (resStage < -3)
                    resStage = -3;
            }
            get { return resStage; }
        }
        int spdStage = 0;
        public int SPDSTAGE
        {
            set
            {
                spdStage = value;
                if (spdStage > 3)
                    spdStage = 3;
                else if (spdStage < -3)
                    spdStage = -3;
            }
            get { return spdStage; }
        }
        string skill1;
        public string Skill1
        {
            set { skill1 = value; }
            get { return skill1; }
        }
        string skill2;
        public string Skill2
        {
            set { skill2 = value; }
            get { return skill2; }
        }
        string skill3;
        public string Skill3
        {
            set { skill3 = value; }
            get { return skill3; }
        }
    }

    class Battle
    {
        public Fighter[] allies = new Fighter[5];
        public Fighter[] enemies = new Fighter[5];
        Game1 parent;
        MapWidget battlefield;

        public Battle(Game1 p)
        {
            parent = p;
            battlefield = p.play.battle;
        }
        
        public void AddFighter(Character c, Fighter[] team, int slot)
        {
            Fighter f = new Fighter();
            f.CurrentHP = c.CurrentHP;
            f.CurrentMP = c.CurrentMP;
            int[] stats = parent.CalculateStats(c);
            f.HP = stats[0];
            f.MP = stats[1];
            f.ATK = stats[2];
            f.DEF = stats[3];
            f.MAG = stats[4];
            f.RES = stats[5];
            f.SPD = stats[6];
            f.Skill1 = c.Skill1;
            f.Skill2 = c.Skill2;
            f.Skill3 = c.Skill3;
            f.Character = c;
            CharData info = parent.SearchChar(c.CharacterID);
            int size = 32;
            if (info.SpriteSheet == "characterSpritesheetLarge")
            {
                size = 64;
            }
            string slotName = "";
            int xOffset = 4;
            if (team == allies)
                slotName = "A" + slot;
            else
            {
                slotName = "B" + slot;
                xOffset = 12;
            }
            Squared.Tiled.Object slotObj = battlefield.CurrentMap.ObjectGroups["spots"].Objects[slotName];
            int x = slotObj.X + slotObj.Width / 2;
            int y = slotObj.Y + slotObj.Height / 2;
            Squared.Tiled.Object charObj = parent.CreateObject(info.Name,info.SpriteSheet,info.SpriteIndex,x,y,size,size);
            battlefield.CurrentMap.ObjectGroups["5objects"].Objects.Add(info.Name + slotName, charObj);
            parent.GetCurrentFrame(charObj, xOffset);
            team[slot] = f;
        }

        public void AddFighter(OverworldEnemy c, Fighter[] team, int slot)
        {
            Fighter f = new Fighter();
            int[] stats = parent.CalculateStats(parent.SearchChar(c.CharacterID),c.Level);
            f.CurrentHP = stats[0];
            f.CurrentMP = stats[1];
            f.HP = stats[0];
            f.MP = stats[1];
            f.ATK = stats[2];
            f.DEF = stats[3];
            f.MAG = stats[4];
            f.RES = stats[5];
            f.SPD = stats[6];
            string[] skills = parent.GetFirstSkills(c.CharacterID, c.Level);
            f.Skill1 = skills[0];
            f.Skill2 = skills[1];
            f.Skill3 = skills[2];
            CharData info = parent.SearchChar(c.CharacterID);
            int size = 32;
            if (info.SpriteSheet == "characterSpritesheetLarge")
            {
                size = 64;
            }
            string slotName = "";
            int xOffset = 4;
            if (team == allies)
                slotName = "A" + slot;
            else
            {
                slotName = "B" + slot;
                xOffset = 12;
            }
            Squared.Tiled.Object slotObj = battlefield.CurrentMap.ObjectGroups["spots"].Objects[slotName];
            int x = slotObj.X + slotObj.Width / 2;
            int y = slotObj.Y + slotObj.Height / 2;
            Squared.Tiled.Object charObj = parent.CreateObject(info.Name, info.SpriteSheet, info.SpriteIndex, x, y, size, size);
            battlefield.CurrentMap.ObjectGroups["5objects"].Objects.Add(info.Name + slotName, charObj);
            parent.GetCurrentFrame(charObj, xOffset);
            team[slot] = f;
        }
    }
}
