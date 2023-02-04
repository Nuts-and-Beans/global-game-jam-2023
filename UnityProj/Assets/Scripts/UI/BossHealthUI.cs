using System.Collections;
using UnityEngine;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine.UI;
using Random = UnityEngine.Random;

/// <summary>
/// This script is responsible for animating the Bosses health bar UI
/// <summary>
public class BossHealthUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform _healthParent;
    [SerializeField] private HorizontalLayoutGroup _layoutGroup;
    [SerializeField] private PhysicsImage _healthSectionPrefab;

    [Header("Animation Settings")]
    [SerializeField] private float _lerpDuration = 1f;
    [SerializeField] private float2 _torqueRange = new (-50f, 50f);
    [SerializeField] private float2 _forceImpulseRange = new (20f, 60f);

    private delegate IEnumerator LerpDel(PhysicsImage img, float duration);
    private LerpDel LerpTransparentFunc;

    private PhysicsImage[] _barSections;

    private void Start()
    {
        // NOTE(Zack): pre allocating delegate
        LerpTransparentFunc = LerpTransparent;
        CreateHealthBar();

        Boss.OnHealthLostEvent += RemoveHealth;
    }

    private void OnDestroy()
    {
        Boss.OnHealthLostEvent -= RemoveHealth;
    }

    private void CreateHealthBar() 
    {
        int count = Boss.s_maxHealth;
        _barSections = new PhysicsImage[count];

        for (int i = 0; i < count; ++i) {
            PhysicsImage img = Instantiate(_healthSectionPrefab, _healthParent);
            img.rb.simulated = false;
            img.rb.gravityScale = 0f;
            _barSections[i] = img;
        }

        Canvas.ForceUpdateCanvases();

        _layoutGroup.enabled = false;
    }

    private void RemoveHealth(int currentHealth)
    {
        PhysicsImage img = _barSections[currentHealth - 1];
        img.rb.simulated = true;

        // add force downward to the ui entity
        float2 dir = Vector2.down;
        float force = Random.Range(_forceImpulseRange.x, _forceImpulseRange.y);
        img.rb.AddForce(dir * force, ForceMode2D.Impulse);

        // add some directional spin to the ui entity
        float angle = math.radians(Random.Range(_torqueRange.x, _torqueRange.y));
        img.rb.AddTorque(angle * img.rb.inertia, ForceMode2D.Impulse);

        StartCoroutine(LerpTransparent(img, _lerpDuration));
    }

    private IEnumerator LerpTransparent(PhysicsImage img, float duration)
    {
        Color startColour = img.img.color;
        Color endColour   = startColour;
        endColour.a = 0f;

        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            
            img.img.color = LerpColour(startColour, endColour, t);
            yield return null;
        }

        img.img.color = endColour;

        img.rb.simulated = false;
        img.img.enabled = false;
        yield break;
    }

    [BurstCompile]
    private static Color LerpColour(Color start, Color end, float t)
    {
        Color c;
        c.r = math.lerp(start.r, end.r, t);
        c.g = math.lerp(start.g, end.g, t);
        c.b = math.lerp(start.b, end.b, t);
        c.a = math.lerp(start.a, end.a, t);
        return c;
    }

    private static float2 RandomVectorRange(float2 min, float2 max)
    {
        float x = Random.Range(min.x, max.x);
        float y = Random.Range(min.y, max.y);
        return new (x, y);
    }
}
