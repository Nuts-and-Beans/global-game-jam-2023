using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinLoseTextScript : MonoBehaviour
{
   [SerializeField] public TMP_Text myText;
   [SerializeField] public bool playerWin;

   private void Awake()
   {
      myText.text = playerWin ? "You Win!" : "You Lose!";
   }

   public void ExitGame()
   {
      print("You have exited the game");
      Application.Quit();
   }
}
