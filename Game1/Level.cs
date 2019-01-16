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
using System.Xml;

namespace Game1
{
    public class Level
    {
        public Map map;
        public Squared.Tiled.Object cameraFocus;
        Layer collision;
        ObjectGroup objects;
        List<Squared.Tiled.Object> telePortLocations = new List<Squared.Tiled.Object>();
        ObjectGroup teleports;
        ObjectGroup teleportLocations;
        Vector2 viewportPosition;
        int tilepixel;
        Game1 game;
        public Squared.Tiled.Object player;
        Dictionary<string,Texture2D> textures = new Dictionary<string,Texture2D>();
        Rectangle sourceRect = new Rectangle();
        Random rng = new Random();
        List<OverworldEnemy> enemies = new List<OverworldEnemy>();
        XmlDocument xmlDoc = new XmlDocument();
        XmlNodeList xmlNodes;
        int totalEnemyCount = 0;

        int noOfTurns;
        public bool levelOver;

        public string[] DatabaseSearch(string tableName, string searchFieldName, string searchValue, string[] returnFieldName)
        {
            string[] values = new string[returnFieldName.Length];
            xmlNodes = xmlDoc.SelectNodes(tableName);
            Console.WriteLine("Search started");
            Console.WriteLine(xmlNodes.Count);
            foreach (XmlNode node in xmlNodes)
            {
                string foundValue = node.SelectSingleNode(searchFieldName).Value;
                Console.WriteLine(foundValue);
                if (foundValue == searchValue)
                {
                    for (int i= 0; i < returnFieldName.Length; i++)
                    { 
                        values[i] = node.SelectSingleNode(returnFieldName[i]).Value;
                    }
                    break;
                }
            }
            Console.WriteLine("Search ended");
            return values;
        }
        
        public void Init(Game1 test, int not, Map mapName)
        {
            game = test;
            noOfTurns = not;
            levelOver = false;
            FileStream fs = new FileStream("Database.xml", FileMode.Open, FileAccess.Read);
            xmlDoc.Load(fs);
            LoadMap(mapName);
        }

        private void LoadMap(Map mapName)
        {
            map = mapName;
            collision = mapName.Layers["1collision"];
            teleports = mapName.ObjectGroups["teleport"];
            teleportLocations = mapName.ObjectGroups["teleportDestination"];
            objects = mapName.ObjectGroups["5objects"];
            //if (player != null)
            //    objects.Objects.Remove(player.Name);
            foreach (OverworldEnemy enemy in enemies)
                objects.Objects.Remove(enemy.Character.Name);
            enemies.Clear();
            tilepixel = mapName.TileWidth;
            if (teleportLocations.Objects.ContainsKey("defaultPosition"))
            {
                Squared.Tiled.Object dest = teleportLocations.Objects["defaultPosition"];
                CreatePlayer(dest.X, dest.Y);
            }
            else
                CreatePlayer(0, 0);
            List<int> enemySpawnList = new List<int>();
            xmlNodes = xmlDoc.SelectNodes("/mapchar");
            foreach(XmlNode node in xmlNodes)
            {
                string mapId = node.Attributes.GetNamedItem("MapID").Value;
            }
            for (int i = 0; i < 4; i++)
            {
                OverworldEnemy newEnemy = CreateEnemy();
                enemies.Add(newEnemy);
            }
        }

        private void ChangeMap(Squared.Tiled.Object entity)
        {
            game.play.TransitionVisible(true);
            string newMapID;
            string destination;
            entity.Properties.TryGetValue("mapID", out newMapID);
            string[] found = DatabaseSearch("map","MapID",newMapID,new string[]{"Name","MapFileName"});
            game.play.trans.Text = found[0];
            entity.Properties.TryGetValue("destination", out destination);
            ChangeMap(found[1], destination);
            game.play.TransitionVisible(false);
        }
        private void ChangeMap(string newMapName, string destination)
        {
            game.play.mapWidget.Init(newMapName + ".tmx", game, game.graphics);
            LoadMap(game.play.mapWidget.CurrentMap);
            Squared.Tiled.Object dest = teleportLocations.Objects[destination];
            player.X = dest.X + player.Width/2;
            player.Y = dest.Y + player.Height/2;
        }

        private void CreatePlayer(int X, int Y)
        {
            player = CreateObject("player", "characterSpritesheet", 0, X, Y, 16, 16);
        }

        public void Update(GraphicsDeviceManager g)
        {
            cameraFocus = player;
            viewportPosition = new Vector2(cameraFocus.X - (g.PreferredBackBufferWidth / 2), cameraFocus.Y - (g.PreferredBackBufferHeight / 2));
            foreach (OverworldEnemy enemy in enemies)
            {
                if (enemy.AIType == "Default")
                {
                    bool successful = MoveCharacter(enemy, enemy.Character, enemy.Direction);
                    if (!successful)
                    {
                       int random = rng.Next(0, 300);
                       if (random <= 75)
                          enemy.Direction = 0;
                       else if (random <= 150)
                          enemy.Direction = 1;
                       else if (random <= 225)
                          enemy.Direction = 2;
                       else
                          enemy.Direction = 3;
                    }
                }
            }
        }

        public void Draw(SpriteBatch s, GraphicsDevice g)
        {
            //map.Draw(s, new Rectangle(0, 0, g.Viewport.Width, g.Viewport.Height), viewportPosition);

        }

        public Texture2D GetCurrentFrame(Squared.Tiled.Object character, int xOffset)
        {
            string y;
            character.Properties.TryGetValue("id", out y);
            int yOffset = Convert.ToInt32(y);
            string spriteSheetName;
            character.Properties.TryGetValue("spritesheet", out spriteSheetName);
            int frameWidth = 16;
            int frameHeight = 16;
            if (spriteSheetName == "characterSpritesheetLarge")
            {
                frameWidth = 64;
                frameHeight = 64;
            }
            sourceRect = new Rectangle(game.currentFrame * frameWidth + xOffset * frameWidth * game.frameCount, yOffset * frameHeight, frameWidth, frameHeight);
            Texture2D charTexture = game.Content.Load<Texture2D>(spriteSheetName);
            //charTexture = textures[spriteSheetName];
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
            Texture2D frame = new Texture2D(game.graphics.GraphicsDevice,frameWidth,frameHeight);//game.Content.Load<Texture2D>("frametext");
            frame.SetData(test);
            character.Texture = frame;
            
            return frame;
        }

        public bool MoveCharacter(OverworldChar characterData, Squared.Tiled.Object character, int direction)
        {
            bool successfulMove = false;
            int xOffset = 0;
            if (direction == 0) //up
            {
                xOffset = 2;
                for (int i = 0; i < characterData.WalkSpeed; i++)
                {
                    character.Y -= 1;
                    characterData.Direction = direction;
                    if (CheckBounds(character))
                    {
                        character.Y += 1;
                        break;
                    }
                    else
                        successfulMove = true;
                }
            }
            else if (direction == 1) //right
            {
                xOffset = 1;
                for (int i = 0; i < characterData.WalkSpeed; i++)
                {
                    character.X += 1;
                    characterData.Direction = direction;
                    if (CheckBounds(character))
                    {
                        character.X -= 1;
                        break;
                    }
                    else
                        successfulMove = true;
                }
            }
            else if (direction == 2) //down
            {
                xOffset = 0;
                for (int i = 0; i < characterData.WalkSpeed; i++)
                {
                    character.Y += 1;
                    characterData.Direction = direction;
                    if (CheckBounds(character))
                    {
                        character.Y -= 1;
                        break;
                    }
                    else
                        successfulMove = true;
                }
            }
            else if (direction == 3) //left
            {
                xOffset = 3;
                for (int i = 0; i < characterData.WalkSpeed; i++)
                {
                    character.X -= 1;
                    characterData.Direction = direction;
                    if (CheckBounds(character))
                    {
                        character.X += 1;
                        break;
                    }
                    else
                        successfulMove = true;
                }
            }
            character.Texture = GetCurrentFrame(character,xOffset);
            characterData.Position = new int[] { character.X, character.Y };
            return successfulMove;
        }

        private Squared.Tiled.Object CreateObject(string name, string spriteSheet, int id, int X, int Y, int width, int height)
        {
            Squared.Tiled.Object obj = new Squared.Tiled.Object();
            obj.Name = name;
            obj.X = X;
            obj.Y = Y;
            obj.Width = width;
            obj.Height = height;
            obj.Properties.Add("spritesheet", spriteSheet);
            obj.Properties.Add("id", id.ToString());
            if (!textures.ContainsKey(spriteSheet))
            {
                textures.Add(spriteSheet, game.Content.Load<Texture2D>(spriteSheet));
            }
            obj.Texture = GetCurrentFrame(obj,0);
            objects.Objects.Add(name, obj);
            return obj;
        }

        private Vector2 FindSpawnLocation()
        {
            Vector2 position;
            Vector2[] spawns = new Vector2[map.Width * map.Height];
            Vector2 playerPos = new Vector2(player.X-player.Width/2,player.Y-player.Height/2);
            int spawnCount = 0;
            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    if (collision.GetTile(x, y) == 84)
                    {
                        Vector2 tilePos = new Vector2(x * tilepixel, y * tilepixel);
                        double magnitude = Math.Sqrt(Math.Pow((playerPos.X-tilePos.X),2)+ Math.Pow((playerPos.Y - tilePos.Y), 2));
                        if (magnitude >= 32.0)
                        {
                            spawns[spawnCount] = new Vector2(x * tilepixel, y * tilepixel);
                            spawnCount++;
                        }
                    }
                }
            }
            position = spawns[rng.Next(0,spawnCount)];
            return position;
        }

        private OverworldEnemy CreateEnemy()
        {
            OverworldEnemy enemy = new OverworldEnemy();
            Vector2 pos = FindSpawnLocation();
            int id = (rng.Next(1, 150) / 10);
            Squared.Tiled.Object character = CreateObject("enemy"+totalEnemyCount, "characterSpritesheet", id, Convert.ToInt16(pos.X), Convert.ToInt16(pos.Y), 16, 16);
            enemy.Character = character;
            totalEnemyCount++;
            return enemy;
        }

        private bool CheckBounds(Squared.Tiled.Object character)
        {
            bool check = false;

            Rectangle playrec = new Rectangle(
                character.X-character.Width/2,
                character.Y-character.Height/2,
                character.Width,
                character.Height
                );

            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    if (collision.GetTile(x, y) != 0 && collision.GetTile(x, y) != 84)
                    {
                        Rectangle tile = new Rectangle(
                            (int)x * tilepixel,
                            (int)y * tilepixel,
                            tilepixel,
                            tilepixel
                            );

                        if (playrec.Intersects(tile))
                            check = true;
                    }
                }
            }
            foreach (KeyValuePair<string, Squared.Tiled.Object> entity in objects.Objects.ToArray())
            {
                Rectangle objrec = new Rectangle(entity.Value.X-entity.Value.Width/2, entity.Value.Y-entity.Value.Height/2, entity.Value.Width, entity.Value.Height);
                string collidableV;
                entity.Value.Properties.TryGetValue("Collidable", out collidableV);
                bool collidable = true; //Convert.ToBoolean(collidableV);
                if (entity.Value != character && playrec.Intersects(objrec) && collidable == true)
                    check = true;
            }
            if (character == player)
            {
                foreach (KeyValuePair<string, Squared.Tiled.Object> entity in teleports.Objects.ToArray())
                {
                    Rectangle objrec = new Rectangle(entity.Value.X, entity.Value.Y, entity.Value.Width, entity.Value.Height);
                    if (playrec.Intersects(objrec))
                    {
                        ChangeMap(entity.Value);
                        break;
                    }
                }
            }
            return check;
        }
    }
}
