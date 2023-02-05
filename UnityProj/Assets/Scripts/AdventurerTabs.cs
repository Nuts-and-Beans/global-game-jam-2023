using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventurerTabs : MonoBehaviour
{
    [SerializeField] private List<Character> _tabs;
    [SerializeField] private int _maxTabs;
    private void Awake()
    {
        _tabs = new List<Character>();

        for (int i = 0; i < _maxTabs; i++)
        {
            Character c = Adventurers.GetNextCharacter();
            _tabs.Add(c);
            Debug.Log(_tabs[i].name);
        }
        
    }

    public void OnNeedNewCharacter(int index)

    {
        Adventurers.ReturnCharacter(_tabs[index]);
        _tabs[index] = Adventurers.GetNextCharacter();
        Character movingCharacterIndex = _tabs[index];
        _tabs.RemoveAt(index);
        _tabs.Add(movingCharacterIndex);
    }

    public int GetMaxTabs()
    {
        return _maxTabs;
    }
    
    public Character GetTabInfo(int index)
    {
        return _tabs[index];
    }
}
