using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ChromaticAberrationIntensity : MonoBehaviour
{
    [SerializeField] private VolumeProfile _volumeProfile;
    [SerializeField] private int _chromaticAberrationIndex;
    [Space]
    [SerializeField] private float _startIntensity;
    [SerializeField] private float _endIntensity;
    [SerializeField] private float _duration;
    
    private ChromaticAberration _chromaticAberration => _volumeProfile.components[_chromaticAberrationIndex] as ChromaticAberration;

    private IEnumerator Start()
    {
        while (true)
        {
            yield return Lerp(_startIntensity, _endIntensity);
            yield return Lerp(_endIntensity, _startIntensity);
        }
    }
    
    private IEnumerator Lerp(float start, float end)
    {
        float timeElapsed  = 0.0f;

        while (timeElapsed < _duration)
        {
            float t = Quartic(timeElapsed / _duration);
            _chromaticAberration.intensity.value = math.lerp(start, end, t);
            
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    
        _chromaticAberration.intensity.value = end;
    }
    
    [BurstCompile, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Sinusoidal(float val) => math.sin(val * (math.PI * 0.5f));
    
    [BurstCompile, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Quartic(float val) {
        if ((val *= 2f) < 1f) return 0.5f * val * val * val * val;
        return -0.5f * ((val -= 2f) * val * val * val - 2f);
    }


}