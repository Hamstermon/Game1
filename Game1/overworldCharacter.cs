﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Game1
{
    public class OverworldChar
    {
        int direction = 2;
        public int Direction
        {
            set
            {
                if (direction >= 0 && direction <= 3)
                    direction = value;
            }
            get { return direction; }
        }

        int walkSpeed = 4;
        public int WalkSpeed
        {
            set { walkSpeed = value; }
            get { return walkSpeed; }
        }

        int currentFrame = 0;
        public int CurrentFrame
        {
            set { currentFrame = value; }
            get { return currentFrame; }
        }

        int elapsedTime = 0;
        public int ElapsedTime
        {
            set { elapsedTime = value; }
            get { return elapsedTime; }
        }

        int frameCount = 0;
        public int FrameCount
        {
            set { frameCount = value; }
            get { return frameCount; }
        }

        int animationSpeed = 0;
        public int AnimationSpeed
        {
            set { animationSpeed = value; }
            get { return animationSpeed; }
        }

        int index = 0;
        public int SpriteIndex
        {
            set { index = value; }
            get { return index; }
        }

        string sheet = "characterSpritesheet";
        public string SpriteSheet
        {
            set { sheet = value; }
            get { return sheet; }
        }
        
        public void Animation(GameTime gameTime)
        {
            elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (elapsedTime > animationSpeed)
            {
                currentFrame++;
                if (currentFrame == frameCount)
                {
                    currentFrame = 0;
                }
                elapsedTime = 0;
            }
        }
    }

    public class OverworldEnemy : OverworldChar
    {
        string aiType = "Default";
        public string AIType
        {
            set { aiType = value; }
            get { return aiType; }
        }
        Squared.Tiled.Object character;
        public Squared.Tiled.Object Character
        {
            set { character = value; }
            get { return character; }
        }
        int passiveSpeed = 4;
        public int PassiveSpeed
        {
            set { passiveSpeed = value; }
            get { return passiveSpeed; }
        }
        int agressiveSpeed = 4;
        public int AgressiveSpeed
        {
            set { agressiveSpeed = value; }
            get { return agressiveSpeed; }
        }
        int charID = 0;
        public int CharacterID
        {
            set { charID = value; }
            get { return charID; }
        }
        int lvl = 0;
        public int Level
        {
            set { lvl = value; }
            get { return lvl; }
        }
        bool despawn = false;
        public bool Despawn
        {
            set { despawn = value; }
            get { return despawn; }
        }
        int[] bonusStats = new int[7] { 0, 0, 0, 0, 0, 0, 0 };
        public int[] BonusStats
        {
            set { bonusStats = value; }
            get { return bonusStats; }
        }
    }

    public class OverworldPlayer : OverworldChar
    {
        
    }
}
