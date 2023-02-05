using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class EquipmentEncounter : IEncounter
{
    
    public enum EquipmentTypes : int
    {
        NONE = -1,
        BOOTS = 0,
        LEGGINGS,
        CHEST,
        GLOVES,
        HELMET,
        COUNT,
    }
   
    public EncounterType EncounterType => EncounterType.EQUIPMENT;
    public int2 CellIndex => _cellIndex;
    private int2 _cellIndex;
    
    public EquipmentTypes _equipment;
    private int _healthBonusAmount;
    private int _attackBonusAmount;
    private int _agilityBonusAmount;
    
    public void Init(int2 cellIndex)
    {
        _cellIndex = cellIndex;
        
        // Randomly select an equipment type
        _equipment = (EquipmentTypes)Random.Range(0, (int)EquipmentTypes.COUNT);
        InitValues();
    }

    private void InitValues()
    {
        if (_equipment == EquipmentTypes.BOOTS)
        {
            _agilityBonusAmount = 2;
        }
        if (_equipment == EquipmentTypes.LEGGINGS)
        {
            _healthBonusAmount = 1;
        }
        if (_equipment == EquipmentTypes.CHEST)
        {
            _healthBonusAmount = 2;
        }
        if (_equipment == EquipmentTypes.GLOVES)
        {
            _attackBonusAmount = 1;
        }
        if (_equipment == EquipmentTypes.HELMET)
        {
            _attackBonusAmount = 1;
        }
    }
    
    public EncounterState AdventurerInteract(Character character)
    {
        Debug.Log("Interacted with Equipment/Armor encounter");
        character.health += _healthBonusAmount;
        character.attack += _attackBonusAmount;
        character.agility += _agilityBonusAmount;
        
        //Keeps equipment on the grid
        return EncounterState.ADVENTURER_PASSED;
    }
}
