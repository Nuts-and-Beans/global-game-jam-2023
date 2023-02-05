using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridEncounters : MonoBehaviour
{
    [Header("Encounter Settings")]
    [SerializeField] private int2 _encounterSpawnAmountRange;
    
    public Dictionary<int2, IEncounter> encounters;

    public void InitEncounters(Grid grid)
    {
        int encounterAmount = Random.Range(_encounterSpawnAmountRange.x, _encounterSpawnAmountRange.y);
        
        List<int2> validCells = grid.GetValidCells();

        encounters = new Dictionary<int2, IEncounter>(encounterAmount);
        for (int i = 0; i < encounterAmount; i++)
        {
            EncounterType type = (EncounterType)Random.Range(0, (int)EncounterType.COUNT);
            
            IEncounter encounter = type switch
            {
                EncounterType.CREATURE => new CreatureEncounter(),
                EncounterType.PUZZLE   => new PuzzleEncounter(),
                EncounterType.TRAP     => new TrapEncounter(),
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
}