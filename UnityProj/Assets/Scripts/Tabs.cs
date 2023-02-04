using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tabs : MonoBehaviour
{
    [SerializeField] private GameObject _tab;
    [SerializeField] private Transform _firstTabTransform;
    [SerializeField] private AdventurerTabs _adventurerTabs;

    private List<GameObject> _tabGameObjects = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < _adventurerTabs.GetMaxTabs(); i++)
        {

            GameObject tabGameObject = Instantiate(_tab,
                new Vector3(_firstTabTransform.position.x, _firstTabTransform.position.y + (-250 * i),
                    _firstTabTransform.position.z), Quaternion.identity);
            tabGameObject.transform.SetParent(this.transform);
            _tabGameObjects.Add(tabGameObject);
        }

        UpdateTabs();
    }

    private void UpdateTabs()
    {
        for (int i = 0; i < _adventurerTabs.GetMaxTabs(); i++)
        {
            Tab t = _tabGameObjects[i].GetComponent<Tab>();
            Character c = _adventurerTabs.GetTabInfo(i);
            t.NameTextMeshProUGUI.text = c.name;
            t.HealthTextMeshProUGUI.text = "HP "+ c.health.ToString();
            t.AttackTextMeshProUGUI.text = "ATK " + c.attack.ToString();
            t.AgilityTextMeshProUGUI.text = "AGL " + c.agility.ToString();

        }
    }
}
