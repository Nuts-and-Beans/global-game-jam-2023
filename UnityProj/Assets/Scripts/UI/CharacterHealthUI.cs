using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterHealthUI : MonoBehaviour
{
    [SerializeField] private GameObject _characterGameObject;
    
    [SerializeField] private Image _heartFullImage;
    [SerializeField] private Image _heartEmptyImage;

    
    private Transform _heartStartingPosition;
    private float _heartYOffset = 0.5f;

    private Transform _characterTransform;
    private Character _character;
    
    private Image[] _hearts; 
    
    private float _heartAlpha = 1f;

    private void Awake()
    {
        _characterTransform = _characterGameObject.transform;
        _character = _characterGameObject.GetComponent<Character>();

        _heartStartingPosition = _characterTransform.transform;
    }

    private void Start()
    {
        _hearts = new Image[_character.maxHealth];

        _character.OnCharacterHealthEvent += UpdateHeartAmount;

        Vector3 heartLocalPos = _heartStartingPosition.localPosition;

        //Note(Seb): Creating a heart image pool
        for (int i = 0; i < _hearts.Length; i++)
        {
            Debug.Log("Creating heart img" + i);

            Image newHeart = Instantiate(_heartEmptyImage,
                new Vector3(heartLocalPos.x, heartLocalPos.y + _heartYOffset, heartLocalPos.z),
                Quaternion.identity);
            newHeart.transform.SetParent(transform, false);
            newHeart.enabled = true;

            Color tempColor = newHeart.color;
            tempColor.a = _heartAlpha;
            newHeart.color = tempColor;

            _hearts[i] = newHeart;
        }
        
        UpdateHeartAmount(_character.health);
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
