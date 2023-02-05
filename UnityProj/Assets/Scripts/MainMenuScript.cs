using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class MainMenuScript : MonoBehaviour
{

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
        selected++;
        if(selected >= num_buttons )
        {
            selected =0;
        }
        print(selected);
    }
    public void OptionUp(InputAction.CallbackContext callback)
    {
        selected--;
        if(selected < 0)
        {
            selected = num_buttons - 1 ;
        }
        print(selected);
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

    public void ExitGame()
    {
        Application.Quit();
        //print("You have quit the game");
    }
}
