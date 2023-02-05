using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevelScript : MonoBehaviour
{
    // Start is called before the first frame update
    public static Boss.EventDel OnBossDefeatedEvent;
    public static Timer.TimerDel OnTimerEndEvent;
    public static bool playerWin;
    private void Awake()
    {
        OnBossDefeatedEvent += PlayerWin;
        OnTimerEndEvent += PlayerLose;
        playerWin = false;

    }
    void PlayerWin()
    {
        playerWin = true;
        SceneManager.LoadScene("GameOverWinLose");
    }
    void PlayerLose()
    {
        playerWin = false;
        SceneManager.LoadScene("GameOverWinLose");
    }

    // Update is called once per frame
    

}
