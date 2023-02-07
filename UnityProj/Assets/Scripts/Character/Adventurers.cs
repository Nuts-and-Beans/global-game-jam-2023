using System.Collections.Generic;
using Random = UnityEngine.Random;

/// <summary>
/// This script pre-allocates all of the stats for all possible characters, and adds them to a Stack like structure
/// </summary>
public static class Adventurers 
{
    private static StackArray<Character> characters;

    static Adventurers() 
    {
        int count = Names.names.Length;
        characters = new (count);

        for (int i = 0; i < count; ++i) 
        {
            int r = Random.Range(0, 3);
            CharacterType type = (CharacterType)r;
            Character c = new ();
            c.type = type;
            c.name = Names.names[i];
            c.spriteIndex = Random.Range(0, 3); // HACK(Zack): being lazy with regards to the 

            //REVIEW(Sebadam2010): Not sure if this is a safe way to do it? 
            // c.equipment = new Equipment(EquipmentType.NONE);

            switch (type) 
            {
                case CharacterType.BARBARIAN:
                {
                    c.health  = 3;
                    c.attack  = 2;
                    c.agility = 1;
                } break;
                case CharacterType.ARCHER: 
                {
                    c.health  = 2;
                    c.attack  = 3;
                    c.agility = 1;
                } break;
                case CharacterType.ASSASSIN:
                {
                    c.health  = 1;
                    c.attack  = 2;
                    c.agility = 3;
                } break;
            }

            characters.Push(c); 
        }
    }

    public static Character GetNextCharacter() => characters.Pop();
    
    public static void ReturnCharacter(Character c) 
    {
        c.ResetCharacter();
        characters.Push(c);
    }
}
