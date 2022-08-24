using System;
using UnityEngine;
using System.Collections;

public class CompletedItem : MonoBehaviour
{
    [SerializeField] int speed;
    [SerializeField] Transform[] endPosition;
    [SerializeField] string endPosHolder;
    [SerializeField] string itemName;

    public static Action PalmReached;
    public static Action completeArmReached;

    private void Start()
    {
        endPosition[0] = GameObject.Find(endPosHolder).transform.GetChild(0);
        endPosition[1] = GameObject.Find(endPosHolder).transform.GetChild(1);
        endPosition[2] = GameObject.Find(endPosHolder).transform.GetChild(2);
        StartCoroutine(Movement());
    }

    IEnumerator Movement ()
    {
        int i = UnityEngine.Random.Range(0, endPosition.Length);
        while (transform.position != endPosition[i].position)
        {
            transform.position = Vector3.MoveTowards(transform.position, endPosition[i].position, speed * Time.deltaTime);
            yield return null;
        }
        if (itemName == "palm")
            PalmReached?.Invoke();
        else if (itemName == "halfArm")
            PalmReached?.Invoke();
        else if (itemName == "completeArm")
            completeArmReached?.Invoke();
    }
}