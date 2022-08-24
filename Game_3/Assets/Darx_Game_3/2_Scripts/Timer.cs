using TMPro;
using System;
using UnityEngine;
using System.Collections;

public class Timer : MonoBehaviour
{
    public static Action<float> ChangeStationNumber;
    public static Action TimeOut;
    public static Action TimeStart;

    // before improvement time out 
    public static Action B_Station1TimeOut;
    public static Action B_Station2TimeOut;
    public static Action B_Station3TimeOut;
    
    // after improvement time out 
    public static Action A_Station1TimeOut;
    public static Action A_Station2TimeOut;
    public static Action A_Station3TimeOut;

    public TMP_Text timeText;
    [SerializeField] bool showTime = true;

    [Header ("Time")]
    [SerializeField] bool isCounting = true;
    [SerializeField] bool stopCounting = true;
    [SerializeField] bool wasCounting = false;
    [SerializeField] float idelTime = 0;
    [SerializeField] GameObject playerInActive;
    [SerializeField] GameObject getActiveArea;

    public float timeRemaining;
    public float timeDifference = 1;

    public WaitForSeconds timerWait = new WaitForSeconds(1);

    [Header("Station Check")]
    [SerializeField] bool palmDone = false;
    [SerializeField] bool halfArm = false;
    [SerializeField] bool completeArm = false;

    private void OnEnable()
    {
        Timer.ChangeStationNumber += _StopConting;
        UiManager.A_StationComplete += _StopConting;

        Item.Station1LastPart += Station1Done;
        Item.Station2LastPart += Station2Done;
        Item.Station3LastPart += Station3Done;
    }

    private void OnDisable()
    {
        Timer.ChangeStationNumber -= _StopConting;
        UiManager.A_StationComplete -= _StopConting;

        Item.Station1LastPart -= Station1Done;
        Item.Station2LastPart -= Station2Done;
        Item.Station3LastPart -= Station3Done;
    }

    public void _StartCounting(int time)
    {
        isCounting = true;
        // timer will start when timer is on
        Debug.Log("Start Counting");
        if (time != 0)
            timeRemaining = time;
        else
        {
            if (StationManager.instance.station == StationManager.Station.station1)
                timeRemaining = 20;

            else if (StationManager.instance.station == StationManager.Station.station2)
                timeRemaining = 15;

            else if (StationManager.instance.station == StationManager.Station.station3)
                timeRemaining = 10;
            else
                timeRemaining = 10;
        }
        StartCoroutine(nameof(StartTimer));
    }

    public void _StopConting (float i)
    {
        isCounting = false;
        // timer will end when station is solved 
        Debug.Log("Stop counting");
        StopCoroutine(nameof(StartTimer));
        timeText.text = $"00:00";
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
        isCounting = false;
        TimeOut?.Invoke();
        CheckTimeOut();     // check on what station time ran out 
    }

    // convert seconds in minutes 
    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // check when station time out is done
    private void CheckTimeOut ()
    {
        if (StationManager.instance.station == StationManager.Station.station1)
        {
            if (palmDone)
                ChangeStationNumber?.Invoke(9.30f);         // event when station 1 is complete
            else
                B_Station1TimeOut?.Invoke();
        }

        else if (StationManager.instance.station == StationManager.Station.station2)
        {
            if (halfArm)
                ChangeStationNumber?.Invoke(33f);
            else
                B_Station2TimeOut?.Invoke();
        }
   
        else if (StationManager.instance.station == StationManager.Station.station3)
        {
            if (completeArm)
                ChangeStationNumber?.Invoke(0);
            else
                B_Station3TimeOut?.Invoke();
        }
   
        else if (StationManager.instance.station == StationManager.Station.A_station1)
        {
            A_Station1TimeOut?.Invoke();
        }
    
        else if (StationManager.instance.station == StationManager.Station.A_station2)
        {
            A_Station2TimeOut?.Invoke();
        }
    
        else if (StationManager.instance.station == StationManager.Station.A_station3)
        {
            A_Station3TimeOut?.Invoke();
        }
    }

    public void _StartTimer() => TimeStart?.Invoke();

    void Station1Done() => palmDone = true;
    void Station2Done() => halfArm = true;
    void Station3Done() => completeArm = true;
}