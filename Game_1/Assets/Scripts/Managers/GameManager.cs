using System.Collections;
using System.Web;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] Preset[] AllPresets;
    [SerializeField] ForkliftController forklift;
    [SerializeField] AIForkliftController aiForklift;
    [SerializeField] UIManager UIManager;
    [SerializeField] GameData gameData;
    [SerializeField] GameObject personMessage;
    [SerializeField] Transform flashedBoxHolder;

    [Header("Set at Runtime")]
    public Box selectedBox;
    public Place selectedPlace;
    public GameState gameState;
    public Preset currentPreset;

    [Header("Interact Properties")]
    public bool gameStarted = false;
    public bool gameEnded = true;
    public bool gamePaused = false;
    public bool canInteract = false;
    public bool isImproving = false;
    public bool isRightBox = false;

    [Header("")]
    public bool isSelectBeneathWarningDone = false;
    public bool isStartboxFlashed = false;
    public bool isFirstImprovDone = false;
    public bool isSecondImprovDone = false;

    int totalBoxCount = 12;
    public int shipedBoxCount = 0;

    [Header("Flashing Properties")]
    public int flashCount;
    public bool isFlashing = false;

    public bool CanSelect => canInteract && gameStarted && !gameEnded && !isImproving && !gamePaused;

    WaitForSeconds tipDelay;

    [DllImport("__Internal")]
    private static extern string GetURLFromPage();

    string postUrl = "";

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        gameState = GameState.SelectingBox;
        gameStarted = false;
        gameEnded = true;
        canInteract = false;
        tipDelay = new WaitForSeconds(UIManager.tipDuration);
        SetupScene();
        flashCount = GetRandomFlashNumber();

#if !UNITY_EDITOR && UNITY_WEBGL
        postUrl = GetURLFromPage();
        // Debug.Log(postUrl);
        postUrl = postUrl.Split('?')[1];
        // Debug.Log(postUrl);
        postUrl = postUrl.Split('=')[1];
        // Debug.Log(postUrl);
        postUrl = HttpUtility.UrlDecode(postUrl);
#endif
    }

    private void SetupScene()
    {
        if (GameStartController.instance.isSameSeed)
            currentPreset = AllPresets[GameStartController.instance.seedNumber];
        else
            currentPreset = AllPresets[GameStartController.instance.GetSeedNumber(AllPresets.Length)];

        currentPreset.gameObject.SetActive(true);
    }

    public void MoveForkliftToBox(Box box)
    {
        // Debug.Log("Moving to pickup box");
        isRightBox = box.OnTop();
        canInteract = false;
        selectedBox = box;
        forklift.targetPos = selectedBox.transform.position;
        forklift.MoveVehicle();

        HandleBoxFlashing();
    }

    public void MoveForkliftToPlace(Place place)
    {
        // Debug.Log("Moveing to place box");
        canInteract = false;
        selectedPlace = place;
        forklift.targetPos = selectedPlace.transform.position;
        forklift.MoveVehicle();
    }

    public void SelectBox(Box box)
    {
        if (GameStartController.instance.guideLoadingArea)
        {
            if (currentPreset.IsFirstBox(box))
            {
                box.Select();
                HideSelectBoxHint();
            }
        }
        else 
        {
            box.Select();
        }
    }

    public void SelectPlace(Place place)
    {
        if (GameStartController.instance.guideRackingArea)
        {
            if (currentPreset.IsFirstPlace(place))
            {
                place.Select();
                HideSelectPlaceHint();
            }
        }
        else
        {
            place.Select();
        }
    }

    public bool CanPickupBox()
    {
        if (selectedBox.OnTop())
        {
            PickupBox();
            return true;
        }
        else
        {
            selectedBox.parentStack.FallBoxesFrom(selectedBox);
            // EndGame(GameEndReason.LoseByStockFallBrock);UIManager.StopTheTimer();
            UIManager.StopTheTimer();
            canInteract = false;
            isImproving = true;
            if (isFlashing)
            {
                isFlashing = false;
                if(flashingBox != null)
                {
                    flashingBox.CancleFlashing();
                    flashingBox = null;
                }
            }

            if(aiForklift.flashingBox != null)
            {
                aiForklift.flashingBox = null;
            }
            
            gameState = GameState.SelectingBox;
            UIManager.ShowStockFellAndBrock();

            forklift.StopVehicle();
            forklift.SetToStartPoition();
            forklift.MoveVehicle();
            return false;
        }
    }

    public void PickupBox()
    {
        selectedBox.parentStack.Pop();
        selectedBox.transform.SetParent(forklift.boxHolder);
        selectedBox.transform.localPosition = Vector3.zero;
        selectedBox.spriteRenderer.sortingLayerName = "ForkLift";
        // selectedBox.isSelectable = false;
        gameState = GameState.BoxPicked;
        // Debug.Log("Picked up the box");
    }

    public void PutdownBox()
    {
        if(selectedBox == null)
            return;

        selectedBox.transform.SetParent(selectedPlace.transform);
        selectedBox.transform.localPosition = Vector3.zero;
        selectedBox.spriteRenderer.sortingLayerName = "Box";
        selectedBox.DisableCollider();
        GameStartController.instance.PlayBellSound();
        // selectedPlace.isAvailable = false;
        selectedPlace.parentStack.Pop();
        gameState = GameState.BoxPlaced;
        // Debug.Log("Put down the box");

        selectedBox = null;
        selectedPlace = null;

        shipedBoxCount += 1;

        if (shipedBoxCount >= totalBoxCount)
        {
            EndGame(GameEndReason.Win);
        }
    }

    public void ResetScene()
    {
        if(!isFirstImprovDone && !isSecondImprovDone)
        {
            GameStartController.instance.isSameSeed = true;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else if (isFirstImprovDone && !isSecondImprovDone)
        {
            StopFlashingLines();
            currentPreset.ResetArea();
        }
        else
        {
            PlayAgainAfterImprov();
        }
    }

    public void StartImprovementsPhase(ImproveReason improveReason)
    {
        if(isFlashing)
        {
            currentPreset.CancleFlashingBox();
            isFlashing = false;
        }

        if(selectedBox != null && gameState != GameState.SelectingBox)
        {
            // selectedBox.PlayIdleAnimation();
            selectedBox.gameObject.SetActive(false);
            selectedBox = null;
        }

        if(!isFirstImprovDone || !isSecondImprovDone)
        {
            isImproving = true;
            UIManager.StopTheTimer();

            forklift.StopVehicle();
            forklift.SetToStartPoition();
            forklift.MoveVehicle();

            HideAllHints();
            // Debug.Log("Its time for Improvements");

            if (improveReason == ImproveReason.TimeRunOut)
                UIManager.ShowTimeOut();
            else if (improveReason == ImproveReason.StockFallBelowMinimum)
                UIManager.ShowStockFellBelowMinimum();
        }
        else
        {
            UIManager.StopTheTimer();
            canInteract = false;
            isImproving = true;
            if (isFlashing)
                isFlashing = false;
            gameState = GameState.SelectingBox;
            UIManager.ShowFailedAfterImprove();

            forklift.StopVehicle();
            forklift.SetToStartPoition();
            forklift.MoveVehicle();
        }
    }

    public void StartGame()
    {
        gameStarted = true;
        gameEnded = false;
        canInteract = true;
        isImproving = false;
        shipedBoxCount = 0;
        gameState = GameState.SelectingBox;

        selectedBox = null;
        selectedPlace = null;

        // totalBoxCount = currentPreset.GetTotalBoxCount();

        UIManager.ContinueTimer();

        UIManager.StartTheTimer();

        if (GameStartController.instance.guideLoadingArea)
        {
            GuideLoadingArea();
        }
    }

    public void PauseGame()
    {
        gamePaused = true;
        UIManager.PauseGame();
    }

    public void ContinueGame()
    {
        gamePaused = false;
    }
    
    public void PauseGameForGuiding()
    {
        canInteract = false;
        UIManager.PauseTimer();
    }

    public void ContinueGameFromGuiding()
    {
        canInteract = true;
        UIManager.ContinueTimer();
    }

    public void EndGame(GameEndReason reason)
    {
        gameStarted = false;
        gameEnded = true;
        canInteract = false;

        UIManager.StopTheTimer();

        switch(reason)
        {
            case GameEndReason.Win :
                // Debug.Log("Game is finished. <color=green> You won </color>");
                _SendPostRequest();
                GameStartController.instance.isSameSeed = false;
                UIManager.ShowSeccessGameWin();
                break;

            case GameEndReason.LoseByStockFallBrock :
                // Debug.Log("Game is finished. <color=red> You lose </color>");
                GameStartController.instance.isSameSeed = true;
                UIManager.ShowStockFellAndBrock();
                break;
        }
    }

    public void ShowSelectBeneathWarning()
    {
        isSelectBeneathWarningDone = true;
        UIManager.ShowSelectBeneathBox();
    }

    public void ShowSelectPlaceWarning()
    {
        UIManager.ShowWrongPlaceWarning();
    }

    public void GuideLoadingArea()
    {
        // Debug.Log("Guide Loading Area");
        // Highlight Top Boxes
        ShowSelectBoxHint();
        PauseGameForGuiding();
    }

    public void GuideRackingArea()
    {
        if(isImproving)
            return;

        if(isFirstImprovDone)
            return;
        // Debug.Log("Guide Racking Area");
        // Highlight minimum stock level (Red line) with tip
        StartFlashingLines();
        // Highlight available places
        ShowSelectPlaceHint();
        PauseGameForGuiding();

    }

    public void ShowSelectBoxHint()
    {
        UIManager.ShowSelectBoxHint();
        currentPreset.ShowBoxHint();
    }

    public void HideSelectBoxHint()
    {
        GameStartController.instance.guideLoadingArea = false;
        currentPreset.HideBoxHint();
    }

    public void ShowSelectPlaceHint()
    {
        UIManager.ShowRackingAreaTip();
        currentPreset.ShowPlaceHint();
        FlashInitialBoxes();
    }

    public void HideSelectPlaceHint()
    {
        GameStartController.instance.guideRackingArea = false;
        currentPreset.HidePlaceHint();
        StopFlashingLines();
    }

    public void HideAllHints()
    {
        if(GameStartController.instance.guideLoadingArea)
            HideSelectBoxHint();

        if(GameStartController.instance.guideRackingArea)
            HideSelectPlaceHint();
    }

    public void StartFlashingLines()
    {
        currentPreset.StartFlashingLines();
        personMessage.SetActive(true);
    }

    public void StopFlashingLines()
    {
        currentPreset.StopFlashingLines();
        personMessage.SetActive(false);
    }

    public void DoFirstImprovement()
    {
        currentPreset.DoFirstImprovement();
        StopFlashingLines();
    }

    public void DoSecondImprovement()
    {
        forklift.SetupSpeeds(1);
        currentPreset.DoSecondImprovement();
        StopFlashingLines();
    }

    public void PlayAgainAfterImprov()
    {
        currentPreset.ResetLoadingAndRacking();
        StopFlashingLines();
    }

    public Sprite GetSprite(BoxType type)
    {
        switch (type)
        {
            case BoxType.Red:
                return gameData.red;

            case BoxType.Green:
                return gameData.green;

            case BoxType.Yellow:
                return gameData.yellow;

            case BoxType.Blue:
                return gameData.blue;

            case BoxType.Purple:
                return gameData.purple;

            default:
                return gameData.red;
        }
    }

    public float GetSelectBoxPosition()
    {
        return currentPreset.GetClosestPosition();
    }

    public bool CanFlashWrongBox() => !isFirstImprovDone && UIManager.curTime < 70f;

    public void AddToFlashedBoxList(Box box)
    {
        box.transform.parent = flashedBoxHolder;
        box.transform.localPosition = Vector3.zero;
    }

    public Box[] GetFlashedBoxes()
    {
        return flashedBoxHolder.GetComponentsInChildren<Box>(true);
    }

    public void HandleBoxFlashing()
    {
        if(isFlashing)
            StartMovingAIForklift();
        else
            StartFlashingRandomBox();
    }

    public Box flashingBox;

    public void FlashInitialBoxes()
    {
        if(isStartboxFlashed)
            return;

        isStartboxFlashed = true;
        currentPreset.StartFlashingInitialBoxes();
    }

    public void StartFlashingRandomBox()
    {
        if(flashCount <= 0)
        {
            isFlashing = true;
            currentPreset.StartFlashingRandomBox();
        }
        else
        {
            flashCount -= 1;
        }
    }

    public void StartMovingAIForklift()
    {
        if(!isRightBox)
            return;

        StartCoroutine(nameof(StartDistanceCalculation));
    }

    IEnumerator StartDistanceCalculation()
    {
        if (flashingBox == null)
            yield break;

        while(forklift.transform.position.x - flashingBox.transform.position.x > GetDistance())
        {
            yield return null;

            if (flashingBox == null)
                yield break;
        }
        MoveAIForkliftToBox();
    }

    public void MoveAIForkliftToBox()
    {
        // Debug.Log("MoveAIForkliftToBox");
        if(flashingBox == null)
            return;

        if(!isRightBox)
            return;

        isFlashing = false;
        aiForklift.flashingBox = flashingBox;
        aiForklift.targetPos = flashingBox.transform.position;
        aiForklift.SetNear();
        aiForklift.MoveVehicle();
    }

    public float GetDistance()
    {
        if(!isFirstImprovDone || !isSecondImprovDone)
            return 5f;
        else
            return 6f;

    }

    public void StopFlashingRandomBox()
    {
        currentPreset.StopFlashingRandomBox();
        flashCount = GetRandomFlashNumber();
    }

    public void PickupFlashingBox()
    {
        flashingBox.transform.SetParent(aiForklift.boxHolder);
        flashingBox.transform.localPosition = Vector3.zero;
        flashingBox.spriteRenderer.sortingLayerName = "ForkLift";
        StopFlashingRandomBox();
        currentPreset.ResetFlashingPlace();
    }

    public void CheckLineStatus()
    {
        if (currentPreset.AnyBelowMinimumLevel())
            StartFlashingLines();
        else
            StopFlashingLines();
    }

    public void CheckForBelowMinimumLevel()
    {
        if(currentPreset.AnyBelowMinimumLevel())
        {
            StartImprovementsPhase(ImproveReason.StockFallBelowMinimum);
        }
    }

    int GetRandomFlashNumber() => Random.Range(1, 3);


    public TMPro.TMP_Text responce;

    public void _SendPostRequest() => StartCoroutine(nameof(Coroutine_SendRequest));

    IEnumerator Coroutine_SendRequest()
    {
        PostData postData;
        postData.metadata.isGameComplete = true;

        string jsonData = JsonUtility.ToJson(postData);
        Debug.Log($"Post URL : {postUrl}");
        Debug.Log($"Json String : {jsonData}");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        
        UnityWebRequest request = new UnityWebRequest(postUrl, "POST");
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if(request.result != UnityWebRequest.Result.Success)
        {
            responce.text = $"--- ERROR : {request.responseCode} ---";
            Debug.Log(request.error);
        }
        else
        {
            Debug.Log($"--- SUCCESS ---");
            responce.text = jsonData;
        }
    }
}

[System.Serializable]
public struct PostData
{
    public MetaData metadata;
}

[System.Serializable]
public struct MetaData
{
    public bool isGameComplete;
}

public enum BoxType
{
    Red,
    Green,
    Yellow,
    Blue,
    Purple,
    Null,
}

public enum GameState
{
    SelectingBox,
    SelectingPlace,
    BoxPicked,
    BoxPlaced,
}

public enum GameEndReason
{
    Win,
    LoseByStockFallBrock,
    LoseByTimeAfterImprove,
    LoseByMinStockAfterImprove,
}

public enum ImproveReason
{
    TimeRunOut,
    StockFallBelowMinimum,
}