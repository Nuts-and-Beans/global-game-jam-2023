using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventurerTabs : MonoBehaviour
{
    [SerializeField] private Character[] _tabs;
    [SerializeField] private int _maxTabs;
    private void Awake()
    {
        _tabs = new Character[_maxTabs];

        for (int i = 0; i < _maxTabs; i++)
        {
            _tabs[i] = Adventurers.GetNextCharacter();
            Debug.Log(_tabs[i].name);
        }
        
    }

    private void OnNeedNewCharacter(int index)

    {
        Adventurers.ReturnCharacter(_tabs[index]);
        _tabs[index] = Adventurers.GetNextCharacter();
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
