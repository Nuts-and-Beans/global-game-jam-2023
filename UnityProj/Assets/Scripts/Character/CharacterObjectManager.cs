using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

public class CharacterObjectManager : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private Grid _grid;
    [SerializeField] private GridRouteInput _gridRouteInput;
    [SerializeField] private GridEncounters _gridEncounters;
    [SerializeField] private Boss _boss;
    
    [Header("Character Movement")]
    [SerializeField] private CharacterObject _characterObjectPrefab;
    [SerializeField] private Transform _characterObjectTransform;
    [SerializeField] private int _initialCharObjectSpawnAmount = 25;
    
    private List<CharacterObject> _characterObjectPool;
    private List<CharacterObject> _activeCharacterObjects;

    private void Awake()
    {
        _characterObjectPool    = new List<CharacterObject>(_initialCharObjectSpawnAmount);
        _activeCharacterObjects = new List<CharacterObject>(_initialCharObjectSpawnAmount);

        for (int i = 0; i < _initialCharObjectSpawnAmount; i++)
        {
            SpawnCharacterObject();
        }
        
        _gridRouteInput.OnMoveDirectionsConfirmed += OnMoveDirectionsConfirmed;
    }

    private void OnDestroy()
    {
        _gridRouteInput.OnMoveDirectionsConfirmed -= OnMoveDirectionsConfirmed;
    }

    private void OnMoveDirectionsConfirmed(Character character, List<MoveDirection> moveDirections)
    {
        List<MoveDirection> moveDir = new List<MoveDirection>(moveDirections);

        CharacterObject cm = GetCharacterObject();
        cm.SetCharacter(character, moveDir);
        
        cm.gameObject.SetActive(true);
        StartCoroutine(cm.Move());
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SpawnCharacterObject()
    {
        CharacterObject charObject = Instantiate(_characterObjectPrefab, _characterObjectTransform);
        charObject.grid                   = _grid;
        charObject.characterObjectManager = this;
        
        charObject.gameObject.SetActive(false);
        
        _characterObjectPool.Add(charObject);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private CharacterObject GetCharacterObject()
    {
        if (_characterObjectPool.Count <= 0)
        {
            SpawnCharacterObject();
        }
        
        CharacterObject characterObject = _characterObjectPool[0];
        _characterObjectPool.RemoveAt(0);
        
        _activeCharacterObjects.Add(characterObject);
        
        return characterObject;
    }

    public void ReturnCharacterObjectToPool(CharacterObject charObject)
    {
        _activeCharacterObjects.Remove(charObject);
        _characterObjectPool.Add(charObject);
    }

    public EncounterState CheckForGridCellInteraction(Character character, int2 cellIndex)
    {
        // Check for boss tile
        if ((_grid.GridBossIndex == cellIndex).All())
        {
            Boss.RemoveHealth();
            return EncounterState.ADVENTURER_DEAD;
        }
        
        // Check for other grid interactions
        if (!_gridEncounters.encounters.ContainsKey(cellIndex)) return EncounterState.ADVENTURER_PASSED;
        
        _gridEncounters.VisitEncounter(cellIndex);
        
        Debug.Log("Doing Encounter Interaction");
        
        IEncounter encounter = _gridEncounters.encounters[cellIndex];
        EncounterState state = encounter.AdventurerInteract(character);

        if (state == EncounterState.ENCOUNTER_COMPLETE)
        {
            _gridEncounters.encounters.Remove(cellIndex);
            _gridEncounters.RemoveEncounter(cellIndex);
        }
        
        return state;
    }
}
