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
    
    [Header("Puzzle Encounter Icons")]
    [SerializeField] private Sprite _steppingStonesSprite;
    [SerializeField] private Sprite _statueSprite;
    [SerializeField] private Sprite _findKeySprite;
    [SerializeField] private Sprite _potionSprite;
    [SerializeField] private Sprite _solveRiddleSprite;
    [SerializeField] private Sprite _freePrisonerSprite;
    
    [Header("Trap Encounter Icons")]
    [SerializeField] private Sprite _swingingBladeSprite;
    [SerializeField] private Sprite _pitFallSprite;
    [SerializeField] private Sprite _poisonGasSprite;
    [SerializeField] private Sprite _poisonDartSprite;
    [SerializeField] private Sprite _fireFloorSprite;
    [SerializeField] private Sprite _fallingCeilingSprite;
    
    [Header("Equipment Encounter Icons")]
    [SerializeField] private Sprite _bootsSprite;
    [SerializeField] private Sprite _leggingsSprite;
    [SerializeField] private Sprite _chestSprite;
    [SerializeField] private Sprite _glovesSprite;
    [SerializeField] private Sprite _helmetSprite;
    
    
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
                EncounterType.CREATURE => new CreatureEncounter(),
                EncounterType.PUZZLE   => new PuzzleEncounter(),
                EncounterType.TRAP     => new TrapEncounter(),
                EncounterType.EQUIPMENT => new EquipmentEncounter(),
                EncounterType.COUNT    =>  throw new ArgumentOutOfRangeException(),
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
        if (!encounters.ContainsKey(cellIndex))    return;
        if (visitedEncounters.Contains(cellIndex)) return;
        
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
                        break;
                    }
                    case CreatureEncounter.CreatureType.Skeletons:
                    {
                        iconSprite = _skeletonSprite;
                        break;
                    }
                    case CreatureEncounter.CreatureType.Zombie:
                    {
                        iconSprite = _zombieSprite;
                        break;
                    }
                    case CreatureEncounter.CreatureType.Rats:
                    {
                        iconSprite = _ratsSprite;
                        break;
                    }
                    case CreatureEncounter.CreatureType.Cube:
                    {
                        iconSprite = _cubeSprite;
                        break;
                    }
                    case CreatureEncounter.CreatureType.Knight:
                    {
                        iconSprite = _knightSprite;
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
                        break;
                    }
                    case PuzzleEncounter.PuzzleTypes.Statues:
                    {
                        iconSprite = _statueSprite;
                        break;
                    }
                    case PuzzleEncounter.PuzzleTypes.FindKey:
                    {
                        iconSprite = _findKeySprite;
                        break;
                    }
                    case PuzzleEncounter.PuzzleTypes.Potion:
                    {
                        iconSprite = _potionSprite;
                        break;
                    }
                    case PuzzleEncounter.PuzzleTypes.SolveRiddle:
                    {
                        iconSprite = _solveRiddleSprite;
                        break;
                    }
                    case PuzzleEncounter.PuzzleTypes.FreePrisoner:
                    {
                        iconSprite = _freePrisonerSprite;
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
                        break;
                    }
                    case TrapEncounter.TrapsTypes.PitFall:
                    {
                        iconSprite = _pitFallSprite;
                        break;
                    }
                    case TrapEncounter.TrapsTypes.PoisonGas:
                    {
                        iconSprite = _poisonGasSprite;
                        break;
                    }
                    case TrapEncounter.TrapsTypes.PoisonDart:
                    {
                        iconSprite = _poisonDartSprite;
                        break;
                    }
                    case TrapEncounter.TrapsTypes.FireFloor:
                    {
                        iconSprite = _fireFloorSprite;
                        break;
                    }
                    case TrapEncounter.TrapsTypes.FallingCeiling:
                    {
                        iconSprite = _fallingCeilingSprite;
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
                        iconSprite = _bootsSprite;
                        break;
                    }
                    case EquipmentEncounter.EquipmentTypes.CHEST:
                    {
                        iconSprite = _bootsSprite;
                        break;
                    }
                    case EquipmentEncounter.EquipmentTypes.GLOVES:
                    {
                        iconSprite = _bootsSprite;
                        break;
                    }
                    case EquipmentEncounter.EquipmentTypes.HELMET:
                    {
                        iconSprite = _bootsSprite;
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
        
        ReturnEncounterIconToPool(cellIndex);
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