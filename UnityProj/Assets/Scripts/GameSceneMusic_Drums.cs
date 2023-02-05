using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneMusic_Drums : MonoBehaviour
{
    [SerializeField] private AudioSource drums;
    [SerializeField] private Boss boss;
    bool isPlaying = false;

    private void Update()
    { 
        if (!isPlaying)
        {
            if (Timer.s_currentTime <= 30)
            {
                Debug.Log("Playing drums loop");
                drums.Play();
                isPlaying = true;
            }
            else if (Boss.s_currentHealth < 3)
            {
                Debug.Log("Playing drums loop");
                drums.Play();
                isPlaying = true;
            }
        }
    }
}
