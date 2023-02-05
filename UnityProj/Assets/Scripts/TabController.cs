using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class TabController : MonoBehaviour
{
    [SerializeField] private GameObject _tab;
    [SerializeField] private Transform _firstTabTransform;
    [SerializeField] private AdventurerTabs _adventurerTabs;
     private Vector3 _distanceBetweenTabs = new Vector3(-500,-250,0);
    
    [SerializeField] private float _animDuration;

    private Vector3 _startpos;
    private Vector3 _startAnimPos;
    private Vector3 _endAnimPos;
    private List<GameObject> _tabGameObjects = new List<GameObject>();
    public float addTabWaitTime = 5.0f;

    private float _addTabTime;
    private Coroutine _coroutine1;
    private int index1Corutine;
    private Coroutine _coroutine2;
    private Boolean tabCoroutineRunning = false;

    public delegate void TabTimerDelegate();

    public static event TabTimerDelegate AddTabEvent;


    [SerializeField] private GridRouteInput _gridRouteInput;

    [SerializeField] public bool readingInput = true;

    private int current_selection = 0;

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
            _startpos = tabGameObject.transform.localPosition;

            
        };

        UpdateTabs();
        AddTabEvent += AddTab;
        Input.Actions.Selection.Select.performed += SelectCharacter;
        Input.Actions.Selection.Up.performed += SelectHoverUp;
        Input.Actions.Selection.Down.performed += SelectHoverDown;
        
        // _gridRouteInput.StartRoute(_adventurerTabs.GetTabInfo(0));

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

    public void SelectCharacter(InputAction.CallbackContext context)
    {
        if (readingInput)
        { 
            readingInput = false;
           //_tabGameObjects[current_selection].gameObject.SetActive(false);
           TabRemoved();
            _gridRouteInput.StartRoute(_adventurerTabs.GetTabInfo(current_selection));
            _gridRouteInput._readingInput = true;
        }
        
    }
    public void SelectHoverUp(InputAction.CallbackContext context)
    {
        if (readingInput)
        {
            current_selection--;
            if (current_selection < 0)
            {
                current_selection = _adventurerTabs.GetMaxTabs() - 1;
            }
            print(current_selection);
        }

    }
    public void SelectHoverDown(InputAction.CallbackContext context)
    {
        if (readingInput)
        {
            current_selection++;
            if (current_selection > _adventurerTabs.GetMaxTabs() - 1)
            {
                current_selection = 0;
            }
            var current_tab = _tabGameObjects[current_selection].GetComponent<Tab>();
            if (current_tab != null)
            {
                current_tab._adventurerRImage.color = Color.red;
            }

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
                    index1Corutine = i;
                 StartCoroutine(TabAnimation(_startAnimPos, _endAnimPos, index1Corutine));
                 
                }
                else
                {
                  
                    _tabGameObjects[i].transform.localPosition =_startpos
                         +
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
    private IEnumerator TabAnimation(Vector3 startPos, Vector3 endPos, int tabIndex)
    {
        tabCoroutineRunning = true;
        float animationTimer = float.Epsilon;
       
        while (animationTimer <= _animDuration)
        {
            _tabGameObjects[tabIndex].transform.localPosition = Vector3.Lerp(startPos, endPos,
                animationTimer / _animDuration);
            animationTimer += Time.deltaTime;
            yield return null;
        }

        tabCoroutineRunning = false;
            yield break;
    }

    private IEnumerator RemovingTab()
    {
        while (tabCoroutineRunning)
        {
            yield return null;
        }
           GameObject removeCharacter = _tabGameObjects[current_selection];
                _adventurerTabs.OnNeedNewCharacter(current_selection);
                _tabGameObjects.RemoveAt(current_selection);
                _tabGameObjects.Add(removeCharacter);
                UpdateTabs();
                
                float animationTimer = float.Epsilon;
       
             for (int i = 0; i < _adventurerTabs.GetMaxTabs(); i++) 
             { 
                 if (i >= current_selection) 
                 {
                            Vector3 s = new Vector3(_tabGameObjects[i].transform.localPosition.x,_tabGameObjects[i].transform.localPosition.y,_tabGameObjects[i].transform.localPosition.z);
                            while (animationTimer <= _animDuration)
                            {
                                _tabGameObjects[i].transform.localPosition = Vector3.Lerp(s, s + new Vector3(0, 250, 0),
                                    animationTimer / _animDuration);
                                animationTimer += Time.deltaTime;
                                yield return null;

                 
                        }
                    }         
                }
                yield break;
    }


    private void TabRemoved()

    {
        _tabGameObjects[current_selection].gameObject.SetActive(false);
        StartCoroutine(RemovingTab());

    }

}
