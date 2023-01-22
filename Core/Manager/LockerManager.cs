using System.Collections.Generic;
using ECS2022_23.Core.Entities.Items;
using ECS2022_23.Core.Loader;
using ECS2022_23.Core.Ui.Inventory.InventoryTypes;
using ECS2022_23.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ECS2022_23.Core.Manager;

public static class LockerManager
{
    private static Pocket _playerPocket;
    private static bool _lockerIsActive;

    private static Texture2D _spriteSheet;
    private static readonly Rectangle _sourceRecOfArrow = new(9 * 16, 3 * 16, 16, 16);
    private static readonly Point _scale = new(6, 6);

    private static List<ItemType> _itemsInLocker = new();
    private static Pocket _locker { get; set; }

    public static void Init(List<ItemType> itemsInLocker)
    {
        _locker = new Pocket(3, 2, InventoryType.LockerInventory);
        _playerPocket = new Pocket(3, 3, InventoryType.PocketInventory);
        _spriteSheet = UiLoader.SpriteSheet;
        _lockerIsActive = true;

        _itemsInLocker = itemsInLocker;

        for (int i = 2; i < 7; i++)
        {
            _itemsInLocker.Remove((ItemType) i );
        }
        _itemsInLocker.Add(ItemType.Phaser);
        
        foreach (var itemType in _itemsInLocker) _locker.AddItem(ItemLoader.CreateItem(Vector2.Zero, itemType));

    }

    public static void AddToPocket(Item item)
    {
        _playerPocket.AddItem(item);
    }

    public static void RemoveFromPocket(Item item)
    {
        _playerPocket.RemoveItem(item);
    }

    public static void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_spriteSheet, new Rectangle(Game1.ScreenWidth / 2 - 16 * _scale.X / 2,
            Game1.ScreenHeight / 2 - 16 * _scale.Y / 2,
            16 * _scale.X, 16 * _scale.Y), _sourceRecOfArrow, Color.White);
        _locker.Draw(spriteBatch);
        _playerPocket.Draw(spriteBatch);
    }

    public static void HandleInput(Keys actionKey)
    {
        SwitchActiveInventory(actionKey);
        switch (actionKey)
        {
            case Keys.Right:
                MoveIndex(1);
                break;
            case Keys.Left:
                MoveIndex(-1);
                break;
            case Keys.Enter:
                if (_lockerIsActive)
                    TransferSelectedItem(_locker, _playerPocket);
                else
                    TransferSelectedItem(_playerPocket, _locker);
                break;
        }
    }

    private static void MoveIndex(int i)
    {
        switch (_lockerIsActive)
        {
            case true:
                MoveIndex(_locker, i);
                break;
            case false:
                MoveIndex(_playerPocket, i);
                break;
        }
    }

    private static void MoveIndex(Inventory inventory, int i)
    {
        switch (i)
        {
            case > 0:
                inventory.IncreaseIndex();
                break;
            case < 0:
                inventory.DecreaseIndex();
                break;
        }
    }

    private static void TransferSelectedItem(Pocket fromInventory, Pocket toInventory)
    {
        var toTransfer = fromInventory.GetSelectedItem();

        if (toTransfer == null) return;

        if (toTransfer.ItemType == ItemType.Key || toTransfer.GetType() == typeof(Key)) return;

        if (toTransfer.GetType() == typeof(Weapon))
            if (SwitchBothWeapons(fromInventory, toInventory, toTransfer))
                return;

        switch (fromInventory.Type)
        {
            case InventoryType.LockerInventory:
                if (InventoryManager.AddItem(toTransfer))
                {
                    fromInventory.RemoveItem(toTransfer);
                    _itemsInLocker.Remove(toTransfer.ItemType);
                }

                break;
            case InventoryType.PocketInventory:
                if (_locker.AddItem(toTransfer))
                {
                    InventoryManager.RemoveItem(toTransfer);
                    _itemsInLocker.Add(toTransfer.ItemType);
                }

                break;
        }
    }

    private static bool SwitchBothWeapons(Pocket fromInventory, Pocket toInventory, Item toTransfer)
    {
        if (!_locker.WeaponLimitReached || !_playerPocket.WeaponLimitReached) return false;

        var toSwitch = toInventory.GetWeapon();

        switch (fromInventory.Type)
        {
            case InventoryType.LockerInventory:
                InventoryManager.RemoveItem(toSwitch);
                if (InventoryManager.AddItem(toTransfer))
                {
                    fromInventory.RemoveItem(toTransfer);
                    fromInventory.AddItem(toSwitch);
                    _itemsInLocker.Remove(toTransfer.ItemType);
                    _itemsInLocker.Add(toSwitch.ItemType);
                }

                break;
            case InventoryType.PocketInventory:
                _locker.RemoveItem(toSwitch);
                _itemsInLocker.Remove(toSwitch.ItemType);
                if (_locker.AddItem(toTransfer))
                {
                    InventoryManager.RemoveItem(toTransfer);
                    InventoryManager.AddItem(toSwitch);
                    _itemsInLocker.Add(toTransfer.ItemType);
                }

                break;
        }

        return true;
    }

    private static void SwitchActiveInventory(Keys actionKey)
    {
        switch (actionKey)
        {
            case Keys.Right:
            {
                if (_lockerIsActive && _locker.IsAtLastIndex()) _lockerIsActive = false;
                break;
            }
            case Keys.Left:
            {
                if (!_lockerIsActive && _playerPocket.IsAtFirstIndex()) _lockerIsActive = true;
                break;
            }
        }
    }
}