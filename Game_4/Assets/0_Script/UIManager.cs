using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;

public class UIManager : MonoBehaviour
{
    public static Action StartGame;             // player started the game
    public static Action OnReset;
    public static Action ShowStationInfo;
    public static Action hideStationInfo;
    public static Action ListChecked;
    public static Action ResetWorkers;
    public static Action wrongTaskRead;

    public static UIManager instance;

    public GameObject checkButton;

    public GameObject taskPanel;
    public GameObject resetButton;
    [SerializeField] public GameObject handPosePopup;
    [SerializeField] public GameObject inGamePanelHolder;
    [SerializeField] public GameObject wrongTaskCompletePanel;


    [SerializeField] GameObject defectCanvas;
    [SerializeField] GameObject restApiPanel;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] GameObject defectOneCheck;

    [Header ("Welcome Panel")]
    public GameObject gameIntroductionPanel;
    public GameObject timeToFindFirstDefect;
    public GameObject orderDefectPanel;
    public GameObject healthDefectPanel;
    public GameObject skillDefectPanel;

    [Header("Sucess Panel")]
    public GameObject orderSucessPanel;
    public GameObject healthSucessPanel;
    public GameObject gameComplete;
    public GameObject congratulationsPanel;
    public GameObject SkillFailedPanel;
    public GameObject outOfTimeSkillPanel;
    public Text SkillFailedText;

    [Header("Order Defect")]
    [SerializeField] GameObject wrongTaskListPanel;

    [Header("Station Information Panel")]
    public GameObject slowWorkerPanel;
    public GameObject stationInfnoPanel;
    public GameObject stationInfoButtons;
    public Text firstSkillText;
    public Text secondSkillText;
    public Text stationNameText;
    public string[] fristSkillAnswer;
    public string[] secondSkillAnswer;

    [Header("Worker Information Panel")]
    public GameObject workerInfoPanel;
    public Text firstSkillWorkerText;
    public Text secondSkillWorkerText;
    //public Text workerFailedText;

    public enum Skills
    {
        first,
        second,
        Both,
    }
    public static Skills skills;

    public Text demoText;

    public WaitForSeconds waitingTime = new WaitForSeconds(1);

    [DllImport("__Internal")]
    private static extern float GetOrientation();

    private void Awake()
    {
        Application.targetFrameRate = 60;
        instance = this;
    }

    private void OnEnable()
    {
        Boxes.SlowWorker += SlowWorkerPanel;
        Item.wrongTaskComplete += WrongTaskComplete;
        // show game over panel when player is not able to solve defect one
        Timer.TimeOut += ShowGameOverPanel;
        DefectOneCheck.TimeOut += ShowGameOverPanel;

        // if the game is comeplted
        DefectManager.GameCompleted += GamecComplete;
        // to show the worker skills 
        Worker.ShowWorkerSkills += ShowWorkerSkill;
        Item.orderTaskComplete += TaskOrderDone;            // if the order task list is done 
        DefectManager.healthTaskDone += HealthTaskDone;     // if the health task is done 
        DefectManager.OrderTaskDone += OrderTaskDone;       // if the order task is completly done

        StartCoroutine(nameof(CheckOrientation));
    }

    private void OnDisable()
    {
        Boxes.SlowWorker -= SlowWorkerPanel;
        Item.wrongTaskComplete -= WrongTaskComplete;
        Timer.TimeOut -= ShowGameOverPanel;
        DefectOneCheck.TimeOut -= ShowGameOverPanel;
        DefectManager.GameCompleted -= GamecComplete;
        Worker.ShowWorkerSkills -= ShowWorkerSkill;
        Item.orderTaskComplete -= TaskOrderDone;
        DefectManager.healthTaskDone -= HealthTaskDone;
        DefectManager.OrderTaskDone -= OrderTaskDone;
    }

    // change panel as desire
    public void ChangePanel (GameObject activePanel)
    {
        demoText.text += "Phase1\n";
        ShutAllPanel();
        activePanel.SetActive(true);
        demoText.text += "Phase2\n";
    }

    // shut all panels in-game
    public void ShutAllPanel ()
    {
        wrongTaskCompletePanel.SetActive(false); 
        slowWorkerPanel.SetActive(false);
        // welcome panel
        gameIntroductionPanel.SetActive(false);
        timeToFindFirstDefect.SetActive(false);
        orderDefectPanel.SetActive(false);
        healthDefectPanel.SetActive(false);
        skillDefectPanel.SetActive(false);

        // defect 1
        wrongTaskListPanel.SetActive(false);

        // defect 3
        SkillFailedPanel.SetActive(false);
        outOfTimeSkillPanel.SetActive(false);

        // sucess panel
        orderSucessPanel.SetActive(false);
        healthSucessPanel.SetActive(false);
        gameComplete.SetActive(false);
        congratulationsPanel.SetActive(false);

        // worker information panel
        stationInfnoPanel.SetActive(false);
        workerInfoPanel.SetActive(false);

        // other panel
        gameOverPanel.SetActive(false);
    }

    // okay button basic function
    public void _OkayButton ()
    {
        print("Okay Btn");
        ShutAllPanel();
    }

    public void _IntroductionOkayButton ()
    {
        ShutAllPanel();
        defectCanvas.SetActive(true);
    }

    public void _OrderDefectButton ()
    {
        ChangePanel(wrongTaskListPanel);
    }

    public void _RearrangePanel ()
    {
        taskPanel.SetActive(true);
    }

    public void __RestButton()
    {
        OnReset?.Invoke();
    }

    void OrderTaskDone ()
    {
        StartCoroutine(nameof(TaskDoneFunction));
    }

    IEnumerator TaskDoneFunction ()
    {
        resetButton.SetActive(false);
        yield return waitingTime;
        taskPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            _RestartGame();
    }

    // when player completes the order task
    void TaskOrderDone()
    {
        Invoke("InvokeSuccessPanel", 1);
    }

    void InvokeSuccessPanel ()
    {
        ChangePanel(orderSucessPanel);
    }

    // when player completes the health task
    void HealthTaskDone()
        => ChangePanel(healthSucessPanel);

    // show information according to which station player has clicked 
    public void __ShowStationInfoPanel (int count)
    {
        // the game is showing station information 
        ShowStationInfo?.Invoke();
        // get information ready in all text
        stationNameText.text = $"Station {count}";
        firstSkillText.text = fristSkillAnswer[count].ToString();
        secondSkillText.text = secondSkillAnswer[count].ToString();
        // show the panel
        ChangePanel(stationInfnoPanel);
    }

    // change the UI according to which worker player has clicked 
    void ShowWorkerSkill ()
    {
        stationInfoButtons.SetActive(false);
        if (skills == Skills.first)
        {
            firstSkillWorkerText.text = "YES";
            secondSkillWorkerText.text = "NO";
        }
        else if (skills == Skills.second)
        {
            firstSkillWorkerText.text = "NO";
            secondSkillWorkerText.text = "YES";
        }
        else if (skills == Skills.Both)
        {
            firstSkillWorkerText.text = "YES";
            secondSkillWorkerText.text = "YES";
        }
        ChangePanel(workerInfoPanel);
    }

    public void _HideStationInformation()
        => hideStationInfo?.Invoke();

    void GamecComplete ()
    {
        PostFunction.instance.PostData();
        defectCanvas.SetActive(false);
        ChangePanel(gameComplete);
    }

    public void _SkillDefectOkay ()
    {
        // show congratulations panel
        ChangePanel(congratulationsPanel);
    }

    // restart the game 
    public void _RestartGame ()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex * 0);
    }

    IEnumerator CheckOrientation()
    {
        while (true)
        {
            // width < height which is portrait 
            if (GetOrientation() < 1f)
            {
                restApiPanel.SetActive(true);
                if (handPosePopup.activeInHierarchy)
                {
                    handPosePopup.SetActive(false);
                    inGamePanelHolder.SetActive(true);
                }
            }
            // width > height which is landscape
            else if (GetOrientation() > 1f)
            {
                restApiPanel.SetActive(false);
                if (!handPosePopup.activeInHierarchy)
                {
                    handPosePopup.SetActive(true);          // hand pose panel will be on
                    inGamePanelHolder.SetActive(false);     // all other panel will be disable 
                }
            }
            yield return waitingTime;
        }
    }

    public void _StartButton ()
    {
        StartGame?.Invoke();
    }

    bool entredSkillDefect = false;

    public void _EntredSkillDefect()
        => entredSkillDefect = true;

    void ShowGameOverPanel()
    {
        if (entredSkillDefect)
        {
            ChangePanel(outOfTimeSkillPanel);
            if (checkButton.activeInHierarchy)
                checkButton.SetActive(false);
        }
        else if (!entredSkillDefect || DefectManager.instnace.defects == DefectManager.Defects.taskOrderIncorrect)
        {
            defectCanvas.SetActive(false);
            ChangePanel(gameOverPanel);
        }
    }

    public void _DeleteDefectOneCheck()
        => Destroy(defectOneCheck);

    public void CheckedList()
        => ListChecked?.Invoke();

    void WrongTaskComplete ()
    {
        ChangePanel(wrongTaskCompletePanel);
    }

    void SlowWorkerPanel()
        => ChangePanel(slowWorkerPanel);

    [SerializeField] GameObject clickableBox;
    [SerializeField] GameObject dragAndDropBox;

    public void _ChangeBoxes()
    {
        clickableBox.SetActive(false);
        dragAndDropBox.SetActive(true);
    }

    public void CheckWorkers()
    {
        checkButton.SetActive(true);
    }

    public void _ResetWorkers ()
    {
        print("reset");
        ResetWorkers?.Invoke();
    }

    public void _HideObject(GameObject obj)
    {
        obj.SetActive(false);
    }

    public void _ShowObject(GameObject obj)
    {
        obj.SetActive(true);
    }

    public void _WrongTaskRead()
        => wrongTaskRead?.Invoke();

    public void _TurnOnCollider(string colliderName)
    {
        Collider2D collider = GameObject.Find(colliderName).GetComponent<Collider2D>();
        collider.enabled = true;
    }
}   