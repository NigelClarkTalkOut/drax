using UnityEngine;
using System.Collections;

public class Preset : MonoBehaviour
{
    [SerializeField] LoadingArea loadingArea;
    [SerializeField] RackingArea rackingArea;

    private void Start()
    {
        // if (!GameStartController.instance.guideRackingArea)
        //     StartFlashingInitialBoxes();
    }

    public void StartFlashingLines() => rackingArea.StratFlasingLines();
    public void StopFlashingLines() => rackingArea.StopFlashingLines();

    public void ShowBoxHint() => loadingArea.ShowBoxHint();
    public void HideBoxHint() => loadingArea.HideBoxHint();

    public void ShowPlaceHint() => rackingArea.ShowPlaceHint();
    public void HidePlaceHint() => rackingArea.HidePlaceHint();

    public void StartFlashingInitialBoxes() => rackingArea.StartFlashingInitialBoxes();

    public bool IsFirstBox(Box box) => box.Equals(loadingArea.firstBox);
    public bool IsFirstPlace(Place place) => place.Equals(rackingArea.firstPlace);

    public void DoFirstImprovement()
    {
        loadingArea.DoFirstImprovement();
        rackingArea.DoFirstImprovement();
        GameManager.instance.isFirstImprovDone = true;
    }

    public void DoSecondImprovement() 
    {
        StartCoroutine(nameof(ResetAndImprove));
        // loadingArea.DoSecondImprovement();
        // rackingArea.DoSecondImprovement();
        // GameManager.instance.isSecondImprovDone = true;
    }

    public void ResetLoadingAndRacking()
    {
        StartCoroutine(nameof(ResetAndImprove));
    }

    public float GetClosestPosition() => loadingArea.GetRightMostStackPos();

    // public int GetTotalBoxCount() => loadingArea.GetBoxCount();

    public Box GetFlashingBox() => rackingArea.flashingPlaceStack.flashingPlace.box;

    BoxType flashType;
    public void StartFlashingRandomBox()
    {
        flashType = loadingArea.GetTopPossibleBoxType();
        if(flashType != BoxType.Null)
        {
            rackingArea.StartBoxFlashingOfType(flashType);
            GameManager.instance.flashingBox = GetFlashingBox();
        }
    }

    public void StopFlashingRandomBox()
    {
        if(flashType != BoxType.Null)
        {
            rackingArea.StopBoxFlashing();
            GameManager.instance.flashingBox = null;
        }
    }

    public void CancleFlashingBox()
    {
        if(flashType != BoxType.Null)
            rackingArea.CancleFlashingBox();
    }

    public void DeleteFlashingBox()
    {
        rackingArea.flashingPlaceStack.flashingPlace.DeleteBox();
    }

    public void ResetFlashingPlace() => rackingArea.flashingPlaceStack.flashingPlace.Reset();

    public bool AnyBelowMinimumLevel() => rackingArea.IsAnyBelowMinimumLevel();

    Box[] flashedBoxes;

    public void ResetArea()
    {
        StartCoroutine(nameof(ResetAreaAndStart));
    }

    IEnumerator ResetAreaAndStart()
    {
        yield return StartCoroutine(nameof(ResetArea_Coroutine));
        GameManager.instance.StartGame();
    }

    IEnumerator ResetArea_Coroutine()
    {
        flashedBoxes = GameManager.instance.GetFlashedBoxes();

        for (int i = 0; i < flashedBoxes.Length; i++)
        {
            flashedBoxes[i].ResetToInitial();
            yield return null;
        }

        // Reset all box properties
        for (int i = 0; i < loadingArea.allBoxStack.Length; i++)
        {
            for (int j = 0; j < loadingArea.allBoxStack[i].elements.Length; j++)
            {
                loadingArea.allBoxStack[i].elements[j].ResetToInitial();
                yield return null;
            }
            loadingArea.allBoxStack[i].SetStackTop();
            yield return null;
        }

        // Reset all place properties
        rackingArea.ResetArea();

    }

    IEnumerator ResetAndImprove()
    {
        yield return StartCoroutine(nameof(ResetArea_Coroutine));

        if(!GameManager.instance.isFirstImprovDone)
        {
            // failed at the start
            loadingArea.DoFirstImprovement();
            GameManager.instance.isFirstImprovDone = true;
        }
        else if(!GameManager.instance.isSecondImprovDone)
        {
            // failed after first improvement
            loadingArea.DoSecondImprovement();
            GameManager.instance.isSecondImprovDone = true;
        }
        else
        {
            // failed after both improvements
            GameManager.instance.StartGame();
        }
    }
}
