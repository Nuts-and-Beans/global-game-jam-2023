using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Audio;

public class CharacterObjectManager : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private Grid _grid;
    [SerializeField] private GridRouteInput _gridRouteInput;
    [SerializeField] private GridEncounters _gridEncounters;
    [SerializeField] private AdventurerSprites _adventurerSprites; 

    [Header("Character Movement")]
    [SerializeField] private CharacterObject _characterObjectPrefab;
    [SerializeField] private Transform _characterObjectTransform;
    [SerializeField] private int _initialCharObjectSpawnAmount = 25;
    
    [Header("Boss Settings")]
    [SerializeField] private Boss _boss;
    [SerializeField] private EncounterIcon _bossEncounterIcon;
    [SerializeField] private AudioClip _bossEncounterSFX;
    
    private bool _visitedBoss = false;

    private List<CharacterObject> _characterObjectPool;
    private List<CharacterObject> _activeCharacterObjects;

    private void Awake()
    {
        Debug.Assert(_adventurerSprites != null, "Adventurer Sprites has not been set in the inspector", this);

        _bossEncounterIcon.Disable();
        
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

        Sprite sprite = _adventurerSprites.GetSprite(character);
        CharacterObject cm = GetCharacterObject();
        cm.SetCharacter(character, sprite,  moveDir);
        
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

            if (!_visitedBoss)
            {
                _bossEncounterIcon.transform.position = _grid.GetGridPos(cellIndex);
                _bossEncounterIcon.EnableBossIcon();
                AudioManager.PlayBossOneShot(_bossEncounterSFX);
                _visitedBoss = true;
            }
            
            return EncounterState.ADVENTURER_DEAD;
        }
        
        // Check for other grid interactions
        if (!_gridEncounters.encounters.ContainsKey(cellIndex)) return EncounterState.ADVENTURER_PASSED;
        
        _gridEncounters.VisitEncounter(cellIndex);
        
#if UNITY_EDITOR
        Debug.Log("Doing Encounter Interaction");
#endif

        IEncounter encounter = _gridEncounters.encounters[cellIndex];
        EncounterState state = encounter.AdventurerInteract(character);

        if (state == EncounterState.ENCOUNTER_COMPLETE)
        {
            _gridEncounters.RemoveEncounter(cellIndex);
        }
        
        return state;
    }
}
