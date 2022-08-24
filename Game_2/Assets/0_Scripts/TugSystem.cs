using UnityEngine;
using System.Collections;

public class TugSystem : MonoBehaviour
{
    public static TugSystem instance;
        
    public Transform[] points;                      // all points in the game
    public int updatingIndex = 1;                   // for adding in array for tug path system
    public int routeCount = 1;
    public float tugRunningSpeed = 3;
    public float tugSpeed;                          // speed of the tug

    int index = 0;                                  // for getting next end point      
    Transform  startPoint, endPoint;                // for tugs start point and end point (start/in-game)
    readonly WaitForSeconds recivingTime = new WaitForSeconds(1.3f);   // waiting for the package to recive

    // defining the instance
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        tugSpeed = tugRunningSpeed;
        updatingIndex = 0;
        // getting sprite componenet and sprite transperent 
    }

    private void OnEnable()
    {
        // adding function for observation
        GameManager.OnClear += ResetTrack;
        PackageSystem.TugArrived += StartCoroutineTugArrived;
    }

    // button function 
    public void __StartUpdatingLoop()
    {
        // start coroutine loop button
        StartCoroutine(nameof(UpdatingLoop));
    }

    // end the coroutine button 
    private void OnDisable()
    {
        // clearing all obervation functions and coroutines 
        StopCoroutine(nameof(UpdatingLoop));
        GameManager.OnClear -= ResetTrack;
        PackageSystem.TugArrived -= StartCoroutineTugArrived;
    }

    IEnumerator UpdatingLoop ()
    {
        // get the first point and the last point of the first track 
        startPoint = points[0];
        endPoint = points[1];

        while (true)
        {
            // tug will look at the next point 
            Vector2 direction
                = new Vector2 (endPoint.position.x - transform.position.x, endPoint.position.y - transform.position.y);
            // rotate the tug in the next point's direction
            transform.up = -direction;

            // tug movement 
            TugMovement();
            yield return null;      // wait for the nexr frame
        }
    }

    private void TugMovement()
    {
        // move from starting point to end point
        transform.position = Vector3.MoveTowards(transform.position, endPoint.position, tugSpeed * Time.deltaTime);

        // if the tug reaches the end position
        if (transform.position == endPoint.position)
        {
            // and if the array have the next point, set the next point as the end point 
            if (index < points.Length - 1 && points[index + 1] != null)
            {
                index++;    // add to index
                // update the start and end point
                endPoint = points[index];
            }
        }
    }

    void ResetTrack ()
    {
        // reseting track points 
        updatingIndex = 0;
        routeCount = 1;
        index = 0;
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = null;
        }
        // reseting tug color
        StopCoroutine(nameof(UpdatingLoop));    // stop the corountine
    }   

    // start coroutine on the function waiting for action event
    void StartCoroutineTugArrived ()
        => StartCoroutine(nameof(RecivePackage));

    IEnumerator RecivePackage ()
    {
        tugSpeed = 0;       // make tug stop moving
        // wait for 3 seconds 
        yield return recivingTime;
        tugSpeed = tugRunningSpeed;       // return to normal speed
    }
}