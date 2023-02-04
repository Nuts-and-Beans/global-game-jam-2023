public enum CharacterType 
{
    BARBARIAN = 0,
    ARCHER,
    ASSASSIN,
};

public class Character
{
    public CharacterType type;
    public string name;
    public int health;
    public int attack; 
    public int agility;
}
