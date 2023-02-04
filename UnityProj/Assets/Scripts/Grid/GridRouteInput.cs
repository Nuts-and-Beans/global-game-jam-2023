using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

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

public class GridRouteInput : MonoBehaviour
{
    [Header("Grid References")]
    [SerializeField] private Grid _grid;

    public bool _readingInput = false;
    public int2 _currentCellPos;
    public List<MoveDirection> _moveDirections = new List<MoveDirection>();
    
    private bool _canMove = true;
    
    private const float DeadZone = 0.25f;

    private void Awake()
    {
        Input.Actions.Grid.Route.performed += OnRoutePerformed;
        Input.Actions.Grid.Route.canceled  += OnRouteCancelled;
        
        StartRoute();
    }
    
    private void OnDestroy()
    {
        Input.Actions.Grid.Route.performed -= OnRoutePerformed;
        Input.Actions.Grid.Route.canceled  -= OnRouteCancelled;
    }

    private void OnRoutePerformed(InputAction.CallbackContext context)
    {
        if (!_readingInput) return;
        
        float2 axis = (float2)context.ReadValue<Vector2>();

        // NOTE(WSWhitehouse): This ensures the input is above the dead zone
        if (math.abs(axis.x) < DeadZone && math.abs(axis.y) < DeadZone)
        {
            _canMove = true;
            return;
        }
        
        if (!_canMove) return;
        _canMove = false;
        
        bool xMax = math.abs(axis.x) >= math.abs(axis.y);

        MoveDirection direction;
        if (xMax)
        {
            direction = axis.x > 0.0f ? MoveDirection.RIGHT : MoveDirection.LEFT;
        }
        else
        {
            direction = axis.y > 0.0f ? MoveDirection.UP : MoveDirection.DOWN;
        }
        
        AddDirection(direction);
    }

    private void OnRouteCancelled(InputAction.CallbackContext context)
    {
        _canMove = true;
    }

    public void StartRoute()
    {
        _readingInput   = true;
        _currentCellPos = _grid.GridStartIndex;
        _moveDirections.Clear();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int2 GetMoveDirectionIndex(MoveDirection direction)
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
    private MoveDirection GetOppositeDirection(MoveDirection direction)
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
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private MoveDirection GetValidMoveDirections()
    {
        MoveDirection outDirection = MoveDirection.ALL;

        // Check for X out of bounds
        if (_currentCellPos.x <= 0)
        {
            outDirection &= ~MoveDirection.LEFT;
        }
        
        if (_currentCellPos.x >= _grid.GridCellCount.x)
        {
            outDirection &= ~MoveDirection.RIGHT;
        }
        
        // Check for Y out of bounds
        if (_currentCellPos.y <= 0)
        {
            outDirection &= ~MoveDirection.UP;
        }
        
        if (_currentCellPos.y >= _grid.GridCellCount.y)
        {
            outDirection &= ~MoveDirection.DOWN;
        }

        if (_moveDirections.Count > 0)
        {
            MoveDirection prevDirection = _moveDirections[^1];
            outDirection &= ~GetOppositeDirection(prevDirection);
        }

        for (int i = 0; i < 4; i++)
        {
            MoveDirection direction = (MoveDirection)(1 << i);
            if ((outDirection & direction) == 0) continue;
            
            int2 index = _currentCellPos + GetMoveDirectionIndex(direction);
            if (_grid.GridCells[index].isWall)
            {
                outDirection &= ~direction;
            }
        }
        
        return outDirection;
    }

    private void AddDirection(MoveDirection direction)
    {
        MoveDirection validDirections = GetValidMoveDirections();
        
        // If valid directions doesnt contain the requested direction then return
        if (((validDirections & direction) == 0)) return;
        
        _moveDirections.Add(direction);
        _currentCellPos += GetMoveDirectionIndex(direction);
    }
}