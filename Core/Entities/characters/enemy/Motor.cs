﻿using Microsoft.Xna.Framework;

namespace ECS2022_23.Core.entities.characters.enemy;

public abstract class Motor
{
    private Pathfinding path;

    public abstract Vector2 Move(Vector2 position, int velocity);
}