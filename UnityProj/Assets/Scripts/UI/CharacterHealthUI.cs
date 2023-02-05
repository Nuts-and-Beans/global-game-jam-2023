using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CharacterHealthUI : MonoBehaviour
{
    [SerializeField] private GameObject _characterGameObject;
    
    [SerializeField] private Image _heartFullImage;
    [SerializeField] private Image _heartEmptyImage;

    private CharacterObject _characterObject;
    
    [SerializeField] private Image[] _hearts;

    private void Awake()
    {
        //NOTE(Sebadam2010): This is here for testing mainly.
        //Ideally, would have the script responsible for putting the character on the tab into the map do the setting of the character.
        setCharacter(_characterGameObject);
    }

    private void Start()
    {
        _characterObject._character.OnCharacterHealthEvent += UpdateHeartAmount;
        UpdateHeartAmount(_characterObject._character.health);
    }

    private void OnDestroy()
    {
        _characterObject._character.OnCharacterHealthEvent -= UpdateHeartAmount;
    }
    
    public void setCharacter(GameObject character)
    {
        _characterGameObject = character;
        _characterObject = _characterGameObject.GetComponent<CharacterObject>();
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
    
}
