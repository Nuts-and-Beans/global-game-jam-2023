using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class WinLoseTextScript : MonoBehaviour
{
   [SerializeField] public TMP_Text myText;
   public static bool playerWin;

    [SerializeField] private Button _playButton;
    [SerializeField] private TMPro.TMP_Text _playText;
    [SerializeField] private TMPro.TMP_Text _exitText;

    [SerializeField] private AudioSource _winAudio;
    [SerializeField] private AudioSource _loseAudio;
    [SerializeField] private AudioSource _confirmAudio;
    [SerializeField] private AudioSource _cancelAudio;

    [SerializeField] private Button _exitButton;
    int selected = 0;
    [SerializeField] int num_buttons;
    public static bool playerwin;
    
    private const string MAIN_MENU_STR = "MAIN MENU";
    private const string EXIT_GAME_STR = "EXIT GAME";
    private const string MAIN_MENU_BLINK_STR = "*MAIN MENU*";
    private const string EXIT_GAME_BLINK_STR = "*EXIT GAME*";


    private void Awake()
    {
        Input.Actions.Selection.Select.performed += SelectOption;
        Input.Actions.Selection.Up.performed += OptionUp;
        Input.Actions.Selection.Down.performed += OptionDown;
        myText.text = EndLevelScript.playerWin ? "You Win!" : "You Lose!";
    }

    private void Start()
    {
        
        if (EndLevelScript.playerWin)
        {
            _winAudio.Play();

        }
        else
        {
            _loseAudio.Play();
        }

        _playText.text = MAIN_MENU_BLINK_STR;
    }

    public void PlayGame()
    {
        EndLevelScript.playerWin = false;
        PlayAgain();
    }

    public void OptionDown(InputAction.CallbackContext callback)
    {
        int prev = selected;
        revert(prev);
        selected++;

        if (selected >= num_buttons)
        {
            selected = 0;
        }
        highlight(selected);

    }
    public void OptionUp(InputAction.CallbackContext callback)
    {
        int prev = selected;
        revert(prev);
        selected--;
        if (selected < 0)
        {
            selected = num_buttons - 1;
        }
        highlight(selected);

    }

    public void SelectOption(InputAction.CallbackContext callback)
    {
        switch (selected)
        {
            case 0:
                {
                    _confirmAudio.Play();
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
    private void revert(int previous)
    {
        if (previous == 0)
        {
            _playText.text = MAIN_MENU_STR;
            // _playText.fontStyle = TMPro.FontStyles.Normal;
        }
        else if (previous == 1)
        {
            _exitText.text = EXIT_GAME_STR;
            // _exitText.fontStyle = TMPro.FontStyles.Normal;
        }
    }
    private void highlight(int selected)
    {
        if (selected == 0)
        {
            _playText.text = MAIN_MENU_BLINK_STR;
            // _playText.fontStyle = TMPro.FontStyles.Underline;
        }
        else if (selected == 1)
        {
            _exitText.text = EXIT_GAME_BLINK_STR;
            // _exitText.fontStyle = TMPro.FontStyles.Underline;
        }
    }

   public void ExitGame()
   {
        // NOTE(Zack): so that we're able to stop the editor using the buttons 
#if UNITY_EDITOR
       UnityEditor.EditorApplication.isPlaying = false;
#else
      Application.Quit();
#endif
   }

   public void PlayAgain()
   {
      SceneManager.LoadScene(0);
   }
}
