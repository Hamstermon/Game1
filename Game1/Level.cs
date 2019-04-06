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
        List<Node> nodes;
        public MapData mapData;
        public Squared.Tiled.Object cameraFocus;
        Layer collision;
        public ObjectGroup objects;
        List<Squared.Tiled.Object> telePortLocations = new List<Squared.Tiled.Object>();
        ObjectGroup teleports;
        ObjectGroup teleportLocations;
        public Vector2 viewportPosition;
        int tilepixel;
        Game1 game;
        public Squared.Tiled.Object player;
        Random rng = new Random();
        public List<OverworldEnemy> enemies = new List<OverworldEnemy>();
        List<MapChar> enemySpawnPool = new List<MapChar>();
        int totalEnemyCount = 0;
        int noOfTurns;
        public bool levelOver;

        public void Init(Game1 test, int not, Map mapName, int x, int y)
        {
            game = test;
            noOfTurns = not;
            levelOver = false;
            LoadMap(mapName,0,x,y);
        }

        private void LoadMap(Map mapName,int mapID,int x, int y)
        {
            game.mapID = mapID;
            map = mapName;
            collision = mapName.Layers["1collision"];
            teleports = mapName.ObjectGroups["teleport"];
            teleportLocations = mapName.ObjectGroups["teleportDestination"];
            objects = mapName.ObjectGroups["5objects"];
            mapData = game.SearchMap(mapID);
            nodes = GetNodes();
            //if (player != null)
            //    objects.Objects.Remove(player.Name);
            foreach (OverworldEnemy enemy in enemies)
                objects.Objects.Remove(enemy.Character.Name);
            enemies.Clear();
            tilepixel = mapName.TileWidth;
            if (x != -1 && y != -1)
            {
                CreatePlayer(x, y);
            }
            else if (teleportLocations.Objects.ContainsKey("defaultPosition"))
            {
                Squared.Tiled.Object dest = teleportLocations.Objects["defaultPosition"];
                CreatePlayer(dest.X, dest.Y);
            }
            else
                CreatePlayer(0, 0);
            for (int i = 0; i < objects.Objects.Values.Count; i++)
            {
                Squared.Tiled.Object npc = objects.Objects.Values[i];
                if (npc.Type == "NPC")
                {
                    string name = npc.Properties["npc"];
                    string spritesheet = npc.Properties["spritesheet"];
                    int index = Convert.ToInt32(npc.Properties["id"]);
                    int xOffset = 0;
                    if (npc.Properties["direction"] == "0")
                        xOffset = 8;
                    else if (npc.Properties["direction"] == "1")
                        xOffset = 4;
                    else if (npc.Properties["direction"] == "2")
                        xOffset = 0;
                    else if (npc.Properties["direction"] == "3")
                        xOffset = 12;
                    npc.Texture = game.GetCurrentFrame(npc, xOffset);
                    bool visible = IsNPCVisible(npc);
                    if (!visible)
                    {
                        Console.WriteLine("removal xd");
                        objects.Objects.Remove(npc.Name);
                    }
                }
            }
            List<MapChar> enemySpawnList = game.FilterMapChar(mapID);
            enemySpawnPool = new List<MapChar>();
            foreach (MapChar m in enemySpawnList)
            {
                for (int i = 0; i < m.Weight*10; i++)
                {
                    enemySpawnPool.Add(m);
                }
            }
            Console.WriteLine("Load Map char count:" +game.mapChar.Count);
            for (int i = 0; i < 3; i++)
            {
                MapChar selected = enemySpawnPool[rng.Next(0, enemySpawnPool.Count)];
                OverworldEnemy newEnemy = CreateEnemy(game.characters[selected.CharID], rng.Next(selected.MinLevel * 100, selected.MaxLevel * 100) / 100);
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
            LoadMap(game.play.mapWidget.CurrentMap,mapID,-1,-1);
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
                //int x = cameraFocus.X - (g.PreferredBackBufferWidth / 2) - (cameraFocus.Width / 2);
                //int y = cameraFocus.Y - (g.PreferredBackBufferHeight / 2) - (cameraFocus.Height / 2);

                //if (x + (g.PreferredBackBufferWidth / 2) + (cameraFocus.Width / 2) > map.Width * map.TileWidth)
                //{
                //    x = map.Width * map.TileWidth;
                //}
                //else if (x - (g.PreferredBackBufferWidth / 2) - (cameraFocus.Width / 2) < 0)
                //{
                //    x = 0;
                //}
                //viewportPosition = new Vector2(x, y);
                foreach (OverworldEnemy enemy in enemies)
                {
                    if (enemy.Despawn == false)
                    {
                        string AIType = enemy.AIType;
                        if (AIType == "Aggressive")
                        {
                            float distance = (float)Math.Sqrt(Math.Pow(player.X - enemy.Character.X,2)+Math.Pow(player.Y-enemy.Character.Y,2));
                            if (distance < 96)
                            {
                                //Pathfind();
                            }
                            else
                                AIType = "Default";

                        }
                        if (AIType == "Default")
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
                    }
                }
                game.player.Animation(gameTime);
            }
        }

        public void Draw(SpriteBatch s, GraphicsDevice g)
        {
            //map.Draw(s, new Rectangle(0, 0, g.Viewport.Width, g.Viewport.Height), viewportPosition);

        }

        class Node
        {
            Vector2 pos;
            public Vector2 Position
            {
                set { pos = value; }
                get { return pos; }
            }
            bool[] adj;
            public bool[] Adjacent
            {
                set { adj = value; }
                get { return adj; }
            }
        }

        private List<Node> GetNodes()
        {
            List<Node> nodes = new List<Node>();
            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    if (collision.GetTile(x, y) == 6)
                    {
                        Node temp = new Node();
                        temp.Position = new Vector2(x, y);
                        temp.Adjacent = new bool[4] { false, false, false, false };
                        if (x - 1 >= 0 && collision.GetTile(x - 1, y) == 6)
                        {
                            temp.Adjacent[1] = true;
                        }
                        if (x + 1 <= map.Width && collision.GetTile(x + 1, y) == 6)
                        {
                            temp.Adjacent[3] = true;
                        }
                        if (y - 1 >= 0 && collision.GetTile(x, y - 1) == 6)
                        {
                            temp.Adjacent[2] = true;
                        }
                        if (y + 1 <= map.Height && collision.GetTile(x, y + 1) == 6)
                        {
                            temp.Adjacent[0] = true;
                        }
                    }
                }
            }
            return nodes;
        }

        private Node FindNode(Vector2 pos)
        {
            Node node = new Node();
            foreach (Node i in nodes)
            {
                if (i.Position == pos)
                {
                    node = i;
                    break;
                }
            }
            return node;
        }

        private Node NodeFromObject(Squared.Tiled.Object character)
        {
            double x = character.X / map.TileWidth;
            double y = character.Y / map.TileHeight;
            Vector2 pos = new Vector2((float)Math.Round(x, 0), (float)Math.Round(y, 0) );
            Node node = FindNode(pos);
            return node;
        }

        private void SortNodes(Vector2 start)
        {
            List<Node> nodesSort = nodes.OrderBy(x => x.Position - start).ToList();
        }

        public void Pathfind(Vector2 start, Vector2 end)
        {
            SortNodes(start);
        }

        public bool MoveCharacter(OverworldChar characterData, Squared.Tiled.Object character, int direction)
        {
            bool successfulMove = false; 
            int xOffset = 0; //used for animation to use
            if (direction == 0) //up
            {
                xOffset = 8;
                character.Y -= characterData.WalkSpeed;
                for (int i = 0; i < characterData.WalkSpeed; i++)
                {
                    characterData.Direction = direction;
                    if (CheckBounds(character))
                    {
                        character.Y += 1;
                    }
                    else
                    {
                        successfulMove = true;
                        break;
                    }
                }
            }
            else if (direction == 1) //right
            {
                xOffset = 4;
                character.X += characterData.WalkSpeed;
                for (int i = 0; i < characterData.WalkSpeed; i++)
                {
                    characterData.Direction = direction;
                    if (CheckBounds(character))
                    {
                        character.X -= 1;
                    }
                    else
                    {
                        successfulMove = true;
                        break;
                    }
                }
            }
            else if (direction == 2) //down
            {
                xOffset = 0;
                character.Y += characterData.WalkSpeed;
                for (int i = 0; i < characterData.WalkSpeed; i++)
                {
                    characterData.Direction = direction;
                    if (CheckBounds(character))
                    {
                        character.Y -= 1;
                    }
                    else
                    {
                        successfulMove = true;
                        break;
                    }
                }
            }
            else if (direction == 3) //left
            {
                xOffset = 12;
                character.X -= characterData.WalkSpeed;
                for (int i = 0; i < characterData.WalkSpeed; i++)
                {
                    characterData.Direction = direction;
                    if (CheckBounds(character))
                    {
                        character.X += 1;
                    }
                    else
                    {
                        successfulMove = true;
                        break;
                    }
                }
            }
            game.GetCurrentFrame(characterData, character,xOffset);
            return successfulMove;
        }

        public void Interact()
        {
            int xOffset = 0;
            int yOffset = 0;
            if (game.player.Direction == 0)
                yOffset -= 32;
            else if (game.player.Direction == 1)
                xOffset += 32;
            else if (game.player.Direction == 2)
                yOffset += 32;
            else if (game.player.Direction == 3)
                xOffset -= 32;
            //the hitbox for interacting with an npc is displaced depending on the direction the player is facing
            Rectangle box = new Rectangle() { X = player.X + xOffset + 8, Y = player.Y + yOffset + 8, Width = 16, Height = 16 };
            foreach (Squared.Tiled.Object npc in objects.Objects.Values)
            {
                Rectangle enemyrec = new Rectangle(
                npc.X,
                npc.Y,
                npc.Width,
                npc.Height
                );
                if (box.Intersects(enemyrec) && npc.Type == "NPC") //if the object is an npc and it intersects the hitbox
                {
                    game.State = Game1.GameState.Dialog;
                    game.npcName = npc.Properties["npc"];
                    game.dialogEvent = "";
                    switch (npc.Properties["npc"]) //logic for each of the npcs
                    {
                        case "test":
                            game.newDialogName = "test1";
                            break;
                        case "bubbles":
                            game.newDialogName = "bubbles1";
                            break;
                        default: //if the npc has no logic return to the playing state
                            game.State = Game1.GameState.Playing;
                            break;
                    }
                }
            }
        }

        public bool IsNPCVisible(Squared.Tiled.Object npc)
        {
            //checks if an npc should be spawned when the map is loaded
            bool visible = true;
            switch (npc.Properties["npc"])
            {
                case "test":
                    break;
                case "bubbles":
                    if (game.FindEvent("bubblesBefriend") >= 0)
                    {
                        visible = false;
                    }
                    break;
            }
            return visible;
        }

        private Vector2 FindSpawnLocation()
        {
            Vector2 position = new Vector2();
            List<Vector2> spawns = new List<Vector2>();
            Vector2 playerPos = new Vector2(player.X-player.Width/2,player.Y-player.Height/2);
            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    if (collision.GetTile(x, y) == 6) //Checking if the tile is a tile that can be spawned on
                    {
                        Vector2 tilePos = new Vector2(x * tilepixel, y * tilepixel);
                        double magnitude = Math.Sqrt(Math.Pow((playerPos.X - tilePos.X), 2) + Math.Pow((playerPos.Y - tilePos.Y), 2));
                        if (magnitude >= 128.0) //if the tile is not too close to the player
                        {
                            bool valid = true;
                            foreach (Squared.Tiled.Object obj in objects.Objects.Values) //Checks if any npcs/other enemies interect this tile
                            {
                                Rectangle objRect = new Rectangle(obj.X - obj.Width / 2, obj.Y - obj.Height / 2, obj.Width, obj.Height);
                                Rectangle tilRect = new Rectangle(x * tilepixel, y * tilepixel, tilepixel, tilepixel);
                                if (objRect.Intersects(tilRect))
                                {
                                    valid = false;
                                    break;
                                }
                            }
                            if (valid) //if no npc intersect the tile it can be spawned on
                            {
                                spawns.Add(tilePos);
                            }
                        }
                    }
                }
            }
            if (spawns.Count > 0)
                position = spawns[rng.Next(0,spawns.Count)]; //picks a random spawnable tile
            return position;
        }

        private void CreatePlayer(int X, int Y)
        {
            player = game.CreateObject("player", "characterSpritesheet", 3, X, Y, 32, 32);
            objects.Objects.Add("player", player);
        }

        private OverworldEnemy CreateEnemy(CharData data, int level)
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
            enemy.Level = level;
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
                if (i.Despawn == true) //if the enemy is set to despawn regardless of distance
                {
                    toRemove.Add(i);
                }
                else
                {
                    double magnitude = Math.Sqrt(Math.Pow((playerPos.X - charPos.X), 2) + Math.Pow((playerPos.Y - charPos.Y), 2));
                    if (magnitude >= 1024.0) //if the enemy is too far from the player it will have a chance to despawn
                    {
                        if (rng.Next(0, 100) < 50)
                        {
                            toRemove.Add(i);
                        }
                    }
                }
            }
            foreach (OverworldEnemy i in toRemove) //despawns the enemies
            {
                objects.Objects.Remove(i.Character.Name);
                enemies.Remove(i);
            }
            if (enemies.Count < mapData.SpawnCap) //checking if a new enemy can be spawned
            {
                MapChar selected = enemySpawnPool[rng.Next(0, enemySpawnPool.Count)];
                OverworldEnemy newEnemy = CreateEnemy(game.characters[selected.CharID], rng.Next(selected.MinLevel*100,selected.MaxLevel*100)/100);
                enemies.Add(newEnemy);
            }
        }

        public bool CheckCollisionBattle(OverworldEnemy enemy)
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

        public List<OverworldEnemy> GetEnemies(int x, int y, int range, bool despawn)
        {
            List<OverworldEnemy> fighters = new List<OverworldEnemy>();
            foreach (OverworldEnemy e in enemies)
            {
                double magnitude = Math.Sqrt(Math.Pow((e.Character.X - x), 2) + Math.Pow((e.Character.Y - y), 2));
                if (magnitude <= range)
                {
                    e.Despawn = despawn;
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

            //collisions with the map
            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    if (collision.GetTile(x, y) != 0 && collision.GetTile(x, y) != 6)
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
            //collisions with objects
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
                //collisions with teleporters
                foreach (KeyValuePair<string, Squared.Tiled.Object> entity in teleports.Objects.ToArray())
                {
                    Rectangle objrec = new Rectangle(entity.Value.X, entity.Value.Y, entity.Value.Width, entity.Value.Height);
                    if (playrec.Intersects(objrec))
                    {
                        game.pause = true;
                        ChangeMap(entity.Value);
                        break;
                    }
                }
            }
            return check;
        }
    }
}
