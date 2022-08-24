using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Timer : MonoBehaviour
{
    public static Timer timerInstance;
    public static Action TimeOut;

    public Text timeText;
    [SerializeField] Animator conveyorBeltAnimation;
    [SerializeField] GameObject stationInfoPanel;
    [SerializeField] GameObject stationsButton;
    [SerializeField] bool wantToTurnZero = true;
    [SerializeField] bool showTime = true;

    [Header("Inactivity Panel")]
    [SerializeField] GameObject playerInActive;
    [SerializeField] bool stopCounting = false;
    [SerializeField] bool isCounting = false;
    [SerializeField] bool wasCounting = false;
    [SerializeField] float idelTime = 0;

    public float timeRemaining;
    public float timeDifference = 1;

    public WaitForSeconds timerWait = new WaitForSeconds(1);

    private void Awake()
    {
        timerInstance = this;
    }

    private void OnEnable()
    {
        DefectManager.GameCompleted += _StopConting;
        // start counting once the defect is solved
        Item.orderTaskComplete += StartCounting;
        // when player is not able to solve defect one
        DefectOneCheck.TimeOut += OnTimeOut;
        UIManager.StartGame += StartCounting;       // game will start counting 
    }

    public void OnDisable()
    {
        DefectManager.GameCompleted -= _StopConting;
        Item.orderTaskComplete -= StartCounting;
        DefectOneCheck.TimeOut -= OnTimeOut;
        // stop the timer function
        StopCoroutine(nameof(StartTimer));
        UIManager.StartGame -= StartCounting;
    }

    public void StartCounting()
    {
        isCounting = true;
        // Timer will start when the player will start
        conveyorBeltAnimation.enabled = true;           // start conveyor belt animation  
        StartCoroutine(nameof(StartTimer));
    }

    public void _StopConting ()
    {
        isCounting = false;
        print("Stop counting");
        conveyorBeltAnimation.enabled = false;          // stop conveyor belt animation
        StopCoroutine(nameof(StartTimer));
    }

    //private void Update()
    //{
    //    if (Input.GetMouseButton(0))
    //    {
    //        idelTime = 0;
    //        if (stopCounting)
    //        {
    //            playerInActive.SetActive(false);
    //            stopCounting = false;
    //            if (wasCounting)
    //            {
    //                StartCoroutine(nameof(StartTimer));
    //                wasCounting = false;
    //                conveyorBeltAnimation.enabled = true;
    //                DefectManager.instnace.StartBelt();
    //            }
    //            stopCounting = false;
    //        }
    //    }

    //    if (!stopCounting)
    //    {
    //        idelTime += Time.deltaTime;
    //        if (idelTime >= 10)
    //        {
    //            stopCounting = true;
    //            playerInActive.SetActive(true);
    //            if (isCounting)
    //            {
    //                StopCoroutine(nameof(StartTimer));
    //                wasCounting = true;
    //                conveyorBeltAnimation.enabled = false;
    //                DefectManager.instnace._StopBelt();
    //            }
    //        }
    //    }
    //}

    IEnumerator StartTimer ()
    {
        // wait till timer get 0
        while (timeRemaining >= -1)
        {
            timeRemaining -= Time.deltaTime * timeDifference;
            //timeRemaining -= Time.deltaTime * 1;
            if (timeRemaining >= -1 && showTime)
                DisplayTime(timeRemaining);     // up date in UI

            yield return null;
        }
        stationsButton.SetActive(false);
        //conveyorBeltAnimation.enabled = false;
        stationInfoPanel.SetActive(false);      // player will not able to interact with station once time runs out
        TimeOut?.Invoke();
        print("Stop");
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

    void OnTimeOut()
    {
        showTime = false;
        conveyorBeltAnimation.enabled = false;
    }

    public void _ChangeTimer (float newTime)
    {
        timeRemaining = newTime;
    }

    public void _ChangeTimeToZero()
        => timeText.text = $"00:00";
}