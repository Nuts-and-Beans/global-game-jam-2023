using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tab : MonoBehaviour
{
    [SerializeField] public RawImage _adventurerRImage;
    [SerializeField] public TextMeshProUGUI _adventurerNameText;
    [SerializeField] public TextMeshProUGUI _adventurerHealthText;
    [SerializeField] public TextMeshProUGUI _adventurerAttackText;
    [SerializeField] public TextMeshProUGUI _adventurerAgilityText;
   

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
