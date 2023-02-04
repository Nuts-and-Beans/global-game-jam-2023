using UnityEngine;
using UnityEngine.UI;

public class GridTexture : MonoBehaviour
{
    [SerializeField] private RawImage _rawImage;
    
    private void Start()
    {
        _rawImage.texture = Grid.s_CameraRT;
    }
}