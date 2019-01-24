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
            Battle,
            EndGame
        }
        private GameState state = GameState.MainMenu;
        public GameState State
        {
            get { return state; }
            set
            {
                state = value;
                //switchstate
            }
        }
        public enum InGameMenuState
        {
            Closed,
            Open,
            Party,
            Inventory_Consumables,
            Inventory_Key,
            Options
        }

        string mainMenuState = "Title";
        Texture2D pixel;
        public OverworldPlayer player = new OverworldPlayer();
        public PlayerSaveData playerSaveData = new PlayerSaveData();
        //Screens
        MainMenu main;
        OverWorldMenu menu;
        public Playing play;
        public Level level;
        int elapsedTime = 0;
        
        public List<Attack> attacks = new List<Attack>();
        public List<CharData> characters = new List<CharData>();
        public List<MapData> maps = new List<MapData>();
        public List<MapChar> mapChar = new List<MapChar>();
        public List<CharAttack> charAtk = new List<CharAttack>();
        static JsonSerializer serializer = new JsonSerializer();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1080;
            graphics.PreferredBackBufferHeight = 600;
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
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            uiManager = UIManagerComponent.CreateAndInit(this, new InputManager(this), "Content").Manager;
            
            var styleSystem = uiManager.UIStyle;
            var styles = styleSystem.LoadStyles("Content/UI/Metro/style.xml", "UI/Metro", GraphicsDevice);
            styleSystem.StyleResolver.StyleRules.AddRange(styles);

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
            play.mapWidget.Init("testMap1.tmx", this, graphics);
            
            level.Init(this, 50, play.mapWidget.CurrentMap);
            pixel = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            pixel.SetData(new[] { Color.White });

            //Default Player Values
            player.Direction = 2;
            player.AnimationSpeed = 100;
            player.FrameCount = 4;

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
            elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (elapsedTime > 10000)
            {
                level.SpawnCycle();
                elapsedTime = 0;
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
                    UpdateOverworld(gameTime);
                    break;
                case GameState.Battle:
                    UpdateBattle();
                    break;
                case GameState.EndGame:
                    UpdateEndGame();
                    break;
            }
            base.Update(gameTime);
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
            Console.WriteLine("waited");
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
                if (play.trans.Visibility == Visibility.Visible)
                {
                    Wait(1000);
                    play.TransitionVisible(false);
                }
                uiManager.Root.Content = play;
                bool moving = false;
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
                if (!moving)
                    player.CurrentFrame = 0;
                if (Keyboard.GetState().IsKeyDown(Keys.E))
                {
                    State = GameState.Overworld;
                    Console.WriteLine("Show OverWorld");
                }
                if (Keyboard.GetState().IsKeyDown(Keys.M))
                {
                    float newScale = play.mapWidget.CurrentMap.zoom * 2;
                    if (newScale <= 8)
                        play.mapWidget.CurrentMap.zoom = newScale;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.N))
                {
                    float newScale = play.mapWidget.CurrentMap.zoom / 2;
                    if (newScale >= 0.25)
                        play.mapWidget.CurrentMap.zoom = newScale;
                }
            }

            //update level
            level.Update(graphics,gameTime);

            if (level.levelOver)
                State = GameState.EndGame;
            
        }

        public void UpdateBattle() 
        {

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
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            switch (state)
            {
                case GameState.MainMenu:
                    DrawMainMenu();
                    break;
                case GameState.Overworld:
                    DrawOverworld();
                    spriteBatch.Begin();
                    
                    spriteBatch.End();
                    break;
                case GameState.Battle:
                    DrawBattle();
                    break;
                case GameState.EndGame:
                    DrawEndGame();
                    break;
                case GameState.Playing:                    
                    spriteBatch.Begin();
                    //draws level
                    level.Draw(spriteBatch, graphics.GraphicsDevice);
                    spriteBatch.End();
                    break;
            }

            base.Draw(gameTime);
        }

        public void DrawMainMenu()
        {
            GraphicsDevice.Clear(Color.Black);
            switch (mainMenuState)
            {
                case "Title":

                    
                    break;
            }

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
