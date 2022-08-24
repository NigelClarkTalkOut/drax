using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Timer : MonoBehaviour
{
    [SerializeField] bool wantToTurnZero = true;
    [SerializeField] TimeChanger timeChangerInstance;
    public float timeRemaining;
    public float timeDifference = 1;
    [SerializeField] Text timeText;
    [SerializeField] Text losePanelText;
    [SerializeField] CreatingPathWays pathWaySystem;

    public static Action threeTimes;
    public int timerTimer = 1;
    public WaitForSeconds timerWait = new WaitForSeconds(1);

    private void Start()
    {
        pathWaySystem = GameObject.FindGameObjectWithTag("PathWaySystem").GetComponent<CreatingPathWays>();
        if (SelectSeed.instance.phases == SelectSeed.Phases.loadBalancingPhaseOne)
            wantToTurnZero = false;
        else wantToTurnZero = true;
    }

    public void ___StartTimer()
    {
        // Timer will start when the player will start
        StartCoroutine(nameof(StartTimer));
    }

    private void OnEnable()
    {
        ResultSystem.GameEnded += StopTheClock;
    }

    public void OnDisable()
    {
        ResultSystem.GameEnded -= StopTheClock;
        // stop the timer function
        StopCoroutine(nameof(StartTimer));
    }

    IEnumerator StartTimer ()
    {
        // wait till timer get 0
        while (timeRemaining >= -1)
        {
            if (!GameManager.instance.gameCompleted)
            {
                // subtract time each second with 1
                if (SeedSystem.instance.falsePath)
                    timeRemaining -= Time.deltaTime * timeChangerInstance.timeToAdd;
                if (!SeedSystem.instance.falsePath)
                    timeRemaining -= Time.deltaTime * timeDifference;
                //timeRemaining -= Time.deltaTime * 1;
                if (timeRemaining >= -1)
                    DisplayTime(timeRemaining);     // up date in UI
            }
            yield return null;
        }
        TugSystem.instance.enabled = false;
        // if the player runs out of time, show player they have ran out if time 
        SelectSeed.instance.tries++;
        if (SelectSeed.instance.tries > 2)
        {
            print("Call timer");
            threeTimes?.Invoke();
        }
        else
        {
            UIManager.instance.ChangePannel(UIManager.instance.OutOfTime);
            if (SelectSeed.instance.phases == SelectSeed.Phases.improvementPhase)
            {
                if (pathWaySystem.leftPoint != 1 || pathWaySystem.rightPoint != 2)
                {
                    print("Change the text");
                    losePanelText.text = $"Oh no! It looks like the road you added is not the most efficient. We will now change this for you for the next phase.";
                }
            }
        }
    }

    // convert seconds in minutes 
    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void StopTheClock()
    {
        if (wantToTurnZero)
        {
            timeRemaining = 0;
            timeText.text = $"00:00";
        }
        enabled = false;
    }
}