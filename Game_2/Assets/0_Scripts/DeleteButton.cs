using System;
using UnityEngine;

public class DeleteButton : MonoBehaviour
{
    public static Action pathWayDeleted;
    [SerializeField] GameObject objectToDelete;
    [SerializeField] SpriteRenderer[] highlights;
    [SerializeField] bool canDelete = true;
    [SerializeField] bool deletePathWay = false;
    [SerializeField] bool ifRackingArea = false;
    [SerializeField] bool ifSubAssembly = true;


    private void Awake()
    {
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
    }

    private void Update()
    {
        // in editing mode editable objects will highlights 
        if (GameManager.instance.isEditing)
        {
            // all the highlights will be enable 
            for (int i = 0; i < highlights.Length; i++)
            {
                highlights[i].enabled = true;
            }
        }
        else
        {
            // all the highlights will be disable
            for (int i = 0; i < highlights.Length; i++)
            {
                highlights[i].enabled = false;
            }
        }
    }

    // the object will be deleted
    private void OnMouseDown()
    {
        // if in editable mode 
        if (GameManager.instance.isEditing)
        {
            // if player can delete
            if (canDelete)
            {
                // remove the points
                if (deletePathWay)
                {
                    if (!SelectSeed.instance.pathPointForfirstTime)
                    {
                        UIManager.instance.ChangePannel(UIManager.instance.firstTimePathPoint);
                        SelectSeed.instance.pathPointForfirstTime = true;
                    }
                    GameObject pathWay = GameObject.FindGameObjectWithTag("ExtraPath");
                    Destroy(pathWay);

                    SelectSeed.instance.deleteThepathWay = true;
                    pathWayDeleted?.Invoke();
                    SeedSystem.instance.AfterChanginSubassembiles();
                    Destroy(gameObject);
                }

                // if its the racking area and player can get out of the edit mode 
                else if (ifRackingArea)
                    SelectSeed.instance.removedRackings = true;
                Destroy(objectToDelete);
            }
            else    // if player cannot delete
            {
                if (ifSubAssembly)
                    UIManager.instance.ChangePannel(UIManager.instance.doNotDeleteSubAssembly);
                else if (!ifSubAssembly)
                    UIManager.instance.ChangePannel(UIManager.instance.doNotDeleteTip);
            }
        }
    }
}