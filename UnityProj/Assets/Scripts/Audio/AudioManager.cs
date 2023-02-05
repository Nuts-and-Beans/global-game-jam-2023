using System;
using UnityEngine;

// yeah i know, a singleton. sue me.

public class AudioManager : MonoBehaviour
{
    [Header("Audio References")]
    [SerializeField] private AudioSource _oneShotAudioSource;
    
    private static AudioManager s_instance { get; set; }

    private void Awake()
    {
        if (s_instance != null)
        {
            Debug.LogError("Multiple Audio Managers are in the scene! Only one should be in the scene");
            Destroy(this.gameObject);
            return;
        }
        
        s_instance = this;
    }

    private void OnDestroy()
    {
        if (s_instance != this) return;
        s_instance = null;
    }

    public static void PlayOneShot(AudioClip clip)
    {
        if (clip == null) return;
        s_instance._oneShotAudioSource.PlayOneShot(clip);
    }
}
