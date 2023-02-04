//Auther: Tane Cotterell_eat (Roonstar96)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TrapsTypes
{
    SwingingBlade,
    PitFall,
    PoisonGas,
    PoisonDart,
    FireFloor,
    FallingCeiling 
}
public class Traps : MonoBehaviour
{
    [SerializeField] private TrapsTypes _trap;

    [SerializeField] private int _damage;
    [SerializeField] private int _agilityCheck;

    public Character charcter;
    private void Awake()
    {
        if (_trap == TrapsTypes.SwingingBlade)
        {
            _damage = 3;
            _agilityCheck = 1;
        }   
        if (_trap == TrapsTypes.PitFall)
        {
            _damage = 2;
            _agilityCheck = 3;
        }
        if (_trap == TrapsTypes.PoisonGas)
        {
            _damage = 2;
            _agilityCheck = 1;
        }
        if (_trap == TrapsTypes.PoisonDart)
        {
            _damage = 1;
            _agilityCheck = 3;
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

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Character")
        {
            charcter = other.GetComponent<Character>();
            //Do stuff on the map with an icon
            //Do stuff with the statue update thing in the UI
            TrapActive();
        }
    }

    private void TrapActive()
    {
        if (charcter.agility > _agilityCheck)
        {
            return;
        }

        else if (charcter.agility < _agilityCheck)
        {
            charcter.health -= _damage;
        }

        else if (charcter.agility == _agilityCheck)
        {
            int i = Random.Range(0, 2);
            int j = Random.Range(0, 2);

            if (i == j)
            {
                charcter.health = 0;
            }
            else
            {
                return;
            }
        }
    }
}
