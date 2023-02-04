using System;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

[Flags]
public enum MoveDirection
{
    NONE  = 0,
    UP    = 1,
    DOWN  = 2,
    LEFT  = 4,
    RIGHT = 8,
    ALL   = UP | DOWN | LEFT | RIGHT
}

public static class MoveDirectionUtil
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int2 GetDirectionIndex(MoveDirection direction)
    {
        switch (direction)
        {
            case MoveDirection.UP:    return new int2( 0, -1);
            case MoveDirection.DOWN:  return new int2( 0,  1);
            case MoveDirection.LEFT:  return new int2(-1,  0);
            case MoveDirection.RIGHT: return new int2( 1,  0);

            case MoveDirection.NONE:
            case MoveDirection.ALL:
            default:
            {
                Debug.LogError("Move Direction should not be a flag!");
                break;
            }
        }
        
        return new int2(0, 0);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MoveDirection GetOppositeDirection(MoveDirection direction)
    {
        switch (direction)
        {
            case MoveDirection.UP:    return MoveDirection.DOWN;
            case MoveDirection.DOWN:  return MoveDirection.UP;
            case MoveDirection.LEFT:  return MoveDirection.RIGHT;
            case MoveDirection.RIGHT: return MoveDirection.LEFT;
            
            case MoveDirection.NONE:
            case MoveDirection.ALL:
            default:
            {
                return MoveDirection.NONE;
            }
        }
    }
    
    
}