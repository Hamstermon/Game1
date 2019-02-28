﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Squared.Tiled;
using Steropes.UI.Components;

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
    public class Fighter
    {
        string name = "";
        public string Name
        {
            set { name = value; }
            get { return name; }
        }
        int id = 0;
        public int ID
        {
            set { id = value; }
            get { return id; }
        }
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
        bool defending = false;
        public bool Defending
        {
            set
            {
                defending = value;
                if (value == true)
                {
                    currentMP = Convert.ToInt32(currentMP + mp * 0.2);
                }
            }
            get { return defending; }
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
            set
            {
                currentHP = value;
                if (currentHP > hp)
                    currentHP = hp;
                else if (currentHP < 0)
                    currentHP = 0;
            }
            get { return currentHP; }
        }
        int currentMP;
        public int CurrentMP
        {
            set
            {
                currentMP = value;
                if (currentMP > mp)
                    currentMP = mp;
                else if (currentMP < 0)
                    currentMP = 0;
            }
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
        int skill1;
        public int Skill1
        {
            set { skill1 = value; }
            get { return skill1; }
        }
        int skill2;
        public int Skill2
        {
            set { skill2 = value; }
            get { return skill2; }
        }
        int skill3;
        public int Skill3
        {
            set { skill3 = value; }
            get { return skill3; }
        }
    }
    public class BattleAction
    {
        public enum Command
        {
            Attack1,
            Attack2,
            Attack3,
            Defend,
            Move,
            Flee
        }
        public Command command;
        public bool[] target = new bool[5] { false, false, false, false, false };
    }


    public class Battle
    {
        public Fighter[] allies = new Fighter[5];
        public Fighter[] enemies = new Fighter[5];
        Game1 parent;
        MapWidget battlefield;
        BattleUI ui;
        public BattleAction playerAction;
        public bool canFlee = true;

        public List<Fighter> chars = new List<Fighter>();
        public List<Fighter> order = new List<Fighter>();
        public int turnNumber = 0;
        Random rng = new Random();

        public Battle(Game1 p)
        {
            parent = p;
            battlefield = p.play.battle;
            ui = p.play.battleUI;
        }
        
        public void AddFighter(Character c, Fighter[] team, int slot)
        {
            Fighter f = new Fighter();
            f.Name = parent.SearchChar(c.CharacterID).Name;
            f.Level = c.Level;
            f.ID = c.CharacterID;
            int[] stats = parent.CalculateStats(c);
            f.HP = stats[0];
            f.MP = stats[1];
            f.CurrentHP = c.CurrentHP;
            f.CurrentMP = c.CurrentMP;
            Console.WriteLine("STAT" + stats[0]);
            Console.WriteLine("HP" + f.HP);
            Console.WriteLine("CHP" + f.CurrentHP);
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
            f.Char = charObj;
            team[slot] = f;
        }

        public void AddFighter(OverworldEnemy c, Fighter[] team, int slot)
        {
            Fighter f = new Fighter();
            f.Name = parent.SearchChar(c.CharacterID).Name;
            f.Level = c.Level;
            f.ID = c.CharacterID;
            int[] stats = parent.CalculateStats(parent.SearchChar(c.CharacterID),c.Level);
            f.HP = stats[0];
            f.MP = stats[1];
            f.CurrentHP = stats[0];
            f.CurrentMP = stats[1];
            Console.WriteLine("STAT" + stats[0]);
            Console.WriteLine("HP" + f.HP);
            Console.WriteLine("CHP" + f.CurrentHP);
            f.ATK = stats[2];
            f.DEF = stats[3];
            f.MAG = stats[4];
            f.RES = stats[5];
            f.SPD = stats[6];
            int[] skills = parent.GetFirstSkills(c.CharacterID, c.Level);
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
            f.Char = charObj;
            team[slot] = f;
        }
        
        public void UpdateFighters()
        {
            for (int i = 0; i < 5; i++)
            {
                Fighter fighter = allies[i];
                if (fighter != null)
                {
                    Squared.Tiled.Object chr = fighter.Char;
                    string slotName = "";
                    int xOffset = 4;
                    slotName = "A" + i;
                    if (fighter.CurrentHP == 0)
                        xOffset = 21;
                    Squared.Tiled.Object slotObj = battlefield.CurrentMap.ObjectGroups["spots"].Objects[slotName];
                    int x = slotObj.X + slotObj.Width / 2;
                    int y = slotObj.Y + slotObj.Height / 2;
                    chr.X = x;
                    chr.Y = y;
                    chr.Texture = parent.GetCurrentFrame(chr, xOffset);
                }
                fighter = enemies[i];
                if (fighter != null)
                {
                    Squared.Tiled.Object chr = fighter.Char;
                    string slotName = "";
                    int xOffset = 4;
                    slotName = "B" + i;
                    xOffset = 12;
                    if (fighter.CurrentHP == 0)
                        xOffset = 27;
                    Squared.Tiled.Object slotObj = battlefield.CurrentMap.ObjectGroups["spots"].Objects[slotName];
                    int x = slotObj.X + slotObj.Width / 2;
                    int y = slotObj.Y + slotObj.Height / 2;
                    chr.X = x;
                    chr.Y = y;
                    chr.Texture = parent.GetCurrentFrame(chr, xOffset);
                }
            }
            ui.RefreshFighters();
        }

        public BattleAction Ai(Fighter i)
        {
            BattleAction action = new BattleAction();
            CharData data = parent.SearchChar(i.ID);
            if (data.BattleAI == "Default")
            {
                bool valid = false;
                do
                {
                    int skill = rng.Next(1, 3);
                    List<bool[]> selections;
                }
                while (valid == false);
            }
            return action;
        }

        public void TurnCycle()
        {
            Console.WriteLine("NEW TURN");
            chars = new List<Fighter>();
            order = new List<Fighter>();
            foreach (Fighter i in allies)
            {
                if (i != null)
                {
                    if (i.CurrentHP > 0)
                    {
                        Console.WriteLine("DED");
                        chars.Add(i);
                    }
                }
            }
            
            foreach (Fighter i in enemies)
            {
                if (i != null)
                {
                    if (i.CurrentHP > 0)
                    {
                        Console.WriteLine("DED");
                        chars.Add(i);
                    }
                }
            }
            Console.WriteLine("b4");
            Console.WriteLine(chars.Count);
            Console.WriteLine(order.Count);
            do
            {
                Fighter fastest = new Fighter();
                foreach (Fighter x in chars)
                {
                    if (x.SPD >= fastest.SPD)
                    {
                        fastest = x;
                    }
                }
                order.Add(fastest);
                chars.Remove(fastest);
            }
            while (chars.Count > 0);
            Console.WriteLine("");
            Console.WriteLine(chars.Count);
            Console.WriteLine(order.Count);
            turnNumber = 0;
        }

        public void Turn(Fighter fighter, BattleAction action)
        {
            bool successful = true;
            bool[] selection = ui.commands.selection;
            ui.commands.selection = new bool[5] { false, false, false, false, false };
            ui.commands.UpdateSelection();
            fighter.Defending = false;
            Fighter[] team = GetTeam(fighter);
            Fighter[] enemyTeam;
            Attack atkData;
            if (team == allies)
                enemyTeam = enemies;
            else
                enemyTeam = allies;
            switch (action.command)
            {
                case BattleAction.Command.Attack1:
                    atkData = parent.SearchAttack(fighter.Skill1);
                    Attack(fighter, enemyTeam, selection, atkData);
                    break;
                case BattleAction.Command.Attack2:
                    atkData = parent.SearchAttack(fighter.Skill2);
                    Attack(fighter, enemyTeam, selection, atkData);
                    break;
                case BattleAction.Command.Attack3:
                    atkData = parent.SearchAttack(fighter.Skill3);
                    Attack(fighter, enemyTeam, selection, atkData);
                    break;
                case BattleAction.Command.Defend:
                    fighter.Defending = true;
                    break;
                case BattleAction.Command.Move:
                    successful = false;
                    int newPos = 0;
                    for (int i = 0; i < 5; i++)
                    {
                        if (action.target[i] == true)
                        {
                            newPos = i;
                            break;
                        }
                    }
                    Fighter other = team[newPos];
                    if (other == null)
                    {
                        int pos = GetPosOfFighter(fighter);
                        team[pos] = null;
                        team[newPos] = fighter;
                    }
                    break;
                case BattleAction.Command.Flee:
                    if (canFlee)
                    {
                        EndBattle(true);
                    }
                    break;
            }
            UpdateFighters();
            if (successful)
            {
                turnNumber++;
                if (turnNumber >= order.Count)
                {
                    TurnCycle();
                }
            }
        }

        public void Attack(Fighter attacker, Fighter[] target, bool[] selection, Attack data)
        {
            int attack = 0;
            if (data.Category == "Physical")
                attack = attacker.ATK;
            else if (data.Category == "Magical")
                attack = attacker.MAG;
            bool fullPower = true;
            int power = data.Power;
            if (attacker.CurrentMP < data.MP)
            {
                power = power * (attacker.CurrentMP / data.MP);
                fullPower = false;
            }
            attacker.CurrentMP = attacker.CurrentMP - data.MP;
            for (int i = 0; i < 5; i++)
            {
                Fighter defender = target[i];
                if (selection[i] == true && defender != null)
                {
                    int defense = 0;
                    if (data.Category == "Physical")
                        defense = defender.DEF;
                    else if (data.Category == "Magical")
                        defense = defender.RES;
                    bool hit = true;
                    int damage = 0;
                    if (rng.Next(1, 1000) > data.Accuracy * 10)
                        hit = false;
                    if (hit)
                    {
                        damage = attack * data.Power / defense;
                        defender.CurrentHP = defender.CurrentHP - damage;
                    }
                    if (fullPower)
                    {
                        Effect(attacker, defender, data.Effect1Name, data.Effect1Chance, damage);
                        Effect(attacker, defender, data.Effect2Name, data.Effect2Chance, damage);
                    }
                }
            }
        }

        public void Effect(Fighter attacker, Fighter defender, string effect, int chance, int damage)
        {
            bool hit = true;
            if (rng.Next(1, 1000) > chance * 10)
                hit = false;
            if (hit)
            {

            }
        }

        public int GetPosOfFighter(Fighter fighter)
        {
            int pos = 0;
            for (int i = 0; i < 5; i++)
            {
                if (allies[i] == fighter || enemies[i] == fighter)
                {
                    pos = i;
                    break;
                }
            }
            return pos;
        }

        public Fighter[] GetTeam(Fighter fighter)
        {
            Fighter[] team = new Fighter[5];
            for (int i = 0; i < 5; i++)
            {
                if (allies[i] == fighter)
                {
                    team = allies;
                    break;
                }
                else if (enemies[i] == fighter)
                {
                    team = enemies;
                    break;
                }
            }
            return team;
        }

        public void EndBattle(bool fled)
        {
            parent.State = Game1.GameState.Playing;
            if (!fled)
            {

            }
            for (int i = 0; i < 5; i++)
            {
                Fighter fighter = allies[i];
                if (fighter != null)
                {
                    fighter.Character.CurrentHP = fighter.CurrentHP;
                    fighter.Character.CurrentMP = fighter.CurrentMP;
                }
            }
            parent.play.Remove(parent.play.battle);
            parent.play.Remove(parent.play.battleUI);
            parent.play.Add(parent.play.mapWidget);
            parent.pause = false;
        }
    }
}
