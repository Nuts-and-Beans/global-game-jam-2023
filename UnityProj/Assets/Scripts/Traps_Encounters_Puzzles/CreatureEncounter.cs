//Auther: Tane Cotterell_eat (Roonstar96)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Creatures
{ 
	Goblins, 
	Skeletons, 
	Zombie, 
	Rats, 
	Cube, 
	Armor 
}
public class CreatureEncounter : MonoBehaviour
{
	[SerializeField] private Creatures _creatures;

	[SerializeField] private int _attack;
	[SerializeField] private int _health;
	[SerializeField] private int _groupSize;

	public Character charcter;
	public ParticleSystem particle;

	void Awake()
    {
		particle = gameObject.GetComponent<ParticleSystem>();
		gameObject.active = false;

		if ( _creatures == Creatures.Goblins)
        {
			_attack = _groupSize;
			_health = _groupSize;
		}
		if (_creatures == Creatures.Skeletons)
        {
			_attack = _groupSize + 1;
			_health = _groupSize;
        }
		if (_creatures == Creatures.Zombie)
        {
			_attack = _groupSize;
			_health = _groupSize - 1;
        }
		if (_creatures == Creatures.Rats)
        {
			_attack = 3;
			_health = 1;
			_groupSize = 0;
        }
		if (_creatures == Creatures.Cube)
        {
			_attack = 1;
			_health = 2;
			_groupSize = 0;
		}
		if (_creatures == Creatures.Armor)
        {
			_attack = 2;
			_health = 3;
			_groupSize = 0;
		}
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Character")
        {
			charcter = other.GetComponent<Character>();
			particle.Play();
			//Do stuff on the map with an icon
			//Do stuff with the statue update thing in the UI
			GroupEncounter();
		}
    }

    private void GroupEncounter()
	{
		gameObject.active = true;

		if (charcter.attack > _attack)
		{
			_health -= charcter.attack;
		}

		else if (charcter.attack < _attack)
		{
			charcter.health -= _attack;
		}

		else if (charcter.attack == _attack)
        {
			int i = Random.Range(0, 2);
			int j = Random.Range(0, 2);
			if (i == j)
            {
				charcter.health = 0;
            }
			else
            {
				StartCoroutine(Dead());
            }
        }

		if (_health <= 0)
		{
			StartCoroutine(Dead());
		}
	}

	private IEnumerator Dead()
	{
		particle.startColor = Color.white;
		particle.Play();
		gameObject.active = false;

		yield return new WaitForSeconds(2.5f);
		Destroy(gameObject);
	}
}
