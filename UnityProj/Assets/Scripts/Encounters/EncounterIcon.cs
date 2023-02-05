using UnityEngine;
using UnityEngine.UI;

public class EncounterIcon : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private ParticleSystem _particleSystem;

    public void Enable(Sprite sprite)
    {
        _image.sprite = sprite;
        gameObject.SetActive(true);
        
        if (_particleSystem != null)
        {
            _particleSystem.Play();
        }
    }

    public void Disable()
    {
        if (_particleSystem != null)
        {
            _particleSystem.Stop();
        }
        
        gameObject.SetActive(false);
    }
}
