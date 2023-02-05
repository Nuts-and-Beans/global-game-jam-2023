using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridEncounters : MonoBehaviour
{
    [Header("Encounter Settings")]
    [SerializeField] private int2 _encounterSpawnAmountRange;
    
    [Header("Grid Settings")]
    [SerializeField] private Grid _grid;

    [Header("Icon Settings")]
    [SerializeField] private int _initialIconSpawnAmount = 50;
    [SerializeField] private Transform _iconPoolParent;
    [SerializeField] private EncounterIcon _encounterIconPrefab;
    
    [Header("Character Encounter Icons")]
    [SerializeField] private Sprite _goblinSprite;
    [SerializeField] private Sprite _skeletonSprite;
    [SerializeField] private Sprite _zombieSprite;
    [SerializeField] private Sprite _ratsSprite;
    [SerializeField] private Sprite _cubeSprite;
    [SerializeField] private Sprite _knightSprite;
    [Space]
    [SerializeField] private AudioClip _goblinEncounterSFX;
    [SerializeField] private AudioClip _skeletonEncounterSFX;
    [SerializeField] private AudioClip _zombieEncounterSFX;
    [SerializeField] private AudioClip _ratsEncounterSFX;
    [SerializeField] private AudioClip _cubeEncounterSFX;
    [SerializeField] private AudioClip _knightEncounterSFX;
    [Space]
    [SerializeField] private AudioClip _goblinDeathSFX;
    [SerializeField] private AudioClip _skeletonDeathSFX;
    [SerializeField] private AudioClip _zombieDeathSFX;
    [SerializeField] private AudioClip _ratsDeathSFX;
    [SerializeField] private AudioClip _cubeDeathSFX;
    [SerializeField] private AudioClip _knightDeathSFX;
     
    [Header("Puzzle Encounter Icons")]
    [SerializeField] private Sprite _steppingStonesSprite;
    [SerializeField] private Sprite _statueSprite;
    [SerializeField] private Sprite _findKeySprite;
    [SerializeField] private Sprite _potionSprite;
    [SerializeField] private Sprite _solveRiddleSprite;
    [SerializeField] private Sprite _freePrisonerSprite;
    [Space]
    [SerializeField] private AudioClip _steppingStonesEncounterSFX;
    [SerializeField] private AudioClip _statuesEncounterSFX;
    [SerializeField] private AudioClip _findKeyEncounterSFX;
    [SerializeField] private AudioClip _potionEncounterSFX;
    [SerializeField] private AudioClip _solveRiddleEncounterSFX;
    [SerializeField] private AudioClip _freePrisonerEncounterSFX;
    
    [Header("Trap Encounter Icons")]
    [SerializeField] private Sprite _swingingBladeSprite;
    [SerializeField] private Sprite _pitFallSprite;
    [SerializeField] private Sprite _poisonGasSprite;
    [SerializeField] private Sprite _poisonDartSprite;
    [SerializeField] private Sprite _fireFloorSprite;
    [SerializeField] private Sprite _fallingCeilingSprite;
    [Space]
    [SerializeField] private AudioClip _swingingBladeEncounterSFX;
    [SerializeField] private AudioClip _pitFallEncounterSFX;
    [SerializeField] private AudioClip _poisonGasEncounterSFX;
    [SerializeField] private AudioClip _poisonDartEncounterSFX;
    [SerializeField] private AudioClip _fireFloorEncounterSFX;
    [SerializeField] private AudioClip _fallingCeilingEncounterSFX;
    
    [Header("Equipment Encounter Icons")]
    [SerializeField] private Sprite _bootsSprite;
    [SerializeField] private Sprite _leggingsSprite;
    [SerializeField] private Sprite _chestSprite;
    [SerializeField] private Sprite _glovesSprite;
    [SerializeField] private Sprite _helmetSprite;
    [SerializeField] private Sprite _swordSprite;
    [Space]
    [SerializeField] private AudioClip _equipmentSFX;

    [NonSerialized] public Dictionary<int2, IEncounter> encounters;
    [NonSerialized] public List<int2> visitedEncounters;
    
    [NonSerialized] public List<EncounterIcon> _iconPool;
    [NonSerialized] public Dictionary<int2, EncounterIcon> _activeIcons;

    private void Awake()
    {
        _iconPool    = new List<EncounterIcon>(_initialIconSpawnAmount);
        _activeIcons = new Dictionary<int2, EncounterIcon>(_initialIconSpawnAmount);
        for (int i = 0; i < _initialIconSpawnAmount; i++)
        {
            SpawnEncounterIcon();
        }
    }

    public void InitEncounters(Grid grid)
    {
        int encounterAmount = Random.Range(_encounterSpawnAmountRange.x, _encounterSpawnAmountRange.y);
        
        List<int2> validCells = grid.GetValidCells();

        encounters        = new Dictionary<int2, IEncounter>(encounterAmount);
        visitedEncounters = new List<int2>(encounterAmount);
        
        for (int i = 0; i < encounterAmount; i++)
        {
            EncounterType type = (EncounterType)Random.Range(0, (int)EncounterType.COUNT);
            
            IEncounter encounter = type switch
            {
                EncounterType.CREATURE  => new CreatureEncounter(),
                EncounterType.PUZZLE    => new PuzzleEncounter(),
                EncounterType.TRAP      => new TrapEncounter(),
                EncounterType.EQUIPMENT => new EquipmentEncounter(),
                
                EncounterType.COUNT => throw new ArgumentOutOfRangeException(),
                _ => throw new ArgumentOutOfRangeException()
            };
            
            int validCellIndex = Random.Range(0, validCells.Count);
            int2 cellIndex = validCells[validCellIndex];
            validCells.RemoveAt(validCellIndex);
            
            encounter.Init(cellIndex);
            encounters.Add(cellIndex, encounter);
        }
    }

    public void VisitEncounter(int2 cellIndex)
    {
        if (!encounters.ContainsKey(cellIndex)) return;
        
        
        if (visitedEncounters.Contains(cellIndex))
        {
            IEncounter encounter2 = encounters[cellIndex];
            EncounterIcon icon2 = _activeIcons[cellIndex];

            // play the ps if we are equipment. useless comment i know.
            if (encounter2.EncounterType == EncounterType.EQUIPMENT)
            {
                icon2.PlayParticleSystem();
                AudioManager.PlayOneShot(_equipmentSFX);
            }

            return;
        }
        
        visitedEncounters.Add(cellIndex);
        IEncounter encounter = encounters[cellIndex];

        Sprite iconSprite;

        switch (encounter.EncounterType)
        {
            case EncounterType.CREATURE:
            {
                CreatureEncounter creature = encounter as CreatureEncounter;
                switch (creature._creatureType)
                {
                    case CreatureEncounter.CreatureType.Goblins:
                    {
                        iconSprite = _goblinSprite;
                        AudioManager.PlayOneShot(_goblinEncounterSFX);
                        break;
                    }
                    case CreatureEncounter.CreatureType.Skeletons:
                    {
                        iconSprite = _skeletonSprite;
                        AudioManager.PlayOneShot(_skeletonEncounterSFX);
                        break;
                    }
                    case CreatureEncounter.CreatureType.Zombie:
                    {
                        iconSprite = _zombieSprite;
                        AudioManager.PlayOneShot(_zombieEncounterSFX);
                        break;
                    }
                    case CreatureEncounter.CreatureType.Rats:
                    {
                        iconSprite = _ratsSprite;
                        AudioManager.PlayOneShot(_ratsEncounterSFX);
                        break;
                    }
                    case CreatureEncounter.CreatureType.Cube:
                    {
                        iconSprite = _cubeSprite;
                        AudioManager.PlayOneShot(_cubeEncounterSFX);
                        break;
                    }
                    case CreatureEncounter.CreatureType.Knight:
                    {
                        iconSprite = _knightSprite;
                        AudioManager.PlayOneShot(_knightEncounterSFX);
                        break;
                    }
                    
                    case CreatureEncounter.CreatureType.COUNT:
                    default: throw new ArgumentOutOfRangeException();
                }
                
                break;
            }
            case EncounterType.PUZZLE:
            {
                PuzzleEncounter puzzle = encounter as PuzzleEncounter;
                switch (puzzle._puzzle)
                {
                    case PuzzleEncounter.PuzzleTypes.SteppingStones:
                    {
                        iconSprite = _steppingStonesSprite;
                        AudioManager.PlayOneShot(_steppingStonesEncounterSFX);
                        break;
                    }
                    case PuzzleEncounter.PuzzleTypes.Statues:
                    {
                        iconSprite = _statueSprite;
                        AudioManager.PlayOneShot(_statuesEncounterSFX);
                        break;
                    }
                    case PuzzleEncounter.PuzzleTypes.FindKey:
                    {
                        iconSprite = _findKeySprite;
                        AudioManager.PlayOneShot(_findKeyEncounterSFX);
                        break;
                    }
                    case PuzzleEncounter.PuzzleTypes.Potion:
                    {
                        iconSprite = _potionSprite;
                        AudioManager.PlayOneShot(_potionEncounterSFX);
                        break;
                    }
                    case PuzzleEncounter.PuzzleTypes.SolveRiddle:
                    {
                        iconSprite = _solveRiddleSprite;
                        AudioManager.PlayOneShot(_solveRiddleEncounterSFX);
                        break;
                    }
                    case PuzzleEncounter.PuzzleTypes.FreePrisoner:
                    {
                        iconSprite = _freePrisonerSprite;
                        AudioManager.PlayOneShot(_freePrisonerEncounterSFX);
                        break;
                    }
                    
                    case PuzzleEncounter.PuzzleTypes.COUNT:
                    default: throw new ArgumentOutOfRangeException();
                }
                
                break;
            }
            case EncounterType.TRAP:
            {
                TrapEncounter trap = encounter as TrapEncounter;
                switch (trap._trap)
                {
                    case TrapEncounter.TrapsTypes.SwingingBlade:
                    {
                        iconSprite = _swingingBladeSprite;
                        AudioManager.PlayOneShot(_swingingBladeEncounterSFX);
                        break;
                    }
                    case TrapEncounter.TrapsTypes.PitFall:
                    {
                        iconSprite = _pitFallSprite;
                        AudioManager.PlayOneShot(_pitFallEncounterSFX);
                        break;
                    }
                    case TrapEncounter.TrapsTypes.PoisonGas:
                    {
                        iconSprite = _poisonGasSprite;
                        AudioManager.PlayOneShot(_poisonGasEncounterSFX);
                        break;
                    }
                    case TrapEncounter.TrapsTypes.PoisonDart:
                    {
                        iconSprite = _poisonDartSprite;
                        AudioManager.PlayOneShot(_poisonDartEncounterSFX);
                        break;
                    }
                    case TrapEncounter.TrapsTypes.FireFloor:
                    {
                        iconSprite = _fireFloorSprite;
                        AudioManager.PlayOneShot(_fireFloorEncounterSFX);
                        break;
                    }
                    case TrapEncounter.TrapsTypes.FallingCeiling:
                    {
                        iconSprite = _fallingCeilingSprite;
                        AudioManager.PlayOneShot(_fallingCeilingEncounterSFX);
                        break;
                    }
                    
                    case TrapEncounter.TrapsTypes.COUNT:
                    default: throw new ArgumentOutOfRangeException();
                }
                
                break;
            }
            
            case EncounterType.EQUIPMENT:
            {
                EquipmentEncounter equipment = encounter as EquipmentEncounter;
                switch (equipment._equipment)
                {
                    case EquipmentEncounter.EquipmentTypes.BOOTS:
                    {
                        iconSprite = _bootsSprite;
                        break;
                    }
                    case EquipmentEncounter.EquipmentTypes.LEGGINGS:
                    {
                        iconSprite = _leggingsSprite;
                        break;
                    }
                    case EquipmentEncounter.EquipmentTypes.CHEST:
                    {
                        iconSprite = _chestSprite;
                        break;
                    }
                    case EquipmentEncounter.EquipmentTypes.GLOVES:
                    {
                        iconSprite = _glovesSprite;
                        break;
                    }
                    case EquipmentEncounter.EquipmentTypes.HELMET:
                    {
                        iconSprite = _helmetSprite;
                        break;
                    }
                    case EquipmentEncounter.EquipmentTypes.SWORD:
                    {
                        iconSprite = _swordSprite;
                        break;
                    }

                    case EquipmentEncounter.EquipmentTypes.COUNT:
                    default: throw new ArgumentOutOfRangeException();
                }
                
                break;
            }
            
            case EncounterType.COUNT:
            default: throw new ArgumentOutOfRangeException();
        }
        
        EncounterIcon icon = GetEncounterIcon(cellIndex);
        icon.transform.position = _grid.GetGridPos(cellIndex);
        icon.Enable(iconSprite, encounter.EncounterType);
    }

    public void RemoveEncounter(int2 cellIndex)
    {
        if (!visitedEncounters.Contains(cellIndex)) return;
        if (!_activeIcons.ContainsKey(cellIndex))   return;
        
        IEncounter encounter = encounters[cellIndex];
        if (encounter.EncounterType == EncounterType.CREATURE)
        {
            OnDeathSFX(encounter as CreatureEncounter);
        }
        
        ReturnEncounterIconToPool(cellIndex);
    }

    private void OnDeathSFX(CreatureEncounter encounter)
    {
        switch (encounter._creatureType)
        {
            case CreatureEncounter.CreatureType.Goblins:
            {
                AudioManager.PlayOneShot(_goblinDeathSFX);
                break;
            }
            case CreatureEncounter.CreatureType.Skeletons:
            {
                AudioManager.PlayOneShot(_skeletonDeathSFX);
                break;
            }
            case CreatureEncounter.CreatureType.Zombie:
            {
                AudioManager.PlayOneShot(_zombieDeathSFX);
                break;
            }
            case CreatureEncounter.CreatureType.Rats:
            {
                AudioManager.PlayOneShot(_ratsDeathSFX);
                break;
            }
            case CreatureEncounter.CreatureType.Cube:
            {
                AudioManager.PlayOneShot(_cubeDeathSFX);
                break;
            }
            case CreatureEncounter.CreatureType.Knight:
            {
                AudioManager.PlayOneShot(_knightDeathSFX);
                break;
            }
            
            case CreatureEncounter.CreatureType.COUNT:
            default: throw new ArgumentOutOfRangeException();
        }
    }

    private void SpawnEncounterIcon()
    {
        EncounterIcon icon = Instantiate(_encounterIconPrefab, _iconPoolParent);
        icon.gameObject.SetActive(false);
        _iconPool.Add(icon);
    }

    private EncounterIcon GetEncounterIcon(int2 cellIndex)
    {
        if (_iconPool.Count <= 0)
        {
            SpawnEncounterIcon();
        }
        
        EncounterIcon icon = _iconPool[0];
        _iconPool.RemoveAt(0);
        
        _activeIcons.Add(cellIndex, icon);
        return icon;
    }

    private void ReturnEncounterIconToPool(int2 cellIndex)
    {
        EncounterIcon icon = _activeIcons[cellIndex];
        _activeIcons.Remove(cellIndex);
        
        icon.Disable();
        _iconPool.Add(icon);
    }
}