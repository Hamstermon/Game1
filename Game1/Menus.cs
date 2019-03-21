using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using Squared.Tiled;
using System.Collections.Generic;
using Steropes.UI;
using Steropes.UI.Components;
using Steropes.UI.Input;
using Steropes.UI.Styles;
using Steropes.UI.Util;
using Steropes.UI.Widgets;
using Steropes.UI.Widgets.Container;
using Steropes.UI.Widgets.TextWidgets;
using Steropes.UI.Platform;
using System.Threading;

namespace Game1
{
    public class Playing : Grid
    {
        public MapWidget mapWidget;
        public TextField trans;
        public MapWidget battle;
        public BattleUI battleUI;
        public DialogUI dialogUI;

        public Playing(IUIStyle s, Game1 parent, GraphicsDeviceManager man) : base(s)
        {
            mapWidget = new MapWidget(s);
            battle = new MapWidget(s);
            trans = new TextField(s)
            {                
                Anchor = AnchoredRect.CreateFixed(0, 0, 1080, 800),
                Color = Color.Black,
                TextColor = Color.White
            };
            battleUI = new BattleUI(s, parent);
            dialogUI = new DialogUI(s, parent);
            Add(mapWidget);
        }

        public void TransitionVisible(bool visible)
        {
            if (visible)
            {
                Add(trans);
                Remove(mapWidget);
            }
            else
            {
                Add(mapWidget);
                Remove(trans);
            }
        }
    }

    public class DialogUI : DockPanel
    {
        int width = 1080;
        int height = 800;
        public DialogBox box;
        public DialogOptions options;
        public DialogUI(IUIStyle s, Game1 parent) : base(s)
        {
            Grid grid = new Grid(s) { Anchor = AnchoredRect.CreateFixed(0, 0, width, height) };
            box = new DialogBox(s, parent, width, height / 4) { Anchor = AnchoredRect.CreateFixed(0, 3 * height / 4, width, height) };
            options = new DialogOptions(s, parent);
            grid.Add(box);
            Add(grid);
            Add(options,DockPanelConstraint.Left);
        }
    }

    public class DialogOptions : DockPanel
    {
        List<Button> options = new List<Button>();
        IUIStyle style;
        Game1 game;
        public DialogOptions(IUIStyle s, Game1 parent) : base(s)
        {
            style = s;
            game = parent;
        }
        public void NewOptions(Dialog dialog)
        {
            foreach (Button i in options)
            {
                Remove(i);
            }
            if (dialog.OptionName1 != "")
            {
                Button button = new Button(style, dialog.OptionName1)
                {
                    OnActionPerformed = (se, a) =>
                    {
                        game.newDialogName = dialog.OptionNext1;
                    }
                };
                Add(button);
            }
            if (dialog.OptionName2 != "")
            {
                Button button = new Button(style, dialog.OptionName2)
                {
                    OnActionPerformed = (se, a) =>
                    {
                        game.newDialogName = dialog.OptionNext2;
                    }
                };
                Add(button);
            }
            if (dialog.OptionName3 != "")
            {
                Button button = new Button(style, dialog.OptionName3)
                {
                    OnActionPerformed = (se, a) =>
                    {
                        game.newDialogName = dialog.OptionNext3;
                    }
                };
                Add(button);
            }
        }
    }

    public class DialogBox : Grid
    {
        TextField bg;
        Label n;
        Label txt;
        public DialogBox(IUIStyle s, Game1 parent, int width, int height) : base(s)
        {
            bg = new TextField(s) { Color = Color.LightCyan, ReadOnly = true, Anchor = AnchoredRect.CreateFixed(0, 0, width, height) };
            n = new Label(s, "name") { Anchor = AnchoredRect.CreateFixed(0, 0, width, Convert.ToInt32(height * 0.25)) };
            txt = new Label(s, "text") { Anchor = AnchoredRect.CreateFixed(0, Convert.ToInt32(height * 0.25), width, Convert.ToInt32(height * 0.75)) };
        }
        public string Name
        {
            set { n.Text = value; }
            get { return n.Text; }
        }
        public string Text
        {
            set { txt.Text = value; }
            get { return txt.Text; }
        }
    }

    public class BattleUI : DockPanel
    {
        int width = 1080;
        int height = 800;
        UITexture uiTexture;
        public BattleCommands commands;
        PartySlotB[] playerSlot;
        PartySlotB[] enemySlot;
        TextField message;
        Game1 game;

        public BattleUI(IUIStyle s, Game1 parent) : base(s)
        {
            game = parent;
            Grid grid = new Grid(s) { Anchor = AnchoredRect.CreateFixed(0,0,width,height) };
            DockPanel enemies = new DockPanel(s);
            enemySlot = new PartySlotB[5];
            for (int i = 0; i < 5; i++)
            {
                PartySlotB temp = new PartySlotB(s, parent, width / 5, height / 9);
                enemySlot[i] = temp;
                enemies.Add(temp, DockPanelConstraint.Left);
            }
            DockPanel players = new DockPanel(s) { Anchor = AnchoredRect.CreateFixed(0, 8*height/9, width, height) };
            playerSlot = new PartySlotB[5];
            for (int i = 0; i < 5; i++)
            {
                PartySlotB temp = new PartySlotB(s, parent, width / 5, height / 9);
                playerSlot[i] = temp;
                
                players.Add(temp, DockPanelConstraint.Left);
            }
            commands = new BattleCommands(s, parent);
            message = new TextField(s) { Anchor = AnchoredRect.CreateFixed(0, 6 * height / 9, width, height / 9), TextColor = Color.Black, Color = Color.LightGray};
            grid.Add(enemies);
            grid.Add(players);
            Add(grid);
        }
        public void RefreshFighters()
        {
            Battle battle = game.battle;
            for (int i = 0; i < 5; i++)
            {
                if (battle.allies[i] != null)
                {
                    playerSlot[i].Visibility = Visibility.Visible;
                    playerSlot[i].Name = battle.allies[i].Name;
                    playerSlot[i].Level = Convert.ToString(battle.allies[i].Level);
                    playerSlot[i].hp.Update(battle.allies[i].CurrentHP, battle.allies[i].HP);
                    playerSlot[i].mp.Update(battle.allies[i].CurrentMP, battle.allies[i].MP);
                }
                else
                {
                    playerSlot[i].Visibility = Visibility.Hidden;
                }
                if (battle.enemies[i] != null)
                {
                    enemySlot[i].Visibility = Visibility.Visible;
                    enemySlot[i].Name = battle.enemies[i].Name;
                    enemySlot[i].Level = Convert.ToString(battle.enemies[i].Level);
                    enemySlot[i].hp.Update(battle.enemies[i].CurrentHP, battle.enemies[i].HP);
                    enemySlot[i].mp.Update(battle.enemies[i].CurrentMP, battle.enemies[i].MP);
                }
                else
                {
                    enemySlot[i].Visibility = Visibility.Hidden;
                }
            }
        }
        public void Message(string msg)
        {
            message.Text = msg;
            if (msg == "")
            {
                if (message.Parent == this)
                {
                    Remove(message);
                }
            }
            else
            {
                if (message.Parent != this)
                {
                    Add(message);
                }
            }
        }
    }

    public class PartySlotB : Grid
    {
        Label n;
        Label l;
        public HPBar hp;
        public MPBar mp;
        TextField bg;
        UITexture uiTexture;
        string location;
        int pos;
        public PartySlotB(IUIStyle s, Game1 parent, int width, int height) : base(s)
        {
            bg = new TextField(s) { Color = Color.LightCyan, ReadOnly = true, Anchor = AnchoredRect.CreateFixed(0, 0, width, height) };
            n = new Label(s, "aaaaa") { Anchor = AnchoredRect.CreateFixed(0, 0, Convert.ToInt32(width * 0.6), Convert.ToInt32(height * 0.3)) };
            l = new Label(s, "lv.0") { Anchor = AnchoredRect.CreateFixed(Convert.ToInt32(width * 0.6), 0, Convert.ToInt32(width * 0.4), Convert.ToInt32(height * 0.3)) };
            hp = new HPBar(s) { Anchor = AnchoredRect.CreateFixed(0, Convert.ToInt32(height * 0.3), 0, 0), Width = width, Height = Convert.ToInt32(height * 0.35) };
            mp = new MPBar(s) { Anchor = AnchoredRect.CreateFixed(0, Convert.ToInt32(height * 0.65), 0, 0), Width = width, Height = Convert.ToInt32(height * 0.35) };
            Add(bg);
            Add(n);
            Add(l);
            Add(hp);
            Add(mp);
        }
        public string Name
        {
            set { n.Text = value; }
        }
        public string Level
        {
            set
            {
                if (value == "")
                    l.Text = "";
                else
                    l.Text = "Lv." + value;
            }
        }
    }

    public class BattleCommands : DockPanel
    {
        int skillSelected;
        enum Mode
        {
            Skill,
            Move
        };
        Mode mode;
        public bool[] selection = new bool[5] { false, false, false, false, false };
        public bool fixedSelection = false;
        public bool selectAlly = false;
        Game1 game;
        DockPanel main;
        public DockPanel skills;
        public Button atk1;
        public Button atk2;
        public Button atk3;
        DockPanel aiming;
        List<Squared.Tiled.Object> selects = new List<Squared.Tiled.Object>();
        IUIStyle style;
        public BattleCommands(IUIStyle s, Game1 parent) : base(s)
        {
            game = parent;
            style = s;
            Add(new Label(s) { Visibility = Visibility.Hidden });
            Add(new Label(s) { Visibility = Visibility.Hidden });
            main = new DockPanel(s);
            skills = new DockPanel(s);
            aiming = new DockPanel(s);
            var attack = new Button(s, "Attack")
            {
                OnActionPerformed = (se, a) =>
                {
                    if (main.Parent == this)
                        Remove(main);
                    if (skills.Parent != this)
                        Add(skills);
                }
            };
            var defend = new Button(s, "Defend")
            {
                OnActionPerformed = (se, a) =>
                {
                    BattleAction action = new BattleAction();
                    action.command = BattleAction.Command.Defend;
                    parent.battle.playerAction = action;
                }
            };
            var move = new Button(s, "Move")
            {
                OnActionPerformed = (se, a) =>
                {
                    mode = Mode.Move;
                    selection = new bool[5] {false,false,false,false,false};
                    selection[parent.battle.GetPosOfFighter(parent.currentFighter)] = true;
                    selectAlly = true;
                    fixedSelection = false;
                    UpdateSelection();
                    if (main.Parent == this)
                        Remove(main);
                    if (aiming.Parent != this)
                        Add(aiming);
                }
            };
            var flee = new Button(s, "Flee")
            {
                OnActionPerformed = (se, a) =>
                {
                    BattleAction action = new BattleAction();
                    action.command = BattleAction.Command.Flee;
                    parent.battle.playerAction = action;
                }
            };
            main.Add(attack);
            main.Add(defend);
            main.Add(move);
            main.Add(flee);
            Add(main);
            var back = new Button(s, "Back")
            {
                OnActionPerformed = (se, a) =>
                {
                    if (skills.Parent == this)
                        Remove(skills);
                    if (main.Parent != this)
                        Add(main);
                }
            };
            atk1 = new Button(s, "1")
            {
                OnActionPerformed = (se, a) =>
                {
                    mode = Mode.Skill;
                    skillSelected = 1;
                    if (skills.Parent == this)
                        Remove(skills);
                    if (aiming.Parent != this)
                    {
                        Add(aiming);
                        (selection, fixedSelection) = game.battle.LoadSelection(game.SearchAttack(game.currentFighter.Skill1), game.battle.allies);
                        UpdateSelection();
                    }
                }
            };
            atk2 = new Button(s, "2")
            {
                OnActionPerformed = (se, a) =>
                {
                    mode = Mode.Skill;
                    skillSelected = 2;
                    if (skills.Parent == this)
                        Remove(skills);
                    if (aiming.Parent != this)
                    {
                        Add(aiming);
                        (selection,fixedSelection) = game.battle.LoadSelection(game.SearchAttack(game.currentFighter.Skill2), game.battle.allies);
                        UpdateSelection();
                    }
                }
            };
            atk3 = new Button(s, "3")
            {
                OnActionPerformed = (se, a) =>
                {
                    mode = Mode.Skill;
                    skillSelected = 3;
                    if (skills.Parent == this)
                        Remove(skills);
                    if (aiming.Parent != this)
                    {
                        Add(aiming);
                        (selection, fixedSelection) = game.battle.LoadSelection(game.SearchAttack(game.currentFighter.Skill3), game.battle.allies);
                        UpdateSelection();
                    }
                }
            };
            skills.Add(atk1);
            skills.Add(atk2);
            skills.Add(atk3);
            skills.Add(back);
            var cancel = new Button(s, "Cancel")
            {
                OnActionPerformed = (se, a) =>
                {
                    selection = new bool[5] { false, false, false, false, false };
                    UpdateSelection();
                    if (aiming.Parent == this)
                        Remove(aiming);
                    if (main.Parent != this)
                        Add(main);
                }
            };
            var confirm = new Button(s, "Confirm")
            {
                OnActionPerformed = (se, a) =>
                {
                    if (mode == Mode.Skill)
                    {
                        BattleAction action = new BattleAction();
                        bool hits = false;
                        for (int i = 0; i < 5; i++)
                        {
                            if (selection[i] == true && parent.battle.enemies[i] != null)
                            {
                                hits = true;
                                break;
                            }
                        }
                        if (hits)
                        {
                            if (skillSelected == 1)
                            {
                                action.command = BattleAction.Command.Attack1;
                            }
                            else if (skillSelected == 2)
                            {
                                action.command = BattleAction.Command.Attack2;
                            }
                            else if (skillSelected == 3)
                            {
                                action.command = BattleAction.Command.Attack3;
                            }
                            action.target = selection;
                            parent.battle.playerAction = action;
                        }
                    }
                    else if (mode == Mode.Move)
                    {
                        BattleAction action = new BattleAction();
                        action.command = BattleAction.Command.Move;
                        action.target = selection;
                        parent.battle.playerAction = action;
                    }
                }
            };
            aiming.Add(cancel);
            aiming.Add(confirm);
        }

        public void UpdateSelection()
        {
            ObjectGroup effects = game.play.battle.CurrentMap.ObjectGroups["effects"];
            ObjectGroup spots = game.play.battle.CurrentMap.ObjectGroups["spots"];
            foreach (Squared.Tiled.Object i in selects)
            {
                effects.Objects.Remove(i.Name);
            }
            for (int i = 0; i < 5; i++)
            {
                if (selection[i] == true)
                {
                    Squared.Tiled.Object temp = new Squared.Tiled.Object();
                    temp.Texture = game.Content.Load<Texture2D>("selection");
                    Squared.Tiled.Object spot;
                    if (selectAlly)
                        spot = spots.Objects["A" + i];
                    else
                        spot = spots.Objects["B" + i];
                    temp.X = spot.X + spot.Width / 2;
                    temp.Y = spot.Y + spot.Height / 2;
                    temp.Width = 64;
                    temp.Height = 64;
                    temp.Name = "selection" + i;
                    selects.Add(temp);
                    effects.Objects.Add("selection"+i,temp);
                }
            }
        }
        public bool[] MoveSelection(bool[] select, bool up)
        {
            if (!fixedSelection)
            {
                if (up && select[0] == false)
                {
                    select[0] = select[1];
                    select[1] = select[2];
                    select[2] = select[3];
                    select[3] = select[4];
                    select[4] = false;
                }
                else if (!up && select[4] == false)
                {
                    select[4] = select[3];
                    select[3] = select[2];
                    select[2] = select[1];
                    select[1] = select[0];
                    select[0] = false;
                }
            }
            return select;
        }
        public void MoveSelection(bool up)
        {
            if (!fixedSelection)
            {
                if (up && selection[0] == false)
                {
                    selection[0] = selection[1];
                    selection[1] = selection[2];
                    selection[2] = selection[3];
                    selection[3] = selection[4];
                    selection[4] = false;
                }
                else if (!up && selection[4] == false)
                {
                    selection[4] = selection[3];
                    selection[3] = selection[2];
                    selection[2] = selection[1];
                    selection[1] = selection[0];
                    selection[0] = false;
                }
                UpdateSelection();
            }
        }
        public void ResetVisibility()
        {
            if (main.Parent != this)
            {
                Add(main);
            }
            if (skills.Parent == this)
            {
                Remove(skills);
            }
            if (aiming.Parent == this)
            {
                Remove(aiming);
            }
        }
        public void UpdateAttacks(Attack a1, Attack a2, Attack a3)
        {
            skills.Remove(atk1);
            skills.Remove(atk2);
            skills.Remove(atk3);
            atk1 = new Button(style, a1.Name + " : " + a1.MP + " MP")
            {
                OnActionPerformed = (se, a) =>
                {
                    mode = Mode.Skill;
                    skillSelected = 1;
                    if (skills.Parent == this)
                        Remove(skills);
                    if (aiming.Parent != this)
                    {
                        Add(aiming);
                        (selection, fixedSelection) = game.battle.LoadSelection(game.SearchAttack(game.currentFighter.Skill1), game.battle.allies);
                        UpdateSelection();
                    }
                }
            };
            atk2 = new Button(style, a2.Name + " : " + a2.MP + " MP")
            {
                OnActionPerformed = (se, a) =>
                {
                    mode = Mode.Skill;
                    skillSelected = 2;
                    if (skills.Parent == this)
                        Remove(skills);
                    if (aiming.Parent != this)
                    {
                        Add(aiming);
                        (selection, fixedSelection) = game.battle.LoadSelection(game.SearchAttack(game.currentFighter.Skill2), game.battle.allies);
                        UpdateSelection();
                    }
                }
            };
            atk3 = new Button(style, a3.Name + " : " + a3.MP + " MP")
            {
                OnActionPerformed = (se, a) =>
                {
                    mode = Mode.Skill;
                    skillSelected = 3;
                    if (skills.Parent == this)
                        Remove(skills);
                    if (aiming.Parent != this)
                    {
                        Add(aiming);
                        (selection, fixedSelection) = game.battle.LoadSelection(game.SearchAttack(game.currentFighter.Skill3), game.battle.allies);
                        UpdateSelection();
                    }
                }
            };
            skills.Add(atk1, DockPanelConstraint.Top);
            skills.Add(atk2, DockPanelConstraint.Top);
            skills.Add(atk3, DockPanelConstraint.Top);
        }
    }

    class MainOW : DockPanel
    {
        int width = 1080;
        int height = 800;
        UITexture uiTexture;

        public MainOW(IUIStyle s, Game1 parent, OverWorldMenu menu) : base(s)
        {
            var party = new Button(s, "Party")
            {
                OnActionPerformed = (se, a) =>
                {
                    menu.MenuState = OverWorldMenu.OverworldMenuState.Party;
                }
            };
            var inv = new Button(s, "Inventory")
            {
                OnActionPerformed = (se, a) =>
                {
                    menu.MenuState = OverWorldMenu.OverworldMenuState.Inventory;
                }
            };
            var opt = new Button(s, "Options")
            {
                OnActionPerformed = (se, a) =>
                {
                    menu.MenuState = OverWorldMenu.OverworldMenuState.Options;
                }
            };
            var exit = new Button(s, "Exit")
            {
                //Anchor = AnchoredRect.CreateBottomRightAnchored(),
                Color = Color.Red,
                OnActionPerformed = (se, a) =>
                {
                    parent.State = Game1.GameState.Playing;
                }
            };
            this.Add(party);
            this.Add(inv);
            this.Add(opt);
            this.Add(exit);
        }
    }

    class PartySlot : DockPanel
    {
        Label n;
        Label l;
        UITexture uiTexture;
        string location;
        int pos;
        public PartySlot(IUIStyle s, Game1 parent, OverWorldMenu menu, Party party) : base(s)
        {
            n = new Label(s,"aaaaa");
            l = new Label(s,"lv.0");
            var button = new Button(s, "Select")
            {
                OnActionPerformed = (se, a) =>
                {
                    party.CharSelect(location,pos);
                }
            };
            Add(n);
            Add(l);
            Add(button);
        }
        public string Name
        {
            set { n.Text = value; }
        }
        public string Level
        {
            set
            {
                if (value == "")
                    l.Text = "";
                else
                    l.Text = "Level: " + value;
            }
        }
        public string Location
        {
            set { location = value; }
            get { return location; }
        }
        public int Position
        {
            set { pos = value; }
            get { return pos; }
        }
    }

    class PartyTasks : DockPanel
    {
        UITexture uiTexture;
        Button arrange;
        Button stats;
        public PartyTasks(IUIStyle s, Game1 parent, OverWorldMenu menu, Party party) : base(s)
        {
            arrange = new Button(s, "Arrange")
            {
                OnActionPerformed = (se, a) =>
                {
                    party.Mode = Party.EditMode.Arrange;
                }
            };
            stats = new Button(s, "Stats")
            {
                OnActionPerformed = (se, a) =>
                {
                    party.Mode = Party.EditMode.Stats;
                }
            };
            Add(arrange, DockPanelConstraint.Left);
            Add(stats, DockPanelConstraint.Left);
        }

        public void Task(Party.EditMode mode)
        {
            arrange.Color = Color.White;
            stats.Color = Color.White;
            switch (mode)
            {
                case Party.EditMode.Arrange:
                    arrange.Color = Color.LightGreen;
                    break;
                case Party.EditMode.Stats:
                    stats.Color = Color.LightGreen;
                    break;
            }
        }
    }

    class Stat : DockPanel
    {
        string name;
        string val;
        Label n;
        Label v;
        public Stat(IUIStyle s, string Name, string Value) : base(s)
        {
            name = Name;
            val = Value;
            n = new Label(s, Name + ":")
            {
                TextColor = Color.Black
            };
            v = new Label(s, Value)
            {
            };
            Add(n, DockPanelConstraint.Left);
            Add(v, DockPanelConstraint.Right);
        }
        public string Name
        {
            set
            {
                name = value;
                n.Text = name+":";
            }
            get { return name; }
        }
        public string Value
        {
            set
            {
                val = value;
                v.Text = val;
            }
            get { return val; }
        }
    }

    public class Bar : LayeredPane
    {
        public TextField bar;
        public TextField bg;
        public Label txt;
        public int width = 200;
        public int height = 40;
        public float percent = 1;
        public Bar(IUIStyle s) : base(s)
        {
            bg = new TextField(s)
            {
                ReadOnly = true,
                Color = Color.Black,
                Anchor = AnchoredRect.CreateFixed(0, 0, width, height)
            };
            bar = new TextField(s)
            {
                ReadOnly = true,
                Color = Color.LightCyan,
                Anchor = AnchoredRect.CreateFixed(0, 0, width, height)
            };
            txt = new Label(s)
            {
                Anchor = AnchoredRect.CreateFixed(0, 0, width, height),
                TextColor = Color.White
            };
            Add(bg);
            Add(bar);
            Add(txt);
        }
        public void Update(int current, int max)
        {
            percent = (float)current / (float)max;
            bar.Anchor = AnchoredRect.CreateFixed(0, 0, Convert.ToInt32(percent * width), height);
        }
        public int Width
        {
            set
            {
                width = value;
                bg.Anchor = AnchoredRect.CreateFixed(0, 0, width, height);
                bar.Anchor = AnchoredRect.CreateFixed(0, 0, Convert.ToInt32(percent * width), height);
                txt.Anchor = AnchoredRect.CreateFixed(0, 0, width, height);
            }
            get { return width; }
        }
        public int Height
        {
            set
            {
                height = value;
                bg.Anchor = AnchoredRect.CreateFixed(0, 0, width, height);
                bar.Anchor = AnchoredRect.CreateFixed(0, 0, Convert.ToInt32(percent * width), height);
                txt.Anchor = AnchoredRect.CreateFixed(0, 0, width, height);
            }
            get { return height; }
        }
    }
    class XPBar : Bar
    {
        public XPBar(IUIStyle s) : base(s)
        {
            txt.Text = "XP";
        }
    }

    public class HPBar : Bar
    {
        Label Value;
        public HPBar(IUIStyle s) : base(s)
        {
            txt.Text = "HP";
            bar.Color = Color.LightGreen;
            Value = new Label(s, "0/0")
            {
                Anchor = AnchoredRect.CreateFixed(0, 0, Width, Height),
                Alignment = Alignment.End,
                TextColor = Color.White
            };
            Add(Value);
        }
        public new void Update(int current, int max)
        {
            percent = (float)current / (float)max;
            bar.Anchor = AnchoredRect.CreateFixed(0, 0, Convert.ToInt32(percent * width), height);
            Value.Text = current + "/" + max;
        }
        public new int Width
        {
            set
            {
                width = value;
                bg.Anchor = AnchoredRect.CreateFixed(0, 0, width, height);
                bar.Anchor = AnchoredRect.CreateFixed(0, 0, Convert.ToInt32(percent * width), height);
                txt.Anchor = AnchoredRect.CreateFixed(0, 0, width, height);
                Value.Anchor = AnchoredRect.CreateFixed(0, 0, width, height);
            }
            get { return width; }
        }
        public new int Height
        {
            set
            {
                height = value;
                bg.Anchor = AnchoredRect.CreateFixed(0, 0, width, height);
                bar.Anchor = AnchoredRect.CreateFixed(0, 0, Convert.ToInt32(percent * width), height);
                txt.Anchor = AnchoredRect.CreateFixed(0, 0, width, height);
                Value.Anchor = AnchoredRect.CreateFixed(0, 0, width, height);
            }
            get { return height; }
        }
    }

    public class MPBar : Bar
    {
        Label Value;
        public MPBar(IUIStyle s) : base(s)
        {
            txt.Text = "MP";
            bar.Color = Color.Cyan;
            Value = new Label(s, "0/0")
            {
                Anchor = AnchoredRect.CreateFixed(0, 0, Width, Height),
                Alignment = Alignment.End,
                TextColor = Color.White
            };
            Add(Value);
        }
        public new void Update(int current, int max)
        {
            percent = (float)current / (float)max;
            bar.Anchor = AnchoredRect.CreateFixed(0, 0, Convert.ToInt32(percent * width), height);
            Value.Text = current + "/" + max;
        }
        public new int Width
        {
            set
            {
                width = value;
                bg.Anchor = AnchoredRect.CreateFixed(0, 0, width, height);
                bar.Anchor = AnchoredRect.CreateFixed(0, 0, Convert.ToInt32(percent * width), height);
                txt.Anchor = AnchoredRect.CreateFixed(0, 0, width, height);
                Value.Anchor = AnchoredRect.CreateFixed(0, 0, width, height);
            }
            get { return width; }
        }
        public new int Height
        {
            set
            {
                height = value;
                bg.Anchor = AnchoredRect.CreateFixed(0, 0, width, height);
                bar.Anchor = AnchoredRect.CreateFixed(0, 0, Convert.ToInt32(percent * width), height);
                txt.Anchor = AnchoredRect.CreateFixed(0, 0, width, height);
                Value.Anchor = AnchoredRect.CreateFixed(0, 0, width, height);
            }
            get { return height; }
        }
    }

    class Info : DockPanel
    {
        Label name;
        Stat lvl;
        XPBar xp;
        HPBar hp;
        MPBar mp;
        Stat atk;
        Stat def;
        Stat mag;
        Stat res;
        Stat spd;
        Game1 parent;
        public Info(IUIStyle s, Game1 p, OverWorldMenu menu, Party party) : base(s)
        {
            parent = p;
            name = new Label(s, "Name");
            lvl = new Stat(s, "Level", "0");
            xp = new XPBar(s);
            hp = new HPBar(s);
            mp = new MPBar(s);
            atk = new Stat(s, "Attack", "0");
            def = new Stat(s, "Defense", "0");
            mag = new Stat(s, "Magic", "0");
            res = new Stat(s, "Resist", "0");
            spd = new Stat(s, "Speed", "0");
            Add(name);
            Add(lvl);
            Add(xp);
            Add(hp);
            Add(mp);
            Add(atk);
            Add(def);
            Add(mag);
            Add(res);
            Add(spd);
        }

        public void LoadInfo(Character character)
        {
            CharData info = parent.SearchChar(character.CharacterID);
            name.Text = info.Name;
            lvl.Value = Convert.ToString(character.Level);
            xp.Update(character.XP, character.Level * (character.Level + 4));
            int[] stats = parent.CalculateStats(character);
            hp.Update(character.CurrentHP, stats[0]);
            mp.Update(character.CurrentMP, stats[1]);
            atk.Value = Convert.ToString(stats[2]);
            def.Value = Convert.ToString(stats[3]);
            mag.Value = Convert.ToString(stats[4]);
            res.Value = Convert.ToString(stats[5]);
            spd.Value = Convert.ToString(stats[6]);
        }
    }

    class Party : DockPanel
    {
        int width = 1080;
        int height = 800;
        UITexture uiTexture;
        PartySlot[] slotsP;
        PartySlot[] slotsR;
        PartyTasks tasks;
        Info info;
        public enum EditMode
        {
            Arrange,
            Stats
        }
        EditMode mode;
        public EditMode Mode
        {
            set
            {
                mode = value;
                tasks.Task(mode);
            }
            get { return mode; }
        }
        Game1 parent;
        public Party(IUIStyle s, Game1 p, OverWorldMenu menu) : base(s)
        {
            parent = p;
            var chars = new DockPanel(s);
            slotsP = new PartySlot[5];
            slotsR = new PartySlot[10];
            var back = new Button(s, "Back")
            {
                Anchor = AnchoredRect.CreateBottomLeftAnchored(),
                Color = Color.OrangeRed,
                OnActionPerformed = (se, a) =>
                {
                    menu.MenuState = OverWorldMenu.OverworldMenuState.Main;
                }
            };
            tasks = new PartyTasks(s,parent,menu, this);
            var partyT = new Label(s, "PARTY") { TextColor = Color.Black };
            var party = new DockPanel(s);
            for (int i = 0; i < 5; i++)
            {
                slotsP[i] = new PartySlot(s, parent, menu, this);
                slotsP[i].Location = "P";
                slotsP[i].Position = i;
                party.Add(slotsP[i], DockPanelConstraint.Right);
            }
            var resT = new Label(s, "RESERVE") { TextColor = Color.Black };
            var reserve = new DockPanel(s);
            for (int i = 0; i < 5; i++)
            {
                slotsR[i] = new PartySlot(s, parent, menu, this);
                slotsR[i].Location = "R";
                slotsR[i].Position = i;
                reserve.Add(slotsR[i], DockPanelConstraint.Right);
            }
            var reserve2 = new DockPanel(s);
            for (int i = 0; i < 5; i++)
            {
                slotsR[i+5] = new PartySlot(s, parent, menu, this);
                slotsR[i+5].Location = "R";
                slotsR[i+5].Position = i+5;
                reserve2.Add(slotsR[i+5], DockPanelConstraint.Right);
            }
            chars.Add(tasks);
            chars.Add(partyT);
            chars.Add(party);
            chars.Add(resT);
            chars.Add(reserve);
            chars.Add(reserve2);
            chars.Add(back);
            Add(chars, DockPanelConstraint.Right);
            info = new Info(s, parent, menu, this);
            Add(info, DockPanelConstraint.Right);
            Mode = EditMode.Stats;
        }

        public void UpdateParty()
        {
            PlayerSaveData psd = parent.playerSaveData;
            for (int i = 0; i < 5; i++)
            {
                int id = psd.Party[i];
                PartySlot p = slotsP[i];
                if (id != -1)
                {
                    Character c = psd.CharacterList[id];
                    CharData info = parent.SearchChar(c.CharacterID);
                    p.Name = info.Name;
                    p.Level = Convert.ToString(c.Level);
                }
                else
                {
                    p.Name = "";
                    p.Level = "";
                }
            }
        }

        public void CharSelect(string location, int pos)
        {
            switch (mode)
            {
                case EditMode.Arrange:
                    break;
                case EditMode.Stats:
                    if (location == "P")
                    {
                        PlayerSaveData psd = parent.playerSaveData;
                        Character c = psd.CharacterList[psd.Party[pos]];
                        if (c != null)
                            info.LoadInfo(c);
                    }
                    break;
            }
        }
    }


    class Inventory : DockPanel
    {
        int width = 1080;
        int height = 800;
        UITexture uiTexture;

        public Inventory(IUIStyle s, Game1 parent, OverWorldMenu menu) : base(s)
        {
            var back = new Button(s, "Back")
            {
                //Anchor = AnchoredRect.CreateFixed(10, height - 100, 120, 80),
                Color = Color.OrangeRed,
                OnActionPerformed = (se, a) =>
                {
                    menu.MenuState = OverWorldMenu.OverworldMenuState.Main;
                }
            };
            this.Add(back);
        }
    }

    class OverWorldMenu :Grid
    {
        public enum OverworldMenuState
        {
            Main,
            Party,
            Inventory,
            Quests,
            Options
        }
        OverworldMenuState menuState = OverworldMenuState.Main;
        MainOW main;
        Party party;
        Inventory inventory;
        Options options;
        public OverWorldMenu(IUIStyle s, Game1 parent) : base(s)
        {
            main = new MainOW(s, parent, this);
            party = new Party(s, parent, this);
            inventory = new Inventory(s, parent, this);
            options = new Options(s, parent, this);
            this.Add(main);
        }

        public OverworldMenuState MenuState
        {
            set
            {
                switch (menuState)
                {
                    case OverworldMenuState.Main:
                        this.Remove(main);
                        break;
                    case OverworldMenuState.Party:
                        this.Remove(party);
                        break;
                    case OverworldMenuState.Inventory:
                        this.Remove(inventory);
                        break;
                    case OverworldMenuState.Options:
                        this.Remove(options);
                        break;
                }
                menuState = value;
                switch (value)
                {
                    case OverworldMenuState.Main:
                        this.Add(main);
                        break;
                    case OverworldMenuState.Party:
                        this.Add(party);
                        party.UpdateParty();
                        break;
                    case OverworldMenuState.Inventory:
                        this.Add(inventory);
                        break;
                    case OverworldMenuState.Options:
                        this.Add(options);
                        break;
                }
            }
            get { return menuState; }
        }
    }

    class TitleScreen : Grid
    {
        int width = 1080;
        int height = 800;
        UITexture uiTexture;

        public TitleScreen(IUIStyle s, Game1 parent, MainMenu menu) : base(s)
        {
            Texture2D background = parent.Content.Load<Texture2D>("characterSpritesheet");
            uiTexture = new UITexture(background);
            var bg = new Image(s)
            {
                Texture = uiTexture.Rebase(background, new Rectangle(0, 0, width, height), "")
            };
            var play = new Button(s, "Play")
            {
                Anchor = AnchoredRect.CreateFixed(10, 120, 120, 80),
                Color = Color.Aqua,
                OnActionPerformed = (se, a) =>
                {
                    menu.MenuState = MainMenu.MainMenuState.SaveFile;
                }
            };
            var options = new Button(s, "Options")
            {
                Anchor = AnchoredRect.CreateFixed(10, 220, 120, 80),
                Color = Color.Aqua,
                OnActionPerformed = (se, a) =>
                {
                    menu.MenuState = MainMenu.MainMenuState.Options;
                }
            };
            this.Add(bg);
            this.Add(play);
            this.Add(options);
        }
    }

    class SaveFile : DockPanel
    {
        int width = 1080;
        int height = 800;
        UITexture uiTexture;

        public SaveFile(IUIStyle s, Game1 parent, MainMenu menu) : base(s)
        {
            var back = new Button(s, "Back")
            {
                //Anchor = AnchoredRect.CreateFixed(10, height-100, 120, 80),
                Color = Color.OrangeRed,
                OnActionPerformed = (se, a) =>
                {
                    menu.MenuState = MainMenu.MainMenuState.Title;
                }
            };
            var continue1 = new Button(s, "Continue Game")
            {
                OnActionPerformed = (se, a) =>
                {
                    //parent.LoadGame(1);
                },
                //Anchor = AnchoredRect.CreateFixed(0, 0, 200, 80),
                Color = Color.OrangeRed,
            };
            var new1 = new Button(s, "New Game")
            {
                OnActionPerformed = (se, a) =>
                {
                    parent.CreateNewGame(1);
                },
                //Anchor = AnchoredRect.CreateFixed(0, 0, 200, 80),
                Color = Color.OrangeRed,
            };
            this.Add(continue1);
            this.Add(new1);
            this.Add(back);
        }
    }

    class Options : DockPanel
    {
        int width = 1080;
        int height = 800;
        UITexture uiTexture;

        public Options(IUIStyle s, Game1 parent, OverWorldMenu menu) : base(s)
        {
            var back = new Button(s, "Back")
            {
                //Anchor = AnchoredRect.CreateFixed(10, height - 100, 120, 80),
                Color = Color.OrangeRed,
                OnActionPerformed = (se, a) =>
                {
                    menu.MenuState = OverWorldMenu.OverworldMenuState.Main;
                }
            };
            this.Add(back);
        }

        public Options(IUIStyle s, Game1 parent, MainMenu menu) : base(s)
        {
            var back = new Button(s, "Back")
            {
                //Anchor = AnchoredRect.CreateFixed(10, height - 100, 120, 80),
                Color = Color.OrangeRed,
                OnActionPerformed = (se, a) =>
                {
                    menu.MenuState = MainMenu.MainMenuState.Title;
                }
            };
            this.Add(back);
        }
    }

    class MainMenu:Grid
    { 
        public enum MainMenuState
        {
            Title,
            SaveFile,
            Options
        }
        MainMenuState menuState = MainMenuState.Title;
        TitleScreen title;
        SaveFile saveFile;
        Options options;     

        public MainMenu(IUIStyle s, Game1 parent) : base(s)
        {
            title = new TitleScreen(s, parent, this);
            saveFile = new SaveFile(s, parent, this);
            options = new Options(s, parent, this);
            this.Add(title);
        }

        public MainMenuState MenuState
        {
            set
            {
                switch (menuState)
                {
                    case MainMenuState.Title:
                        this.Remove(title);
                        break;
                    case MainMenuState.SaveFile:
                        this.Remove(saveFile);
                        break;
                    case MainMenuState.Options:
                        this.Remove(options);
                        break;
                }
                menuState = value;
                switch (value)
                {
                    case MainMenuState.Title:
                        this.Add(title);
                        break;
                    case MainMenuState.SaveFile:
                        this.Add(saveFile);
                        break;
                    case MainMenuState.Options:
                        this.Add(options);
                        break;
                }
            }
            get { return menuState; }
        }
    }
}
