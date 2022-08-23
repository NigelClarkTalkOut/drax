using UnityEngine;

public class ItemBox : MonoBehaviour
{

    [SerializeField] GameObject[] finger;
    public int partCount = 0;

    [SerializeField] Transform parentForSpawn;
    [SerializeField] bool station1 = false;
    [SerializeField] bool station2 = false;
    [SerializeField] bool station3 = false;
    [SerializeField] bool A_station1 = false;
    [SerializeField] bool A_station2 = false;
    [SerializeField] bool A_station3 = false;

    public bool timeOut = false;                // check if time is out or not

    private void OnEnable()
    {
        // time related actions
        Timer.TimeOut += TimeIsOut;
        Timer.TimeStart += TimeIsNotOut;

        // when station timer is out 
        Timer.B_Station1TimeOut += WhenTimeReset;
        Timer.B_Station2TimeOut += WhenTimeReset;
        Timer.B_Station3TimeOut += WhenTimeReset;
    
        // when station timer is out 
        Timer.A_Station1TimeOut += WhenTimeReset;
        Timer.A_Station2TimeOut += WhenTimeReset;
        Timer.A_Station3TimeOut += WhenTimeReset;

        CompletedItem.PalmReached += WhenTimeReset;
    }

    private void OnDisable()
    {
        // time related actions
        Timer.TimeOut -= TimeIsOut;
        Timer.TimeStart -= TimeIsNotOut;

        Timer.B_Station1TimeOut -= WhenTimeReset;
        Timer.B_Station2TimeOut -= WhenTimeReset;
        Timer.B_Station3TimeOut -= WhenTimeReset;

        // when station timer is out 
        Timer.A_Station1TimeOut -= WhenTimeReset;
        Timer.A_Station2TimeOut -= WhenTimeReset;
        Timer.A_Station3TimeOut -= WhenTimeReset;

        CompletedItem.PalmReached -= WhenTimeReset;
    }

    private void OnMouseDown()
    {
        if (timeOut) return;                            // item will not spawn if time is out 
        if (partCount >= finger.Length) return;         // if the box runs out of items

        print("Items left");
        GameObject item = Instantiate(finger[partCount], transform.position, Quaternion.identity, parentForSpawn);
        // give refrance to itembox 
        item.GetComponent<Item>().itemBox = this;
    }

    // when station timer and out and station should be reset
    void WhenTimeReset ()
    {
        if (StationManager.instance.station == StationManager.Station.station1)
        {
            if (station1)
                partCount = 0;
        }
   
        else if (StationManager.instance.station == StationManager.Station.station2)
        {
            if (station2)
                partCount = 0;
        }
    
        else if (StationManager.instance.station == StationManager.Station.station3)
        {
            if (station3)
                partCount = 0;
        }
   
        else if (StationManager.instance.station == StationManager.Station.A_station1)
        {
            if (A_station1)
                partCount = 0;
        }
    
        else if (StationManager.instance.station == StationManager.Station.A_station2)
        {
            if (A_station2)
                partCount = 0;
        }
   
        else if (StationManager.instance.station == StationManager.Station.A_station3)
        {
            if (A_station3)
                partCount = 0;
        }
    }

    void TimeIsOut ()
    {
        timeOut = true;
    }

    void TimeIsNotOut ()
    {
        timeOut = false;
    }
}