using System.Collections;
using UnityEngine;

public class GhostTugSystem : MonoBehaviour
{
    public Transform[] points;                      // all points in the game

    [SerializeField] bool canRotate = true;
    Transform startPoint, endPoint;                // for tugs start point and end point (start/in-game)
    [SerializeField] int index = 0;                                  // for getting next end point
    [SerializeField] float tugSpeed;                // speed of the tug

    // start moving ghost tugs
    private void OnEnable()
    {
        StartCoroutine(nameof(UpdatingLoop));
    }

    // stop moving ghost tugs 
    private void OnDisable()
    {
        StopCoroutine(nameof(UpdatingLoop));
    }

    IEnumerator UpdatingLoop()
    {
        // get the first point and the last point of the first track 
        startPoint = points[index - 1];
        endPoint = points[index];

        while (true)
        {
            // tug will look at the next point
            if (canRotate)
            {
                Vector2 direction
                  = new Vector2(endPoint.position.x - transform.position.x, endPoint.position.y - transform.position.y);
                // rotate the tug in the next point's direction
                transform.up = direction;
            }

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
            if (index < points.Length - 1)
            {
                index++;    // add to index
                // update the start and end point
                endPoint = points[index];
            }
            else
            {
                index = 1;
                endPoint = points[index];
            }
        }
    }
}