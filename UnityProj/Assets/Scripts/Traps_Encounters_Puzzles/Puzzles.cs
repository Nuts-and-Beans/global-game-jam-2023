using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PuzzleTypes
{
    SteppingStones,
    Statues,
    FindKey,
    Potion,
    SolveRiddle,
    FreePrisoner
}
public class Puzzles : MonoBehaviour
{
    [SerializeField] private PuzzleTypes _puzzle;

    [SerializeField] private int _chance;
    [SerializeField] private int _damage;

    public Character charcter;
    // Start is called before the first frame update
    private void Awake()
    {
        if (_puzzle == PuzzleTypes.SteppingStones)
        {
            _chance = 5;
            _damage = 2;
        }
        if (_puzzle == PuzzleTypes.Statues)
        {
            _chance = 5;
            _damage = 0;
        }
        if (_puzzle == PuzzleTypes.FindKey)
        {
            _chance = 10;
            _damage = 0;
        }
        if (_puzzle == PuzzleTypes.Potion)
        {
            _chance = 2;
            _damage = 3;
        }
        if (_puzzle == PuzzleTypes.SolveRiddle)
        {
            _chance = 10;
            _damage = 0;
        }
        if (_puzzle == PuzzleTypes.FreePrisoner)
        {
            _chance = 2;
            _damage = 1;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Character")
        {
            charcter = other.GetComponent<Character>();
            //Do stuff on the map with an icon
            //Do stuff with the statue update thing in the UI
            PuzzleActive();
        }
    }

    private void PuzzleActive()
    {
        int i = Random.Range(0, _chance + 1);

        if (i == _chance)
        {
            Solved();
        }
        else
        {
            charcter.health -= _damage;
        }
    }

    private void Solved()
    {
        return;
    }
}
