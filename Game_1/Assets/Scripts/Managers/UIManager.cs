using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Runtime.InteropServices;

public class UIManager : MonoBehaviour
{

    [SerializeField] GameObject isomatricScreen;
    [SerializeField] GameObject welcomeScreen;

    [Header("Failed UI")]
    [SerializeField] GameObject stockFellAndBrock;
    [SerializeField] GameObject failedAfterImprove;
    // [SerializeField] GameObject stockFellBelowMinimum;
    // [SerializeField] GameObject failedTimeOut;
    [SerializeField] GameObject failedImproveOne;
    [SerializeField] GameObject failedImproveTwo;

    [Header("Success UI")]
    [SerializeField] GameObject successImproveOne;
    [SerializeField] GameObject successImproveTwo;
    [SerializeField] GameObject successGameWin;
    [SerializeField] GameObject improvementOne;
    [SerializeField] GameObject improvementTwo;

    [Header("Pop-up UI")]
    [SerializeField] GameObject selectBeneathBox;
    [SerializeField] GameObject selectWrongPlace;
    [SerializeField] GameObject productionRampUp;
    [SerializeField] GameObject rackingAreaTip;
    [SerializeField] GameObject selectBoxTip;
    [SerializeField] GameObject selectPlaceTip;
    [SerializeField] GameObject orientationPopup;
    [SerializeField] GameObject wrongAnswer;
    [SerializeField] GameObject pauseGamePopup;

    [Header("Durations")]
    public float tipDuration;
    [SerializeField] float popupDelay;

    [Header("Timer Settings")]
    [SerializeField] GameObject timer;
    [SerializeField] TMP_Text timerText;
    public float curTime;
    [Tooltip("In Seconds")] [SerializeField] int totalTime = 120;
    public bool isTimerPaused = true;


    WaitForSeconds popupWait;
    WaitForSeconds tipDelay;
    WaitForSeconds oneSec = new WaitForSeconds(1f);

    [DllImport("__Internal")]
    private static extern float GetOrientation();
    
    private void Start()
    {
        tipDelay = new WaitForSeconds(tipDuration);
        popupWait = new WaitForSeconds(popupDelay);
        timerText.text = ConvertToTimeFormat(totalTime);
        ShowStartGameScreen();
#if !UNITY_EDITOR && UNITY_WEBGL
        StartCoroutine(nameof(CheckOrientation));
#endif
    }

    public void ShowStockFellAndBrock()
    {
        HidePopupsOnFail();
        StartCoroutine(nameof(ShowDelayedPopup), stockFellAndBrock);
    }

    public void HideStockFellAndBrock()
    {
        stockFellAndBrock.SetActive(false);
    }

    public void HidePopupsOnFail()
    {
        if(selectBeneathBox.activeInHierarchy)
            selectBeneathBox.SetActive(false);

        if(selectWrongPlace.activeInHierarchy)
            selectWrongPlace.SetActive(false);
    }

    public void ShowFailedAfterImprove()
    {
        HidePopupsOnFail();
        failedAfterImprove.SetActive(true);
    }

    public void ShowFailedBeforeImprove()
    {
        if(!GameManager.instance.isFirstImprovDone)
        {
            failedImproveOne.SetActive(true);
        }
        else if(!GameManager.instance.isSecondImprovDone)
        {
            failedImproveTwo.SetActive(true);
        }
    }

    public void ShowStockFellBelowMinimum()
    {
        HidePopupsOnFail();
        // stockFellBelowMinimum.SetActive(true);
        ShowFailedBeforeImprove();
    }
    
    public void ShowTimeOut()
    {
        HidePopupsOnFail();
        // failedTimeOut.SetActive(true);
        ShowFailedBeforeImprove();
    }

    public void ShowSeccessGameWin()
    {
        HidePopupsOnFail();
        successGameWin.SetActive(true);
    }

    public void ShowSelectBoxHint()
    {
        selectBoxTip.SetActive(true);
    }

    public void _HideSelectBoxHint()
    {
        selectBoxTip.SetActive(false);
    }

    public void ShowSelectBeneathBox()
    {
        if (!selectBeneathBox.activeInHierarchy)
            StartCoroutine(nameof(ShowPopup), selectBeneathBox);
    }

    public void ShowWrongPlaceWarning()
    {
        if (!selectWrongPlace.activeInHierarchy)
            StartCoroutine(nameof(ShowPopup), selectWrongPlace);
    }

    public void ShowProductionRampup()
    {
        if (!productionRampUp.activeInHierarchy)
            StartCoroutine(nameof(ShowPopup), productionRampUp);
    }

    public void ShowStartGameScreen()
    {
        if (GameStartController.instance.guideLoadingArea)
            ShowWelcomeScreen();
        else
            GameManager.instance.StartGame();
    }

    public void _IsomatricStart()
    {
        isomatricScreen.SetActive(false);
        ShowWelcomeScreen();
    }

    public void ShowWelcomeScreen()
    {
        welcomeScreen.SetActive(true);
    }

    public void ShowRackingAreaTip()
    {
        rackingAreaTip.SetActive(true);
    }

    public void _HideRackingAreaTip()
    {
        rackingAreaTip.SetActive(false);
    }

    IEnumerator ShowDelayedPopup(GameObject go)
    {
        yield return popupWait;
        go.SetActive(true);
    }

    IEnumerator ShowPopup(GameObject tipObject)
    {
        tipObject.SetActive(true);
        yield return tipDelay;
        tipObject.SetActive(false);
    }

    public void _StartImprovements()
    {
        // if(failedTimeOut.activeInHierarchy)
        //     failedTimeOut.SetActive(false);
        
        // if(stockFellBelowMinimum.activeInHierarchy)
        //     stockFellBelowMinimum.SetActive(false);
        
        if(!GameManager.instance.isFirstImprovDone)
        {
            failedImproveOne.SetActive(false);
            improvementOne.SetActive(true);
        }
        else if(!GameManager.instance.isSecondImprovDone)
        {
            failedImproveTwo.SetActive(false);
            improvementTwo.SetActive(true);
        }
    }

    public void _PlayWithFirstImprovement()
    {
        successImproveOne.SetActive(false);
        timerText.text = ConvertToTimeFormat(totalTime);
        GameManager.instance.StartGame();
    }

    public void _PlayWithSecondImprovement()
    {
        successImproveTwo.SetActive(false);
        timerText.text = ConvertToTimeFormat(totalTime);
        GameManager.instance.StartGame();
    }

    public void _PlayAgainAfterImprovements()
    {
        failedAfterImprove.SetActive(false);
        timerText.text = ConvertToTimeFormat(totalTime);
        GameManager.instance.PlayAgainAfterImprov();
    }

    public void _DoFirstImproment()
    {
        if (wrongAnswer.activeInHierarchy)
            wrongAnswer.SetActive(false);

        improvementOne.SetActive(false);
        GameManager.instance.DoFirstImprovement();
        StartCoroutine(nameof(ShowDelayedPopup), successImproveOne);
    }

    public void _DoSecondImprovement()
    {
        if(wrongAnswer.activeInHierarchy)
            wrongAnswer.SetActive(false);

        improvementTwo.SetActive(false);
        GameManager.instance.DoSecondImprovement();
        StartCoroutine(nameof(ShowDelayedPopup), successImproveTwo);
    }

    public void _WrongAnswer()
    {
        if (!wrongAnswer.activeInHierarchy)
            StartCoroutine(nameof(ShowPopup), wrongAnswer);
    }

    public void _Retry()
    {
        HideStockFellAndBrock();
        GameManager.instance.ResetScene();
    }

    public void _Ok()
    {
        welcomeScreen.SetActive(false);
        // GameStartController.instance.StartBackgrounAudio();
        GameManager.instance.StartGame();
    }

    public void _PlayAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void PauseGame()
    {
        PauseTimer();
        pauseGamePopup.SetActive(true);
    }

    public void _ContinueGame()
    {
        pauseGamePopup.SetActive(false);
        ContinueTimer();
        GameManager.instance.ContinueGame();
    }

    public void _TipOkay() => GameManager.instance.ContinueGameFromGuiding();

    public void StartTheTimer() => StartCoroutine(nameof(StartTimer));

    public void StopTheTimer() => StopCoroutine(nameof(StartTimer));

    public void PauseTimer() => isTimerPaused = true;

    public void ContinueTimer() => isTimerPaused = false;

    IEnumerator StartTimer()
    {
        curTime = totalTime;
        if (!timer.activeInHierarchy)
            timer.SetActive(true);

        while(curTime > 0f)
        {
            if(isTimerPaused)
                yield return null;
            else
            {
                yield return null;
                curTime -= Time.deltaTime;
                timerText.text = ConvertToTimeFormat(curTime + 1f);
            }            
        }

        if (curTime <= 0f)
        {
            GameManager.instance.StartImprovementsPhase(ImproveReason.TimeRunOut);
        }
    }

    private string ConvertToTimeFormat(float seconds) => string.Format("{0:00}:{1:00}", Mathf.FloorToInt(seconds / 60), Mathf.FloorToInt(seconds % 60));

    IEnumerator CheckOrientation()
    {
        while(true)
        {
            // width < height which is portrait 
            if (GetOrientation() < 1f)
            {
                if (!orientationPopup.activeInHierarchy)
                    orientationPopup.SetActive(true);
            }
            // width > height which is landscape
            else if (GetOrientation() > 1f)
            {
                if (orientationPopup.activeInHierarchy)
                    orientationPopup.SetActive(false);
            }

            yield return oneSec;
        }
    }
}
