using UnityEngine;
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

        // NOTE(Zack): this updating of the character health relies on [_characterObject] getting a new character from [Adventurers] in it's [Awake()] function.
        // else we just get a garbage value
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
        UpdateHeartAmount(_characterObject._character.health);
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
