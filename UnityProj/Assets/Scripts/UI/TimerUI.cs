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

    private StringBuilder _timeStringBuilder = new StringBuilder(100);

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
    }
}
