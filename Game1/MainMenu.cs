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

        public Playing(IUIStyle s, Game1 parent, GraphicsDeviceManager man) : base(s)
        {
            mapWidget = new MapWidget(s);
            battle = new MapWidget(s) { Visibility = Visibility.Hidden };
            trans = new TextField(s)
            {
                //Alignment = Alignment.Center,
                
                Visibility = Visibility.Hidden,
                Anchor = AnchoredRect.CreateFixed(0,0,1080,800),
                Color = Color.Black,
                TextColor = Color.White
            };
            Add(mapWidget);
            Add(trans);
            Add(battle);
        }

        public void TransitionVisible(bool visible)
        {
            if (visible)
            {
                trans.Visibility = Visibility.Visible;
                mapWidget.Visibility = Visibility.Hidden;
            }
            else
            {
                trans.Visibility = Visibility.Hidden;
                mapWidget.Visibility = Visibility.Visible;
            }
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
            percent = current / max;
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

    class HPBar : Bar
    {
        Label value;
        public HPBar(IUIStyle s) : base(s)
        {
            txt.Text = "HP";
            bar.Color = Color.LightGreen;
            value = new Label(s, "0/0")
            {
                Anchor = AnchoredRect.CreateFixed(0, 0, Width, Height),
                Alignment = Alignment.End,
                TextColor = Color.White
            };
            Add(value);
        }
        public new void Update(int current, int max)
        {
            percent = current / max;
            bar.Anchor = AnchoredRect.CreateFixed(0, 0, Convert.ToInt32(percent * width), height);
            value.Text = current + "/" + max;
        }
        public new int Width
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
        public new int Height
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

    class MPBar : Bar
    {
        Label value;
        public MPBar(IUIStyle s) : base(s)
        {
            txt.Text = "MP";
            bar.Color = Color.Cyan;
            value = new Label(s, "0/0")
            {
                Anchor = AnchoredRect.CreateFixed(0, 0, Width, Height),
                Alignment = Alignment.End,
                TextColor = Color.White
            };
            Add(value);
        }
        public new void Update(int current, int max)
        {
            percent = current / max;
            bar.Anchor = AnchoredRect.CreateFixed(0, 0, Convert.ToInt32(percent * width), height);
            value.Text = current + "/" + max;
        }
        public new int Width
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
        public new int Height
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
