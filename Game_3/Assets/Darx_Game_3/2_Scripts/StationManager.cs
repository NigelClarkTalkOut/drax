using System;
using UnityEngine;

public class StationManager : MonoBehaviour
{
    public static StationManager instance;

    public static Action Station1Complete;
    public static Action Station2Complete;
    public static Action Station3Complete;
    public static Action A_Station1Complete;
    public static Action A_Station2Complete;
    public static Action A_Station3Complete;

    // number of station
    public enum Station
    {
        station1,
        station2,
        station3,
        improvement,

        A_station1,
        A_station2,
        A_station3,
    }
    public Station station;
   
    [Header("Station Buttons")]
    [SerializeField] GameObject station1Button;
    [SerializeField] GameObject station2Button;
    [SerializeField] GameObject station3Button;
    [SerializeField] GameObject A_station1Button;
    [SerializeField] GameObject A_station2Button;
    [SerializeField] GameObject A_station3Button;
    [SerializeField] GameObject nullButton;

    [SerializeField] GameObject afterImprovementStation;

    [Header("Station 2 Item Box")]
    [SerializeField] ItemBox itemBox1;
    [SerializeField] ItemBox itemBox2;
    [SerializeField] ItemBox itemBox3;
    [SerializeField] ItemBox itemBox4;
    
    [Header("Station 3 Item Box")]
    [SerializeField] ItemBox itemBox1_S3;

    #region Events
    private void OnEnable()
    {
        Timer.ChangeStationNumber += ChangeStations;
        UiManager.A_StationComplete += ChangeStations;
    }

    private void OnDisable()
    {
        Timer.ChangeStationNumber -= ChangeStations;
        UiManager.A_StationComplete -= ChangeStations;
    }
    #endregion

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // game will always start with station 1
        station = Station.station1;
        UpdateButtons();                        // update buttons according to the station user solved 
    }

    // update buttons according to which station player will play
    public void UpdateButtons ()
    {
        if (station == Station.station1)
            ChangeStation(station1Button);

        else if (station == Station.station2)
            ChangeStation(station2Button);

        else if (station == Station.station3)
            ChangeStation(station3Button);
        
        else if (station == Station.improvement)
            ChangeStation(nullButton);

        else if (station == Station.A_station1)
            ChangeStation(A_station1Button);

        else if (station == Station.A_station2)
            ChangeStation(A_station2Button);

        else if (station == Station.A_station3)
            ChangeStation(A_station3Button);

        else return;

    }

    void ChangeStation (GameObject ActiveButton)
    {
        station1Button.SetActive(false);
        station2Button.SetActive(false);
        station3Button.SetActive(false);
        A_station1Button.SetActive(false);
        A_station2Button.SetActive(false);
        A_station3Button.SetActive(false);
        nullButton.SetActive(false);

        // set the active button on
        ActiveButton.SetActive(true);
    }

    public void ChangeStations(float i)
    {
        if (station == Station.station1)
        {
            Station1Complete?.Invoke();
            station = Station.station2;
            UpdateButtons();                // update buttons according to the station user solved 
        }
    
        else if (station == Station.station2)
        {
            Station2Complete?.Invoke();
            station = Station.station3;
            UpdateButtons();                // update buttons according to the station user solved 
        }
   
        else if (station == Station.station3)
        {
            Station3Complete?.Invoke();
            station = Station.improvement;
            UpdateButtons();                // update buttons according to the station user solved 
        }
        
        else if (station == Station.improvement)
        {
            Station3Complete?.Invoke();
            station = Station.A_station1;
            UpdateButtons();                // update buttons according to the station user solved 
        }

        else if (station == Station.A_station1)
        {
            A_Station1Complete?.Invoke();
            station = Station.A_station2;
            UpdateButtons();                // update buttons according to the station user solved 
        }

        else if (station == Station.A_station2)
        {
            A_Station2Complete?.Invoke();
            station = Station.A_station3;
            UpdateButtons();                // update buttons according to the station user solved 
        }
   
        else if (station == Station.A_station3)
        {
            A_Station3Complete?.Invoke();
        }
    }

    public void _ShowAfterImprovementLayOut()
    {
        station = Station.A_station1;
        afterImprovementStation.SetActive(true);
    }

    public void _Station2ItemBox()
    {
        itemBox1.timeOut = false;
        itemBox2.timeOut = false;
        itemBox3.timeOut = false;
        itemBox4.timeOut = false;
    }

    public void _Station3ItemBox ()
    {
        itemBox1_S3.timeOut = false;
    }
}