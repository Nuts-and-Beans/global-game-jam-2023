using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private bool _isTimerRunning = true;
    
    //NOTE(Sebadam2010): Starting the timer from 3 minutes.
    public static float s_currentTime = 180.0f;
    
    public delegate void TimerDel();
    public static event TimerDel OnTimerEndEvent;
    
    private void Update()
    {
        if (_isTimerRunning)
        {
            s_currentTime -= Time.deltaTime;
            if (s_currentTime <= 0)
            {
                s_currentTime = 0;
                _isTimerRunning = false;
                OnTimerEndEvent?.Invoke();
            }
        }
    }
}
