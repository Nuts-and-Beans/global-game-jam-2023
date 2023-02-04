using UnityEngine;

/// <summary>
/// This script is the interface in which all Boss event interactions should be routed through
/// <summary>
public class Boss : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int _maxHealth = 10;

#if UNITY_EDITOR
    [Header("DEBUG Testing - WILL NOT BE COMPILED INTO BUILD")]
    [SerializeField] private bool _removeHealth = false;
#endif // UNITY_EDITOR

    public delegate void HealthLostEventDel(int currentHealth);
    public delegate void EventDel();
    public static HealthLostEventDel OnHealthLostEvent;
    public static EventDel OnBossDefeatedEvent;

    public  static int  s_maxHealth;
    public  static int  s_currentHealth;
    private static bool s_dead = false; // HACK(Zack): being lazy and just using a boolean

    private void Awake() 
    {
        s_maxHealth     = _maxHealth;
        s_currentHealth = s_maxHealth;
        s_dead = false;
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (!_removeHealth) return;
        _removeHealth = !_removeHealth;
        RemoveHealth();
    }
#endif // UNITY_EDITOR

    public static void RemoveHealth() 
    {
        if (s_dead) return;

        OnHealthLostEvent?.Invoke(s_currentHealth);
        s_currentHealth -= 1;

        if (s_currentHealth > 0) return;
        s_dead = true;
        OnBossDefeatedEvent?.Invoke();
    }
}
