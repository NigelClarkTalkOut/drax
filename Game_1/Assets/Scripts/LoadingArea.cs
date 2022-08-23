using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class LoadingArea : MonoBehaviour
{
    public Box firstBox;
    [SerializeField] Animator boxHint;

    public BoxStack[] allBoxStack;

    public GameObject startBoxes;
    public GameObject improvedBoxesOne;
    // public GameObject improvedBoxesTwo;
    public BoxStack[] improvOneBoxStacks;
    // public BoxStack[] improvTwoBoxStacks;
    [SerializeField] Vector3[] arrangedStackPos;

    [SerializeField] BoxType flashType;
    [SerializeField] List<BoxType> possibeTypes = new List<BoxType>();

    public void ShowBoxHint()
    {
        boxHint.Play("Flashing_Start");
    }

    public void HideBoxHint()
    {
        boxHint.gameObject.SetActive(false);
    }

    public void DoFirstImprovement()
    {
        for (int i = 0; i < allBoxStack.Length; i++)
        {
            allBoxStack[i] = improvOneBoxStacks[i];
        }
        startBoxes.SetActive(false);
        improvedBoxesOne.SetActive(true);
        // StartCoroutine(nameof(ResetBoxPositionType));
    }

    // IEnumerator ResetBoxPositionType()
    // {
    //     for (int i = 0; i < allBoxStack.Length; i++)
    //     {
    //         for (int j = 0; j < allBoxStack[i].elements.Length; j++)
    //         {
    //             allBoxStack[i].elements[j].SetTypeToParent();
    //             yield return null;
    //         }
    //         yield return null;
    //     }
    // }

    public void DoSecondImprovement()
    {
        // for (int i = 0; i < allBoxStack.Length; i++)
        // {
        //     allBoxStack[i] = improvTwoBoxStacks[i];
        // }
        // improvedBoxesOne.SetActive(false);
        // improvedBoxesTwo.SetActive(true);
        StartCoroutine(nameof(ArrengeStacks));
    }

    IEnumerator ArrengeStacks()
    {
        for (int i = 0; i < allBoxStack.Length; i++)
        {
            allBoxStack[i].transform.localPosition = arrangedStackPos[i];
            yield return null;
        }
    }

    public float GetRightMostStackPos()
    {
        // -3 because if any is on the right then to change pos the first time.
        float pos = -3f;

        for (int i = 0; i < allBoxStack.Length; i++)
        {
            if (allBoxStack[i].IsEmpty())
                continue;

            if(pos < allBoxStack[i].transform.position.x)
                pos = allBoxStack[i].transform.position.x;
        }

        // If there are no boxes left then make pos to initial position
        if(pos == -3f)
            pos = .259f;

        return pos + .349f;
    }
    
    public BoxType GetTopPossibleBoxType()
    {
        flashType = BoxType.Null;
        possibeTypes.Clear();
        for (int i = 0; i < allBoxStack.Length; i++)
        {
            if (allBoxStack[i].IsEmpty())
                continue;

            if(GameManager.instance.selectedBox.type == allBoxStack[i].Top.type)
                continue;

            if(possibeTypes.Contains(allBoxStack[i].Top.type))
                continue;

            possibeTypes.Add(allBoxStack[i].Top.type);
        }

        if(GameManager.instance.CanFlashWrongBox())
        {
            // Debug.Log("Flash Wrong Box");
            for (int i = 0; i < allBoxStack.Length; i++)
            {
                if(allBoxStack[i].IsEmpty())
                    continue;

                if(GameManager.instance.selectedBox.type == allBoxStack[i].elements[0].type)
                    continue;

                if(possibeTypes.Contains(allBoxStack[i].elements[0].type))
                    continue;

                flashType = allBoxStack[i].elements[0].type;
                break;
            }
        }
        else
        {
            if (possibeTypes.Count > 0)
            {
                int random = Mathf.Clamp(0, possibeTypes.Count - 1, Random.Range(0, possibeTypes.Count));
                flashType = possibeTypes[random];
            }
        }

        return flashType;
    }

    // public int GetBoxCount()
    // {
    //     int boxNumber = 0;

    //     for (int i = 0; i < allBoxStack.Length; i++)
    //     {
    //         for (int j = 0; j < allBoxStack[i].elements.Length; j++)
    //         {
    //             boxNumber += 1;
    //         }
    //     }

    //     return boxNumber;
    // }
}
