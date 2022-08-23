using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class TaskSlot : MonoBehaviour, IDropHandler
{
    public static Action playSound;

    [SerializeField] DefectManager defectInstance;
    [SerializeField] int indexCount = 0;
    [SerializeField] Transform taskHolder;

    [SerializeField] RectTransform[] taskPosition;

    private void OnEnable()
    {
        UIManager.OnReset += ResetFunction;
    }

    private void OnDisable()
    {
        UIManager.OnReset -= ResetFunction;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            // if the task is already set 
            if (eventData.pointerDrag.GetComponent<TaskFunction>().itemSet)
                return;
            // if the task is correct 
            if (eventData.pointerDrag.GetComponent<TaskFunction>().correctTaskIndex == indexCount)
            {
                playSound?.Invoke();        // play sucess sound
                eventData.pointerDrag.GetComponent<TaskFunction>().correctTaskIndicater.SetActive(true);
                eventData.pointerDrag.GetComponent<TaskFunction>().correctItem = true;
            }
            // if the task is in the slot
            eventData.pointerDrag.GetComponent<TaskFunction>().itemSet = true;
            eventData.pointerDrag.GetComponent<Transform>().SetParent(taskHolder);
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = taskPosition[indexCount].anchoredPosition;
            indexCount++;
            // check if the task is completed or not 
            defectInstance.CheckTaskOne();
        }
    }

    void ResetFunction ()
    {
        indexCount = 0;
    }
}