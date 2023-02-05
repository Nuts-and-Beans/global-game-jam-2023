using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinLoseTextScript : MonoBehaviour
{
   [SerializeField] public TMP_Text myText;
   public static bool playerWin;

   private void Awake()
   {
      myText.text = EndLevelScript.playerWin ? "You Win!" : "You Lose!";
   }

   public void ExitGame()
   {
      
      Application.Quit();
   }

   public void PlayAgain()
   {
      SceneManager.LoadScene(0);
   }
}
