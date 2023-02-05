using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevelScript : MonoBehaviour
{
    // Start is called before the first frame update
    public static Boss.EventDel OnBossDefeatedEvent;
    //public static Timer.TimerDel OnTimerEndEvent;
    public static bool playerWin;
    private void Awake()
    {
        OnBossDefeatedEvent += PlayerWin;
        Timer.OnTimerEndEvent += PlayerLose;
        playerWin = false;

    }
    void PlayerWin()
    {
        playerWin = true;
        SceneManager.LoadScene("GameOverWinLose");
    }
    void PlayerLose()
    {
        Debug.Log("Game over bitch");
        playerWin = false;
        SceneManager.LoadScene(2);
    }

    // Update is called once per frame
    

}
