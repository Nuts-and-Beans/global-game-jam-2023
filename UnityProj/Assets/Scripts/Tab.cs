using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Mathematics;

public class Tab : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image _adventurerImage;
    [SerializeField] private TextMeshProUGUI _adventurerNameText;
    [SerializeField] private TextMeshProUGUI _adventurerHealthText;
    [SerializeField] private TextMeshProUGUI _adventurerAttackText;
    [SerializeField] private TextMeshProUGUI _adventurerAgilityText;
   
    [Header("Stat Image References")]
    [SerializeField] private Image[] _health;
    [SerializeField] private Image[] _attack;
    [SerializeField] private Image[] _agility;

    public RawImage _adventurerRImage;

    [HideInInspector] public Character info;
    [HideInInspector] public bool onScreen = false; // HACK(Zack): used to be able to check if a tab is able to be used for moving a character off

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
