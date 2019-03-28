using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using System.Diagnostics;
using System;
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
using System.Xml;
using Newtonsoft.Json;

namespace Game1
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;
        public Map map;
        public IUIManager uiManager;
        public enum GameState
        {
            MainMenu,
            Overworld,
            Playing,
            Dialog,
            Battle,
            EndGame
        }
        GameState state = GameState.MainMenu;
        public GameState State
        {
            get { return state; }
            set
            {
                state = value;
                switch (state)
                {
                    case GameState.MainMenu:
                    case GameState.Overworld:
                    case GameState.Battle:
                    case GameState.EndGame:
                    case GameState.Dialog:
                        pause = true;
                        break;
                    case GameState.Playing:
                        pause = false;
                        break;
                }
            }
        }
        Texture2D pixel;
        public OverworldPlayer player = new OverworldPlayer();
        public PlayerSaveData playerSaveData = new PlayerSaveData();
        //Screens
        MainMenu main;
        OverWorldMenu menu;
        public Battle battle;
        public Playing play;
        public Level level;
        int elapsedTime = 0;
        int saveFileID = 1;
        public int mapID = 0;
        public bool pause = false;
        public Fighter currentFighter;
        Random rng = new Random();

        Dialog currentDialog;
        public string npcName;
        public string newDialogName;
        public string dialogEvent = "";
                
        public List<Attack> attacks = new List<Attack>();
        public List<CharData> characters = new List<CharData>();
        public List<MapData> maps = new List<MapData>();
        public List<MapChar> mapChar = new List<MapChar>();
        public List<CharAttack> charAtk = new List<CharAttack>();
        public List<Dialog> dialogs = new List<Dialog>();
        static JsonSerializer serializer = new JsonSerializer();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1080;
            graphics.PreferredBackBufferHeight = 800;
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        public enum Type
        {
            Normal,
            Fire,
            Water,
            Electric,
            Grass,
            Ice,
            Fighting,
            Poison,
            Ground,
            Flying,
            Psychic,
            Bug,
            Rock,
            Ghost,
            Dragon,
            Dark,
            Steel,
            Light
        }
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            uiManager = UIManagerComponent.CreateAndInit(this, new InputManager(this), "Content").Manager;
            var styleSystem = uiManager.UIStyle;
            var styles = styleSystem.LoadStyles("Content/style.xml", "UI/Metro", GraphicsDevice);
            styleSystem.StyleResolver.StyleRules.AddRange(styles);
            int[,] matchup = new int[18, 18];
            for (int i = 0; i< 18; i++)
            {
            }

            main = new MainMenu(styleSystem, this);
            menu = new OverWorldMenu(styleSystem, this);
            play = new Playing(styleSystem, this, graphics);

            using (StreamReader sr = new StreamReader("Content/attacks.txt"))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                attacks = serializer.Deserialize<List<Attack>>(reader);
            }
            using (StreamReader sr = new StreamReader("Content/character.txt"))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                characters = serializer.Deserialize<List<CharData>>(reader);
            }
            using (StreamReader sr = new StreamReader("Content/maps.txt"))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                maps = serializer.Deserialize<List<MapData>>(reader);
            }
            using (StreamReader sr = new StreamReader("Content/mapchar.txt"))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                mapChar = serializer.Deserialize<List<MapChar>>(reader);
            }
            using (StreamReader sr = new StreamReader("Content/charattack.txt"))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                charAtk = serializer.Deserialize<List<CharAttack>>(reader);
            }
            using (StreamReader sr = new StreamReader("Content/dialogs.txt"))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                dialogs = serializer.Deserialize<List<Dialog>>(reader);
            }

            //CreateNewGame(0);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            level = new Level();
            mapID = 0;
            play.mapWidget.Init("testMap1.tmx", this, graphics);
            
            level.Init(this, 50, play.mapWidget.CurrentMap);
            pixel = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            pixel.SetData(new[] { Color.White });

            //Default Player Values
            player.Direction = 2;
            player.AnimationSpeed = 100;
            player.FrameCount = 4;
            player.SpriteIndex = 3;

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (pause == false)
            {
                elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (elapsedTime > 10000)
                {
                    //level.SpawnCycle();
                    elapsedTime = 0;
                }
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            switch (state)
            {
                case GameState.MainMenu:
                    UpdateMainMenu();
                    break;
                case GameState.Playing:
                case GameState.Overworld:
                case GameState.Dialog:
                    UpdateOverworld(gameTime);
                    break;
                case GameState.Battle:
                    UpdateBattle(gameTime);
                    break;
                case GameState.EndGame:
                    UpdateEndGame();
                    break;
            }
            base.Update(gameTime);
        }

        public Attack SearchAttack(int id)
        {
            Attack atk = new Attack();
            foreach (Attack i in attacks)
            {
                if (i.AttackID == id)
                {
                    atk = i;
                    break;
                }
            }
            return atk;
        }
        public CharData SearchChar(int id)
        {
            CharData chr = new CharData();
            foreach (CharData i in characters)
            {
                if (i.CharID == id)
                {
                    chr = i;
                    break;
                }
            }
            return chr;
        }
        public MapData SearchMap(int id)
        {
            MapData map = new MapData();
            foreach (MapData i in maps)
            {
                if (i.MapID == id)
                {
                    map = i;
                    break;
                }
            }
            return map;
        }
        public List<MapChar> FilterMapChar(int mapID)
        {
            List<MapChar> mapchar = new List<MapChar>();
            foreach (MapChar i in mapChar)
            {
                if (mapID == -1 || i.MapID == mapID)
                {
                    mapchar.Add(i);
                }
            }
            return mapchar;
        }
        public List<CharAttack> FilterCharAttack(int charID)
        {
            List<CharAttack> charatk = new List<CharAttack>();
            foreach (CharAttack i in charAtk)
            {
                if (charID == -1 || i.CharID == charID)
                {
                    charatk.Add(i);
                }
            }
            return charatk;
        }
        public Dialog SearchDialog(string name)
        {
            Dialog dialog = new Dialog();
            foreach (Dialog i in dialogs)
            {
                if (i.Name == name)
                {
                    dialog = i;
                    break;
                }
            }
            return dialog;
        }

        public int FindElement(string tableName, int id)
        {
            int realID = -1;
            if (tableName == "attack")
            {
                for (int i = 0; i < attacks.Count; i++)
                {
                    Attack v = attacks[i];
                    if (v.AttackID == id)
                    {
                        realID = i;
                        break;
                    }
                }
            }
            else if (tableName == "character")
            {
                for (int i = 0; i < characters.Count; i++)
                {
                    CharData v = characters[i];
                    if (v.CharID == id)
                    {
                        realID = i;
                        break;
                    }
                }
            }
            else if (tableName == "map")
            {
                for (int i = 0; i < maps.Count; i++)
                {
                    MapData v = maps[i];
                    if (v.MapID == id)
                    {
                        realID = i;
                        break;
                    }
                }
            }
            else if (tableName == "mapchar")
            {
                for (int i = 0; i < mapChar.Count; i++)
                {
                    MapChar v = mapChar[i];
                    if (v.ID == id)
                    {
                        realID = i;
                        break;
                    }
                }
            }
            else if (tableName == "charattack")
            {
                for (int i = 0; i < charAtk.Count; i++)
                {
                    CharAttack v = charAtk[i];
                    if (v.ID == id)
                    {
                        realID = i;
                        break;
                    }
                }
            }
            return realID;
        }

        public void Wait(int milliseconds)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            do
            {

            }
            while (timer.ElapsedMilliseconds < milliseconds);
            timer.Stop();
        }

        public int[] CalculateStats(Character c)
        {
            int[] stats = new int[7];
            CharData data = SearchChar(c.CharacterID);
            stats = CalculateStats(data, c.Level);
            return stats;
        }

        public int[] CalculateStats(CharData data, int level)
        {
            int[] stats = new int[7];
            stats[0] = (int)(50.0 + ((double)data.HP * (2.0 + (2.0 / 5.0) * (double)level)) / 8.0);
            stats[1] = (int)(50.0 + ((double)data.MP * (2.0 + (2.0 / 5.0) * (double)level)) / 8.0);
            stats[2] = (int)(10.0 + ((double)data.ATK * (2.0 + (2.0 / 5.0) * (double)level)) / 40.0);
            stats[3] = (int)(10.0 + ((double)data.DEF * (2.0 + (2.0 / 5.0) * (double)level)) / 40.0);
            stats[4] = (int)(10.0 + ((double)data.MAG * (2.0 + (2.0 / 5.0) * (double)level)) / 40.0);
            stats[5] = (int)(10.0 + ((double)data.RES * (2.0 + (2.0 / 5.0) * (double)level)) / 40.0);
            stats[6] = (int)(10.0 + ((double)data.SPD * (2.0 + (2.0 / 5.0) * (double)level)) / 40.0);
            return stats;
        }

        public int[] GetFirstSkills(int charID, int level)
        {
            int[] skills = new int[3] { -1, -1, -1 };
            int[] levels = new int[3] { -1, -1, -1 };
            List<CharAttack> movepool = FilterCharAttack(charID);
            foreach (CharAttack a in movepool)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (levels[i] < a.Level && levels[i] <= levels[0] && levels[i] <= levels[1] && levels[i] <= levels[2])
                    {
                        skills[i] = SearchAttack(a.AttackID).AttackID;
                        levels[i] = a.Level;
                        break;
                    }
                }
            }
            if (skills[0] == -1)
                skills[0] = 0;
            return skills;
        }

        public Character CreateCharacter(int id, int lvl, int hp, int mp, int skill1, int skill2, int skill3, bool available)
        {
            Character c = new Character();
            c.CharacterID = id;
            c.Level = lvl;
            c.XP = 0;
            int[] stats = CalculateStats(c);
            if (hp >= 0)
                c.CurrentHP = hp;
            else
                c.CurrentHP = stats[0];
            if (mp >= 0)
                c.CurrentMP = mp;
            else
                c.CurrentMP = stats[1];
            c.Skill1 = skill1;
            c.Skill2 = skill2;
            c.Skill3 = skill3;
            c.Available = available;
            return c;
        }

        public void LoadBattle(List<OverworldEnemy> enemies)
        {
            state = GameState.Battle;
            battle = new Battle(this);
            string battlefieldName = SearchMap(mapID).BattleFileName;
            play.battle.Init(battlefieldName+".tmx", this, graphics);
            play.Add(play.battle);
            play.Add(play.battleUI);
            play.Remove(play.mapWidget);
            //play.battle.Visibility = Visibility.Visible;
            //play.battleUI.Visibility = Visibility.Visible;
            //play.mapWidget.Visibility = Visibility.Hidden;
            for (int i = 0; i < 5; i++)
            {
                if (playerSaveData.Party[i] != -1)
                {
                    Character c = playerSaveData.CharacterList[playerSaveData.Party[i]];
                    battle.AddFighter(c, battle.allies, i);
                    Console.WriteLine("add ally");
                }
            }
            bool[] occupied = new bool[5] { false, false, false, false, false };
            for (int i = 0; i < 5; i++)
            {
               
                if (i < enemies.Count)
                {
                    OverworldEnemy e = enemies[i];
                    int slot = 0;
                    do
                    {
                        slot = rng.Next(0, 4);
                    }
                    while (occupied[slot] == true);
                    occupied[slot] = true;
                    battle.AddFighter(e, battle.enemies, slot);
                    Console.WriteLine("add enemy");
                }
            }
            Vector2 viewportPosition = new Vector2(play.battle.CurrentMap.ObjectGroups["spots"].Objects["field"].X, play.battle.CurrentMap.ObjectGroups["spots"].Objects["field"].Y);
            play.battle.CameraObject = play.battle.CurrentMap.ObjectGroups["spots"].Objects["field"];
            play.battleUI.RefreshFighters();
            battle.TurnCycle();
        }

        public void CreateNewGame(int saveID)
        {
            saveFileID = saveID;
            state = GameState.Playing;
            Character c = CreateCharacter(3, 5, -1, -1, 0, 0, 0, true);
            playerSaveData.CharacterList.Add(c);
            int index = playerSaveData.CharacterList.IndexOf(c);
            playerSaveData.Party = new int[5] { -1, -1, index, -1, -1 };
        }

        public void LoadGame(int saveID)
        {
            saveFileID = saveID;
            using (StreamReader sr = new StreamReader("save"+saveFileID+".txt"))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                playerSaveData = serializer.Deserialize<PlayerSaveData>(reader);
            }
            state = GameState.Playing;
        }

        public void SaveGame()
        {
            using (StreamWriter sw = new StreamWriter("save"+saveFileID+".txt"))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer,playerSaveData);
            }
        }

        public Texture2D GetCurrentFrame(OverworldChar charData, Squared.Tiled.Object character, int xOffset)
        {
            string spriteSheetName = charData.SpriteSheet;
            int yOffset = charData.SpriteIndex;
            int frameWidth = 32;
            int frameHeight = 32;
            if (spriteSheetName == "characterSpritesheetLarge")
            {
                frameWidth = 64;
                frameHeight = 64;
            }
            Rectangle sourceRect = new Rectangle(charData.CurrentFrame * frameWidth + xOffset * frameWidth, yOffset * frameHeight, frameWidth, frameHeight);
            Texture2D charTexture = Content.Load<Texture2D>(spriteSheetName);
            //if (charTexture == null)
            //    charTexture = game.Content.Load<Texture2D>("characterSpritesheet");
            Color[] FrameTextureData = new Color[charTexture.Width * charTexture.Height];

            charTexture.GetData(FrameTextureData);

            Color[] test = new Color[frameHeight * frameWidth];

            int count = 0;
            for (int c = sourceRect.Top; c < sourceRect.Bottom; c++)
            {
                for (int r = sourceRect.Left; r < sourceRect.Right; r++)
                {
                    Color colorA = FrameTextureData[r + (c * charTexture.Width)];
                    test[count] = colorA;
                    count++;
                }
            }
            Texture2D frame = new Texture2D(graphics.GraphicsDevice, frameWidth, frameHeight);
            frame.SetData(test);
            character.Texture = frame;

            return frame;
        }

        public Texture2D GetCurrentFrame(Squared.Tiled.Object character, int xOffset)
        {
            string y = character.Properties["id"];
            int yOffset = Convert.ToInt32(y);
            string spriteSheetName = character.Properties["spritesheet"];
            int frameWidth = 32;
            int frameHeight = 32;
            if (spriteSheetName == "characterSpritesheetLarge")
            {
                frameWidth = 64;
                frameHeight = 64;
            }
            Rectangle sourceRect = new Rectangle(xOffset * frameWidth, yOffset * frameHeight, frameWidth, frameHeight);
            Texture2D charTexture = Content.Load<Texture2D>(spriteSheetName);
            Color[] FrameTextureData = new Color[charTexture.Width * charTexture.Height];

            charTexture.GetData(FrameTextureData);

            Color[] test = new Color[frameHeight * frameWidth];

            int count = 0;
            for (int c = sourceRect.Top; c < sourceRect.Bottom; c++)
            {
                for (int r = sourceRect.Left; r < sourceRect.Right; r++)
                {
                    Color colorA = FrameTextureData[r + (c * charTexture.Width)];
                    test[count] = colorA;
                    count++;
                }
            }
            Texture2D frame = new Texture2D(graphics.GraphicsDevice, frameWidth, frameHeight);
            frame.SetData(test);
            character.Texture = frame;

            return frame;
        }

        public Squared.Tiled.Object CreateObject(string name, string spriteSheet, int id, int X, int Y, int width, int height)
        {
            Squared.Tiled.Object obj = new Squared.Tiled.Object();
            obj.Name = name;
            obj.X = X;
            obj.Y = Y;
            obj.Width = width;
            obj.Height = height;
            obj.Properties.Add("spritesheet", spriteSheet);
            obj.Properties.Add("id", id.ToString());
            obj.Texture = GetCurrentFrame(obj, 0);
            //objects.Objects.Add(name, obj);
            return obj;
        }

        public void UpdateMainMenu()
        {
            if (State == GameState.MainMenu)
            {
               uiManager.Root.Content = main;
            }
        }

        public void UpdateOverworld(GameTime gameTime)
        {
            if (State == GameState.Overworld)
            {
                uiManager.Root.Content = menu;
            }
            else if (State == GameState.Playing)
            {
                if (play.trans.Parent == play)
                {
                    Wait(1000);
                    play.TransitionVisible(false);
                }
                uiManager.Root.Content = play;
                bool moving = false;
                if (pause == false)
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.Left))
                    {
                        level.MoveCharacter(player, level.player, 3);
                        moving = true;
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.Right))
                    {
                        level.MoveCharacter(player, level.player, 1);
                        moving = true;
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.Up))
                    {
                        level.MoveCharacter(player, level.player, 0);
                        moving = true;
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.Down))
                    {
                        level.MoveCharacter(player, level.player, 2);
                        moving = true;
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.E))
                    {
                        State = GameState.Overworld;
                        Console.WriteLine("Show OverWorld");
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.Z))
                    {
                        level.Interact();
                    }
                }
                if (moving)
                {
                    foreach (OverworldEnemy enemy in level.enemies)
                    {
                        bool fight = level.CheckCollisionBattle(enemy);
                        if (fight)
                        {
                            pause = true;
                            List<OverworldEnemy> fighters = level.GetEnemies(enemy.Character.X, enemy.Character.Y, 256, true);
                            foreach (OverworldEnemy e in fighters)
                            {
                                GetCurrentFrame(e, e.Character, 29);
                            }
                            LoadBattle(fighters);
                        }
                    }
                    level.SpawnCycle();
                }
                else
                    player.CurrentFrame = 0;
            }
            else if (State == GameState.Dialog)
            {
                if (play.dialogUI.Parent != play)
                    play.Add(play.dialogUI);
                if (dialogEvent != "")
                {
                    switch (npcName)
                    {
                        case "test":
                            switch (dialogEvent)
                            {
                                case "heal":
                                    foreach (Character i in playerSaveData.CharacterList)
                                    {
                                        int[] stats = CalculateStats(i);
                                        i.CurrentHP = stats[0];
                                        i.CurrentMP = stats[1];
                                    }
                                    break;
                            }
                            break;
                    }
                    dialogEvent = "";
                }
                if (newDialogName != null)
                {
                    currentDialog = SearchDialog(newDialogName);
                    if (currentDialog.Name != null)
                    {
                        play.dialogUI.box.Name = currentDialog.CharName;
                        play.dialogUI.box.Text = currentDialog.Text;
                        play.dialogUI.options.NewOptions(currentDialog);
                    }
                    else
                    {
                        if (play.dialogUI.Parent == play)
                            play.Remove(play.dialogUI);
                        State = GameState.Playing;
                    }
                    newDialogName = null;
                }
            }

            //update level
            level.Update(graphics,gameTime);

            if (level.levelOver)
                State = GameState.EndGame;
            
        }

        bool battleKeyDown = false;
        public void UpdateBattle(GameTime gameTime) 
        {
            if (battle.state == BattleState.Regular)
            {
                currentFighter = battle.order[battle.turnNumber];
                BattleAction action;
                bool playerControlled = false;
                if (Keyboard.GetState().IsKeyDown(Keys.Up) && battleKeyDown == false)
                {
                    battleKeyDown = true;
                    play.battleUI.commands.MoveSelection(true);
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.Down) && battleKeyDown == false)
                {
                    battleKeyDown = true;
                    play.battleUI.commands.MoveSelection(false);
                }
                else if (!(Keyboard.GetState().IsKeyDown(Keys.Up) || (Keyboard.GetState().IsKeyDown(Keys.Down))))
                {
                    battleKeyDown = false;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Down))
                {
                    level.MoveCharacter(player, level.player, 2);
                }
                if (battle.GetTeam(currentFighter) == battle.allies)
                    playerControlled = true;
                if (playerControlled)
                {
                    if (play.battleUI.commands.Parent != play.battleUI)
                    {
                        play.battleUI.Add(play.battleUI.commands, DockPanelConstraint.Left);
                        play.battleUI.commands.ResetVisibility();
                        Attack atk1 = SearchAttack(currentFighter.Skill1);
                        Attack atk2 = SearchAttack(currentFighter.Skill2);
                        Attack atk3 = SearchAttack(currentFighter.Skill3);
                        play.battleUI.commands.UpdateAttacks(atk1, atk2, atk3);
                    }
                    //play.battleUI.commands.Visibility = Visibility.Visible;
                    if (battle.playerAction != null)
                    {
                        Console.WriteLine("player turn");
                        battle.Turn(currentFighter, battle.playerAction);
                        play.battleUI.Remove(play.battleUI.commands);
                        //play.battleUI.commands.Visibility = Visibility.Hidden;
                        battle.playerAction = null;
                    }
                }
                else
                {
                    action = battle.Ai(currentFighter);
                    Console.WriteLine("CHOSEN TARGET:" + action.target[0] + action.target[1] + action.target[2] + action.target[3] + action.target[4]);
                    battle.Turn(currentFighter, action);
                    Console.WriteLine("EnemyTurn");
                }
                BattleResult battleOutcome = battle.CheckVictory();
                if (battleOutcome != BattleResult.None)
                {
                    Console.WriteLine("dieded" + battleOutcome.ToString());
                    battle.EndBattle(battleOutcome);
                }
            }
            else if (battle.state == BattleState.Animation)
            {
                battle.elapsedTime += gameTime.ElapsedGameTime.Milliseconds;
                int xOffset = GetXOffsetOfAnimation(battle.Animation[battle.currentKeyFrame].CharacterAnimation);
                if (battle.tempFighter != null)
                    battle.tempFighter.Char.Texture = GetCurrentFrame(battle.tempFighter.Char, xOffset);
                xOffset = GetXOffsetOfAnimation(battle.Animation[battle.currentKeyFrame].TargetAnimation);
                for (int i = 0; i < 5; i++)
                {
                    if (battle.tempEnemyTeam[i] != null && battle.Animation[battle.currentKeyFrame].Selection[i] == true)
                    {
                        battle.tempEnemyTeam[i].Char.Texture = GetCurrentFrame(battle.tempEnemyTeam[i].Char, xOffset);
                    }
                }
                if (battle.elapsedTime > 200 && battle.incrementS == false)
                {
                    battle.currentFrameS++;
                    battle.incrementS = true;
                }
                if (battle.elapsedTime > 400)
                {
                    battle.elapsedTime = 0;
                    battle.currentFrameS++;
                    battle.currentFrameC++;
                }
                if (battle.currentFrameC >= 2)
                {
                    battle.elapsedTime = 0;
                    battle.currentFrameS = 0;
                    battle.currentFrameC = 0;
                    battle.currentKeyFrame++;
                    if (battle.currentKeyFrame >= battle.Animation.Count)
                    {
                        battle.state = BattleState.Regular;
                        if (battle.endingBattle == false)
                            battle.Attack(battle.tempFighter, battle.tempEnemyTeam, battle.tempSelection, battle.tempAtkData);
                        else
                            battle.ResumeEndBattle();
                        battle.UpdateFighters();
                        play.battleUI.Message("");
                    }
                }
            }
        }

        public int GetXOffsetOfAnimation(CharAnimation animation)
        {
            int xOffset = 0;
            if (animation == CharAnimation.None)
            {
                xOffset = 12;
                if (battle.GetTeam(battle.tempFighter) == battle.enemies)
                    xOffset = 4;
            }
            else if (animation == CharAnimation.Physical)
            {
                xOffset = 16 + battle.currentFrameC;
                if (battle.GetTeam(battle.tempFighter) == battle.enemies)
                    xOffset = 22 + battle.currentFrameC;
            }
            else if (animation == CharAnimation.Ranged)
            {
                xOffset = 18 + battle.currentFrameC;
                if (battle.GetTeam(battle.tempFighter) == battle.enemies)
                    xOffset = 24 + battle.currentFrameC;
            }
            else if (animation == CharAnimation.Hit)
            {
                xOffset = 20;
                if (battle.GetTeam(battle.tempFighter) == battle.enemies)
                    xOffset = 26;
            }
            else if (animation == CharAnimation.Happy)
            {
                xOffset = 28 + battle.currentFrameC;
            }
            return xOffset;
        }

        public void UpdateEndGame()
        {

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // TODO: Add your drawing code here
            switch (state)
            {
                case GameState.MainMenu:
                    DrawMainMenu();
                    break;
                case GameState.Overworld:
                    DrawOverworld();
                    break;
                case GameState.Battle:
                    DrawBattle();
                    break;
                case GameState.EndGame:
                    DrawEndGame();
                    break;
                case GameState.Playing:
                case GameState.Dialog:
                    break;
            }

            base.Draw(gameTime);
        }

        public void DrawMainMenu()
        {

        }

        public void DrawOverworld()
        {

        }

        public void DrawBattle()
        {

        }

        public void DrawEndGame()
        {

        }

        
    }
}
