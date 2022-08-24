using System;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum TaskOrderPhase
    {
        wires,
        shoulder,
        arm,
        packageBox,
    }
    public TaskOrderPhase orderPhase;                   // what item player is moving 

    public static Action orderTaskComplete;
    public static Action wrongTaskComplete;
    public static Action packagePlaced;
    public static Action playSound;

    Camera mainCamera;
    Vector2 mousePosition;
    [SerializeField] DefectManager defectInstance;

    Vector3 defaultPosition;
    [SerializeField] GameObject correctPosition;
    [SerializeField] Collider2D nextCollider;
    [SerializeField] bool listChecked = false;          // if the player have seen the list or not 
    [SerializeField] bool listDone = false;             // if the player have solved the list or not 
    [SerializeField] bool onPosition = false;           // if the item is not correct position or not 
    [SerializeField] bool isDragging = false;           // if player dragging the item or not 
    [SerializeField] bool turnRed = false;              // make item red when player is placing on incorrect position 
    [SerializeField] bool timeOut = false;              // if player is out of time or not 

    [Header("Colors")]
    [SerializeField] Color normalColor;
    [SerializeField] Color greenColor;
    [SerializeField] Color wrongColor;                  // when player is placing on wrong position 

    [Header("Health Defect")]
    [SerializeField] Transform packageHolder;
    [SerializeField] bool boxPlaced = false;

    [Header("Defects")]
    [SerializeField] bool orderTaskItem = false;
    [SerializeField] bool healthTaskItem = false;

    [Header("Numbers")]
    [SerializeField] GameObject number1;
    [SerializeField] GameObject number2;
    [SerializeField] GameObject number3;

    private void Awake()
    {
        defectInstance = GameObject.FindGameObjectWithTag("Defect Manager").GetComponent<DefectManager>();
        mainCamera = Camera.main;
    }
    #region Observe Events 
    private void OnEnable()
    {
        UIManager.ListChecked += ListChecked;
        // when timer runs out 
        Timer.TimeOut += TimeOut;
        DefectManager.OrderTaskDone += TaskIsDone;

        UIManager.wrongTaskRead += showNumber1;
    }

    private void OnDisable()
    {
        UIManager.ListChecked -= ListChecked;
        Timer.TimeOut -= TimeOut;
        DefectManager.OrderTaskDone -= TaskIsDone;
        UIManager.wrongTaskRead -= showNumber1;

    }
    #endregion

    private void Start()
    {
        if (orderPhase == TaskOrderPhase.shoulder || orderPhase == TaskOrderPhase.arm)
        {
            correctPosition.SetActive(false);
            defaultPosition = transform.position;
        }
        else if (orderPhase == TaskOrderPhase.packageBox)
            defaultPosition = transform.position;

        //if (orderPhase == TaskOrderPhase.wires || orderPhase == TaskOrderPhase.shoulder)
        //    nextCollider.enabled = false;

        //if (defectInstance.defects == DefectManager.Defects.taskOrder)
        //{// for order defect
        //    if (orderTaskItem || healthTaskItem)
        //    {
        //        if (orderPhase == TaskOrderPhase.shoulder || orderPhase == TaskOrderPhase.arm)
        //        {
        //            correctPosition.SetActive(false);
        //            defaultPosition = transform.position;
        //        }
        //        else if (orderPhase == TaskOrderPhase.packageBox)
        //            defaultPosition = transform.position;
        //    }
        //}
    }

    private void OnMouseDown()
    {
        if (timeOut) return;
        if (defectInstance.defects == DefectManager.Defects.taskOrder)
        {
            // for order defect 
            if (orderTaskItem && listDone)
            {
                // if it's a wire will delete the wire 
                if (orderPhase == TaskOrderPhase.wires)
                {
                    playSound?.Invoke();
                    Destroy(gameObject);
                    nextCollider.enabled = true;
                    return;
                }
                // task for shoulder an arm will have drag and drop system
                else if (orderPhase == TaskOrderPhase.shoulder || orderPhase == TaskOrderPhase.arm)
                {
                    defaultPosition = transform.position;
                    correctPosition.SetActive(true);
                }
            }
        }
        else if (defectInstance.defects == DefectManager.Defects.taskOrderIncorrect)
        {
            // task for shoulder an arm will have drag and drop system
            if (orderPhase == TaskOrderPhase.shoulder || orderPhase == TaskOrderPhase.arm)
            {
                defaultPosition = transform.position;
                correctPosition.SetActive(true);
            }
        }
        else if (defectInstance.defects == DefectManager.Defects.heathTask && turnRed && !boxPlaced)
        {
            GetComponent<SpriteRenderer>().color = wrongColor;
        }
    }

    private void OnMouseDrag()
    {
        if (timeOut) return;
        // if it's task order defect 
        if (defectInstance.defects == DefectManager.Defects.taskOrder)
        {
            if (orderTaskItem)
            {
                // to move one place to another 
                if (orderPhase == TaskOrderPhase.shoulder || orderPhase == TaskOrderPhase.arm)
                {
                    // for order defect 
                    if (orderTaskItem && listDone)
                    {
                        transform.position = GetMousePosition();
                        isDragging = true;
                    }
                }
            }
        }

        // if it's health task defect
        else if (defectInstance.defects == DefectManager.Defects.heathTask)
        {
            // if its health task 
            if (healthTaskItem)
            {
                if (orderPhase == TaskOrderPhase.packageBox && !boxPlaced)
                {
                    transform.position = GetMousePosition();
                    isDragging = true;
                }
            }
        }

        else if (defectInstance.defects == DefectManager.Defects.taskOrderIncorrect)
        {
            // to move one place to another 
            if (orderPhase == TaskOrderPhase.shoulder || orderPhase == TaskOrderPhase.arm)
            {// if player have checked the list 
                if (listChecked)
                {
                    transform.position = GetMousePosition();
                    isDragging = true;
                }
            }
        }
    }

    private void OnMouseUp()
    {
        if (timeOut) return;
        // if the player is playing task order defect 
        if (defectInstance.defects == DefectManager.Defects.taskOrder)
        {
            // if the item is from task order and the task is done 
            if (orderTaskItem && listDone)
            {
                // if the phase of order is two 
                if (orderPhase == TaskOrderPhase.shoulder)
                {
                    // if the player is dragging on the correct position from /////position check
                    if (onPosition)
                    {
                        
                        playSound?.Invoke();
                        nextCollider.enabled = true;
                        transform.position = correctPosition.transform.position;
                        GetComponent<Collider2D>().enabled = false;
                        return;
                    }
                    // if not on correct position, return to the default position 
                    transform.position = defaultPosition;
                    correctPosition.SetActive(false);
                }
                // if it's the last phase
                else if (orderPhase == TaskOrderPhase.arm)
                {
                    // if the player is dragging on the correct position from /////position check
                    if (onPosition)
                    {
                        playSound?.Invoke();
                        Invoke(nameof(TaskDoneInvoke), 1);
                        transform.position = correctPosition.transform.position;
                        GetComponent<Collider2D>().enabled = false;
                        return;
                    }
                    // if not on correct position, return to the default position 
                    transform.position = defaultPosition;
                    correctPosition.SetActive(false);
                }
            }
        }

        else if (defectInstance.defects == DefectManager.Defects.taskOrderIncorrect)
        {
            if (orderPhase == TaskOrderPhase.shoulder)
            {
                // if the player is dragging on the correct position from /////position check
                if (onPosition)
                {
                    number1.SetActive(false);
                    number2.SetActive(true);
                    playSound?.Invoke();
                    nextCollider.enabled = true;        // for arm 
                    transform.position = correctPosition.transform.position;
                    GetComponent<Collider2D>().enabled = false;
                    return;
                }
                // if not on correct position, return to the default position 
                transform.position = defaultPosition;
                correctPosition.SetActive(false);
            }
            // fixing arm of the robot
            else if (orderPhase == TaskOrderPhase.arm)
            {
                // if the player is dragging on the correct position from /////position check
                if (onPosition)
                {
                    number3.SetActive(true);
                    number2.SetActive(false);
                    playSound?.Invoke();
                    nextCollider.enabled = true;        // for wires 
                    transform.position = correctPosition.transform.position;
                    GetComponent<Collider2D>().enabled = false;
                    return;
                }
                // if not on correct position, return to the default position 
                transform.position = defaultPosition;
                correctPosition.SetActive(false);
            }
            // if it's the last phase
            else if (orderPhase == TaskOrderPhase.wires)
            {
                number1.SetActive(false);
                number2.SetActive(false);
                number3.SetActive(false);
                print("Wires");
                playSound?.Invoke();
                wrongTaskComplete?.Invoke();            // event for completing the wrong task
                GetComponent<Collider2D>().enabled = false;
                defectInstance.defects = DefectManager.Defects.taskOrder;
            }
        }


        else if (defectInstance.defects == DefectManager.Defects.heathTask)
        {
            if (orderPhase == TaskOrderPhase.packageBox && !boxPlaced)
            {
                // change the color back to normal
                GetComponent<SpriteRenderer>().color = normalColor;
                // if the player is dragging on the correct position from /////position check
                if (onPosition)
                {
                    // add the count 
                    defectInstance.packageCount++;
                    // place the package on it's place
                    transform.SetParent(packageHolder);
                    transform.position = correctPosition.transform.position;
                    packagePlaced?.Invoke();                            // check for the listeners
                    playSound?.Invoke();                                // play sound
                    boxPlaced = true;       // indicate box are placed
                    return;
                }
                // if not on correct position, return to the default position 
                transform.position = defaultPosition;
            }
        }
    }

    void TaskDoneInvoke()
        => orderTaskComplete?.Invoke();

    // get the position or mouse/touch 
    Vector2 GetMousePosition()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
        // get mouse position 
        mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        return mousePosition;
    }

    #region Position Check
    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.name == correctPosition.name)
        {
            onPosition = true;
            if (!turnRed)
                return;
            if (!boxPlaced)
                GetComponent<SpriteRenderer>().color = greenColor;
            else if (boxPlaced)
                GetComponent<SpriteRenderer>().color = normalColor;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name == correctPosition.name)
        {
            onPosition = false;
            if (!turnRed)
                return;
            GetComponent<SpriteRenderer>().color = wrongColor;
        }
    }
    #endregion

    void TaskIsDone()
    {
        print("List task is done");
        listDone = true;
    }

    void TimeOut()
        => timeOut = true;

    void ListChecked()
        => listChecked = true;

    void showNumber1()
    {
        if (defectInstance.defects == DefectManager.Defects.taskOrderIncorrect)
        {
            if (number1 == null) return;
            number1.SetActive(true);
        }

    }
}