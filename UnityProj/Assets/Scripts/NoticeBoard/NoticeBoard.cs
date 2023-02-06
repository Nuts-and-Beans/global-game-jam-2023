using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Burst;
using Unity.Mathematics;
using Random = UnityEngine.Random;

/// <summary>
/// This script combines the functionality from <see cref="TabController"/> and <see cref="AdventurerTabs"/>
/// </summary>
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
    [SerializeField] private float _initialWaitTime = 0.5f;
    [SerializeField] private float _waitBetweenNextTab = 1f;

    [Header("Sprites")] // TODO(Zack): remove the use of these arrays from here, and make a global one, so that we are able to actually have the same sprites in the [Grid]
    [SerializeField] private Sprite[] _barbarianSprites;
    [SerializeField] private Sprite[] _archerSprites;
    [SerializeField] private Sprite[] _assassinSprites; 


    private int _currentTabIndexToUseForSelection = 0;
    private int _currentTabIndexToMove = 0;
    private int _currentTabCountInBoard = 0;
    private int _MaxTabCount => _tabs.Length; 

    // function pointer delegates
    private delegate IEnumerator LerpDel(Tab tab, float3 start, float3 end, float duration);
    private delegate IEnumerator TrySendDel(float delay);
    private TrySendDel WaitAndTrySendNewTabFunc;
    private LerpDel MoveTabOnScreenFunc;
    private LerpDel RemoveTabOffScreenFunc;
    

    private void Awake()
    {
        Debug.Assert(_gridRoute != null, "Grid Route Input is null, set in the inspector", this);

        Input.Actions.Selection.Select.performed += SelectCharacter;

        // delegate allocations
        WaitAndTrySendNewTabFunc = WaitAndTrySendNewTab;
        MoveTabOnScreenFunc      = MoveTabOnScreen;
        RemoveTabOffScreenFunc   = RemoveTabOffScreen;
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
            _tabs[i].transform.position = startPosition;
        }


        // NOTE(Zack): we begin the infinite Coroutine to check if we're able to send a new tab onto the screen, at set intervals
        // This could easily be done in the [Update()] loop, but to simplify some of the code, this method has been chosen instead
        StartCoroutine(WaitAndTrySendNewTabFunc(_initialWaitTime)); 
    }

    [BurstCompile]
    private IEnumerator WaitAndTrySendNewTab(float delay)
    {
        // we wait before we check if we can send a new tab, so that we allow animations etc to finish
        float timer = 0f;
        while (timer < delay)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // if we can send a new tab we lerp it on screen into the correct position
        if (_currentTabCountInBoard < _MaxTabCount) 
        {
            _currentTabCountInBoard += 1;

            Character info = Adventurers.GetNextCharacter();
            // TODO(Zack): remove this setting of the sprite for a character and make a global version to be use in [Grid] as well
            Sprite[] sprites = info.type switch
            {
                CharacterType.BARBARIAN => _barbarianSprites,
                CharacterType.ARCHER    => _archerSprites,
                CharacterType.ASSASSIN  => _assassinSprites,
                _ => null
            };
            
            Debug.Assert(sprites != null, "Sprites array is false, unknown character type chosen", this);
            
            // set the new tabs information, and then move it onto the screen
            int index = Random.Range(0, sprites.Length);
            Sprite sprite = sprites[index];
            Tab freeTab = _tabs[_currentTabIndexToMove];
            freeTab.SetTabInfo(info, sprite);

            float3 startPosition = _offScreenSpawnPositions[_currentTabIndexToMove].position;
            float3 endPosition = _onScreenSpawnPositions[_currentTabIndexToMove].position;

            StartCoroutine(MoveTabOnScreenFunc(freeTab, startPosition, endPosition, _lerpOnScreenDuration));

            // we wrap the current tab index
            _currentTabIndexToMove += 1;
            _currentTabIndexToMove  = WrapIndex(_currentTabIndexToMove, _tabs.Length);
        }


        // we 'recurse' and call the same function again to begin the waiting and check again.
        // (this isn't a real recursion as we are not using the same Stack frame because of how Coroutines work)
        StartCoroutine(WaitAndTrySendNewTabFunc(_waitBetweenNextTab));
        yield break;
    }
    
    [BurstCompile]
    private IEnumerator MoveTabOnScreen(Tab tab, float3 startPos, float3 endPos, float duration)
    {
        float timer = 0f;
        tab.transform.position = startPos;
        while (timer < duration)
        {
            float t = timer / duration;
            tab.transform.position = math.lerp(startPos, endPos, t);
            timer += Time.deltaTime;
            yield return null;
        }
        
        tab.transform.position = endPos;

        // NOTE(Zack): after the animation has finished this bool is set so that we are able,
        // to choose the character info from this tab for the character in the [Grid]
        tab.onScreen = true;
        yield break;
    }

    [BurstCompile]
    private IEnumerator RemoveTabOffScreen(Tab tab, float3 startPos, float3 endPos, float duration)
    {
        // NOTE(Zack): this is set so that when an input is pressed we don't accidentally choose this tab
        // whilst it is animating off of the screen
        tab.onScreen = false;

        float timer = 0f;
        tab.transform.position = startPos;
        while (timer < duration)
        {
            float t = timer/ duration;
            tab.transform.position = math.lerp(startPos, endPos, t);
            timer += Time.deltaTime;
            yield return null;
        }

        tab.transform.position = endPos;
        yield break;
    }

    private void SelectCharacter(InputAction.CallbackContext context)
    {
        if (!_tabs[_currentTabIndexToUseForSelection].onScreen || _gridRoute._readingInput) return;

        // we use the current character tab info for the grid input set
        Tab tab = _tabs[_currentTabIndexToUseForSelection];
        _gridRoute.StartRoute(tab.info);
        _gridRoute._readingInput = true;

        // we then move this tab off of the screen as we're done with it
        float3 startPos = tab.transform.position;
        float3 endPos   = startPos;
        endPos.x -= 2000; // HACK(Zack): hardcoding this for now

        StartCoroutine(RemoveTabOffScreenFunc(tab, startPos, endPos, _lerpFinishedDuration));

        _currentTabIndexToUseForSelection += 1;
        _currentTabIndexToUseForSelection  = WrapIndex(_currentTabIndexToUseForSelection, _tabs.Length);
        
        _currentTabCountInBoard -= 1;
        _currentTabCountInBoard  = math.max(_currentTabCountInBoard, 0); // REVIEW(Zack): this is probably unnecessary, but just to be safe
    }

    private static int WrapIndex(int index, int size) => (index + size) % size;
}
