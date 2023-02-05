using Unity.Mathematics;
using Random = UnityEngine.Random;

public class PuzzleEncounter : IEncounter
{
    public enum PuzzleTypes : int
    {
        SteppingStones = 0,
        Statues,
        FindKey,
        Potion,
        SolveRiddle,
        FreePrisoner,
        COUNT
    }

    public EncounterType EncounterType => EncounterType.PUZZLE;
    public int2 CellIndex => _cellIndex;
    private int2 _cellIndex;
    
    public PuzzleTypes _puzzle;
    private int _chance;
    private int _damage;
    
    public void Init(int2 cellIndex)
    {
        _cellIndex = cellIndex;

        // Randomly select a creature
        _puzzle = (PuzzleTypes)Random.Range(0, (int)PuzzleTypes.COUNT);
        InitValues();
    }

    private void InitValues()
    {
        if (_puzzle == PuzzleTypes.SteppingStones)
        {
            _chance = 2;
            _damage = 3;
        }
        if (_puzzle == PuzzleTypes.Statues)
        {
            _chance = 5;
            _damage = 1;
        }
        if (_puzzle == PuzzleTypes.FindKey)
        {
            _chance = 6;
            _damage = 0;
        }
        if (_puzzle == PuzzleTypes.Potion)
        {
            _chance = 3;
            _damage = 2;
        }
        if (_puzzle == PuzzleTypes.SolveRiddle)
        {
            _chance = 5;
            _damage = 1;
        }
        if (_puzzle == PuzzleTypes.FreePrisoner)
        {
            _chance = 3;
            _damage = 1;
        }
    }

    // private void OnTriggerEnter(Collider other)
    // {
    //     if (other.tag == "Character")
    //     {
    //         charcter = other.GetComponent<Character>();
    //         particle.Play();
    //         //Do stuff on the map with an icon
    //         //Do stuff with the statue update thing in the UI
    //         PuzzleActive();
    //     }
    // }

    public EncounterState AdventurerInteract(Character character)
    {
        int encounterChance = Random.Range(0, _chance + 1);

        if (encounterChance == _chance)
        {
            return EncounterState.ENCOUNTER_COMPLETE;
        }
        
        character.DamageCharacter(_damage);

        if (character.health <= 0)
        {
            return EncounterState.ADVENTURER_DEAD;
        }

        return EncounterState.ADVENTURER_RETRY;
    }

    // private IEnumerator Solved()
    // {
    //     particle.startColor = Color.white;
    //     particle.Play();
    //     gameObject.active = false;
    //
    //     yield return new WaitForSeconds(2.5f);
    //     Destroy(gameObject);
    // }
}
