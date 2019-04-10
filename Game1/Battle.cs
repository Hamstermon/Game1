using System;
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
    public enum BattleResult
    {
        None,
        PlayerWin,
        EnemyWin
    }
    public enum BattleState
    {
        Regular,
        Animation
    }
    public enum PostAnimation
    {
        Attack,
        Done,
        End,
        TrueEnd
    }
    public enum AnimationType
    {
        Hit,
        Fire,
        Water,
        Leaf,
        Electric
    }
    public enum CharAnimation
    {
        None,
        Physical,
        Ranged,
        Happy,
        Hit
    }
    public class BattleAnimation
    {
        AnimationType skillA;
        public AnimationType SkillAnimation
        {
            get { return skillA; }
            set { skillA = value; }
        }
        CharAnimation charA;
        public CharAnimation CharacterAnimation
        {
            get { return charA; }
            set { charA = value; }
        }
        CharAnimation targA;
        public CharAnimation TargetAnimation
        {
            get { return targA; }
            set { targA = value; }
        }
        bool[] selection = new bool[5] { false, false, false, false, false };
        public bool[] Selection
        {
            set { selection = value; }
            get { return selection; }
        }
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
                    CurrentMP = Convert.ToInt32(CurrentMP + mp * 0.2);
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

        public Fighter tempFighter; //fighter, enemyTeam, selection, atkData
        public Fighter[] tempEnemyTeam;
        public bool[] tempSelection;
        public Attack tempAtkData;
        public string dialog;
        public string npc;
        public BattleState state = BattleState.Regular;
        public PostAnimation postAnimation = PostAnimation.Done;

        List<BattleAnimation> animation;
        public int currentFrameS = 0;
        public int currentFrameC = 0;
        public int currentKeyFrame = 0;
        public int elapsedTime = 0;
        public bool incrementS = false;
        public List<BattleAnimation> Animation
        {
            set
            {
                animation = value;
                currentFrameS = 0;
                currentFrameC = 0;
                currentKeyFrame = 0;
                elapsedTime = 0;
                Console.WriteLine("new animation");
            }
            get { return animation; }
        }

        public Battle(Game1 p, string d)
        {
            parent = p;
            battlefield = p.play.battle;
            ui = p.play.battleUI;
            dialog = d;
            npc = p.npcName;
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
            f.HP = stats[0] + c.BonusStats[0];
            f.MP = stats[1] + c.BonusStats[1];
            f.CurrentHP = stats[0] + c.BonusStats[0];
            f.CurrentMP = stats[1] + c.BonusStats[1];
            Console.WriteLine("STAT" + stats[0]);
            Console.WriteLine("HP" + f.HP);
            Console.WriteLine("CHP" + f.CurrentHP);
            f.ATK = stats[2] + c.BonusStats[2];
            f.DEF = stats[3] + c.BonusStats[3];
            f.MAG = stats[4] + c.BonusStats[4];
            f.RES = stats[5] + c.BonusStats[5];
            f.SPD = stats[6] + c.BonusStats[6];
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

        public (bool[],bool,bool) LoadSelection(Attack atk, Fighter[] team)
        {
            bool fixedSelection = atk.Fixed;
            bool[] selection = new bool[5] { false, false, false, false, false };
            bool selectAlly = false;
            Fighter[] targets;
            if (team == allies)
            {
                if (atk.Power >= 0)
                    targets = enemies;
                else
                {
                    targets = allies;
                    selectAlly = true;
                }
            }
            else
            {
                if (atk.Power >= 0)
                    targets = allies;
                else
                {
                    targets = enemies;
                    selectAlly = true;
                }
            }
            for (int i = 0; i < 5; i++)
            {
                string letter = atk.Zones.Substring(i, 1);
                if (letter == "0")
                    selection[i] = false;
                else if (letter == "X")
                    selection[i] = true;
            }
            if (!fixedSelection)
            {
                List<bool[]> preSelects = GetAllPositions(selection,targets);
                if (preSelects.Count > 0)
                {
                    bool[] best = preSelects[0];
                    int count = 0;
                    foreach (bool[] s in preSelects)
                    {
                        int myCount = 0;
                        for (int i = 0; i < 5; i++)
                        {
                            if (s[i] == true && targets[i] != null && targets[i].CurrentHP > 0)
                            {
                                myCount++;
                            }
                        }
                        if (myCount > count)
                        {
                            best = s;
                            count = myCount;
                        }
                    }
                    selection = best;
                }
            }
            return (selection,fixedSelection,selectAlly);
        }
        public List<bool[]> GetAllPositions(bool[] selection, Fighter[] targets)
        {
            List<bool[]> preSelects = new List<bool[]>();
            bool[] s1 = new bool[5] { selection[0], selection[1], selection[2], selection[3], selection[4] };
            bool[] s2 = new bool[5] { selection[0], selection[1], selection[2], selection[3], selection[4] };
            bool[] s3 = new bool[5] { selection[0], selection[1], selection[2], selection[3], selection[4] };
            bool[] s4 = new bool[5] { selection[0], selection[1], selection[2], selection[3], selection[4] };
            bool[] s5 = new bool[5] { selection[0], selection[1], selection[2], selection[3], selection[4] };
            s2 = ui.commands.MoveSelection(s2, true);
            s3 = ui.commands.MoveSelection(s3, false);
            s4 = ui.commands.MoveSelection(ui.commands.MoveSelection(s4, true), true);
            s5 = ui.commands.MoveSelection(ui.commands.MoveSelection(s5, false), false);
            preSelects.Add(s1);
            if (s2 != s1)
                preSelects.Add(s2);
            if (s3 != s2 && s3 != s1)
                preSelects.Add(s3);
            if (s4 != s3 && s4 != s2 && s4 != s1)
                preSelects.Add(s4);
            if (s5 != s4 && s5 != s3 && s5 != s2 && s5 != s1)
                preSelects.Add(s5);
            List<bool[]> toRemove = new List<bool[]>();
            foreach (bool[] s in preSelects)
            {
                bool exist = false;
                for (int i = 0; i < 5; i++)
                {
                    if (s[i] == true && targets[i] != null && targets[i].CurrentHP > 0)
                    {
                        exist = true;
                        break;
                    }
                }
                if (!exist)
                    toRemove.Add(s);
            }
            foreach(bool[] s in toRemove)
            {
                preSelects.Remove(s);
            }
            return preSelects;
        }

        public BattleAction Ai(Fighter i)
        {
            Console.WriteLine("AI THINK");
            BattleAction action = new BattleAction();
            CharData data = parent.SearchChar(i.ID);
            bool[] select = new bool[5] { false, false, false, false, false };
            if (data.BattleAI == "Default")
            {
                //bool valid = false;
                do
                {
                    int skill = 1;
                    bool[] mp = new bool[3] { true, true, true };
                    bool[] selection;
                    bool fixedSelection;
                    bool selectAlly;
                    Attack atk = new Attack();
                    do
                    {
                        skill = rng.Next(0, 399);
                        if (skill%4 == 0)
                        {
                            atk = parent.SearchAttack(i.Skill1);
                            action.command = BattleAction.Command.Attack1;
                        }
                        else if (skill%4 == 1)
                        {
                            atk = parent.SearchAttack(i.Skill2);
                            action.command = BattleAction.Command.Attack2;
                        }
                        else if (skill % 4 == 2)
                        {
                            atk = parent.SearchAttack(i.Skill3);
                            action.command = BattleAction.Command.Attack3;
                        }
                        else
                        {
                            if ((float)i.CurrentMP < (float)i.MP * 0.5)
                            {
                                action.command = BattleAction.Command.Defend;
                                break;
                            }
                        }
                        if (skill % 4 != 3 && atk.MP > i.CurrentMP)
                        {
                            atk = new Attack();
                            mp[skill%4] = false;
                        }
                    }
                    while (atk.Name == "" || mp == new bool[3] { false, false, false });
                    if (mp == new bool[3] { false, false, false })
                    {
                        action.command = BattleAction.Command.Defend;
                    }
                    else if (action.command != BattleAction.Command.Defend)
                    {
                        (selection, fixedSelection, selectAlly) = LoadSelection(atk, enemies);
                        List<bool[]> selections;
                        if (!fixedSelection)
                        {
                            if (atk.Power > 0)
                                selections = GetAllPositions(selection, allies);
                            else
                                selections = GetAllPositions(selection, enemies);
                        }
                        else
                        {
                            selections = new List<bool[]>();
                            selections.Add(selection);
                        }
                        if (selections.Count > 0)
                        {
                            bool[] temp = selections[rng.Next(0, selections.Count)];
                            select[0] = temp[0];
                            select[1] = temp[1];
                            select[2] = temp[2];
                            select[3] = temp[3];
                            select[4] = temp[4];
                            //Console.WriteLine("CHOSEN SELECTION:" + select[0] + select[1] + select[2] + select[3] + select[4]);
                            break;
                        }
                    }
                }
                while (true);
                action.target = select;
            }
            //Console.WriteLine("CHOSEN SELECTION:" + select[0] + select[1] + select[2] + select[3] + select[4]);
            //Console.WriteLine("CHOSEN TARGET:" + action.target[0] + action.target[1] + action.target[2] + action.target[3] + action.target[4]);
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
                        chars.Add(i);
                    }
                }
            }
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
            turnNumber = 0;
        }

        public void Turn(Fighter fighter, BattleAction action)
        {
            bool successful = true;
            bool[] selection = action.target;
            ui.commands.selection = new bool[5] { false, false, false, false, false };
            ui.commands.UpdateSelection();
            fighter.Defending = false;
            Fighter[] team = GetTeam(fighter);
            Fighter[] enemyTeam;
            Attack atkData;
            if (team == allies)
            {
                if (ui.commands.selectAlly)
                    enemyTeam = allies;
                else
                    enemyTeam = enemies;
            }
            else
            {
                if (ui.commands.selectAlly)
                    enemyTeam = enemies;
                else
                    enemyTeam = allies;
            }
            switch (action.command)
            {
                case BattleAction.Command.Attack1:
                    atkData = parent.SearchAttack(fighter.Skill1);
                    ui.Message(fighter.Name + " used " + atkData.Name);
                    AttackAnimation(fighter, enemyTeam, selection, atkData);
                    break;
                case BattleAction.Command.Attack2:
                    atkData = parent.SearchAttack(fighter.Skill2);
                    ui.Message(fighter.Name + " used " + atkData.Name);
                    AttackAnimation(fighter, enemyTeam, selection, atkData);
                    break;
                case BattleAction.Command.Attack3:
                    atkData = parent.SearchAttack(fighter.Skill3);
                    ui.Message(fighter.Name + " used " + atkData.Name);
                    AttackAnimation(fighter, enemyTeam, selection, atkData);
                    break;
                case BattleAction.Command.Defend:
                    ui.Message(fighter.Name + " defended");
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
                        EndBattle(BattleResult.None);
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

        public void AttackAnimation(Fighter attacker, Fighter[] target, bool[] selection, Attack data)
        {
            state = BattleState.Animation;
            postAnimation = PostAnimation.Attack;
            tempFighter = attacker;
            tempEnemyTeam = target;
            tempSelection = selection;
            tempAtkData = data;
            Animation = new List<BattleAnimation>();
            BattleAnimation frame = new BattleAnimation();
            if (data.Category == "Physical")
            {
                frame.CharacterAnimation = CharAnimation.Physical;
            }
            else
                frame.CharacterAnimation = CharAnimation.Ranged;
            frame.Selection = selection;
            frame.TargetAnimation = CharAnimation.Hit;
            Animation.Add(frame);
        }

        public BattleResult CheckVictory()
        {
            BattleResult result = BattleResult.None;
            bool allyDead = true;
            bool enemyDead = true;
            foreach (Fighter i in allies)
            {
                if (i != null && i.CurrentHP > 0)
                {
                    allyDead = false;
                    break;
                }
            }
            foreach (Fighter i in enemies)
            {
                if (i != null && i.CurrentHP > 0)
                {
                    enemyDead = false;
                    break;
                }
            }
            if (enemyDead)
            {
                result = BattleResult.PlayerWin;
            }
            if (allyDead)
            {
                result = BattleResult.EnemyWin;
            }
            return result;
        }

        public void Attack(Fighter attacker, Fighter[] target, bool[] selection, Attack data)
        {
            Console.WriteLine("COMMENCE " + attacker.Name + attacker.Level + "'s ATTACK");
            Console.WriteLine(selection[0] + " " + selection[1] + " " + selection[2] + " " + selection[3] + " " + selection[4]);
            int attack = 0;
            if (data.Category == "Physical")
                attack = attacker.ATK;
            else if (data.Category == "Magical")
                attack = attacker.MAG;
            bool fullPower = true;
            int power = data.Power;
            if (power > 0)
            {
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
                        Console.WriteLine("TARGET FIND");
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
                            if (defender.Defending)
                                damage = damage / 2;
                            CharData cData = parent.SearchChar(defender.ID);
                            double multiplier = 1;
                            if (cData.Weakness1 == data.Type || cData.Weakness2 == data.Type)
                                multiplier = multiplier * 1.5;
                            else if (cData.Resistance1 == data.Type || cData.Resistance2 == data.Type)
                                multiplier = multiplier / 1.5;
                            damage = (int)((float)damage * multiplier);
                            defender.CurrentHP = defender.CurrentHP - damage;
                            Console.WriteLine("DAMAGE DEAL");
                        }
                        if (fullPower)
                        {
                            Effect(attacker, defender, data.Effect1Name, data.Effect1Chance, damage);
                            Effect(attacker, defender, data.Effect2Name, data.Effect2Chance, damage);
                        }
                    }
                }
            }
            else if (fullPower)
            {
                attacker.CurrentMP = attacker.CurrentMP - data.MP;
                for (int i = 0; i < 5; i++)
                {
                    Fighter defender = target[i];
                    if (selection[i] == true && defender != null)
                    {
                        Effect(attacker, defender, data.Effect1Name, data.Effect1Chance,0);
                        Effect(attacker, defender, data.Effect2Name, data.Effect2Chance,0);
                    }
                }
            }
        }

        public void Effect(Fighter attacker, Fighter defender, string effect, int chance, int damage)
        {
            Console.WriteLine("Effect : " + effect);
            bool hit = true;
            if (rng.Next(1, 1000) > chance * 10)
                hit = false;
            Console.WriteLine(hit);
            if (hit)
            {
                state = BattleState.Animation;
                postAnimation = PostAnimation.Done;
                if (effect == "atklow")
                {
                    defender.ATKSTAGE--;
                    ui.Message(defender.Name+"'s Attack fell");
                    Animation = new List<BattleAnimation>();
                    BattleAnimation frame = new BattleAnimation();
                    frame.Selection = tempSelection;
                    frame.TargetAnimation = CharAnimation.Hit;
                    Animation.Add(frame);
                }
                else if (effect == "deflow")
                {
                    defender.DEFSTAGE--;
                    ui.Message(defender.Name + "'s Defense fell");
                    Animation = new List<BattleAnimation>();
                    BattleAnimation frame = new BattleAnimation();
                    frame.Selection = tempSelection;
                    frame.TargetAnimation = CharAnimation.Hit;
                    Animation.Add(frame);
                }
                else if (effect == "maglow")
                {
                    defender.MAGSTAGE--;
                    ui.Message(defender.Name + "'s Magic fell");
                    Animation = new List<BattleAnimation>();
                    BattleAnimation frame = new BattleAnimation();
                    frame.Selection = tempSelection;
                    frame.TargetAnimation = CharAnimation.Hit;
                    Animation.Add(frame);
                }
                else if (effect == "reslow")
                {
                    defender.RESSTAGE--;
                    ui.Message(defender.Name + "'s Resist fell");
                    Animation = new List<BattleAnimation>();
                    BattleAnimation frame = new BattleAnimation();
                    frame.Selection = tempSelection;
                    frame.TargetAnimation = CharAnimation.Hit;
                    Animation.Add(frame);
                }
                else if (effect == "spdlow")
                {
                    defender.SPDSTAGE--;
                    ui.Message(defender.Name + "'s Speed fell");
                    Animation = new List<BattleAnimation>();
                    BattleAnimation frame = new BattleAnimation();
                    frame.Selection = tempSelection;
                    frame.TargetAnimation = CharAnimation.Hit;
                    Animation.Add(frame);
                }
                else if (effect == "atkup")
                {
                    attacker.ATKSTAGE++;
                    ui.Message(attacker.Name+"'s Attack rose");
                    Animation = new List<BattleAnimation>();
                    for (int i = 0; i < 1; i++)
                    {
                        BattleAnimation frame = new BattleAnimation();
                        frame.Selection = tempSelection;
                        frame.CharacterAnimation = CharAnimation.Happy;
                        Animation.Add(frame);
                    }
                }
                else if (effect == "defup")
                {
                    attacker.DEFSTAGE++;
                    ui.Message(attacker.Name + "'s Defense rose");
                    for (int i = 0; i < 1; i++)
                    {
                        BattleAnimation frame = new BattleAnimation();
                        frame.Selection = tempSelection;
                        frame.CharacterAnimation = CharAnimation.Happy;
                        Animation.Add(frame);
                    }
                }
                else if (effect == "magup")
                {
                    attacker.MAGSTAGE++;
                    ui.Message(attacker.Name + "'s Magic rose");
                    for (int i = 0; i < 1; i++)
                    {
                        BattleAnimation frame = new BattleAnimation();
                        frame.Selection = tempSelection;
                        frame.CharacterAnimation = CharAnimation.Happy;
                        Animation.Add(frame);
                    }
                }
                else if (effect == "resup")
                {
                    attacker.RESSTAGE++;
                    ui.Message(attacker.Name + "'s Resistance rose");
                    for (int i = 0; i < 1; i++)
                    {
                        BattleAnimation frame = new BattleAnimation();
                        frame.Selection = tempSelection;
                        frame.CharacterAnimation = CharAnimation.Happy;
                        Animation.Add(frame);
                    }
                }
                else if (effect == "spdup")
                {
                    attacker.SPDSTAGE++;
                    ui.Message(attacker.Name + "'s Speed rose");
                    for (int i = 0; i < 1; i++)
                    {
                        BattleAnimation frame = new BattleAnimation();
                        frame.Selection = tempSelection;
                        frame.CharacterAnimation = CharAnimation.Happy;
                        Animation.Add(frame);
                    }
                }
                else if (effect == "heal")
                {
                    if (defender.CurrentHP > 0)
                    {
                        defender.CurrentHP += (int)((float)defender.HP * 0.25);
                        ui.Message(defender.Name + " was healed");
                        for (int i = 0; i < 1; i++)
                        {
                            BattleAnimation frame = new BattleAnimation();
                            frame.Selection = tempSelection;
                            frame.TargetAnimation = CharAnimation.Happy;
                            Animation.Add(frame);
                        }
                    }
                }
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

        public void EndBattle(BattleResult result)
        {
            for (int i = 0; i < 5; i++)
            {
                Fighter fighter = allies[i];
                if (fighter != null)
                {
                    fighter.Character.CurrentHP = fighter.CurrentHP;
                    fighter.Character.CurrentMP = fighter.CurrentMP;
                }
            }
            if (result == BattleResult.PlayerWin)
            {
                int xpGain = 0;
                foreach (Fighter i in enemies)
                {
                    if (i != null)
                    {
                        CharData data = parent.SearchChar(i.ID);
                        xpGain = xpGain + (data.XP * i.Level / 10);
                    }
                }
                foreach (Fighter i in allies)
                {
                    if (i != null && i.Character != null)
                    {
                        if (i.CurrentHP > 0)
                            i.Character.XP = i.Character.XP + xpGain;
                        else
                            i.Character.XP = i.Character.XP + xpGain/4;
                        if (i.Character.Level > i.Level)
                        {
                            int[] stats = parent.CalculateStats(i.Character);
                            i.Character.CurrentHP = stats[0];
                            i.Character.CurrentMP = stats[1];
                        }
                    }
                }
                ui.Message("You won the battle");
                tempEnemyTeam = allies;
            }
            else if (result == BattleResult.EnemyWin)
            {
                ui.Message("You lost the battle");
                tempEnemyTeam = enemies;
            }
            if (result == BattleResult.None)
            {
                ResumeEndBattle();
            }
            else
            {
                state = BattleState.Animation;
                tempFighter = null;
                tempSelection = new bool[5] { true, true, true, true, true };
                tempAtkData = null;
                if (result == BattleResult.EnemyWin)
                    postAnimation = PostAnimation.TrueEnd;
                else
                    postAnimation = PostAnimation.End;
                Animation = new List<BattleAnimation>();
                for (int i = 0; i < 3; i++)
                {
                    BattleAnimation frame = new BattleAnimation();
                    frame.Selection = tempSelection;
                    frame.TargetAnimation = CharAnimation.Happy;
                    Animation.Add(frame);
                }
            }
        }

        public void ResumeEndBattle()
        {
            ui.Message("");
            parent.play.Remove(parent.play.battle);
            parent.play.Remove(parent.play.battleUI);
            parent.play.Add(parent.play.mapWidget);
            if (dialog == null)
            {
                parent.pause = false;
                parent.State = Game1.GameState.Playing;
            }
            else
            {
                parent.State = Game1.GameState.Dialog;
                parent.npcName = npc;
                parent.dialogEvent = "";
                parent.newDialogName = dialog;
            }
        }
    }
}
