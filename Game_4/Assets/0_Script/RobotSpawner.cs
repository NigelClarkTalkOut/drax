using System;
using UnityEngine;

public class RobotSpawner : MonoBehaviour
{
    public static Action changeRobots;          // when you need to respawn the robots
    public static Action robotCompleted;        // when robots are completed 

    [SerializeField] GameObject[] robots;
    [SerializeField] Transform respawnPlace;
    [SerializeField] int indexToSpawn;

    GameObject robotToDelete;
    string stationName;                 // for comparing name of station
    GameObject correctRobot;            // for getting correct robot part

    private void Start()
    {
        robotToDelete = Instantiate(robots[indexToSpawn], transform.position, Quaternion.identity, transform);
    }

    private void OnTriggerEnter2D(Collider2D info)
    {
        // compare the tag of stations 
        if (info.CompareTag ("Stations"))
        {
            Destroy(robotToDelete);         // destroy previous robots
            // get the name of the stations
            stationName = info.gameObject.name;
            // spawn robots according to name we got
            robotToDelete = Instantiate(GetRobot(), transform.position, Quaternion.identity, transform);
            changeRobots?.Invoke();         // action for changing robots
        }

        else if(info.CompareTag("Respawn"))
        {
            transform.position = respawnPlace.position;
            Destroy(robotToDelete);         // destroy previous robots
            // get the name of the stations
            stationName = info.gameObject.name;
            // spawn robots according to name we got
            robotToDelete = Instantiate(robots[0], transform.position, Quaternion.identity, transform);
            changeRobots?.Invoke();             // action for changing robots and packing them in a box
            robotCompleted?.Invoke();           // action for changing robots and packing them in a box
        }
    }

    // get robots according to name we got from stations
    public GameObject GetRobot()
    {
        for (int i = 0; i < robots.Length; i++)
        {
            // comapre names of robots and stations 
            if (robots[i].name == stationName)
            {
                correctRobot = robots[i];               
            }
        }
        return correctRobot;
    }
}