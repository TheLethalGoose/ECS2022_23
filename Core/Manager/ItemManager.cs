using System;
using System.Collections.Generic;
using ECS2022_23.Core.Entities.Characters;
using ECS2022_23.Core.Entities.Characters.Enemy;
using ECS2022_23.Core.Entities.Items;
using ECS2022_23.Core.Loader;
using ECS2022_23.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ECS2022_23.Core.Manager;

public static class ItemManager
{
    private static List<Item> _activeItems = new();
    private static bool _keyHasDropped;

    public static void Init()
    {
        _keyHasDropped = false;
        _activeItems = new List<Item>();
    }

    public static void Draw(SpriteBatch spriteBatch)
    {
        foreach (var item in _activeItems)
            if (item.GetType() == typeof(Weapon))
            {
                var weapon = (Weapon) item;
                weapon.DrawIcon(spriteBatch);
            }
            else
            {
                item.Draw(spriteBatch);
            }
    }

    private static void AddItem(Item item)
    {
        if (item != null) _activeItems.Add(item);
    }

    public static void DropLoot(Enemy enemy)
    {
        if (!_keyHasDropped)
        {
            var enemyDropsKey = EnemyManager.EnemyDropsKey(enemy);

            if (enemyDropsKey)
            {
                DropKey(enemy.Position);
                _keyHasDropped = true;
            }
            else
            {
                DropRandomLoot(enemy.Position, enemy.ItemSpawnRate);
            }
        }
        else
        {
            if (enemy.IsBoss)
                DropRandomLoot(enemy.Position, enemy.ItemSpawnRate, 100);
            else
                DropRandomLoot(enemy.Position, enemy.ItemSpawnRate);
        }
    }

    private static void DropRandomLoot(Vector2 position, float dropChance, int umlChance = 0)
    {
        var randomDrop = new Random((int) DateTime.Now.Ticks);
        
        var randomFloat = randomDrop.Next(0, 100);

        var weaponChance = 25;
        var trinketChance = 25;

        if (randomFloat <= umlChance)
        {
            AddItem(GenerateItem(position, ItemType.UmlDiagram));
            return;
        }

        if (randomFloat <= trinketChance)
        {
            AddItem(GenerateItem(position, ItemType.SwimmingGoggles));
            return;
        }

        if (randomFloat <= weaponChance)
        {
            AddItem(GetRandomWeapon(position));
            return;
        }

        AddItem(GetRandomConsumable(position));
    }

    private static void DropKey(Vector2 position)
    {
        AddItem(ItemLoader.CreateItem(position, ItemType.Key));
        SoundManager.Play(SoundLoader.DropKeySound);
    }

    private static Item GenerateItem(Vector2 position, ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.UmlDiagram: return ItemLoader.CreateItem(position, ItemType.UmlDiagram);
            case ItemType.SwimmingGoggles: return ItemLoader.CreateItem(position, ItemType.SwimmingGoggles);
            default: return ItemLoader.CreateItem(position, ItemType.SwimmingGoggles);
        }
    }

    private static Weapon GetRandomWeapon(Vector2 position)
    {
        var random = new Random();
        var randomInt = random.Next(100);
        switch (randomInt)
        {
            case <= 30: return (Weapon) ItemLoader.CreateItem(position, ItemType.Phaser);
            case <= 40: return (Weapon) ItemLoader.CreateItem(position, ItemType.Crowbar);
            case <= 45: return (Weapon) ItemLoader.CreateItem(position, ItemType.Stick);
            case <= 50: return (Weapon) ItemLoader.CreateItem(position, ItemType.Knife);

            default: return (Weapon) ItemLoader.CreateItem(position, ItemType.Sword);
        }
    }

    private static Consumable GetRandomConsumable(Vector2 position)
    {
        var random = new Random();
        var randomInt = random.Next(100);
        switch (randomInt)
        {
            case <= 25: return (Consumable) ItemLoader.CreateItem(position, ItemType.Cake);
            case <= 50: return (Consumable) ItemLoader.CreateItem(position, ItemType.Armor);
            default: return (Consumable) ItemLoader.CreateItem(position, ItemType.HealthPotion);
        }
    }

    public static void PickItemUp(Player player)
    {
        foreach (var item in _activeItems)
        {
            if (!item.Rectangle.Intersects(player.Rectangle)) continue;
            if (item.GetType() == typeof(Weapon))
            {
                if (player.Weapon != null)
                {
                    var weapon = player.Weapon;
                    weapon.Position = player.Position;
                    _activeItems.Add(weapon);
                }

                InventoryManager.AddItem(item);
            }
            else
            {
                InventoryManager.AddItem(item);
            }

            _activeItems.Remove(item);

            SoundManager.Play(SoundLoader.PickUpItemSound);

            return;
        }
    }
}