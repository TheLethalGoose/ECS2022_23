using ECS2022_23.Core.Entities.Characters.Enemy.Behavior;
using ECS2022_23.Core.Loader;
using ECS2022_23.Core.Ui;
using ECS2022_23.Core.World;
using Microsoft.Xna.Framework;

namespace ECS2022_23.Core.Entities.Characters.Enemy.EnemyTypes;

public class Chaser : Enemy
{
    
    public Chaser(Level level, Character target) : base(Vector2.Zero, UiLoader.SpriteSheet,
        AnimationLoader.CreateZombieEnemyAnimations(), new Chase(target), level)
    {
        Velocity = 1.5f;
        HP = 10;
        XpReward = 1;
        MoneyReward = 1;
        SpriteHeight = 14;
        SpriteWidth = 14;
        ActivationRectangle.Location = Position.ToPoint();
        Behavior.SetEnemy(this);
        DeathSound = SoundLoader.BlobDeathSound;
        Strength = 1;
    }

    public override void Attack()
    {
        
    }
}