using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CharacterHealthUI : MonoBehaviour
{
    [SerializeField] private Image _heartFullImage;
    [SerializeField] private Image _heartEmptyImage;

    [SerializeField] private CharacterObject _characterObject;
    
    [SerializeField] private Image[] _hearts;

    private void Start()
    {
        _characterObject.OnCharacterSet += SubscribeToEvent;
        _characterObject.OnCharacterRemoved += UnsubscribeToEvent;
        UpdateHeartAmount(_characterObject._character.health);
    }

    private void UpdateHeartAmount(int characterHealth)
    {
        for (int i = 0; i < _hearts.Length; i++)
        {
            if (i >= characterHealth)
            {
                _hearts[i].sprite = _heartEmptyImage.sprite;
            }
            else
            {
                _hearts[i].sprite = _heartFullImage.sprite;
            }
        }
    }

    private void SubscribeToEvent()
    {
        if (_characterObject._character == null)
        {
            return;
        }
        
        _characterObject._character.OnCharacterHealthEvent += UpdateHeartAmount;
    }

    private void UnsubscribeToEvent()
    {
        if (_characterObject._character == null)
        {
            return;
        }
        
        _characterObject._character.OnCharacterHealthEvent -= UpdateHeartAmount;
    }
    
}
