using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevelScript : MonoBehaviour
{
    // Start is called before the first frame update
    public static bool playerWin;





    private void Awake()
    {
        Boss.OnBossDefeatedEvent += PlayerWin;
        Timer.OnTimerEndEvent += PlayerLose;
        playerWin = false;

    }
    void PlayerWin()
    {
        playerWin = true;
        SceneManager.LoadScene(2);
    }
    void PlayerLose()
    {
        playerWin = false;
        SceneManager.LoadScene(2);
    }

    // Update is called once per frame
    

}
