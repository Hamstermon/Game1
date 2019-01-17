using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1
{
    public class Character
    {
        int characterID;
        public int CharacterID
        {
            set { characterID = value; }
            get { return characterID; }
        }
        int level = 1;
        public int Level
        {
            set { level = value; }
            get { return level; }
        }
        int xp;
        public int XP
        {
            set { xp = value; }
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
        bool available;
        public bool Available
        {
            set { available = value; }
            get { return available; }
        }
        
        public void GainXP(int xpGained)
        {
            xp += xpGained;
            while (xp >= level * (level + 4))
            {
                xp -= level * (level + 4);
                level += 1;
            }
        }
    }
}
