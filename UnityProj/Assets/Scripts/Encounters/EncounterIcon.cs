﻿using System;
using UnityEngine;
using UnityEngine.UI;

public class EncounterIcon : MonoBehaviour
{
    [SerializeField] private Image _image;
    
    [SerializeField] private ParticleSystem _puzzlePS;
    [SerializeField] private ParticleSystem _trapPS;
    [SerializeField] private ParticleSystem _creaturePS;
    [SerializeField] private ParticleSystem _equipmentPS;
    [SerializeField] private ParticleSystem _bossPS;
    
    private ParticleSystem _currentPS = null;

    public void EnableBossIcon()
    {
        gameObject.SetActive(true);
        _bossPS.Play();
    }

    public void Enable(Sprite sprite, EncounterType type)
    {
        _image.sprite = sprite;
        gameObject.SetActive(true);
        
        _currentPS = type switch
        {
            EncounterType.CREATURE  => _creaturePS,
            EncounterType.PUZZLE    => _puzzlePS,
            EncounterType.TRAP      => _trapPS,
            EncounterType.EQUIPMENT => _equipmentPS,
            
            EncounterType.COUNT => throw new ArgumentOutOfRangeException(nameof(type), type, null),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
        
        _currentPS.Play();
    }

    public void Disable()
    {
        if (_currentPS != null)
        {
            _currentPS.Stop();
            _currentPS = null;
        }
        
        gameObject.SetActive(false);
    }

    public void PlayParticleSystem()
    {
        if (_currentPS == null) return;
        
        _currentPS.Play();
    }
}
