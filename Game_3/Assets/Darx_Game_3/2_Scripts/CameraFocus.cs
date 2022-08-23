using UnityEngine;
using System.Collections;

public class CameraFocus : MonoBehaviour
{
    Camera mainCamera;
    [SerializeField] GameObject buttonCanvas;

    [Header("Camera Inputs")]
    [SerializeField] float cameraFocusSpeed;
    [SerializeField] float cameraPanSpeed;

    [SerializeField] bool isFocusing = false;

    [Header("Station Position")]
    [SerializeField] Transform defaultPosition;     // zoom out position 
    [SerializeField] Transform changePosition;     // zoom out position 
    [SerializeField] Transform station1;           
    [SerializeField] Transform station2;
    [SerializeField] Transform station3;
    [SerializeField] Transform A_station1;
    [SerializeField] Transform A_station2;
    [SerializeField] Transform A_station3;


    private void Start()
    {
        mainCamera = GetComponent<Camera>();
    }

    private void OnEnable()
    {
        // zoom out when station 1 is done 
        Timer.ChangeStationNumber += _ZoomOut;
        UiManager.A_StationComplete += _ZoomOut;

        UiManager.ZoomOutForChangs += _ZoomOutChanges;
    }

    private void OnDisable()
    {
        Timer.ChangeStationNumber -= _ZoomOut;
        UiManager.A_StationComplete -= _ZoomOut;

        UiManager.ZoomOutForChangs -= _ZoomOutChanges;
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            _ZoomOut(0);
    }

    // will zoom on station one
    public void _ZoomOnStationOne ()
    {
        if (isFocusing)
            return;
        StartCoroutine(FocusOnStation(station1.position, 6.6f));
        buttonCanvas.SetActive(false);
    }
    
    // will zoom on station one
    public void _ZoomOnStationOne_A ()
    {
        if (isFocusing)
            return;
        StartCoroutine(FocusOnStation(A_station1.position, 6.6f));
        buttonCanvas.SetActive(false);

    }

    // will zoom on station two
    public void _ZoomOnStationTwo ()
    {
        if (isFocusing)
            return;
        StartCoroutine(FocusOnStation(station2.position, 6.6f));
        buttonCanvas.SetActive(false);
    }
    
    // will zoom on station two
    public void _ZoomOnStationTwo_A ()
    {
        if (isFocusing)
            return;
        StartCoroutine(FocusOnStation(A_station2.position, 6.6f));
        buttonCanvas.SetActive(false);
    }

    // will zoom on statio three
    public void _ZoomOnStationThree ()
    {
        if (isFocusing)
            return;
        StartCoroutine(FocusOnStation(station3.position, 6.6f));
        buttonCanvas.SetActive(false);
    }
    
    // will zoom on statio three
    public void _ZoomOnStationThree_A ()
    {
        if (isFocusing)
            return;
        StartCoroutine(FocusOnStation(A_station3.position, 6.6f));
        buttonCanvas.SetActive(false);
    }

    // for zooming-out after making an product 
    public void _ZoomOut(float i)
    {
        Debug.Log("Zooming out");
        if (isFocusing)
            return;
        buttonCanvas.SetActive(true);
        StartCoroutine(FocusOnStation(defaultPosition.position, 18));
    }

    // for zooming-out after making an product 
    public void _ZoomOutChanges()
    {
        Debug.Log("Zooming out Change");
        StartCoroutine(FocusOnStation(changePosition.position, 20));
    }

    // camera function for zomming in and out
    IEnumerator FocusOnStation (Vector3 posToFocus, float zoomEffect)
    {
        isFocusing = true;
        while (transform.position != posToFocus)
        {
            transform.position = Vector3.Lerp(transform.position, posToFocus, cameraPanSpeed * Time.deltaTime);
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, zoomEffect, cameraFocusSpeed * Time.deltaTime);
            yield return null;
        }
        isFocusing = false;
    }
}