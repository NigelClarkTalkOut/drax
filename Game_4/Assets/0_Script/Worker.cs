using System;
using UnityEngine;
using System.Collections;

public class Worker : MonoBehaviour
{
    Camera mainCamera;
    Vector2 mousePosition;
    Vector2 defaultPosition;

    public static Action ShowWorkerSkills;
    public static Action workerPlaced;
    public static Action workerPlacedIncorrect;

    [SerializeField] bool first, second;
    [SerializeField] bool dragged = false;
    [SerializeField] bool interaction = true;
    [SerializeField] bool placed = false;
    bool added = false;

    [SerializeField] bool onStation = false;

    [SerializeField] Transform workerParent;
    [SerializeField] GameObject[] correctPosition;
    [SerializeField] Transform positionToPlace;
    [SerializeField] bool onCorrectPosition = false;
    [SerializeField] bool turnItRed = false;
    //[SerializeField] bool onWrongPosition = false;

    [Header("Colors")]
    [SerializeField] Color normalColor;
    [SerializeField] Color redColor;

    private void Start()
    {
        defaultPosition = transform.position;
    }

    private void OnEnable()
    {
        UIManager.ResetWorkers += ResetWorkers;
        // after time out player cannot interract with worker
        Timer.TimeOut += _CannotInteract;
        DefectManager.WorkerArranged += SetDefaultPosition;
        // the game is showing station info soo workers are mot able to move
        UIManager.ShowStationInfo += _CannotInteract;
        // the game is not showing station info soo workers can move 
        UIManager.hideStationInfo += _CanInteract;           
        DefectManager.WrongWorkerAction+= ChangeWorkersToRed;           
        DefectManager.WrokerBackToNormal+= ChangeWorkerToNormal;           
    }

    private void OnDisable()
    {
        UIManager.ResetWorkers -= ResetWorkers;
        Timer.TimeOut -= _CannotInteract;
        DefectManager.WorkerArranged -= SetDefaultPosition;
        UIManager.ShowStationInfo -= _CannotInteract;
        UIManager.hideStationInfo -= _CanInteract;
        DefectManager.WrongWorkerAction -= ChangeWorkersToRed;
        DefectManager.WrokerBackToNormal -= ChangeWorkerToNormal;
    }

    private void OnMouseDown()
    {
        if (!interaction)
            return;
        GetComponent<SpriteRenderer>().sortingLayerName = "User Interface";
        StartCoroutine(nameof(CheckDrag));
    }

    private void OnMouseDrag()
    {
        // if the interaction is off player cannot move 
        if (!interaction)
            return;

        transform.position = GetMousePosition();
    }

    void ChangeWorkersToRed ()
    {
        if (turnItRed)
            GetComponent<SpriteRenderer>().color = redColor;
    }

    void ChangeWorkerToNormal()
    {
        GetComponent<SpriteRenderer>().color = normalColor;
        turnItRed = false;
    }

    private void OnMouseUp()
    {
        if (!interaction)
            return;
        GetComponent<SpriteRenderer>().sortingLayerName = "Workers";
        if (!dragged)
        {
            UIManager.ShowStationInfo?.Invoke();
            // move to default position if player don't drag the worker
            transform.position = defaultPosition;
            // and show the skills accordingly 
            if (first && !second)
            {
                UIManager.skills = UIManager.Skills.first;
            }
            else if (!first && second)
            {
                UIManager.skills = UIManager.Skills.second;
            }
            else if (first && second)
            {
                UIManager.skills = UIManager.Skills.Both;
            }
            ShowWorkerSkills?.Invoke();
        }

        if (dragged)
        {
            if (onStation)
            {
                DefectManager.instnace.totalWorkerPlace++;
                // if worker is out of the station
                if (onCorrectPosition)
                {
                    // if worker is placed on correct place 
                    workerPlaced?.Invoke();
                    placed = true;
                    // place the worker in the position
                    transform.position = positionToPlace.position;
                    positionToPlace.GetComponent<Collider2D>().enabled = false; // this place cannot be used again
                    GetComponent<Collider2D>().enabled = false;
                }

                // if the worker is on corret position
                else if (!onCorrectPosition)
                {
                    turnItRed = true;
                    Debug.Log("<Color=red> Wrong </color>");
                    placed = true;
                    if (placed)
                        workerPlacedIncorrect?.Invoke();
                    // place the worker in the position
                    if (positionToPlace != null)
                        transform.position = positionToPlace.position;
                    positionToPlace.GetComponent<Collider2D>().enabled = false; // this place cannot be used again
                    GetComponent<Collider2D>().enabled = false;
                }
                Debug.Log(DefectManager.instnace.workerCount);
            }

            else if (!onStation)
            {
                transform.position = defaultPosition;
                if (positionToPlace != null)
                    positionToPlace = null;
            }
        }
    }

    // get the position or mouse/touch 
    Vector2 GetMousePosition()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
        // get mouse position 
        mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        return mousePosition;
    }

    IEnumerator CheckDrag ()
    {
        dragged = false;
        yield return new WaitForSeconds(.1f);
        dragged = true;
    }

    #region Position Check
    private void OnTriggerEnter2D(Collider2D collider)
    {
        // if the worker is on the any work station 
        if (collider.CompareTag("Work Station"))
        {
            if (positionToPlace != null)
                positionToPlace = null;

            positionToPlace = collider.gameObject.transform;
            onStation = true;
            for (int i = 0; i < correctPosition.Length; i++)
            {
                if (collider.name == correctPosition[i].name)
                {
                    onCorrectPosition = true;
                    break;
                }
            }
            if (!onCorrectPosition)
            {
                positionToPlace = collider.gameObject.transform;
            }

            //DefectManager.instnace.totalWorkerPlace++;
            //bool found = false;
            //// find for the correctone
            //if (!found)
            //{
            //    positionToPlace = collider.gameObject.transform;
            //    onWrongPosition = true;
            //}
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Work Station"))
        {
            onCorrectPosition = false;
            onStation = false;
            ////if (!placed)
            ////    DefectManager.instnace.totalWorkerPlace--;
            //for (int i = 0; i < correctPosition.Length; i++)
            //{
            //    //onCorrectPosition = false;
            //}
        }
    }
    #endregion

    void SetDefaultPosition ()
    {
        defaultPosition = transform.position;
    }

    void _CanInteract ()
    {
        interaction = true;
    }

    void _CannotInteract ()
    {
        if (!placed)
            transform.position = defaultPosition;
        interaction = false;
    }

    void ResetWorkers()
    {
        //transform.position = defaultPosition;
        placed = false;
        GetComponent<Collider2D>().enabled = true;
        interaction = true;
    }
}