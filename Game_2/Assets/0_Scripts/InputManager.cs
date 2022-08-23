using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    GameManager gameManager;

    [SerializeField] Camera mainCamera;
    [SerializeField] Transform crossHair;
    public GameObject inputArea;
    public Button start_Button;
    public Button clearButton;
    public Button editButton;

    public float idelTime = 0;
    [SerializeField] bool stopCounting = false;
    [SerializeField] bool timeStop = false;
    [SerializeField] LayerMask inputLayer;
    public GameObject newTrack;

    public bool canDraw = false;

    //private void Awake()
    //{
    //    // instance to game manager
    //    gameManager = GameManager.instance;
    //}

    private void OnEnable()
    {
        GameManager.startTheTug += StopCounting;
    }

    private void OnDisable()
    {
        GameManager.startTheTug -= StopCounting;
    }

    private void Update()
    {
        if (GameManager.instance.isEditing || UIManager.instance.pannelOn)
        {
            inputArea.SetActive(false);
            return;
        }
        else if (!GameManager.instance.isEditing && !UIManager.instance.pannelOn)
            inputArea.SetActive(true);

        if (Input.GetMouseButton(0))
        {
            if (idelTime < 10)
                idelTime = 0;

            // if the user is touching user interface, player cannot draw
            if (Physics2D.Raycast(GetTouchPosition(), Vector3.forward, 100, inputLayer))
                canDraw = true;
            else
                canDraw = false;
        }
        else
        {
            //if (!stopCounting)
            //{
            //    idelTime += Time.deltaTime;
            //    if (idelTime >= 10)
            //    {
            //        start_Button.gameObject.SetActive(false);
            //        clearButton.interactable = false;
            //        editButton.interactable = false;
            //        newTrack.SetActive(false);
            //        GameManager.instance.trackSystem.SetActive(false);
            //        UIManager.instance.idelPanel.SetActive(true);
            //    }
            //}
        }
        // if player is not tapping on the user interface, player can draw the path
        if (canDraw && Input.GetMouseButton(0))
            crossHair.position = GetTouchPosition();
    }

    // get the mouse points 
    Vector3 GetTouchPosition()
    {
        // get the position of mouse 
        var mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        return mousePos;
    }

    void StopCounting()
        => stopCounting = true;
}
