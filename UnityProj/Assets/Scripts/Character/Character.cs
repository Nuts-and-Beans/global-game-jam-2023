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
    public int health;
    public int attack; 
    public int agility;
    
    public void ApplyEquipmentToCharacter(Equipment newEquipment)
    {
        equipment = newEquipment;
        
        health += newEquipment.healthBonus;
        attack += newEquipment.attackBonus;
        agility += newEquipment.agilityBonus;
    }
    
    public void RemoveEquipmentFromCharacter()
    {
        health -= equipment.healthBonus;
        attack -= equipment.attackBonus;
        agility -= equipment.agilityBonus;
        
        //REVIEW(Sebadam2010): This method will create a lot of empty Equipment objects in the long term? What is a better way to do it?
        equipment = new Equipment(EquipmentType.NONE);
    }
}
