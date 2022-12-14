﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ECS2022_23.Core.Animations;
using ECS2022_23.Core.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace ECS2022_23.Core.Entities.Characters;

public abstract class Character : Entity
{
    public float HP;
    public float Strength;
    public float Velocity;
    protected int SpriteWidth;
    public int AimDirection;
    public SoundEffect DamageSound;
    public SoundEffect DeathSound;
    public Level Level { get; set; }
    public bool IsAttacking { get; set; }
    
    protected Character(Vector2 spawn, Texture2D texture) : base(spawn, texture)
    {
    }
    
    protected Character(Vector2 spawn, Texture2D texture, Dictionary<string, Animation> animations) : base(spawn, texture, animations)
    {
        
    }

    public abstract void Attack();
    
    public virtual bool Collides(Vector2 velocity)
    {
        var newPoint = (Position + velocity).ToPoint();
        var body = new Rectangle(newPoint, new Point(Texture.Width, Texture.Height));
        var feet = new Point(body.Center.X, body.Bottom);

        if (velocity == Vector2.Zero)
        {
            return true;
        }
        
        foreach (var rectangle in Level.GroundLayer)
        {
            if (rectangle.Contains(feet) && !IsInWater(body))
            {
                return true;
            }
        }

        return false;
    }

    public virtual bool IsInWater(Rectangle body)
    {
        foreach (var rectangle in Level.WaterLayer)
        {
            if (rectangle.Intersects(body))
            {
                return true;
            }
        }
        return false;
    }
    
    public override void Draw(SpriteBatch spriteBatch)
    {
        if (Animations == null)
        {
            base.Draw(spriteBatch);
        }
        else
        {
            AnimationManager.Draw(spriteBatch, Position);
        }
    }

    public bool IsAlive()
    {
        return HP>0;
    }

    public void Kill()
    {
        HP = 0;
    }

}