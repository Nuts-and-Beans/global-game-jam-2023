using System;
using UnityEngine;

public class AdventurerSprites : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Sprite[] _barbarianSprites = Array.Empty<Sprite>(); 
    [SerializeField] private Sprite[] _archerSprites    = Array.Empty<Sprite>(); 
    [SerializeField] private Sprite[] _assassinSprites  = Array.Empty<Sprite>(); 

    public Sprite GetSprite(Character cm)
    {
        Sprite[] sprites = cm.type switch
        {
            CharacterType.BARBARIAN => _barbarianSprites,
            CharacterType.ARCHER    => _archerSprites,
            CharacterType.ASSASSIN  => _assassinSprites,
            _ => throw new ArgumentOutOfRangeException(),
        };

        return sprites[cm.spriteIndex];
    }
}
