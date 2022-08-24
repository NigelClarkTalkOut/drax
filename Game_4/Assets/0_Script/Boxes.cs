using System;
using UnityEngine;
using System.Collections;

public class Boxes : MonoBehaviour
{
    public static Action SlowWorker;            // to show that worker was slow and was not able to create the robots

    public enum Position
    {
        upper,
        lower,
        conveyorBelt,
    }
    public Position position;

    [SerializeField] int workerSpeed;                       // worker speed of movement 
    [SerializeField] Transform worker;
    [SerializeField] Animator workerAnimation;
    [SerializeField] GameObject tapHereHint;                // letting player know where to tap next

    [Header("Position")]
    [SerializeField] Transform upperPlace;
    [SerializeField] Transform lowerPlace;
    [SerializeField] Transform conveyorPosition;            // position for workers movement

    [Header("Colliders")]
    [SerializeField] Collider2D upperCollider;
    [SerializeField] Collider2D lowerCollider;
    [SerializeField] Collider2D conveyorCollider;           // colliders for workers movement

    [Header("Highlight")]
    [SerializeField] GameObject upperHighlight;
    [SerializeField] GameObject lowerHighlight;             // highlights for letting player know where to tap

    private void Start()
    {
        lowerCollider.enabled = false;
        upperHighlight.SetActive(true);
        lowerHighlight.SetActive(false);
    }

    private void OnMouseDown()
    {
        workerAnimation.SetTrigger("Idel");
        DefectManager.instnace.boxCount++;
        if (position == Position.upper)
        {
            StartCoroutine(MoveWorkers(upperPlace.position, 90, 180, "Pick_up"));
            Destroy(upperCollider);
            // set the highlights
            upperHighlight.SetActive(false);
            lowerHighlight.SetActive(true);
        }

        else if (position == Position.lower)
        {
            StartCoroutine(MoveWorkers(lowerPlace.position, -90, 90, "Pick_up_2"));
            Destroy(lowerCollider);
            // set the highlights
            lowerHighlight.SetActive(false);
        }
    
        else if (position == Position.conveyorBelt)
        {
            StartCoroutine(MoveWorkers(conveyorPosition.position, 60, 0, "Idel"));
            Destroy(conveyorCollider);
            Invoke(nameof(EndPhase), 1);
        }
    }

    IEnumerator MoveWorkers (Vector3 place, int zRotation, int pickUpRotation, string animationState)
    {
        // hide the colliders
        if (upperCollider != null)
        if (lowerCollider != null)
            lowerCollider.enabled = false;
        while (worker.position != place)
        {
            worker.position = Vector3.MoveTowards(worker.position, place, workerSpeed * Time.deltaTime);
            worker.rotation = Quaternion.Euler(0, 0, zRotation);
            yield return null;
        }
        if (worker.position == place)
        {
            Debug.Log("On place");
            workerAnimation.ResetTrigger("Idel");
            workerAnimation.SetTrigger(animationState);
        }
        worker.rotation = Quaternion.Euler(0, 0, pickUpRotation);
        // show the colliders
        // hide the colliders
        if (upperCollider != null)
            upperCollider.enabled = true;
        if (lowerCollider != null)
            lowerCollider.enabled = true;
        // if box of the box are clicked then next box hint will be on
        if (DefectManager.instnace.boxCount == 2)
        {
            tapHereHint.SetActive(true);
        }
    }

    void EndPhase()
         => SlowWorker?.Invoke();
}