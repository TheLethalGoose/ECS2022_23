using System;
using System.Collections.Generic;
using System.Linq;
using ECS2022_23.Core.Entities.Characters;
using ECS2022_23.Core.Entities.Characters.Enemy;
using ECS2022_23.Core.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ECS2022_23.Core.Manager;

public static class EnemyManager
{
    public static List<Enemy> Enemies = new();

    private static Enemy _keyEnemy;
    private static readonly List<Vector2> _closedList = new();

    private static readonly EnemyFactory _factory = new();
    public static Player Player { set; get; }
    public static Stage Stage { set; get; }

    private static void AddEnemy(Enemy enemy)
    {
        Enemies.Add(enemy);
    }

    public static void RemoveEnemy(Enemy enemy)
    {
        if (Enemies.Contains(enemy)) Enemies.RemoveAt(Enemies.IndexOf(enemy));
    }

    public static bool EnemyDropsKey(Enemy enemy)
    {
        return _keyEnemy != null && enemy.Equals(_keyEnemy);
    }

    public static void KillEnemies()
    {
        Enemies.Clear();
    }

    public static void SpawnMultipleEnemies(int enemyLimit)
    {
        var rand = new Random();

        foreach (var room in Stage.Rooms.Skip(1))
        {
            if (room.Spawns.Count > 0 && !room.MapName.Contains("boss"))
            {
                var amount = rand.Next(1, Math.Min(room.Spawns.Count, enemyLimit));
                for (var a = 0; a < amount; a++)
                {
                    var rety = 0;
                    do
                    {
                        var en = _factory.CreateRandomEnemy(Stage, Player);
                        var pos = room.GetRandomSpawnPos(en);
                        pos.Floor();
                        if (!_closedList.Contains(pos) && !WithinRange(pos))
                        {
                            _closedList.Add(pos);
                            en.Position = pos;
                            AddEnemy(en);
                            CombatManager.AddEnemy(en);
                            break;
                        }

                        rety++;
                    } while (rety < 4);
                }
            }

            if (room.MapName.Contains("boss"))
            {
                var boss = _factory.CreateGiantBlob(Stage, Player);
                boss.Position = room.Spawns[0];
                AddEnemy(boss);
                CombatManager.AddEnemy(boss);
            }
        }

        ChooseEnemyForKey();
    }

    private static bool WithinRange(Vector2 vec)
    {
        foreach (var closedVec in _closedList)
            if (vec.X - 1 == closedVec.X || vec.X + 1 == closedVec.X)
                if (vec.Y - 1 == closedVec.Y || vec.Y + 1 == closedVec.Y)
                    return true;
        return false;
    }


    public static void Update(GameTime gameTime)
    {
        foreach (var enemy in Enemies) enemy.Update(gameTime);
    }

    public static void Draw(SpriteBatch spriteBatch)
    {
        foreach (var en in Enemies) en.Draw(spriteBatch);
    }

    private static void ChooseEnemyForKey()
    {
        var random = new Random();

        //var randomInt = random.Next(Enemies.Count);
        var randomInt = random.Next(1);

        if (randomInt >= 0 && randomInt < Enemies.Count) _keyEnemy = Enemies[randomInt];
    }
}