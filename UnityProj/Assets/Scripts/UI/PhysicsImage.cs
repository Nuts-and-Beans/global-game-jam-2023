using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class PhysicsImage : MonoBehaviour
{
    public Image img;
    public Rigidbody2D rb;
    public RectTransform trns => img.rectTransform;
}
