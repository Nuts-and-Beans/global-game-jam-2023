using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CharacterObject : MonoBehaviour
{
    [SerializeField] private float stepDuration = 1f;
    [SerializeField] private Transform spriteTransform;
    
    // NOTE(WSWhitehouse): These are set from the CharacterMovementManager script.
    [NonSerialized] public Grid grid;
    [NonSerialized] public CharacterObjectManager characterObjectManager;
    
    private List<MoveDirection> _moveDirections;
    private Character _character;

    public void SetCharacter(Character character, List<MoveDirection> moveDirections)
    {
        _character      = character;
        _moveDirections = moveDirections;
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