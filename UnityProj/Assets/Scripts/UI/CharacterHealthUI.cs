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

    [SerializeField] private float _heartWidth = 35f;
    [SerializeField] private float _heartHeight = 35f;

    [SerializeField] private float _heartParentYOffset = .75f;
    [SerializeField] private float _heartParentXOffset = .3f;
    
    
    [SerializeField] private float _heartSpaceXOffset = 35f;
    
    private Transform _heartStartingPosition;

    private Transform _characterTransform;
    private CharacterObject _characterObject;
    
    private Image[] _hearts;

    private GameObject _heartsParent;
    
    private float _heartAlpha = 1f;

    private void Awake()
    {
        //NOTE(Sebadam2010): This is here for testing mainly.
        //Ideally, would have the script responsible for putting the character on the tab into the map.
        setCharacter(_characterGameObject);
    }

    private void Start()
    {
        _hearts = new Image[_characterObject._character.maxHealth];

        _characterObject._character.OnCharacterHealthEvent += UpdateHeartAmount;

        _heartsParent = new GameObject("HeartParent");
        
        _heartsParent.transform.position = new Vector3(_characterTransform.localPosition.x, _characterTransform.localPosition.y + _heartParentYOffset, _characterTransform.localPosition.z);
        
        _heartsParent.transform.SetParent(transform, false);
        
        Vector3 heartLocalPos = _heartsParent.transform.localPosition;

        //Note(Seb): Creating a heart image pool
        for (int i = 0; i < _hearts.Length; i++)
        {
            Debug.Log("Creating heart img" + i);

            Image newHeart = Instantiate(_heartEmptyImage,
                new Vector3(heartLocalPos.x + i * _heartSpaceXOffset , heartLocalPos.y + _heartParentYOffset, heartLocalPos.z),
                Quaternion.identity);
            newHeart.transform.SetParent(_heartsParent.transform, false);
            
            newHeart.rectTransform.sizeDelta = new Vector2(_heartWidth, _heartHeight);
            
            newHeart.enabled = true;

            Color tempColor = newHeart.color;
            tempColor.a = _heartAlpha;
            newHeart.color = tempColor;

            _hearts[i] = newHeart;
        }
        
        UpdateHeartAmount(_characterObject._character.health);
    }

    private void OnDestroy()
    {
        _characterObject._character.OnCharacterHealthEvent -= UpdateHeartAmount;
    }

    private void Update()
    {
        //REVIEW(Sebadam2010): Is there less expensive way of doing this?
        _heartsParent.transform.position = Camera.main.WorldToScreenPoint(new Vector3(_characterTransform.localPosition.x + _heartParentXOffset, _characterTransform.localPosition.y + _heartParentYOffset, _characterTransform.localPosition.z));
    }
    
    public void setCharacter(GameObject character)
    {
        _characterGameObject = character;
        _characterTransform = _characterGameObject.transform;
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
