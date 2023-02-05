using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _timerText; 
    [SerializeField] private float _scaleStartTime = 10.0f;
    [SerializeField] private float _timerScaleDuration = 0.5f;
    [SerializeField] private float3 _endScale = new float3(2.0f, 2.0f, 2.0f);

    private StringBuilder _timeStringBuilder = new StringBuilder(100);
    
    private Coroutine _timerScaleCoroutine = null;

    private void Update()
    {
        UpdateTimerText();
    }

    private void UpdateTimerText()
    {
        float currentGameTime = Timer.s_currentTime;
        
        _timeStringBuilder.Clear();
        
        int minutes = (int)math.floor(currentGameTime / 60f);
        int seconds = (int)currentGameTime - (60 * minutes);
        
        
        _timeStringBuilder.Append(minutes.ToString("00"));
        _timeStringBuilder.Append(":");
        _timeStringBuilder.Append(seconds.ToString("00"));
        
        _timerText.text = _timeStringBuilder.ToString();

        if (currentGameTime <= _scaleStartTime &&
            _timerScaleCoroutine == null)
        {
            _timerScaleCoroutine = StartCoroutine(TimerScale());
        }
    }

    private IEnumerator TimerScale()
    {
        float3 initialScale = _timerText.transform.localScale;
        while (true)
        {
            yield return LerpScale(initialScale, _endScale);
            yield return LerpScale(_endScale, initialScale);
        }
        
        yield break;
    }

    private IEnumerator LerpScale(float3 start, float3 end)
    {
        float timeElapsed = 0.0f;
        while (timeElapsed < _timerScaleDuration)
        {
            float t = timeElapsed / _timerScaleDuration;
            _timerText.transform.localScale = Vector3.Lerp(start, end, t);
            
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }
}
