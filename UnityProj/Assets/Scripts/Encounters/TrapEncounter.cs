//Auther: Tane Cotterell_eat 

using Unity.Mathematics;
using Random = UnityEngine.Random;

public class TrapEncounter : IEncounter
{
    public enum TrapsTypes : int
    {
        SwingingBlade = 0,
        PitFall,
        PoisonGas,
        PoisonDart,
        FireFloor,
        FallingCeiling ,
        COUNT 
    }
    
    public EncounterType EncounterType => EncounterType.TRAP;
    public int2 CellIndex => _cellIndex;
    private int2 _cellIndex;
    
    public TrapsTypes _trap;
    private int _damage;
    private int _agilityCheck;
    
    public void Init(int2 cellIndex)
    {
        _cellIndex = cellIndex;

        // Randomly select a creature
        _trap = (TrapsTypes)Random.Range(0, (int)TrapsTypes.COUNT);
        InitValues();
    }


    private void InitValues()
    {
        if (_trap == TrapsTypes.SwingingBlade)
        {
            _damage = 3;
            _agilityCheck = 2;
        }   
        if (_trap == TrapsTypes.PitFall)
        {
            _damage = 2;
            _agilityCheck = 3;
        }
        if (_trap == TrapsTypes.PoisonGas)
        {
            _damage = 1;
            _agilityCheck = 3;
        }
        if (_trap == TrapsTypes.PoisonDart)
        {
            _damage = 1;
            _agilityCheck = 2;
        }
        if (_trap == TrapsTypes.FireFloor)
        {
            _damage = 1;
            _agilityCheck = 2;
        }
        if (_trap == TrapsTypes.FallingCeiling)
        {
            _damage = 3;
            _agilityCheck = 2;
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
    //         TrapActive();
    //     }
    // }

    public EncounterState AdventurerInteract(Character character)
    {
        int agility = character.agility + Random.Range(0, 2);
        
        if (agility > _agilityCheck)
        {
            return EncounterState.ADVENTURER_PASSED;
        }

        if (agility < _agilityCheck)
        {
            character.DamageCharacter(_damage);
        }
        

        if (agility == _agilityCheck)
        {
            float percentage = Random.Range(0.0f, 1.0f);

            if (percentage > 0.5f)
            {
                character.DamageCharacter(_damage);
            }
        }

        if (character.health <= 0)
        {
            return EncounterState.ADVENTURER_DEAD;
        }
        
        return EncounterState.ADVENTURER_RETRY;
    }

    // private IEnumerator AdventurerHit()
    // {
    //     particle.startColor = Color.red;
    //     particle.Play();
    //     gameObject.active = false;
    //
    //     yield return new WaitForSeconds(2.5f);
    //     particle.Stop();
    // }
}
