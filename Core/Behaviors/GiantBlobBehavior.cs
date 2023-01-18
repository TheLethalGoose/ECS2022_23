using System.Numerics;
using ECS2022_23.Core.Entities.Characters;
using ECS2022_23.Core.Entities.Characters.Enemy.Behaviors;
using ECS2022_23.Core.Manager;
using ECS2022_23.Core.World;

namespace ECS2022_23.Core.Behaviors;

public class GiantBlobBehavior : Chase
{
    
    private int _bulletWaveDelay;
    private int _shotDelay;
    
    public GiantBlobBehavior(Character target) : base(target)
    {
        
    }

    public override void Attack()
    {
        if (++_bulletWaveDelay>280)
        {
            _bulletWaveDelay = 0;
            SpawnBulletWave();
        }
        _bulletWaveDelay++;

        if (++_shotDelay > 50)
        {
            _shotDelay = 0;
            CombatManager.Shoot(Owner);
        }
    }
    
    private void SpawnBulletWave()
    {
        CombatManager.Shoot(Owner.Position, new Vector2(0,-1), Owner.Stage);
        CombatManager.Shoot(Owner.Position, new Vector2(1,-1), Owner.Stage);
        CombatManager.Shoot(Owner.Position, new Vector2(1,0), Owner.Stage);
        CombatManager.Shoot(Owner.Position, new Vector2(1,1), Owner.Stage);
        CombatManager.Shoot(Owner.Position, new Vector2(0,1), Owner.Stage);
        CombatManager.Shoot(Owner.Position, new Vector2(-1,1), Owner.Stage);
        CombatManager.Shoot(Owner.Position, new Vector2(-1,0), Owner.Stage);
        CombatManager.Shoot(Owner.Position, new Vector2(-1,-1), Owner.Stage);
    }
    
}