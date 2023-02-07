using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Burst;
using Unity.Mathematics;

public class Tab : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image _adventurerImage;
    [SerializeField] private Image _selectionBorder;
    [SerializeField] private TextMeshProUGUI _adventurerNameText;

    [Header("Unused References")]
    [SerializeField] private TextMeshProUGUI _adventurerHealthText;
    [SerializeField] private TextMeshProUGUI _adventurerAttackText;
    [SerializeField] private TextMeshProUGUI _adventurerAgilityText;
   
    [Header("Stat Image References")]
    [SerializeField] private Image[] _health;
    [SerializeField] private Image[] _attack;
    [SerializeField] private Image[] _agility;

    [Header("Selection Flash Settings")]
    [SerializeField] private float _flashDuration = 0.5f;

    public RawImage _adventurerRImage;
    [HideInInspector] public Character info;
    [HideInInspector] public bool onScreen = false; // HACK(Zack): used to be able to check if a tab is able to be used for moving a character off

    private delegate IEnumerator LerpDel(Color start, Color end, float duration);
    private LerpDel FlashSelectionFunc;
    private Coroutine _flashCoroutine;
    private Color _flashOnColour;
    private Color _flashOffColour;

    private void Awake()
    {
        // delegate allocation;
        FlashSelectionFunc = FlashSelection;

        // setup the colour lerping values
        _flashOnColour = _selectionBorder.color;
        _flashOffColour = _flashOnColour;
        _flashOffColour.a = 0f;
    }

    // TODO(Zack): move sprite info into an index for the "Character.cs"
    public void SetTabInfoAndStartPosition(float3 start, Character info, Sprite sprite) 
    {
        this.transform.position = start;
        SetTabInfo(info, sprite);
    }

    public void SetTabInfo(Character info, Sprite sprite) 
    { 
        _adventurerImage.sprite = sprite;
        _adventurerNameText.text = info.name;
            
        // setup the sprites for the notice board
        for (int i = 0; i < 3; ++i) {
            int num = i + 1;
            
            bool healthEnabled  = false;
            bool attackEnabled  = false;
            bool agilityEnabled = false;

            if (info.health  >= num) healthEnabled  = true;
            if (info.attack  >= num) attackEnabled  = true;
            if (info.agility >= num) agilityEnabled = true;

            _health[i].enabled  = healthEnabled;
            _attack[i].enabled  = attackEnabled;
            _agility[i].enabled = agilityEnabled;
        }

        this.info = info;

        _selectionBorder.color = _flashOffColour;
    }


    public void StartFlashing()
    {
        // NOTE(Zack): the border selection starts completely transparent so we need to lerp it to be visible to begin with
        _flashCoroutine = StartCoroutine(FlashSelectionFunc(_flashOffColour, _flashOnColour, _flashDuration));
    }

    public void StopFlashing()
    {
        if (_flashCoroutine == null) return;
        StopCoroutine(_flashCoroutine);
        _flashCoroutine = null;
    }

    private IEnumerator FlashSelection(Color start, Color end, float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            float t = timer / duration;
            t = EaseInOutExp(t);
            _selectionBorder.color = LerpColour(start, end, t);
            timer += Time.deltaTime;
            yield return null;
        }
        
        _selectionBorder.color = end;

        // do a simple swap to change the colours over
        Color temp = start;
        start = end;
        end = temp;

        // BUG(Zack): we're getting an inconsistant stack overflow here??
        _flashCoroutine = StartCoroutine(FlashSelectionFunc(start, end, duration));
        yield break;
    }

    // REVIEW(Zack): we probably don't need to lerp every component of the colours,
    // could get away with just the alpha value;
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

    [BurstCompile]
    public static float EaseInOutExp(float val) 
    {
        if (val <= 0f) return 0f;
        if (val >= 1f) return 1f;

	    if (val < 0.5f) 
        {
    		return 0.5f * Pow2((20f * val) - 10f);
        } 
        else
        {
            return -0.5f * Pow2((-20f * val) + 10f) + 1f;
        }
    }

    [BurstCompile]
    public static float Pow2(float power)
    {
        float exp = power * 8388608f; // 8388608 == (1 << 23)
        return math.asfloat((uint)exp + (127 << 23));
    }

    public TextMeshProUGUI NameTextMeshProUGUI
    {
        get
        {
            return _adventurerNameText;
        }
        set
        {
            _adventurerNameText = value;
        }
    }
    public TextMeshProUGUI HealthTextMeshProUGUI
    {
        get
        {
            return _adventurerHealthText;
        }
        set
        {
            _adventurerHealthText = value;
        }
    }
    public TextMeshProUGUI AttackTextMeshProUGUI
    {
        get
        {
            return _adventurerAttackText;
        }
        set
        {
            _adventurerAttackText = value;
        }
    }  
    public TextMeshProUGUI AgilityTextMeshProUGUI
    {
        get
        {
            return _adventurerAgilityText;
        }
        set
        {
            _adventurerAgilityText = value;
        }
    }
}
