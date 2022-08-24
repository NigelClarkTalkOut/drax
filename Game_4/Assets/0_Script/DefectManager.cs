using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DefectManager : MonoBehaviour
{
    public static DefectManager instnace;
    public int boxCount = 0;

    bool enteredSkillDefect = false;

    [Header("Tug Animation")]
    [SerializeField] float beltSpeed = 0.3f;
    [SerializeField] Animator tugAnimation;                 // tug movement animation
    [SerializeField] GameObject box;                        // box for loadig on tug
    [SerializeField] GameObject binBox;                     // box going into the bin after completing the loop
    [SerializeField] GameObject boxHolder;                  // box for loadig on tug
    WaitForSeconds waitingTime = new WaitForSeconds(2);         // waiting time for box to load
    WaitForSeconds boxHidingTime = new WaitForSeconds(2);       // hiding the box when it goes out of the screen
    WaitForSeconds tugAnimationTimeLeft = new WaitForSeconds(2);    // wait for tug to complete the animation

    public enum Defects
    {
        taskOrderIncorrect,
        taskOrder,
        heathTask,
        skillTask,
        completed,
    }
    public Defects defects;
    public Text defectCount;
    [SerializeField] GameObject checkButtonHolder;

    [SerializeField] Transform conveyorBelt;
    [Header ("Task One")]
    [SerializeField] TaskFunction[] taskOneCompleteCheck;
    public static Action WorkerArranged;
    public static Action GameCompleted;     
    public static Action taskDone;     
    public static Action skillDefectFailed;     

    [Header("Task Two")]
    [SerializeField] Collider2D boxCollider1;
    [SerializeField] Collider2D boxCollider2;

    [Header ("Task Three")]
    [SerializeField] GameObject stationButtons;

    [Header("Task Two Robots")]
    [SerializeField] GameObject[] taskTwoRobotsToActive;
    [SerializeField] GameObject[] taskTwoRobotosToDeactive;
    
    [Header("Task Three Robots")]
    [SerializeField] GameObject[] taskThreeRobotsToActive;
    [SerializeField] GameObject[] taskThreeRobotosToDeactive;
    
    [Header("All Task Complete Robots")]
    [SerializeField] GameObject finalRobotAcitve;
    [SerializeField] GameObject finalRobotDeactive;

    [Header("Workers Information")]
    [SerializeField] GameObject animatedWorker;
    [SerializeField] GameObject normalWorkers;
    public Transform[] workers;
    public Transform[] workersPosition;
    public int workerCount = 0;
    public int totalWorkerPlace = 0;

    [Header ("Buttons For Entring Defects")]
    // buttons for solving the defect 
    public GameObject taskOrderButtons;
    public GameObject healthTaskButton;
    public GameObject skillTaskButton;

    public int packageCount = 0;                // how many package are placed correctly 

    public static Action OrderTaskDone;
    public static Action healthTaskDone;

    [SerializeField] Collider2D[] stationCollider;
    [SerializeField] Transform[] workersNewPosition, workersNewPosition1, currentWorkersPosition;

    public int showButton = 8;

    private void Awake()
    {
        instnace = this;
    }

    private void Start()
    {
        
        _StopBelt();     
        StartCoroutine(nameof(moveConveyor));
        // disable all station buttons
        stationButtons.SetActive(false);
        // the game will start with task order defect
        //defects = Defects.taskOrder;
        UpdateButtons();

        // turn the collider off
        boxCollider1.enabled = false;
        boxCollider2.enabled = false;
    }

    public void _DeleteTheAnimatedWorker ()
    {
        animatedWorker.SetActive(false);
        normalWorkers.SetActive(true);
    }

    #region Observe Events
    private void OnEnable()
    {
        UIManager.ResetWorkers += WorkerReset;
        Timer.TimeOut += _StopBelt;
        // stop the belt when player is not able to solve defect one in time
        DefectOneCheck.TimeOut += _StopBelt;    
        UIManager.StartGame += StartBelt;       // start the belt after introduction panel
        // when robot is completed
        RobotSpawner.robotCompleted += _StartBoxAnimation;
        // if the worker is placed
        Worker.workerPlaced += CheckWorkerPlaced;
        Worker.workerPlacedIncorrect += WrongWorkerPlaced;
        // arrange the workers as player enter skill defect 
        CameraFocus.EnterSkillTask += ArrangeWorkers;
        Item.orderTaskComplete += TaskOrderDone;
        Item.packagePlaced += OnPackagePlaced;          // when package are placed in health defect 
    }

    private void OnDisable()
    {
        UIManager.ResetWorkers -= WorkerReset;
        Timer.TimeOut -= _StopBelt;
        // stop the belt when player is not able to solve defect one in time
        DefectOneCheck.TimeOut -= _StopBelt;
        UIManager.StartGame -= StartBelt;       // start the belt after introduction panel
        // when robot is completed
        RobotSpawner.robotCompleted -= _StartBoxAnimation;
        // if the worker is placed
        Worker.workerPlaced -= CheckWorkerPlaced;
        Worker.workerPlacedIncorrect -= WrongWorkerPlaced;
        // arrange the workers as player enter skill defect 
        CameraFocus.EnterSkillTask -= ArrangeWorkers;
        Item.orderTaskComplete -= TaskOrderDone;
        Item.packagePlaced -= OnPackagePlaced;          // when package are placed in health defect 
    }
    #endregion

    // change buttons according to the defect place is solving 
    public void UpdateButtons()
    {
        CloseAllButtons();
        if (defects == Defects.taskOrderIncorrect)
        {
            defectCount.text = $"3";
            taskOrderButtons.SetActive(true);
        }

        else if (defects == Defects.taskOrder)
        {
            defectCount.text = $"3";
            taskOrderButtons.SetActive(true);
        }

        else if (defects == Defects.heathTask)
        {
            defectCount.text = $"2";
            healthTaskButton.SetActive(true);
        }

        else if (defects == Defects.skillTask)
        {
            defectCount.text = $"1";
            skillTaskButton.SetActive(true);
        }
        else
        {
            defectCount.text = $"0";
        }
    }

    IEnumerator moveConveyor()
    {
        while (true)
        {
            conveyorBelt.position = new Vector3(0, conveyorBelt.position.y + beltSpeed * Time.deltaTime, 0);
            yield return null;
        }
    }

    public void _StopBelt ()
    {
        if (defects == Defects.skillTask) return;
        beltSpeed = 0;
    }

    public void StartBelt ()
    {
        beltSpeed = 0.2f;
    }

    #region Close All Buttons
    public void CloseAllButtons ()
    {
        taskOrderButtons.SetActive(false);
        healthTaskButton.SetActive(false);
        skillTaskButton.SetActive(false);
    }
    #endregion

    public void CheckTaskOne ()
    {
        // check if all the task are completed or not 
        for (int i = 0; i < taskOneCompleteCheck.Length; i++)
        {
            if (taskOneCompleteCheck[i].correctItem != true)
                return;    
        }
        // if all the task are comepleted
        OrderTaskDone?.Invoke();
    }

    // changing the defect on completing the order task 
    void TaskOrderDone()
    {
        StartBelt();                // start the belt which is stopped after entring defect one 
        defects = Defects.heathTask;
        taskDone?.Invoke();         // for updating robots when task is done
        UpdateButtons();
    }

    // if packages are placed | health defect
    void OnPackagePlaced ()
    {
        if (packageCount == 2)
        {

            Timer.timerInstance.StartCounting();
            StartBelt();
            Invoke(nameof(AfterCompletingHealthTask), 1);
            defects = Defects.skillTask;    // change to the next defect 
            UpdateButtons();        // update the buttons for solving the defects 
        }
    }

    void AfterCompletingHealthTask ()
    {
        healthTaskDone?.Invoke();
    }

    void ArrangeWorkers ()
    {
        for (int i = 0; i < workers.Length; i++)
        {
            // change the postion for workers 
            workers[i].position = workersPosition[i].position;
            workers[i].transform.localScale = new Vector3(1, 1, 1);
        }
        WorkerArranged?.Invoke();
    }

    public void _MakeWokrersClickable ()
    {
        for (int i = 0; i < workers.Length; i++)
        {
            // workers can be dragged 
            workers[i].GetComponent<BoxCollider2D>().enabled = true;
        }

        // enable all the station 
        stationButtons.SetActive(true);
    }

    void CheckWorkerPlaced()
    {
        // add count to the workers 
        print("Worker Placed");
        workerCount++;
        AllWorkersArePlaced();
    }

    void AllWorkersArePlaced ()
    {
        Debug.Log("<color=cryn> Worker count </color>" + totalWorkerPlace);
        if (totalWorkerPlace == 7)
        {
            checkButtonHolder.SetActive(true);
            UIManager.instance.checkButton.SetActive(false);
            showButton = 9;
        }
        if (totalWorkerPlace == showButton)
        {
            UIManager.instance.CheckWorkers();
        }
        if (totalWorkerPlace == 7)
        {
            showButton = 8;
        }
    }
    
    void WrongWorkerPlaced()
    {
        print("Wrong Worker Placed");
        AllWorkersArePlaced();
    }

    public void _CheckWorkers ()
    {
        Timer.timerInstance._StopConting();
        // if user is able to set all the workers on correct position 
        if (workerCount == 8)
        {
            tugAnimation.enabled = false;
            defects = Defects.completed;
            defectCount.text = $"0";
            GameCompleted?.Invoke();
            _StopBelt();            // stop robots moving when game is completed 
        }

        // if user is not able to set all the workers 
        else if (workerCount != 8)
        {
            StartCoroutine(ChangeWorker());
        }
    }

    public static Action WrongWorkerAction;
    public static Action WrokerBackToNormal;

    IEnumerator ChangeWorker()
    {
        UIManager.instance.ShutAllPanel();
        WrongWorkerAction?.Invoke();
        yield return waitingTime;
        yield return waitingTime;
        WrokerBackToNormal?.Invoke();
        ChangeWorkersPosition();
        defectCount.text = $"1";
        UIManager.instance.ChangePanel(UIManager.instance.SkillFailedPanel);
        UIManager.instance.SkillFailedText.text = $"YOUR SCORE: {workerCount} / 8 \nPlease try again to assign all operatives to the correct stations to complete the puzzle!";
    }

    public void _DefectTwoCollider ()
    {
        boxCollider1.enabled = true;
        boxCollider2.enabled = true;
    }

    void StartTugAnimation()
        => StartCoroutine(nameof(TugAnimation));

    public void _StartBoxAnimation()
        => BinAnimation();

    // making box and tug move after completing one iteration
    IEnumerator TugAnimation()
    {
        box.gameObject.SetActive(true);
        yield return waitingTime;
        tugAnimation.SetBool("Robot Completed", true);
        // make box follow the tug 
        box.transform.position = boxHolder.transform.position;

        yield return boxHidingTime;
        box.SetActive(false);
        // wait for tug to complete the animation

        yield return tugAnimationTimeLeft;
        tugAnimation.SetBool("Robot Completed", false);
    }

    void BinAnimation ()
    {
        binBox.gameObject.SetActive(false);
        binBox.gameObject.SetActive(true);
    }

    void WorkerReset()
    {
        
        workerCount = 0;
        totalWorkerPlace = 0;
        for (int i = 0; i < stationCollider.Length; i++)
        {
            stationCollider[i].enabled = true;
        }
        ArrangeWorkers();
    }

    void ChangeWorkersPosition ()
    {
        int j = UnityEngine.Random.Range(0, 2);
        if (j == 0)
        {
            for (int i = 0; i < currentWorkersPosition.Length; i++)
            {
                currentWorkersPosition[i].position = workersNewPosition[i].position;
            }
        }

        else if (j == 1)
        {
            for (int i = 0; i < currentWorkersPosition.Length; i++)
            {
                currentWorkersPosition[i].position = workersNewPosition1[i].position;
            }
        }
    }

    public void _EntredSkilDefect()
        => enteredSkillDefect = true;
}