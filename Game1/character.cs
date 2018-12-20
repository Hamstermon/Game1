using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1
{
    public class Character
    {
        bool loadedSaveData = false;
        int characterID;
        public int CharacterID
        {
            set { characterID = value; }
            get { return characterID; }
        }
        int level = 1;
        public int Level
        {
            get { return level; }
        }
        int xp;
        public int XP
        {
            get { return xp; }
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
        int statPoints;
        public int StatUpgradePoints
        {
            get { return statPoints; }
        }
        int hpUP;
        public int HPUpgrade
        {
            get { return hpUP; }
        }
        int atkUP;
        public int AttackUpgrade
        {
            get { return atkUP; }
        }
        int defUP;
        public int DefenseUpgrade
        {
            get { return defUP; }
        }
        int magUP;
        public int MagicUpgrade
        {
            get { return magUP; }
        }
        int resUP;
        public int ResistanceUpgrade
        {
            get { return resUP; }
        }
        int spdUP;
        public int SpeedUpgrade
        {
            get { return spdUP; }
        }
        public void GainXP(int xpGained)
        {
            xp += xpGained;
            while (xp >= level * (level + 4))
            {
                xp -= level * (level + 4);
                level += 1;
            }
            statPoints = level - hpUP - atkUP - defUP - magUP - resUP - spdUP - 1;
        }
        public void UpgradeStat(string statName)
        {
            if (statPoints >= 1)
            {
                switch (statName)
                {
                    case "HP":
                        hpUP += 1;
                        break;
                    case "ATK":
                        atkUP += 1;
                        break;
                    case "DEF":
                        defUP += 1;
                        break;
                    case "MAG":
                        magUP += 1;
                        break;
                    case "RES":
                        resUP += 1;
                        break;
                    case "SPD":
                        spdUP += 1;
                        break;
                }
                statPoints = level - hpUP - atkUP - defUP - magUP - resUP - spdUP - 1;
            }
        }
        public void LoadData(int id, int lvl, int x, int chp, int cmp, string s1, string s2, string s3, int sp, int hup, int aup, int dup, int mup, int rup, int sup)
        {
            if (loadedSaveData == false)
            {
                characterID = id;
                level = lvl;
                xp = x;
                currentHP = chp;
                currentMP = cmp;
                skill1 = s1;
                skill2 = s2;
                skill3 = s3;
                statPoints = sp;
                hpUP = hup;
                atkUP = aup;
                defUP = dup;
                magUP = mup;
                resUP = rup;
                spdUP = sup;

                loadedSaveData = true;
            }
        }
    }
}
