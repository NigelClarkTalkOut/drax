using System;
using UnityEngine;

public class DefectOneCheck : MonoBehaviour
{
    public static Action TimeOut;

    private void OnTriggerEnter2D(Collider2D info)
    {
        if (info.CompareTag("Stations"))
            TimeOut?.Invoke();
    }
}