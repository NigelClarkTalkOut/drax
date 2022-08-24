using UnityEngine;

public class PathChecker : MonoBehaviour
{
    public bool checkTrigger = false;
    public bool correctTrigger = false;
    public bool triggerToSub = false;

    private void OnEnable()
    {
        GameManager.OnClear += OnClearButton;
    }
    
    private void OnDisable()
    {
        GameManager.OnClear -= OnClearButton;
    }

    void OnClearButton ()
    {
        checkTrigger = false;
        gameObject.GetComponent<Collider2D>().enabled = true;
    }
}