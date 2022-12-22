﻿using System.Diagnostics;
using ECS2022_23.Core.Entities.Characters;
using ECS2022_23.Core.Entities.Items;
using ECS2022_23.Core.Ui.InventoryManagement.InventoryTypes;
using Microsoft.Xna.Framework.Graphics;

namespace ECS2022_23.Core.Ui.InventoryManagement;

public static class InventoryManager
{
    private static Pocket _pocket = new(3, 3);
    private static ToolBar _toolBar = new(1, 9);
    private static WeaponSlot _weaponSlot = new WeaponSlot();
    private static TrinketSlot _trinketSlot = new TrinketSlot();
    private static Weapon _prevWeapon;
    private static Trinket _prevTrinket;
    public static bool Show = false;

    public static void Update(Player player)
    {
        UpdateWeapon(player);
        UpdateTrinket(player);
    }
    
    public static void Draw(SpriteBatch spriteBatch)
    {
        if (Show)
        {
            _pocket.Draw(spriteBatch);
        }
        else
        {
            _pocket.SelectIndex(0);
        }
        _toolBar.Draw(spriteBatch);
        _weaponSlot.Draw(spriteBatch);
        _trinketSlot.Draw(spriteBatch);
    }

    public static void UseSelectedItem(Player player)
    {
        var item = _pocket.GetSelectedItem();
        if (item == null) return;
        UseItem(player, item);
    }

    public static void AddItem(Item item)
    {
        if (item.GetType() == typeof(Weapon))
        {
            _weaponSlot.AddItem(item);
        }
        else
        {
            _pocket.AddItem(item);
            _toolBar.AddItem(item);
        }
    }
    
    public static void RemoveItem(Item item)
    {
       _pocket.RemoveItem(item);
       _toolBar.RemoveItem(item);
    }

    public static void IncreaseIndex()
    {
        _pocket.IncreaseIndex();
    }
    public static void DecreaseIndex()
    {
        _pocket.DecreaseIndex();
    }
    
    public static void UseItemAtIndex(Player player, int index)
    {

        var item = _toolBar.GetItemAtIndex(index);
        if (item == null) return;
        UseItem(player, item);
    }

    private static void UseItem(Player player, Item item)
    {
        if (item.GetType() == typeof(Trinket))
        {
            var trinket = (Trinket)item;
            _pocket.SwitchActiveState(trinket);
            _toolBar.SwitchActiveState(trinket);
            if (trinket.Equals(player.Trinket))
            {
                trinket.Unequip(player);
                _prevTrinket = trinket;
            }
            else
            {
                _trinketSlot.AddItem(trinket);
                _prevTrinket = trinket;
                player.UseItem(trinket);
            }
            return;
        }
        player.UseItem(item);
        RemoveItem(item);
    }

    private static void UpdateWeapon(Player player)
    {
        if (player.Weapon != null)
        {
            if (_prevWeapon != null)
            {
                if (!_prevWeapon.Equals(player.Weapon))
                {
                    _weaponSlot.AddItem(player.Weapon);
                    _prevWeapon = player.Weapon;
                }
            }
            else
            {
                _weaponSlot.AddItem(player.Weapon);
                _prevWeapon = player.Weapon;
            }
        }
        else if(_prevWeapon != null)
        {
            _weaponSlot.RemoveItem(_prevWeapon);
            _prevWeapon = null;
        }
    }

    private static void UpdateTrinket(Player player)
    {
        if (player.Trinket != null)
        {
            if (_prevTrinket != null)
            {
                if (!_prevTrinket.Equals(player.Trinket))
                {
                    _trinketSlot.AddItem(player.Trinket);
                    _prevTrinket = player.Trinket;
                }
            }
            else
            {
                _trinketSlot.AddItem(player.Trinket);
                _prevTrinket = player.Trinket;
            }
        }
        else if(_prevTrinket != null)
        {
            _trinketSlot.RemoveItem(_prevTrinket);
            _prevTrinket = null;
        }
    }

    public static void SetWeapon(Weapon weapon)
    {
        _weaponSlot.AddItem(weapon);
    }

    public static void SetTrinket(Trinket trinket)
    {
        _trinketSlot.AddItem(trinket);
    }
}