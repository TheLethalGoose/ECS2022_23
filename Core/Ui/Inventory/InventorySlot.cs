﻿using System;
using ECS2022_23.Core.Entities.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ECS2022_23.Core.Ui.Inventory;

public class InventorySlot
{
    public Rectangle DestinationRec;
    public bool IsUsed = false;
    public Item Item;
    public int ItemCount = 0;
    private Texture2D _backgroundTexture;
    private UiText _text;
    private int _scale;
    public bool Selected;
    public bool Active = false;
    private Texture2D _spriteSheet = UiLoader.GetSpritesheet();
    private Rectangle _selectedSourceRec = new Rectangle(9*16, 4*16, 16, 16);
    private Rectangle _frameSourceRec = new Rectangle(8 * 16, 4 * 16, 16, 16);
    
    public int Index;
    public InventorySlot(Rectangle dest, int scale, int index)
    {
        DestinationRec = dest;
        _backgroundTexture = UiLoader.CreateColorTexture(new Color(192, 91, 255, 255));
        _text = UiLoader.CreateTextElement("");
        _text.DestinationRec = new Rectangle(DestinationRec.X+DestinationRec.Width/8, DestinationRec.Y, DestinationRec.Width / 2,
            DestinationRec.Height / 2);
        _scale = scale;
        _text.Scale = new Vector2(_scale/2, _scale/2);
        Index = index;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        try
        {
            if (Active)
            {
                spriteBatch.Draw(_backgroundTexture, DestinationRec, Color.White);
            }
            spriteBatch.Draw(_spriteSheet, DestinationRec, _frameSourceRec, Color.White);
            if (IsUsed)
            {
                spriteBatch.Draw(Item.Texture, DestinationRec, Item.SourceRect, Color.White);

                _text.Text = "" + ItemCount;
                _text.Draw(spriteBatch);
            }
            if (Selected)
            {
                spriteBatch.Draw(_spriteSheet, DestinationRec, _selectedSourceRec, Color.White);
            }
        }
        catch (ArgumentNullException e)
        {
            return;
        }
    }

    public void AddItem(Item item, int count)
    {
        this.Item = item;
        this.ItemCount = count;
        IsUsed = true;
    }

    public void RemoveItem()
    {
        Item = null;
        IsUsed = false;
        ItemCount = 0;
    }
}