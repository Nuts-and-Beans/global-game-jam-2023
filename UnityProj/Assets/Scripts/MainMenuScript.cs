using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class MainMenuScript : MonoBehaviour
{
    [SerializeField] private Button _playButton;
    [SerializeField] private TMPro.TMP_Text _playText;
    [SerializeField] private TMPro.TMP_Text _exitText;

    [SerializeField] private Button _exitButton;
    int selected = 0;
    [SerializeField] int num_buttons;
    private void Awake()
    {
        Input.Actions.Selection.Select.performed += SelectOption;
        Input.Actions.Selection.Up.performed += OptionUp;
        Input.Actions.Selection.Down.performed += OptionDown;
    }

    public void PlayGame()
    {
        
        SceneManager.LoadScene("SampleScene");
    }

    public void OptionDown(InputAction.CallbackContext callback)
    {
        int prev = selected;
        revert(prev);
        selected++;
        
        if(selected >= num_buttons )
        {
            selected =0;
        }
        highlight(selected);
       
    }
    public void OptionUp(InputAction.CallbackContext callback)
    {
        int prev = selected;
        revert(prev);
        selected--;
        if(selected < 0)
        {
            selected = num_buttons - 1 ;
        }
        highlight(selected);
       
    }

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
    private void revert(int previous)
    {
        if(previous == 0)
         {
            _playText.text = "Play";
            _playText.fontStyle = TMPro.FontStyles.Normal;
        }
        else if (previous == 1)
        {
            _exitText.text = "Exit";
            _exitText.fontStyle = TMPro.FontStyles.Normal;
        }
    }
    private void highlight(int selected)
    {
        if (selected == 0)
        {
            _playText.text = ">Play<";
            _playText.fontStyle = TMPro.FontStyles.Underline
                ;
        }
        else if (selected == 1)
        {
            _exitText.text = ">Exit<";
            _exitText.fontStyle = TMPro.FontStyles.Underline;
        }
    }

    public void ExitGame()
    {
        Application.Quit();
        //print("You have quit the game");
    }
}
