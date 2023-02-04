using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

public class GridRouteInputViewer : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private Grid _grid;
    [SerializeField] private GridRouteInput _gridRouteInput;

    [Header("Route Visualisation")]
    [SerializeField] private RouteIcon _routeIconPrefab;
    [SerializeField] private Transform _routeIconParent;
    [SerializeField] private int _initialRouteIconSpawnAmount = 25;
    
    private List<RouteIcon> _routeIconPool;
    private List<RouteIcon> _activeRouteIcons;

    private void Awake()
    {
        _routeIconPool    = new List<RouteIcon>(_initialRouteIconSpawnAmount);
        _activeRouteIcons = new List<RouteIcon>(_initialRouteIconSpawnAmount);

        for (int i = 0; i < _initialRouteIconSpawnAmount; i++)
        {
            SpawnRouteIcon();
        }
        
        _gridRouteInput.OnRouteStarted       += OnRouteStarted;
        _gridRouteInput.OnRouteEnded         += OnRouteEnded;
        _gridRouteInput.OnMoveDirectionAdded += OnMoveDirectionAdded;
    }

    private void OnDestroy()
    {
        _gridRouteInput.OnRouteStarted       -= OnRouteStarted;
        _gridRouteInput.OnRouteEnded         -= OnRouteEnded;
        _gridRouteInput.OnMoveDirectionAdded -= OnMoveDirectionAdded; 
    }

    private void OnRouteStarted()
    {
        OnMoveDirectionAdded(_grid.GridStartIndex);
    }

    private void OnRouteEnded()
    {
        // Add all active icons back to the pool
        while (_activeRouteIcons.Count > 0)
        {
            RouteIcon icon = _activeRouteIcons[0];
            _activeRouteIcons.RemoveAt(0);
            
            icon.Disable();
            _routeIconPool.Add(icon);
        } 
    }

    private void OnMoveDirectionAdded(int2 currentCellIndex)
    {
        RouteIcon icon = GetRouteIcon();
        icon.SetPosition(_grid.GetGridPos(currentCellIndex));
        icon.Enable();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SpawnRouteIcon()
    {
        RouteIcon icon = Instantiate(_routeIconPrefab, _routeIconParent);
        icon.Disable();
        
        _routeIconPool.Add(icon);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private RouteIcon GetRouteIcon()
    {
        if (_routeIconPool.Count <= 0)
        {
            SpawnRouteIcon();
        }
        
        RouteIcon icon = _routeIconPool[0];
        _routeIconPool.RemoveAt(0);
        
        _activeRouteIcons.Add(icon);
        
        return icon;
    }
}