using System;
using System.Collections;
using UnityEngine;

public class Item : MonoBehaviour
{
    Camera mainCamera;
    Vector2 mousePosition;
    StationManager stationManager;
    public ItemBox itemBox;

    [Header("Actions")]
    //public static Action<float> ChangeStationNumber;
    public static Action lastStation;
    public static Action A_Station1Placed;
    public static Action Itemplaced;

    [Header("Station Action")]
    public static Action Station1LastPart;
    public static Action Station2LastPart;
    public static Action Station3LastPart;

    [SerializeField] string correctPositionName;        // get the correct name for position 
    [SerializeField] GameObject correctPosition;        // hold the correct position from the name
    [SerializeField] bool onPosition = false;           // check if the part is on correct position
    [SerializeField] bool placed = false;               // check is the part is placed on the correct position 
    [SerializeField] bool setRotationToo = false;       // check is the part is placed on the correct position 
    [SerializeField] bool lastPart = false;             // check is the part is placed on the correct position
    [SerializeField] bool station3LastPart = false;     // check is the part is placed on the correct position

    [Header ("Stations")]
    [SerializeField] bool station1 = false;
    [SerializeField] bool station2 = false;
    [SerializeField] bool station3 = false;
    [SerializeField] bool A_station1 = false;
    [SerializeField] bool A_station2 = false;
    [SerializeField] bool A_station3 = false;

    private void OnEnable()
    {
        // when station timer is out before improvements
        Timer.B_Station1TimeOut += WhenTimeReset;
        Timer.B_Station2TimeOut += WhenTimeReset;
        Timer.B_Station3TimeOut += WhenTimeReset;
   
        // when station timer is out after improvements
        Timer.A_Station1TimeOut += WhenTimeReset;
        Timer.A_Station2TimeOut += WhenTimeReset;
        Timer.A_Station3TimeOut += WhenTimeReset;

        Item.Station1LastPart += Station1PartComplete;
        Item.Station2LastPart += Station2PartComplete;
        Item.Station3LastPart += Station3PartComplete;
    }

    private void OnDisable()
    {
        Timer.B_Station1TimeOut -= WhenTimeReset;
        Timer.B_Station2TimeOut -= WhenTimeReset;
        Timer.B_Station3TimeOut -= WhenTimeReset;

        // when station timer is out after improvements
        Timer.A_Station1TimeOut -= WhenTimeReset;
        Timer.A_Station2TimeOut -= WhenTimeReset;
        Timer.A_Station3TimeOut -= WhenTimeReset;

        Item.Station1LastPart -= Station1PartComplete;
        Item.Station2LastPart -= Station2PartComplete;
        Item.Station3LastPart -= Station3PartComplete;
    }

    private void Awake()
    {
        // get instance for station manager
        mainCamera = Camera.main;
        stationManager = StationManager.instance;       // get station manager instance
    }

    private void Start()
    {
        StartCoroutine(nameof(MouseTracking));      // start tracking mouse place
        // hide the correct position
        correctPosition = GameObject.Find(correctPositionName);
        // if the corret position place is found 
        if (correctPosition != null)
            correctPosition.GetComponent<SpriteRenderer>().enabled = true;
    }

    IEnumerator MouseTracking()
    {
        // if the part is placed 
        while (!placed)
        {
            transform.position = GetMousePosition();
            // if the left mouse button is released 
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                // if the part is on the position will snap to on the correct position 1`
                if (onPosition)
                {
                    Itemplaced?.Invoke();       
                    correctPosition.GetComponent<SpriteRenderer>().enabled = false;
                    // set position and rotation for part according to the correct position
                    if (setRotationToo)
                        transform.SetPositionAndRotation(correctPosition.transform.position, correctPosition.transform.rotation);
                    else
                        transform.position = correctPosition.transform.position;
                    GetComponent<Collider2D>().enabled = false;
                    placed = true;
                    // if item belongs to station 1
                    int itemCount = itemBox.partCount++;            // increase the counter for the parts
                                                                    // if all the parts are place
                    if (lastPart)
                    {
                        if (station1)
                        {
                            Station1LastPart?.Invoke();
                        }
                        else if (station2)
                        {
                            Station2LastPart?.Invoke();
                        }
                        else if (station3)
                        {
                            Station3LastPart?.Invoke();
                        }
                        else
                        {
                            Timer.ChangeStationNumber?.Invoke(0);
                        }

                        WhenTimeReset();
                    }
                    if (station3LastPart)
                        WhenTimeReset();
                        //lastStation?.Invoke(); 

                    if (stationManager.station == StationManager.Station.A_station1)
                        A_Station1Placed?.Invoke();
                    yield break;
                }
                // if not on correct position, return to the default position
                Destroy(gameObject);
                // if the corret position is found
                if (correctPosition != null)
                    correctPosition.GetComponent<SpriteRenderer>().enabled = false;
            }
            yield return null;
        }
    }

    // get the position or mouse/touch 
    Vector2 GetMousePosition()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
        // get mouse position 
        mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        return mousePosition;
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (correctPosition == null) return;
           
        // if the part is on the correct position 
        if (collider.name == correctPosition.name)
        {
            onPosition = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (correctPosition == null) return;

        // if the part gets out of the correct position 
        if (collision.name == correctPosition.name)
        {
            onPosition = false;
        }
    }

    // destroy all the items when timer runs out 
    void WhenTimeReset()
    {
        if (correctPosition != null)
            correctPosition.GetComponent<SpriteRenderer>().enabled = false;

        if (StationManager.instance.station == StationManager.Station.station1)
        {
            if (station1)
                Destroy(gameObject);
        }

        else if (StationManager.instance.station == StationManager.Station.station2)
        {
            if (station2)
                Destroy(gameObject);
        }

        else if (StationManager.instance.station == StationManager.Station.station3)
        {
            if (station3)
                Destroy(gameObject);
        }

        else if (StationManager.instance.station == StationManager.Station.A_station1)
        {
            if (A_station1)
            {
                Destroy(gameObject);
                UiManager.instance.A_Station1Parts = 0;
            }
        }
   
        else if (StationManager.instance.station == StationManager.Station.A_station2)
        {
            if (A_station2)
                Destroy(gameObject);
        }
    
        else if (StationManager.instance.station == StationManager.Station.A_station3)
        {
            if (A_station3)
                Destroy(gameObject);
        }
    }

    void Station1PartComplete ()
    {
        if (station1)
        {
            Destroy(gameObject);
        }
    }

    void Station2PartComplete()
    {
        if (station2)
        {
            Destroy(gameObject);
        }
    }

    void Station3PartComplete ()
    {
        if (station3)
        {
            Destroy(gameObject);
        }
    }
}