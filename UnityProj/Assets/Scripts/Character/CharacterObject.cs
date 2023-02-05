using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CharacterObject : MonoBehaviour
{
    [SerializeField] private float _stepDuration = 0.5f;
    [SerializeField] private Transform _spriteTransform;
    [SerializeField] private Image _sprite;
    
    [Header("Character Type Sprites")]
    [SerializeField] private Sprite[] _barbarianSprites = Array.Empty<Sprite>();
    [SerializeField] private Sprite[] _archerSprites    = Array.Empty<Sprite>();
    [SerializeField] private Sprite[] _assassinSprites  = Array.Empty<Sprite>();
    
    // NOTE(WSWhitehouse): These are set from the CharacterMovementManager script.
    [NonSerialized] public Grid grid;
    [NonSerialized] public CharacterObjectManager characterObjectManager;
    
    public Action OnCharacterSet;
    public Action OnCharacterRemoved;
    
    private List<MoveDirection> _moveDirections;
    [NonSerialized] public Character _character;

    private void Start()
    {
        _character = Adventurers.GetNextCharacter();
    }

    public void SetCharacter(Character character, List<MoveDirection> moveDirections)
    {
        _character      = character;
        _moveDirections = moveDirections;
        
        OnCharacterSet?.Invoke();
        
        if (_character == null) return;
        
        Sprite[] sprites = _character.type switch
        {
            CharacterType.BARBARIAN => _barbarianSprites,
            CharacterType.ARCHER    => _archerSprites,
            CharacterType.ASSASSIN  => _assassinSprites,
            _ => throw new ArgumentOutOfRangeException()
        };

        if (sprites.Length > 0)
        {
            int spriteIndex = Random.Range(0, sprites.Length);
            _sprite.sprite = sprites[spriteIndex];
        }
    }

    private void RemoveCharacter()
    {
        OnCharacterRemoved?.Invoke();
        _character = null;
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

            // Remove fog of war
            if (grid.GridCells[currentCellIndex].isFogOfWar)
            {
                grid.GridCells[currentCellIndex].isFogOfWar = false;
                grid.GridCells[currentCellIndex].fogOfWar.SetActive(false);
            }
            
            EncounterState encounterState;
            
            do
            {
                encounterState = characterObjectManager.CheckForGridCellInteraction(_character, currentCellIndex);

                if (encounterState == EncounterState.ADVENTURER_DEAD)
                {
                    RemoveCharacter();
                    gameObject.SetActive(false);
                    characterObjectManager.ReturnCharacterObjectToPool(this);
                    yield break;
                }

                if (encounterState == EncounterState.ADVENTURER_RETRY)
                {
                    float wait = Random.Range(0.5f, 1.5f);
                    yield return new WaitForSeconds(wait);
                }
            }
            while (encounterState == EncounterState.ADVENTURER_RETRY);
            
        }
        
        RemoveCharacter();
        gameObject.SetActive(false);
        characterObjectManager.ReturnCharacterObjectToPool(this);
    }

    private IEnumerator Lerp(float3 start, float3 end)
    {
        float timeElapsed  = 0.0f;
        
        float3 spritePos = _spriteTransform.localPosition;

        while (timeElapsed < _stepDuration)
        {
            float t = timeElapsed / _stepDuration;
            transform.position            = Vector3.Lerp(start, end, t);
            
            if (t > 0.5f) t = 1.0f - t;
            _spriteTransform.localPosition = new Vector3(spritePos.x, math.sin(t), spritePos.z);
            
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    
        transform.position             = end;
        _spriteTransform.localPosition = spritePos;
    }
}