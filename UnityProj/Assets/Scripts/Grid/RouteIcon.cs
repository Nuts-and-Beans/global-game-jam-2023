using System.Collections;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;

public class RouteIcon : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private Canvas _imageCanvas;
    [SerializeField] private float3 _startScale;
    [SerializeField] private float3 _endScale;
    [SerializeField] private float _duration;

    public void SetPosition(float3 position)
    {
        transform.position = position;
    }
    
    public void Enable()
    {
        gameObject.SetActive(true);
        StartCoroutine(DoLerp());
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
    
    private IEnumerator DoLerp()
    {
        yield return Lerp(_startScale, _endScale);
        yield return Lerp(_endScale, _startScale);
    }
    
    private IEnumerator Lerp(float3 start, float3 end)
    {
        float timeElapsed  = 0.0f;
        
        while (timeElapsed < _duration)
        {
            float t = Sinusoidal(timeElapsed / _duration);
            _imageCanvas.transform.localScale = Vector3.Lerp(start, end, t);
            
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    
        _imageCanvas.transform.localScale = end;
    }
    
    [BurstCompile, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Sinusoidal(float val) => math.sin(val * (math.PI * 0.5f));
}
