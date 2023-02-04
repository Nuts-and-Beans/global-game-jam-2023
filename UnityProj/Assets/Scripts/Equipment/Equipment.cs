public class Equipment
{
    public EquipmentType equipmentType;
    public int healthBonus;
    public int attackBonus;
    public int agilityBonus;

    public Equipment()
    {
        
    }
    public Equipment(EquipmentType type)
    {
        equipmentType = type;
        if (equipmentType == EquipmentType.NONE)
        {
            healthBonus = 0;
            attackBonus = 0;
            agilityBonus = 0;
        }
    }
    
    
}


