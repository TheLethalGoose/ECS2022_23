using System;
using System.Diagnostics;
using ECS2022_23.Core.Entities.Characters;
using ECS2022_23.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ECS2022_23.Core.Entities.Items;

public class Key : Item
{
    public Key(Vector2 spawn, Texture2D texture, Rectangle sourceRect) : base(spawn, texture, sourceRect, ItemType.Key)
    {
    }

    public override void Update(GameTime gameTime)
    {
        
    }
    public override bool Use(Player player)
    {
        if (player.Stage.PlayerIsInfrontOfBossDoor)
        {
            player.Stage.OpenBossDoor();
            return true;
        }

        return false;
    }
    
    
}