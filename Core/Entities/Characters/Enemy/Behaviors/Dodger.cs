using ECS2022_23.Enums;
using Microsoft.Xna.Framework;

namespace ECS2022_23.Core.Entities.Characters.Enemy.Behaviors;

public class Dodger : Behavior
{
    private Character Target;

    public Dodger(Character target)
    {
        Target = target;
        State = (int)EnemyStates.Initial;
    }

    public override Vector2 Move(Vector2 position, float velocity)
    {
        Aim(Target);
        
        if (State is (int)EnemyStates.Initial or (int)EnemyStates.Attack)
        {
            return Vector2.Zero;
        }

        if (State == (int) EnemyStates.Move)
        {
            Vector2 direction = -Vector2.Normalize(Target.Position - position) * velocity;
            if (Owner.Collides(direction))
                return direction;
        }
        
        return Vector2.Zero;
    }
    
    public void Aim(Character Target)
    {
        Owner.AimVector = Vector2.Normalize((Target.Position - Owner.Position));
    }
    
}