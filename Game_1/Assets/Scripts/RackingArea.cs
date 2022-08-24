using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RackingArea : MonoBehaviour
{
    public Place firstPlace;
    [SerializeField] Animator placeHint;
    [SerializeField] Animator[] allMinimumLines;
    [SerializeField] Place[] startFlashingPlaces;
    public PlaceStack[] allPlaceStack;
    public GameObject startPlaces;
    public GameObject improvedPlacesOne;
    // public GameObject improvedPlacesTwo;
    public PlaceStack[] improvOnePlaceStacks;
    // public PlaceStack[] improvTwoPlaceStacks;

    public PlaceStack flashingPlaceStack;

    public void DoFirstImprovement()
    {
        for (int i = 0; i < allPlaceStack.Length; i++)
        {
            allPlaceStack[i] = improvOnePlaceStacks[i];
        }
        startPlaces.SetActive(false);
        improvedPlacesOne.SetActive(true);
    }

    // public void DoSecondImprovement()
    // {
    //     for (int i = 0; i < allPlaceStack.Length; i++)
    //     {
    //         allPlaceStack[i] = improvTwoPlaceStacks[i];
    //     }
    //     improvedPlacesOne.SetActive(false);
    //     improvedPlacesTwo.SetActive(true);
    // }

    public void StratFlasingLines()
    {
        for (int i = 0; i < allMinimumLines.Length; i++)
        {
            allMinimumLines[i].Play("Flashing_Start");
        }
    }

    public void StopFlashingLines()
    {
        for (int i = 0; i < allMinimumLines.Length; i++)
        {
            allMinimumLines[i].Play("Idle");
        }
    }

    public void ShowPlaceHint()
    {
        placeHint.Play("Flashing_Start");
    }

    public void HidePlaceHint()
    {
        placeHint.gameObject.SetActive(false);
    }

    public void StartFlashingInitialBoxes()
    {
        StartCoroutine(nameof(StartFlashing));
    }

    IEnumerator StartFlashing()
    {
        for (int i = 0; i < startFlashingPlaces.Length; i++)
        {
            startFlashingPlaces[i].StartFlashing();
        }

        yield return new WaitForSeconds(5);

        for (int i = 0; i < startFlashingPlaces.Length; i++)
        {
            startFlashingPlaces[i].StopFlashing();
            startFlashingPlaces[i].DeleteBox();
        }
    }

    public void StartBoxFlashingOfType(BoxType type)
    {
        List<PlaceStack> availablePlaceStack = new List<PlaceStack>();
        for (int i = 0; i < allPlaceStack.Length; i++)
        {
            if (allPlaceStack[i].Top.type == type)
                availablePlaceStack.Add(allPlaceStack[i]);
        }

        int random = Mathf.Clamp(0, availablePlaceStack.Count - 1, Random.Range(0, availablePlaceStack.Count));

        flashingPlaceStack = availablePlaceStack[random];
        flashingPlaceStack.StartFlashingTopBox();
    }

    public void StopBoxFlashing()
    {
        flashingPlaceStack.StopFlashingTopBox();
    }

    public void CancleFlashingBox()
    {
        flashingPlaceStack.CancleFlashingBox();
    }

    public bool IsAnyBelowMinimumLevel()
    {
        bool value = false;
        
        for (int i = 0; i < allPlaceStack.Length; i++)
        {
            if(allPlaceStack[i].IsBelowMinimumLevel())
            {
                value = true;
                break;
            }
        }

        return value;
    }

    public void ResetArea()
    {
        ResetAllStackTop();
    }

    public void ResetAllStackTop()
    {
        for (int i = 0; i < allPlaceStack.Length; i++)
        {
            allPlaceStack[i].SetStackTop();
        }
    }

}
