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
        public MapData mapData;
        public Squared.Tiled.Object cameraFocus;
        Layer collision;
        ObjectGroup objects;
        List<Squared.Tiled.Object> telePortLocations = new List<Squared.Tiled.Object>();
        ObjectGroup teleports;
        ObjectGroup teleportLocations;
        public Vector2 viewportPosition;
        int tilepixel;
        Game1 game;
        public Squared.Tiled.Object player;
        Random rng = new Random();
        List<OverworldEnemy> enemies = new List<OverworldEnemy>();
        List<MapChar> enemySpawnPool = new List<MapChar>();
        int totalEnemyCount = 0;
        int noOfTurns;
        public bool levelOver;

        public void Init(Game1 test, int not, Map mapName)
        {
            game = test;
            noOfTurns = not;
            levelOver = false;
            LoadMap(mapName,0);
        }

        private void LoadMap(Map mapName,int mapID)
        {
            game.mapID = mapID;
            map = mapName;
            collision = mapName.Layers["1collision"];
            teleports = mapName.ObjectGroups["teleport"];
            teleportLocations = mapName.ObjectGroups["teleportDestination"];
            objects = mapName.ObjectGroups["5objects"];
            mapData = game.SearchMap(mapID);
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
            List<MapChar> enemySpawnList = game.FilterMapChar(mapID);
            enemySpawnPool = new List<MapChar>();
            foreach (MapChar x in enemySpawnList)
            {
                for (int i = 0; i < x.Weight*10; i++)
                {
                    enemySpawnPool.Add(x);
                }
            }
            Console.WriteLine("Load Map char count:" +game.mapChar.Count);
            for (int i = 0; i < 4; i++)
            {
                MapChar selected = enemySpawnPool[rng.Next(0, enemySpawnPool.Count)];
                OverworldEnemy newEnemy = CreateEnemy(game.characters[selected.CharID]);
                enemies.Add(newEnemy);
            }
        }
        
        private void ChangeMap(Squared.Tiled.Object entity)
        {
            game.play.TransitionVisible(true);
            string tempID;
            string destination;
            entity.Properties.TryGetValue("mapID", out tempID);
            int newMapID = Convert.ToInt32(tempID);
            mapData = game.SearchMap(newMapID);
            game.play.trans.Text = mapData.Name;
            entity.Properties.TryGetValue("destination", out destination);
            ChangeMap(mapData.MapFileName, destination, newMapID);
            
        }
        private void ChangeMap(string newMapName, string destination, int mapID)
        {
            game.play.mapWidget.Init(newMapName + ".tmx", game, game.graphics);
            LoadMap(game.play.mapWidget.CurrentMap,mapID);
            Squared.Tiled.Object dest = teleportLocations.Objects[destination];
            player.X = dest.X + player.Width/2;
            player.Y = dest.Y + player.Height/2;
        }

        public void Update(GraphicsDeviceManager g, GameTime gameTime)
        {
            if (game.pause == false)
            {
                cameraFocus = player;
                game.play.mapWidget.CameraObject = cameraFocus;
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
                    enemy.Animation(gameTime);
                    bool fight = CheckCollisionBattle(enemy);
                    if (fight)
                    {
                        game.pause = true;
                        List<OverworldEnemy> fighters = GetEnemies(enemy.Character.X, enemy.Character.Y, 256);
                        foreach (OverworldEnemy e in fighters)
                        {
                            game.GetCurrentFrame(e, e.Character, 29);
                        }
                        game.LoadBattle(fighters);
                    }
                }
                game.player.Animation(gameTime);
            }
        }

        public void Draw(SpriteBatch s, GraphicsDevice g)
        {
            //map.Draw(s, new Rectangle(0, 0, g.Viewport.Width, g.Viewport.Height), viewportPosition);

        }

        public bool MoveCharacter(OverworldChar characterData, Squared.Tiled.Object character, int direction)
        {
            bool successfulMove = false;
            int xOffset = 0;
            if (direction == 0) //up
            {
                xOffset = 8;
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
                xOffset = 4;
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
                xOffset = 12;
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
            character.Texture = game.GetCurrentFrame(characterData, character,xOffset);
            return successfulMove;
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
                        double magnitude = Math.Sqrt(Math.Pow((playerPos.X-tilePos.X),2) + Math.Pow((playerPos.Y - tilePos.Y), 2));
                        if (magnitude >= 128.0)
                        {
                            spawns[spawnCount] = tilePos;
                            spawnCount++;
                        }
                    }
                }
            }
            position = spawns[rng.Next(0,spawnCount)];
            return position;
        }

        private void CreatePlayer(int X, int Y)
        {
            player = game.CreateObject("player", "characterSpritesheet", 3, X, Y, 32, 32);
            objects.Objects.Add("player", player);
        }

        private OverworldEnemy CreateEnemy(CharData data)
        {
            OverworldEnemy enemy = new OverworldEnemy();
            Vector2 pos = FindSpawnLocation();
            int size = 32;
            if (data.SpriteSheet == "characterSpritesheetLarge")
            {
                size = 64;
            }
            Squared.Tiled.Object character = game.CreateObject("enemy"+totalEnemyCount, data.SpriteSheet, data.SpriteIndex, Convert.ToInt32(pos.X)+size/2, Convert.ToInt32(pos.Y)+size/2, size, size);
            objects.Objects.Add("enemy" + totalEnemyCount, character);
            enemy.Character = character;
            enemy.WalkSpeed = data.PassiveSpeed;
            enemy.PassiveSpeed = data.PassiveSpeed;
            enemy.AgressiveSpeed = data.AgressiveSpeed;
            enemy.FrameCount = 4;
            enemy.AnimationSpeed = 100;
            enemy.SpriteSheet = data.SpriteSheet;
            enemy.SpriteIndex = data.SpriteIndex;
            enemy.CharacterID = data.CharID;
            totalEnemyCount++;
            return enemy;
        }

        public void SpawnCycle()
        {
            Vector2 playerPos = new Vector2(player.X, player.Y);
            List<OverworldEnemy> toRemove = new List<OverworldEnemy>();
            foreach (OverworldEnemy i in enemies)
            {
                Vector2 charPos = new Vector2(i.Character.X, i.Character.Y);
                double magnitude = Math.Sqrt(Math.Pow((playerPos.X - charPos.X), 2) + Math.Pow((playerPos.Y - charPos.Y), 2));
                if (magnitude >= 512.0)
                {
                    if (rng.Next(0,100) < 50)
                    {
                        toRemove.Add(i);
                    }
                }
            }
            foreach (OverworldEnemy i in toRemove)
            {
                objects.Objects.Remove(i.Character.Name);
                enemies.Remove(i);
            }
            if (enemies.Count < mapData.SpawnCap)
            {
                MapChar selected = enemySpawnPool[rng.Next(0, enemySpawnPool.Count)];
                OverworldEnemy newEnemy = CreateEnemy(game.characters[selected.CharID]);
                enemies.Add(newEnemy);
            }
        }

        private bool CheckCollisionBattle(OverworldEnemy enemy)
        {
            bool check = false;
            Rectangle playrec = new Rectangle(
               player.X-1,
               player.Y-1,
               player.Width+2,
               player.Height+2
               );
            Rectangle enemyrec = new Rectangle(
                enemy.Character.X,
                enemy.Character.Y,
                enemy.Character.Width,
                enemy.Character.Height
                );
            if (playrec.Intersects(enemyrec))
            {
                check = true;
            }
            return check;
        }

        private List<OverworldEnemy> GetEnemies(int x, int y, int range)
        {
            List<OverworldEnemy> fighters = new List<OverworldEnemy>();
            foreach (OverworldEnemy e in enemies)
            {
                double magnitude = Math.Sqrt(Math.Pow((e.Character.X - x), 2) + Math.Pow((e.Character.Y - y), 2));
                if (magnitude <= range)
                {
                    fighters.Add(e);
                }
            }
            return fighters;
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
                {
                    check = true;
                }
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
