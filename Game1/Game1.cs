using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
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

namespace Game1
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
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
        private GameState state = GameState.Playing;
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
        OverworldPlayer player = new OverworldPlayer();
        //Screens
        MainMenu main;
        OverWorldMenu menu;
        public Playing play;
        public Level level;

        public int elapsedTime;
        public int frameCount;
        public int currentFrame;
        //public int FrameWidth;
        //public int FrameHeight;

        XmlDocument xmlDoc = new XmlDocument();
        XmlNodeList xmlNodes;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1280;
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

            elapsedTime = 0;
            currentFrame = 0;
            frameCount = 4;

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
            if (elapsedTime > 100)
            {
                // Move to the next frame
                currentFrame++;

                // If the currentFrame is equal to frameCount reset currentFrame to zero
                if (currentFrame == frameCount)
                {
                    currentFrame = 0;
                }

                // Reset the elapsed time to zero
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
                    UpdateOverworld();
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

        public void UpdateMainMenu()
        {
            if (State == GameState.MainMenu)
            {
               uiManager.Root.Content = main;
            }
        }

        public void UpdateOverworld()
        {
            if (State == GameState.Overworld)
            {
                uiManager.Root.Content = menu;
            }
            else if (State == GameState.Playing)
            {
                uiManager.Root.Content = play;
                if (Keyboard.GetState().IsKeyDown(Keys.Left))
                    level.MoveCharacter(player, level.player, 3);
                if (Keyboard.GetState().IsKeyDown(Keys.Right))
                    level.MoveCharacter(player, level.player, 1);
                if (Keyboard.GetState().IsKeyDown(Keys.Up))
                    level.MoveCharacter(player, level.player, 0);
                if (Keyboard.GetState().IsKeyDown(Keys.Down))
                    level.MoveCharacter(player, level.player, 2);
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
            level.Update(graphics);

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
