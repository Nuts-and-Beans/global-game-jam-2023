using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTest : MonoBehaviour
{
    //NOTE(Sebadam2010): This is a test script for the CharacterHealthUI script. To allow for monobehaviour and test out a character's health

    public Character _character;
    
    private void Awake()
    {
        _character = Adventurers.GetNextCharacter();
    }
}
