using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;

public class UIManager : MonoBehaviour
{
    [SerializeField] InputManager inputInstance;
    public ResultSystem resultSystem;
    public static UIManager instance;
    public Timer timeInstance;
    [SerializeField] GameObject handAnimation;
    public bool pannelOn = false;
    public bool isLastPhase = false;
    WaitForSeconds oneSec = new WaitForSeconds(1);

    [Header ("IN GAME PANNELS")]
    public GameObject wonPannel;                        // player player wins the phase 
    public GameObject losePannel;                       // when playe losses the phase
    public GameObject OutOfTime;                        // when player runs out of time 
    public GameObject doNotDeleteTip;                   // when player deletes the working racks 
    public GameObject doNotDeleteSubAssembly;           // when player deletes the working racks 
    public GameObject firstTimeEditModePannel;          // when player get in the edit mode for the first time 
    public GameObject CongratulationPannel;             // when player win all the phases
    public GameObject startingTugTip;                   // when player starts the tug for the first time 
    public GameObject handPosePopup;                    // when player enters into landscape mode 
    public GameObject removeRackings;                   // player need to remove racking from racking area 
    public GameObject correctPathIndicationPanel;       // when player enters into landscape mode 
    public GameObject needToEnterEditMode;              // player first need to enter edit mode before playing further
    public GameObject firstTimePathPoint;               // when player clicks point first time 
    public GameObject timePanel;                        // when player enters into landscape mode 
    public GameObject idelPanel;                        // if player is idel for more than 10 seconds 
    public GameObject threeTimeLosePanel;                // after all changes are done

    // text changes
    public GameObject station1TimePanel;                // after all changes are done
    public GameObject station2TimePanel;                // after all changes are done
    public GameObject loadBalancing1TimePanel;                // after all changes are done
    public GameObject loadBalancing2TimePanel;                // after all changes are done
    public GameObject successPanelNormal;                // after all changes are done
    public GameObject successPanelImprovement;                // after all changes are done
    public GameObject successPanelLoadbalancing1;                // after all changes are done
    public GameObject successPanelLoadbalancing1green;                // after all changes are done
    public GameObject successPanelLoadbalancing2;                // after all changes are done

    [Header("STARTING PANNELS")]
    public GameObject introductionPannel;                   // at the starting of the game
    [SerializeField] GameObject introNextButton;            // cycle to the next tip
    [SerializeField] GameObject editModeNextButton;            // cycle to the next tip
    [SerializeField] public GameObject normalPannel;
    [SerializeField] public GameObject improvementPannel;
    [SerializeField] public GameObject loadBalancingPhaseOnePannel;
    [SerializeField] public GameObject loadBalancingPhaseTwoPannel;
    [SerializeField] public GameObject loadBalancingPhaseThreePannel;
    [SerializeField] public GameObject loadBalancingPhaseFourPannel;

    [Header("INTRO PANNEL TEXT")]
    int count;
    int editCount;
    [TextArea]
    [SerializeField] string firstTextIntro;
    [TextArea]
    [SerializeField] string secondTextIntro;
    [TextArea]
    [SerializeField] string thirdTextIntro;

    [TextArea]
    [SerializeField] string firstEditText;
    [TextArea]
    [SerializeField] string secondEditText;

    [DllImport("__Internal")]
    private static extern float GetOrientation();

    private void Awake() =>
        // set the instance 
        instance = this;

    private void Start()
    {
        if (SelectSeed.instance.phases == SelectSeed.Phases.loadBalancingPhaseFour)
            isLastPhase = true;
        
        if (!SelectSeed.instance.welcomePannelShown)
        {
            pannelOn = true;
            handAnimation.SetActive(true);

            // change the pannel according to the pannel needed
            if (SelectSeed.instance.phases == SelectSeed.Phases.normalPhase)
                normalPannel.SetActive(true);                   // if normal phase

            else if (SelectSeed.instance.phases == SelectSeed.Phases.improvementPhase)
                improvementPannel.SetActive(true);              // if improvement phase

            else if (SelectSeed.instance.phases == SelectSeed.Phases.loadBalancingPhaseOne)
                loadBalancingPhaseOnePannel.SetActive(true);

            else if (SelectSeed.instance.phases == SelectSeed.Phases.loadBalancingPhaseTwo)
                loadBalancingPhaseTwoPannel.SetActive(true);

            else if (SelectSeed.instance.phases == SelectSeed.Phases.loadBalancingPhaseThree)
                loadBalancingPhaseThreePannel.SetActive(true);

            else if (SelectSeed.instance.phases == SelectSeed.Phases.loadBalancingPhaseFour)
            {
                loadBalancingPhaseFourPannel.SetActive(true);
                isLastPhase = true;
            }
            SelectSeed.instance.welcomePannelShown = true;
        }
    }

    private void OnEnable()
    {
        StartCoroutine(nameof(CheckOrientation));
    }


    // will change pannel according to the situation
    public void ChangePannel (GameObject activePannel)
    {
        pannelOn = true;
        // all the pannels will be off
        ShutAllPannels();
        // get the pannel you want to active and set that true
        activePannel.SetActive(true);
    }

    public void __OkayButton()
    {
        pannelOn = false;
        if (handAnimation.activeInHierarchy)
            handAnimation.SetActive(false);
        ShutAllPannels();
    }

    public void __LoseOkayButton ()
    {
        pannelOn = false;
        if (handAnimation.activeInHierarchy)
            handAnimation.SetActive(false);
        ShutAllPannels();
        if (SelectSeed.instance.tries <= 2)
        {
            RestartScene();
        }
        else
        {
            SelectSeed.instance.tries = 0;
            correctPathIndicationPanel.SetActive(true);
            resultSystem.ShowOptimalPath();
            if (!isLastPhase)
            {
                print("Not last phase");
                Invoke(nameof(RestartScene), 4);
            }
            // if it's the last pahse of the game
            else if (isLastPhase)
                Invoke(nameof(showLastPanel), 4);
        }   
    }

    void showLastPanel ()
        => ChangePannel(CongratulationPannel);
        

    void RestartScene ()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void __StopCountingOkay()
    {
        inputInstance.idelTime = 0;
        inputInstance.start_Button.gameObject.SetActive(true);
        inputInstance.clearButton.interactable = true;
        if (SelectSeed.instance.phases != SelectSeed.Phases.normalPhase)
            inputInstance.editButton.interactable = true;
        inputInstance.newTrack.SetActive(true);
        GameManager.instance.trackSystem.SetActive(true);
        idelPanel.SetActive(false);
    }

    public void __OkayButtonImprovementPhase()
    {
        pannelOn = false;
        if (handAnimation.activeInHierarchy)
            handAnimation.SetActive(false);
        ShutAllPannels();
        ChangePannel(firstTimeEditModePannel);
    }

    public void __NextButton (Text mainText)
    {
        // change the text according to thee button 
        if (count == 0)
            mainText.text = firstTextIntro;
        // change the text according to thee button 
        else if (count == 1)
        {
            mainText.text = secondTextIntro;
            Destroy(introNextButton);
        }
        // if no text available then delete the next button for starting the game 
        count++;
    }


    public void __EditNextButton(Text mainText)
    {
        editCount++;
        // change the text according to thee button 
        if (editCount == 0)
            mainText.text = firstEditText.ToString();
        // if no text available then delete the next button for starting the game 
        else if (editCount == 1)
        {
            mainText.text = secondEditText.ToString();
            Destroy(editModeNextButton);
        }
        // increasing the counter
    }


    // turning all the pannels off
    private void ShutAllPannels ()
    {
        // turn all the pannel off
        introductionPannel.SetActive(false);
        wonPannel.SetActive(false);
        losePannel.SetActive(false);
        doNotDeleteTip.SetActive(false);
        doNotDeleteSubAssembly.SetActive(false);
        OutOfTime.SetActive(false);
        firstTimeEditModePannel.SetActive(false);
        CongratulationPannel.SetActive(false);
        startingTugTip.SetActive(false);
        removeRackings.SetActive(false);
        needToEnterEditMode.SetActive(false);
        firstTimePathPoint.SetActive(false);
        timePanel.SetActive(false);
        threeTimeLosePanel.SetActive(false);

        // text changes
        station1TimePanel.SetActive(false);
        station2TimePanel.SetActive(false);
        loadBalancing1TimePanel.SetActive(false);
        loadBalancing2TimePanel.SetActive(false);

        successPanelNormal.SetActive(false);
        successPanelImprovement.SetActive(false);
        successPanelLoadbalancing1.SetActive(false);
        successPanelLoadbalancing1green.SetActive(false);
        successPanelLoadbalancing2.SetActive(false);
    }

    public void __ShowTiming ()
    {
        ChangePannel(timePanel);
    }

    IEnumerator CheckOrientation()
    {
        while (true)
        {
            // width < height which is portrait 
            if (GetOrientation() < 1f)
            {
                if (handPosePopup.activeInHierarchy)
                    handPosePopup.SetActive(false);
            }
            // width > height which is landscape
            else if (GetOrientation() > 1f)
            {
                if (!handPosePopup.activeInHierarchy)
                    handPosePopup.SetActive(true);
            }
            yield return oneSec;
        }
    }
}