using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Mathematics;
using Random = UnityEngine.Random;

public class NoticeBoard : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private GridRouteInput _gridRoute;

    [Header("Prefab References")]
    [SerializeField] private Tab[] _tabs;
    [SerializeField] private RectTransform[] _onScreenSpawnPositions;
    [SerializeField] private RectTransform[] _offScreenSpawnPositions;

    [Header("Animation Settings")]
    [SerializeField] private float _lerpOnScreenDuration = 1f;
    [SerializeField] private float _lerpFinishedDuration = 0.5f;

    [Header("Notice Board Wait Settings")]
    [SerializeField] private float _waitBetweenNextTab = 1f;

    [Header("Sprites")]
    [SerializeField] private Sprite[] _barbarianSprites;
    [SerializeField] private Sprite[] _archerSprites;
    [SerializeField] private Sprite[] _assassinSprites; 



    private int _currentTabToUseForSelection = 0;
    private int _currentTabToMove = 0;
    private int _maxTabCount => _tabs.Length; // assuming that we have one tab always free
    private int _currentTabCountInBoard = 0;

    
    private void Awake()
    {
        Debug.Assert(_gridRoute != null, "Grid Route Input is null, set in the inspector", this);

        Input.Actions.Selection.Select.performed += SelectCharacter;
    }

    private void OnDestroy()
    {
        Input.Actions.Selection.Select.performed -= SelectCharacter;
    }


    private void Start()
    {
        for (int i = 0; i < _tabs.Length; ++i)
        {
            float3 startPosition = _offScreenSpawnPositions[i].position;
            Character info = Adventurers.GetNextCharacter();

            Sprite[] sprites = info.type switch
            {
                CharacterType.BARBARIAN => _barbarianSprites,
                CharacterType.ARCHER    => _archerSprites,
                CharacterType.ASSASSIN  => _assassinSprites,
                _ => throw new ArgumentOutOfRangeException()
            };

            
            int index = Random.Range(0, sprites.Length);
            Sprite sprite = sprites[index];

            _tabs[i].SetTabInfoAndStartPosition(startPosition, info, sprite);
        }


        StartCoroutine(WaitAndTrySendNewTab(0.5f)); // hardcode a smaller wait time on the first tab
    }


    private IEnumerator WaitAndTrySendNewTab(float delay)
    {
        float timer = 0f;
        while (timer < delay)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // if we can send a new tab we lerp it on screen into the correct position
        if (_currentTabCountInBoard < _maxTabCount) 
        {
            // if we we lerp it onto the screen
            int indexForPositions = WrapIndex(_currentTabToMove, _tabs.Length - 1);

            float3 startPosition = _offScreenSpawnPositions[_currentTabToMove].position;
            float3 endPosition = _onScreenSpawnPositions[_currentTabToMove].position;

            Character info = Adventurers.GetNextCharacter();
            Sprite[] sprites = info.type switch
            {
                CharacterType.BARBARIAN => _barbarianSprites,
                CharacterType.ARCHER    => _archerSprites,
                CharacterType.ASSASSIN  => _assassinSprites,
                _ => throw new ArgumentOutOfRangeException()
            };

            
            int index = Random.Range(0, sprites.Length);
            Sprite sprite = sprites[index];
            Tab freeTab = _tabs[_currentTabToMove];

            freeTab.SetTabInfo(Adventurers.GetNextCharacter(), sprite);

            // we wrap the current tab index
            _currentTabToMove += 1;
            _currentTabToMove  = WrapIndex(_currentTabToMove, _tabs.Length);

            StartCoroutine(MoveTabOnScreen(freeTab, startPosition, endPosition, _lerpOnScreenDuration));

            _currentTabCountInBoard += 1;
        }

        StartCoroutine(WaitAndTrySendNewTab(_waitBetweenNextTab));
        yield break;
    }
    

    private IEnumerator MoveTabOnScreen(Tab tab, float3 startPosition, float3 endPosition, float duration)
    {
        float timer = 0f;
        tab.transform.position = startPosition;
        while (timer < duration)
        {
            float t = timer / duration;
            tab.transform.position = Vector3.Lerp(startPosition, endPosition, t);
            timer += Time.deltaTime;
            yield return null;
        }
        
        tab.transform.position = endPosition;
        tab.onScreen = true;
        yield break;
    }

    private IEnumerator RemoveTabOffScreen(Tab tab, float3 startPosition, float3 endPosition, float duration)
    {
        tab.onScreen = false;

        float timer = 0f;
        tab.transform.position = startPosition;
        while(timer < duration)
        {
            float t = timer/ duration;
            tab.transform.position = Vector3.Lerp(startPosition, endPosition, t);
            timer += Time.deltaTime;
            yield return null;
        }

        tab.transform.position = endPosition;
        yield break;
    }

    public void SelectCharacter(InputAction.CallbackContext context)
    {
        if (!_tabs[_currentTabToUseForSelection].onScreen || _gridRoute._readingInput) return;

        Tab tab = _tabs[_currentTabToUseForSelection];
        _gridRoute.StartRoute(tab.info);
        _gridRoute._readingInput = true;

        float3 startPos = tab.transform.position;
        float3 endPos   = startPos;
        endPos.x -= 2000; // HACK(Zack): hardcoding this for now

        StartCoroutine(RemoveTabOffScreen(tab, startPos, endPos, _lerpFinishedDuration));

        _currentTabToUseForSelection += 1;
        _currentTabToUseForSelection = WrapIndex(_currentTabToUseForSelection, _tabs.Length);
        
        _currentTabCountInBoard -= 1;
    }

    private static int WrapIndex(int index, int size) => (index + size) % size;
}
