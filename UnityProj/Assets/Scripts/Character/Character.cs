using UnityEngine;

public enum CharacterType 
{
    BARBARIAN = 0,
    ARCHER,
    ASSASSIN,
};

public class Character
{
    public CharacterType type;
    public Equipment equipment;
    public string name;
    
    public int maxHealth = 3;
    
    public int health;
    public int attack; 
    public int agility;
    
    public delegate void CharacterDel(int newHealth);
    public event CharacterDel OnCharacterHealthEvent;
    
    public void HealCharacter(int healAmount)
    {
        health += healAmount;

        if (health > maxHealth)
        {
            health = maxHealth;
        }
        
        OnCharacterHealthEvent?.Invoke(health);
    }

    public void ApplyEquipmentToCharacter(Equipment newEquipment)
    {
        //Won't apply the equipment if the character already has something equipped
        if (equipment.equipmentType != EquipmentType.NONE)
        {
            Debug.Log("Character has equipment already equipped");
            return;
        }
        
        equipment = newEquipment;
        
        health += newEquipment.healthBonus;
        attack += newEquipment.attackBonus;
        agility += newEquipment.agilityBonus;
    }
    
    public void RemoveEquipmentFromCharacter()
    {
        //Returns from this function if character already has nothing equipped
        if (equipment == null || equipment.equipmentType == EquipmentType.NONE)
        {
            Debug.LogWarning("Character already has nothing equipped");
            return;
            
        }
        
        health -= equipment.healthBonus;
        attack -= equipment.attackBonus;
        agility -= equipment.agilityBonus;
        
        //REVIEW(Sebadam2010): This method will create a lot of empty Equipment objects in the long term? What is a better way to do it?
        equipment = new Equipment(EquipmentType.NONE);
    }
    
    public void ResetCharacter()
    {
        //REVIEW(Sebadam2010): This method will create a lot of empty Equipment objects in the long term? What is a better way to do it?
        equipment = new Equipment(EquipmentType.NONE);
        
        switch (type) 
        {
            case CharacterType.BARBARIAN:
            {
                health  = 3;
                attack  = 2;
                agility = 1;
            } break;
            case CharacterType.ARCHER: 
            {
                health  = 2;
                attack  = 3;
                agility = 1;
            } break;
            case CharacterType.ASSASSIN:
            {
                health  = 1;
                attack  = 2;
                agility = 3;
            } break;
        }
    }
}
