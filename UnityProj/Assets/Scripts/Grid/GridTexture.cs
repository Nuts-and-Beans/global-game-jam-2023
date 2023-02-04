using UnityEngine;
using UnityEngine.UI;

public class GridTexture : MonoBehaviour
{
    [SerializeField] private Grid _grid;
    [SerializeField] private RawImage _rawImage;
    
    private void Start()
    {
        _rawImage.texture = _grid.CameraRT;
    }
}