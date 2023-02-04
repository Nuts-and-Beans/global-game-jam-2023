using System.Collections.Generic;
using UnityEngine;

public class CharacterMovementManager : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private Grid _grid;
    [SerializeField] private GridRouteInput _gridRouteInput;
    
    [Header("Character Movement")]
    [SerializeField] private CharacterMovement _characterMovementPrefab;

    private void Awake()
    {
        _gridRouteInput.OnMoveDirectionsConfirmed += OnMoveDirectionsConfirmed;
    }

    private void OnDestroy()
    {
        _gridRouteInput.OnMoveDirectionsConfirmed -= OnMoveDirectionsConfirmed;
    }

    private void OnMoveDirectionsConfirmed(List<MoveDirection> moveDirections)
    {
        CharacterMovement cm = Instantiate(_characterMovementPrefab);
        
        cm.grid           = _grid;
        cm.startCellIndex = _grid.GridStartIndex;
        cm.moveDirections = moveDirections;
        
        StartCoroutine(cm.Move());
    }
}
