using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private float stepDuration = 1f;
    [SerializeField] private Transform spriteTransform;
    
    // NOTE(WSWhitehouse): These are set from the CharacterMovementManager script.
    [NonSerialized] public int2 startCellIndex;
    [NonSerialized] public List<MoveDirection> moveDirections;
    [NonSerialized] public Grid grid;

    public IEnumerator Move()
    {
        int2 currentCellIndex = startCellIndex;
        for (int i = 0; i < moveDirections.Count; i++)
        {
            MoveDirection direction = moveDirections[i];
            int2 endCellIndex = currentCellIndex + MoveDirectionUtil.GetDirectionIndex(direction);
            
            float3 startPos = grid.GetGridPos(currentCellIndex);
            float3 EndPos   = grid.GetGridPos(endCellIndex);
            
            yield return Lerp(startPos, EndPos);
            
            currentCellIndex = endCellIndex;
        }
        
        Destroy(this.gameObject);
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