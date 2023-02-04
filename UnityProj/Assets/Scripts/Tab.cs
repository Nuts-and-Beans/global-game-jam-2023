using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tab : MonoBehaviour
{
    [SerializeField] private RawImage _adventurerRImage;
    [SerializeField] private TextMeshProUGUI _adventurerNameText;
    [SerializeField] private TextMeshProUGUI _adventurerHealthText;
    [SerializeField] private TextMeshProUGUI _adventurerAttackText;
    [SerializeField] private TextMeshProUGUI _adventurerAgilityText;
   

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
    }  public TextMeshProUGUI AgilityTextMeshProUGUI
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
