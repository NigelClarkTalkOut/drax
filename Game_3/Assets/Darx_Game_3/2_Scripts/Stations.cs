using System;
using UnityEngine;

public class Stations : MonoBehaviour
{
    Camera mainCamera;
    Vector2 mousePosition;

    public static Action StationPlaced;
    public static Action BoxPlaced;

    Vector3 defaultPosition;
    [SerializeField] GameObject correctPosition;
    [SerializeField] bool onPosition = false;           // if the item is not correct position or not
    [SerializeField] bool placed = false;               // if the item is placed on the correct position 

    [Header ("Define state")]
    [SerializeField] bool isStation = false;            // if the item is not correct position or not 
    [SerializeField] bool isBox = false;                // if the item is not correct position or not 

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Start()
    {
        correctPosition.SetActive(false);
        //defaultPosition = transform.position;
    }

    private void OnMouseDown()
    {
        defaultPosition = transform.position;
        if (placed) return;
        correctPosition.SetActive(true);
    }

    private void OnMouseDrag()
    {
        if (placed) return;

        // if station is not placed then player will able to drag the stations
        transform.position = GetMousePosition();
    }

    private void OnMouseUp()
    {
        // if the player is dragging on the correct position from /////position check
        if (onPosition)
        {
            // let the game know its placed 
            placed = true;
            // set the position of the item
            //transform.position = correctPosition.transform.position;
            transform.SetPositionAndRotation(correctPosition.transform.position, correctPosition.transform.rotation);
            correctPosition.SetActive(false);           // set the correct position off 
            GetComponent<Collider2D>().enabled = false;
            if (isStation)
                StationPlaced?.Invoke();            // action for station placed
            if (isBox)
                BoxPlaced?.Invoke();                // action for box placed 
            return;
        }
        // if not on correct position, return to the default position 
        transform.position = defaultPosition;
        correctPosition.SetActive(false);
        // if not on correct position, return to the default position 
        transform.position = defaultPosition;
        correctPosition.SetActive(false);
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
        if (collider.name == correctPosition.name)
        {
            onPosition = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name == correctPosition.name)
        {
            onPosition = false;
        }
    }
}