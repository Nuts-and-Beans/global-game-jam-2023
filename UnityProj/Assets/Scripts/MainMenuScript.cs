using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] private TMP_Text _playText;
    [SerializeField] private TMP_Text _exitText;
    [SerializeField] private float _blinkDuration;


    [SerializeField] private AudioClip _confirmAudio;
    
    private int prevSeleted = 0;
    private int selected    = 0;
    
    private TMP_Text _currentText;
    private bool _textReset;
    
    private const string PLAY_STR = "PLAY";
    private const string EXIT_STR = "EXIT";
    
    private const string PLAY_BLINK_STR = "*PLAY*";
    private const string EXIT_BLINK_STR = "*EXIT*";

    private void Awake()
    {
        _currentText = _playText;
        
        Input.Actions.Selection.Select.performed += SelectOption;
        Input.Actions.Selection.Up.performed     += OnOptionUp;
        Input.Actions.Selection.Down.performed   += OnOptionDown;
    }

    private void OnDestroy()
    {
        Input.Actions.Selection.Select.performed -= SelectOption;
        Input.Actions.Selection.Up.performed     -= OnOptionUp;
        Input.Actions.Selection.Down.performed   -= OnOptionDown;
    }

    private IEnumerator Start() 
    {
        // NOTE(WSWhitehouse): Continuously do blink effect
        yield return BlinkEffect();
    }

    public void PlayGame()
    {
        AudioManager.PlayOneShot(_confirmAudio);
        EndLevelScript.playerWin = false;
        SceneManager.LoadScene(1);
    }

    public void OnOptionUp(InputAction.CallbackContext callback)
    {
        prevSeleted = selected;
        selected--;

        OnSelectionChanged();
    }
    
    public void OnOptionDown(InputAction.CallbackContext callback)
    {
        prevSeleted = selected;
        selected++;
        
        OnSelectionChanged();
    }
    
    private void OnSelectionChanged()
    {
        selected = math.clamp(selected, 0, 1);
        
        if (selected != prevSeleted) ResetText();
        
        SetCurrentText();
    }

    // public void OptionUp(InputAction.CallbackContext callback)
    // {
    //     int prev = selected;
    //     revert(prev);
    //     selected--;
    //     if(selected < 0)
    //     {
    //         selected = NUM_BUTTONS - 1 ;
    //     }
    //     highlight(selected);
    //    
    // }

    public void SelectOption(InputAction.CallbackContext callback)
    {
        switch(selected)
        {
            case 0:
                {
                    PlayGame();
                    break;
                }
            case 1:
                {
                    ExitGame();
                        break;
                }
         
        }

    }
    
    private void ResetText()
    {
        _playText.text = PLAY_STR;
        _exitText.text = EXIT_STR;
        _textReset     = true;
    }
    
    private void SetCurrentText()
    {
        _currentText = selected switch
        {
            0 => _playText,
            1 => _exitText,
            _ => null
        };
    }
    
    private string GetDefaultText()
    {
        return selected switch
        {
            0 => PLAY_STR,
            1 => EXIT_STR,
            _ => string.Empty
        };
    }
    
    private string GetBlinkText()
    {      
        return selected switch
        {
            0 => PLAY_BLINK_STR,
            1 => EXIT_BLINK_STR,
            _ => string.Empty
        };
    }

    // private void revert(int previous)
    // {
    //     if(previous == 0)
    //      {
    //         _playText.text = "Play";
    //         _playText.fontStyle = TMPro.FontStyles.Normal;
    //     }
    //     else if (previous == 1)
    //     {
    //         _exitText.text = "Exit";
    //         _exitText.fontStyle = TMPro.FontStyles.Normal;
    //     }
    // }
    // private void highlight(int selected)
    // {
    //     if (selected == 0)
    //     {
    //         _playText.text = "*Play*";
    //         _playText.fontStyle = TMPro.FontStyles.Underline
    //             ;
    //     }
    //     else if (selected == 1)
    //     {
    //         _exitText.text = "*Exit*";
    //         _exitText.fontStyle = TMPro.FontStyles.Underline;
    //     }
    // }

    public void ExitGame()
    {
        Application.Quit();
        //print("You have quit the game");
    }
    
    private IEnumerator BlinkEffect()
    {
        while (true)
        {
            if (_currentText == null) yield return null;
            _textReset = false;

            _currentText.text = GetBlinkText();
            yield return new WaitForSeconds(_blinkDuration);
            
             if (_currentText == null) continue;
             if (_textReset)           continue;

             _currentText.text = GetDefaultText();
            yield return new WaitForSeconds(_blinkDuration);
        }
    }
}
