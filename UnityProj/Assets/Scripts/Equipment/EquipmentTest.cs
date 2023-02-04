using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentTest : MonoBehaviour
{
    private Equipment _equipment;
    private Character _character;
    
    
    public void Awake()
    {
        _equipment = EquipmentList.GetNextEquipment();
        _character = Adventurers.GetNextCharacter();
        
        Debug.Log($"Character: {_character.name} - {_character.type} - Health: {_character.health} - Attack: {_character.attack} - Agility: {_character.agility}");
        Debug.Log($"Equipment: {_equipment.equipmentType} - HealthBonus: {_equipment.healthBonus} - AttackBonus: {_equipment.attackBonus} - AgilityBonus: {_equipment.agilityBonus}");
    }
    
}
