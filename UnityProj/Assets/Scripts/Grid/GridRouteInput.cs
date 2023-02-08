using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

public class GridRouteInput : MonoBehaviour
{
    [Header("Grid References")]
    [SerializeField] private Grid _grid;

    public bool _readingInput = false;
    public int2 _currentCellPos;
    public List<MoveDirection> _moveDirections = new List<MoveDirection>();
    public List<int2> _visitedIndices          = new List<int2>();
    public Character _character = null;
    
    public delegate void OnRouteStartedDel();
    public OnRouteStartedDel OnRouteStarted;
    
    public delegate void OnRouteEndedDel();
    public OnRouteEndedDel OnRouteEnded;
    
    public delegate void OnMoveDirectionAddedDel(int2 currentCellIndex);
    public OnMoveDirectionAddedDel OnMoveDirectionAdded;
    
    public delegate void OnMoveDirectionsConfirmedDel(Character character, List<MoveDirection> moveDirections);
    public OnMoveDirectionsConfirmedDel OnMoveDirectionsConfirmed;

    private bool _canMove = true;
    
    private const float DeadZone = 0.25f;

    private void Awake()
    {
        Input.Actions.Grid.Route.performed    += OnRoutePerformed;
        Input.Actions.Grid.Route.canceled     += OnRouteCancelled;
        Input.Actions.Grid.EndRoute.performed += OnEndRoutePerformed;
    }

    private void OnDestroy()
    {
        Input.Actions.Grid.Route.performed    -= OnRoutePerformed;
        Input.Actions.Grid.Route.canceled     -= OnRouteCancelled;       
        Input.Actions.Grid.EndRoute.performed -= OnEndRoutePerformed;
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

    private void OnEndRoutePerformed(InputAction.CallbackContext context)
    {
        EndRoute();
    }

    public void StartRoute(Character character)
    {
        _readingInput   = true;
        _character      = character;
        _currentCellPos = _grid.GridStartIndex;
        _moveDirections.Clear();
        _visitedIndices.Clear();
        
        OnRouteStarted?.Invoke();
    }

    public void EndRoute()
    {
        _readingInput = false;
        OnRouteEnded?.Invoke();
        OnMoveDirectionsConfirmed?.Invoke(_character, _moveDirections);
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
            outDirection &= ~MoveDirectionUtil.GetOppositeDirection(prevDirection);
        }

        for (int i = 0; i < 4; i++)
        {
            MoveDirection direction = (MoveDirection)(1 << i);
            if ((outDirection & direction) == 0) continue;
            
            int2 nextIndex = _currentCellPos + MoveDirectionUtil.GetDirectionIndex(direction);
            
            if (_grid.GridCells[nextIndex].isWall)
            {
                outDirection &= ~direction;
                continue;
            }

            if (_visitedIndices.Contains(nextIndex))
            {
                outDirection &= ~direction;
                continue;
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
        _currentCellPos += MoveDirectionUtil.GetDirectionIndex(direction);
        _visitedIndices.Add(_currentCellPos);
        
        OnMoveDirectionAdded?.Invoke(_currentCellPos);

        // NOTE(WSWhitehouse): Do another valid move directions check to ensure
        // the adventurer is available to move on the next cell.
        MoveDirection nextValidDirections = GetValidMoveDirections();
        if (nextValidDirections == MoveDirection.NONE)
        {
            EndRoute();
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(GridRouteInput))]
public class GridRouteInputEditor : Editor
{
    private GridRouteInput Target => target as GridRouteInput;
    
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        EditorGUILayout.Space();

        if (GUILayout.Button("Start Route"))
        {
           Target.StartRoute(null);   
        }

        if (GUILayout.Button("End Route"))
        {
            Target.EndRoute();
        }
    }
}
#endif // UNITY_EDITOR