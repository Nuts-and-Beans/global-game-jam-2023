using Unity.Mathematics;
using UnityEngine;

public class RouteIcon : MonoBehaviour
{
    public void SetPosition(float3 position)
    {
        transform.position = position;
    }
    
    public void Enable()
    {
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
}
