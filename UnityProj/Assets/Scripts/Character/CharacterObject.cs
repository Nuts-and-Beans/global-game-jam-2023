using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CharacterObject : MonoBehaviour
{
    [SerializeField] private float stepDuration = 1f;
    [SerializeField] private Transform spriteTransform;
    [SerializeField] private Image sprite;
    
    [SerializeField] private Sprite[] _barbarianSprites = Array.Empty<Sprite>();
    [SerializeField] private Sprite[] _archerSprites    = Array.Empty<Sprite>();
    [SerializeField] private Sprite[] _assassinSprites  = Array.Empty<Sprite>();
    
    // NOTE(WSWhitehouse): These are set from the CharacterMovementManager script.
    [NonSerialized] public Grid grid;
    [NonSerialized] public CharacterObjectManager characterObjectManager;
    
    private List<MoveDirection> _moveDirections;
    private Character _character;

    public void SetCharacter(Character character, List<MoveDirection> moveDirections)
    {
        _character      = character;
        _moveDirections = moveDirections;
        
        if (_character == null) return;
        
        Sprite[] sprites = _character.type switch
        {
            CharacterType.BARBARIAN => _barbarianSprites,
            CharacterType.ARCHER    => _archerSprites,
            CharacterType.ASSASSIN  => _assassinSprites,
            _ => throw new ArgumentOutOfRangeException()
        };

        if (sprites.Length <= 0)
        {
            int spriteIndex = Random.Range(0, sprites.Length);
            sprite.sprite = sprites[spriteIndex];
        }
    }

    public IEnumerator Move()
    {
        int2 currentCellIndex = grid.GridStartIndex;
        for (int i = 0; i < _moveDirections.Count; i++)
        {
            MoveDirection direction = _moveDirections[i];
            int2 endCellIndex = currentCellIndex + MoveDirectionUtil.GetDirectionIndex(direction);
            
            float3 startPos = grid.GetGridPos(currentCellIndex);
            float3 EndPos   = grid.GetGridPos(endCellIndex);
            
            yield return Lerp(startPos, EndPos);
            
            currentCellIndex = endCellIndex;
        }
        
        gameObject.SetActive(false);
        characterObjectManager.ReturnCharacterObjectToPool(this);
    }

    private IEnumerator Lerp(float3 start, float3 end)
    {
        float timeElapsed  = 0.0f;
        
        float3 spritePos = spriteTransform.localPosition;

        while (timeElapsed < stepDuration)
        {
            float t = timeElapsed / stepDuration;
            transform.position            = Vector3.Lerp(start, end, t);
            
            if (t > 0.5f) t = 1.0f - t;
            spriteTransform.localPosition = new Vector3(spritePos.x, math.sin(t), spritePos.z);
            
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    
        transform.position            = end;
        spriteTransform.localPosition = spritePos;
    }
}