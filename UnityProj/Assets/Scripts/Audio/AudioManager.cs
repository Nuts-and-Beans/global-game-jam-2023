using System;
using UnityEngine;

// yeah i know, a singleton. sue me.

public class AudioManager : MonoBehaviour
{
    [Header("Audio References")]
    [SerializeField] private AudioSource _oneShotAudioSource;
    [SerializeField] private AudioSource _bossOneShotAudioSource;
    
    private static AudioManager s_instance { get; set; }

    private void Awake()
    {
        if (s_instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        
        s_instance = this;
        DontDestroyOnLoad(this.gameObject);
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

    public static void PlayBossOneShot(AudioClip clip)
    {
        if (clip == null) return;
        s_instance._bossOneShotAudioSource.PlayOneShot(clip);
    }
}
