
using Unity.Mathematics;

public enum EncounterType
{
    CREATURE,
    PUZZLE,
    TRAP,
    COUNT
}

public enum EncounterState
{
    ADVENTURER_DEAD,    // The adventurer has died 
    ENCOUNTER_COMPLETE, // The adventurer cleared the encounter gets destroyed
    ADVENTURER_PASSED,  // The adventurer passed the encounter but it's still there
    ADVENTURER_RETRY    // The adventurer must retry the encounter
}

public interface IEncounter
{
    public EncounterType EncounterType { get; }
    public int2 CellIndex { get; }
    
    public void Init(int2 cellIndex) {}
    
    // Returns true when adventurer wins the interaction, false when they die
    public EncounterState AdventurerInteract(Character character);
}
