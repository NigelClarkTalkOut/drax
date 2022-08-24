using System;
using UnityEngine;
using System.Collections;

public class CameraFocus : MonoBehaviour
{
    Camera mainCamera;
    public static Action EnterSkillTask;            // when player enters skill task
    [Header ("Camera Inputs")]
    [SerializeField] float cameraFocusSpeed;
    [SerializeField] float cameraPanSpeed;

    [SerializeField] bool isFocusing = false;

    [Header ("User Interface")]
    [SerializeField] GameObject defectCanvas;        // health bar for worker
    [SerializeField] Transform workerHealthBar;     // health bar for worker
    [SerializeField] GameObject workerHealthHolder; 
    [SerializeField] GameObject rearrangingCanvas;  // first task canvas for arranging tasks 

    [Header ("Position For Defects")]
    [SerializeField] Transform defaultPosition;     // default position for camera
    [SerializeField] Transform orderDefact;         // order defect position 
    [SerializeField] Transform healthDefact;        // health defect position

    Vector2 lowHealth = new Vector2(0.4f, 1);
    Vector2 normalHealth = new Vector2(1, 1);


    #region Order Task Complete
    private void OnEnable()
    {
        Item.orderTaskComplete += _ZoomOut;
        DefectManager.healthTaskDone += _ZoomOut;
    }

    private void OnDisable()
    {
        Item.orderTaskComplete -= _ZoomOut;
        DefectManager.healthTaskDone -= _ZoomOut;
    }
    #endregion

    private void Start()
    {
        // hide worker's health when game starts
        workerHealthHolder.SetActive(false);
        mainCamera = GetComponent<Camera>();
        // for arranging the canvas for entring the defects 
        rearrangingCanvas.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            _ZoomOut();
    }

    // for zooming-in in the order defect
    public void _ZoomToOrderDefect()
    {
        if (isFocusing)
            return;
        UIManager.instance.ChangePanel(UIManager.instance.orderDefectPanel);
        StartCoroutine(FocusOnDefect(orderDefact.position, 5));
        workerHealthHolder.SetActive(true);
    }

    // for zooming-in in the health defect 
    public void _ZoomToHealthDefect()
    {
        if (isFocusing)
            return;
        UIManager.instance.ChangePanel(UIManager.instance.healthDefectPanel);
        StartCoroutine(FocusOnDefect(healthDefact.position, 7));
        // show the health bar for worker and lower the health 
        workerHealthHolder.SetActive(true);
        StartCoroutine(nameof(LowHealthAnimation));
    }

    IEnumerator LowHealthAnimation()
    {
        print("Started ");
        while (true)
        {
            workerHealthBar.localScale = Vector2.Lerp(workerHealthBar.localScale, lowHealth, 0.01f);
            yield return null; 
        }
    }

    // for zooming-in the skill defect 
    public void _ZoomToSkillDefect()
    {
        if (isFocusing)
            return;
        EnterSkillTask?.Invoke();           // event for entring into skil defect 
        UIManager.instance.ChangePanel(UIManager.instance.skillDefectPanel);
    }

    // for zooming-out of the defects after solving the defect 
    public void _ZoomOut()
    {
        if (isFocusing)
            return;
        StartCoroutine(FocusOnDefect(defaultPosition.position, 16.5f));
        // hide the health bar for worker and turn that to normal
        workerHealthHolder.SetActive(false);
    }

    // camera function for zomming in and out
    IEnumerator FocusOnDefect (Vector3 posToFocus, float zoomEffect)
    {
        isFocusing = true;
        while (transform.position != posToFocus)
        {
            defectCanvas.SetActive(false);      // hide all the defect canvas
            transform.position = Vector3.Lerp(transform.position, posToFocus, cameraPanSpeed * Time.deltaTime);
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, zoomEffect, cameraFocusSpeed * Time.deltaTime);
            yield return null;
        }
        isFocusing = false;
    }

    public void _ShowDefectCanvas()
        => defectCanvas.SetActive(true);
}