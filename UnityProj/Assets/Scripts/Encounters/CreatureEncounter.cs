//Auther: Tane Cotterell_eat (Roonstar96)

using System;
using Unity.Mathematics;
using Random = UnityEngine.Random;

public class CreatureEncounter : IEncounter
{
    public enum CreatureType : int
    {
        Goblins = 0,
        Skeletons,
        Zombie,
        Rats,
        Cube,
        Knight,
        COUNT
    }

    public EncounterType EncounterType => EncounterType.CREATURE;
    public int2 CellIndex => _cellIndex;
    private int2 _cellIndex;
    
    public CreatureType _creatureType;
    private int _attack;
    private int _health;
    private int _groupSize;

    public void Init(int2 cellIndex)
    {
        _cellIndex = cellIndex;

        // Randomly select a creature
        _creatureType = (CreatureType)Random.Range(0, (int)CreatureType.COUNT);
        InitValues();
    }

    private void InitValues()
    {
        _groupSize = Random.Range(1, 4);
        
        switch (_creatureType)
        {
            case CreatureType.Goblins:
            {
                _attack = _groupSize;
                _health = _groupSize;
                break;
            }
            case CreatureType.Skeletons:
            {
                _attack = _groupSize + 1;
                _health = _groupSize;
                break;
            }
            case CreatureType.Zombie:
            {
                _attack = _groupSize;
                _health = _groupSize - 1;
                break;
            }
            case CreatureType.Rats:
            {
                _attack = 3;
                _health = 1;
                break;
            }
            case CreatureType.Cube:
            {
                _attack = 1;
                _health = 2;
                break;
            }
            case CreatureType.Knight:
            {
                _attack = 2;
                _health = 3;
                break;
            }
            
            case CreatureType.COUNT:
            default: throw new ArgumentOutOfRangeException();
        } 
    }

    public EncounterState AdventurerInteract(Character character)
    {
        if (character.attack > _attack)
        {
            _health -= character.attack;
        }
        else if (character.attack < _attack)
        {
            character.health -= _attack;
        }
        else if (character.attack == _attack)
        {
            float percentage = Random.Range(0.0f, 1.0f);

            if (percentage > 0.5f)
            {
                _health -= character.attack;
            }
            else
            {
                character.health -= _attack;
            }
        }

        if (_health <= 0)
        {
            return EncounterState.ENCOUNTER_COMPLETE;
        }

        if (character.health <= 0)
        {
            return EncounterState.ADVENTURER_DEAD;
        }
        
        return EncounterState.ENCOUNTER_COMPLETE;
    }

    // private IEnumerator Dead()
    // {
    //     particle.startColor = Color.white;
    //     particle.Play();
    //     gameObject.active = false;
    //
    //     yield return new WaitForSeconds(2.5f);
    //     Destroy(gameObject);
    // }
}