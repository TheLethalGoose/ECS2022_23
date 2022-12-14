using System;
using ECS2022_23.Core.Entities.Characters;
using ECS2022_23.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ECS2022_23.Core.Entities.Items;
[Serializable]
public abstract class Item : Entity
{
    public Rectangle SourceRect { get; }
    public ItemType itemType;

    protected Item(Vector2 spawn, Texture2D texture, Rectangle sourceRect, ItemType itemType) : base(spawn, texture)
    {
        SourceRect = sourceRect;
        this.itemType = itemType;
    }

    public virtual bool Use()
    {
        return true;
    }

    public virtual bool Use(Player player)
    {
        return true;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Texture, Position, SourceRect, Color.White);
    }

    public void DrawIcon(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Texture, Position, SourceRect, Color.White);
    }

    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        if (obj.GetType().BaseType != typeof(Item))
        {
            return false;
        }

        var toCompare = (Item)obj;
        return SourceRect.Equals(toCompare.SourceRect) 
               && toCompare.Texture == this.Texture 
               && Position == toCompare.Position;
    }

    public override int GetHashCode()
    {
        return SourceRect.GetHashCode();
    }
}