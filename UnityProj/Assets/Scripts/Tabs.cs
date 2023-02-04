using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst;
using Unity.VisualScripting;
using UnityEngine;

public class Tabs : MonoBehaviour
{
    [SerializeField] private GameObject _tab;
    [SerializeField] private Transform _firstTabTransform;
    [SerializeField] private AdventurerTabs _adventurerTabs;
    [SerializeField] private Vector3 _distanceBetweenTabs;
    
    [SerializeField] private float _animDuration;

    private float _animationTimer;
    private Vector3 _startAnimPos;
    private Vector3 _endAnimPos;
    private List<GameObject> _tabGameObjects = new List<GameObject>();
    public float addTabWaitTime = 5.0f;

    private float _addTabTime;


    public delegate void TabTimerDelegate();

    public static event TabTimerDelegate AddTabEvent;

    // Start is called before the first frame update
    void Start()
    {
        _addTabTime = addTabWaitTime;
        for (int i = 0; i < _adventurerTabs.GetMaxTabs(); i++)
        {

            GameObject tabGameObject = Instantiate(_tab, _firstTabTransform.position - _distanceBetweenTabs,
                Quaternion.identity);
            tabGameObject.transform.localPosition += new Vector3(-500, 0, 0);
            tabGameObject.transform.SetParent(this.transform);
            tabGameObject.SetActive(false);
            _tabGameObjects.Add(tabGameObject);
        }

        UpdateTabs();
        AddTabEvent += AddTab;


    }

    private void OnDestroy()
    {
        AddTabEvent -= AddTab;
    }

    private void Update()
    {
        _addTabTime -= Time.deltaTime;
        if (_addTabTime <= 0)
        {
            _addTabTime = addTabWaitTime;
            AddTabEvent?.Invoke();
        }
    }

    private void AddTab()
    {
        for (int i = 0; i < _adventurerTabs.GetMaxTabs(); i++)
        {
            if (!_tabGameObjects[i].activeSelf)
            {
                _tabGameObjects[i].SetActive(true);
                if (i != 0)
                {
                    _tabGameObjects[i].transform.localPosition =
                        _tabGameObjects[i - 1].transform.localPosition + _distanceBetweenTabs  ;
                    _startAnimPos = _tabGameObjects[i].transform.localPosition;
                    _endAnimPos = new Vector3(_firstTabTransform.localPosition.x,
                        _tabGameObjects[i].transform.localPosition.y,
                        0);
                    StartCoroutine(TabAnimation(_startAnimPos, _endAnimPos, i));

                }
                else
                {
                    _tabGameObjects[i].transform.localPosition =
                        _tabGameObjects[_adventurerTabs.GetMaxTabs() - 1].transform.localPosition +
                        _distanceBetweenTabs;
                    _startAnimPos = _tabGameObjects[i].transform.localPosition;
                    _endAnimPos = new Vector3(_firstTabTransform.localPosition.x,
                        _tabGameObjects[i].transform.localPosition.y,
                        0);
                    StartCoroutine(TabAnimation(_startAnimPos, _endAnimPos, i));


                }

                break;
            }
        }
    }

    private void UpdateTabs()
    {
        for (int i = 0; i < _adventurerTabs.GetMaxTabs(); i++)
        {
            Tab t = _tabGameObjects[i].GetComponent<Tab>();
            Character c = _adventurerTabs.GetTabInfo(i);
            t.NameTextMeshProUGUI.text = c.name;
            t.HealthTextMeshProUGUI.text = "HP " + c.health.ToString();
            t.AttackTextMeshProUGUI.text = "ATK " + c.attack.ToString();
            t.AgilityTextMeshProUGUI.text = "AGL " + c.agility.ToString();

        }
    }


    [BurstCompile]
    private IEnumerator TabAnimation(Vector3 start_pos, Vector3 end_pos, int tab_index)
    {
        _animationTimer = float.Epsilon;
       
        while (_animationTimer <= _animDuration)
        {
            _tabGameObjects[tab_index].transform.localPosition = Vector3.Lerp(start_pos, end_pos,
                _animationTimer / _animDuration);
            _animationTimer += Time.deltaTime;
            yield return null;
        }

      
        yield break;
    }

}