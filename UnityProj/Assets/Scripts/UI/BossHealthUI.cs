using System.Collections;
using UnityEngine;
using Unity.Mathematics;
using Random = UnityEngine.Random;

public class BossHealthUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform _healthParent;
    [SerializeField] private PhysicsImage _healthSectionPrefab;

    [Header("Animation Settings")]
    [SerializeField] private float _lerpDuration = 1f;
    [SerializeField] private float2 _torqueRange = new (-50f, 50f);
    [SerializeField] private float2 _forceImpulseRange = new (20f, 60f);

    // TODO(Zack): remove when [BossHealth] interaction stuff is implemented
#if UNITY_EDITOR
    [Header("TESTING - Health Settings")]
    [SerializeField] private int _health = 10;
    [SerializeField] private bool remove = false;
#endif // UNITY_EDITOR

    private PhysicsImage[] _barSections;
    private PhysicsImage _currentBarSection => _barSections[_currentHealthBarIndex];
    private int _currentHealthBarIndex; // NOTE(Zack): this is used to keep track of current index

    private delegate IEnumerator LerpDel(PhysicsImage img, float duration);
    private LerpDel LerpTransparentFunc;

    private void Start()
    {
        // NOTE(Zack): pre allocating delegate
        LerpTransparentFunc = LerpTransparent;
        CreateHealthBar();

        // TODO(Zack): subscribe to actual BossHealthLost event
        // BossHealth.OnBossHealthLostEvent += RemoveHealth;
    }

#if UNITY_EDITOR
    private void Update() 
    {
       if (!remove) return;
       remove = !remove;

       RemoveHealth();
    }
#endif // UNITY_EDITOR



    private void CreateHealthBar() 
    {
        // TODO(Zack): get the health from a [BossHealth] or similar script
        _barSections = new PhysicsImage[_health];
        for (int i = 0; i < _health; ++i) {
            PhysicsImage img = Instantiate(_healthSectionPrefab, _healthParent);
            img.rb.simulated = false;
            _barSections[i] = img;
        }

        _currentHealthBarIndex = _health - 1;
    }

    // REVIEW(Zack): should we just pass in the current health of the player
    private void RemoveHealth()
    {
        if (_currentHealthBarIndex < 0) return; 
        PhysicsImage img = _currentBarSection;
        img.rb.simulated = true;

        _currentHealthBarIndex -= 1;

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

    private static Color LerpColour(Color start, Color end, float t)
    {
        Color c;
        c.r = math.lerp(start.r, end.r, t);
        c.g = math.lerp(start.g, end.g, t);
        c.b = math.lerp(start.b, end.b, t);
        c.a = math.lerp(start.a, end.a, t);
        return c;
    }

    private static float2 RandomVectorRange(float2 min, float2 max) {
        float x = Random.Range(min.x, max.x);
        float y = Random.Range(min.y, max.y);

        return new (x, y);
    }
}
