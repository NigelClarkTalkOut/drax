using UnityEngine;
using System.Collections;

[System.Serializable]
public class AttackmentPoints
{
    public Transform points;
    public int pointsValue = 1;
}

public class Dragger : MonoBehaviour
{
    InputManager inputInstance;
    Transform crosshair;
    Vector3 originPosition;
    [SerializeField] LineRendererPosition connectingLine;

    [Header("FOR PATH")]
    [SerializeField] BoxCollider2D colliderToDestroy;
    [SerializeField] AttackmentPoints[] attackmentPoint;
    //GameObject trackEndPoint;         // no use
    public bool canDrag = true;                         // if player can draw the path by dragging (mouse/touch)

    private void Awake()
    {
        // get the instance of the crosshair
        inputInstance = FindObjectOfType<InputManager>();
        crosshair = GameObject.FindGameObjectWithTag("Crosshair").transform;
        originPosition = transform.position;
    }

    private void OnEnable()
    {
        // start observing the event 
        GameManager.OnClear += ResetTrack;
        StartCoroutine(nameof(updatingLoop));       // start the coroutine 
    }

    private void OnDisable()
    {
        // stop observing the event 
        GameManager.OnClear -= ResetTrack;
        StopCoroutine(nameof(updatingLoop));        // end the coroutine 
    }

    // update loop
    IEnumerator updatingLoop()
    {
        while (true)
        {
            // player can drag the end point 
            MovePoints();
            yield return null;
        }
    }

    // move points according to the points
    private void MovePoints()
    {
        // the dragger will follow the crosshair
        if (Input.GetMouseButtonUp(0) && canDrag)
        {
            transform.position = originPosition;
            return;
        }
        else if (Input.GetMouseButton(0) && canDrag && inputInstance.canDraw)
        {
            transform.position = crosshair.position;
        }
    }

    // if the obj hits the next point 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // from multiple attackment points [point first]
        for (int i = 0; i < attackmentPoint.Length; i++)
        {
            if (collision.gameObject.name == attackmentPoint[i].points.name)
            {
                //BoxCollider2D connectingCollider = connectingLine.gameObject.AddComponent<BoxCollider2D>();
                //connectingCollider.isTrigger = true;
                //colliderToDestroy = connectingCollider;
                connectingLine.isConnected = true;
                AttachmentSystem(i);
                // if the attachment point is the last one 
                if (collision.CompareTag("Finish"))
                    GameManager.instance.startButton.interactable = true;
            }
        }
    }

    void AttachmentSystem (int count)
    {
        // adding more point to tug system incrementing the index
        TugSystem.instance.updatingIndex++;
        TugSystem.instance.points[TugSystem.instance.updatingIndex] = attackmentPoint[count].points;

        // get how many point tug has covered 
        TugSystem.instance.routeCount += attackmentPoint[count].pointsValue;

        // stop dragging and snawp to the attachment point position 
        canDrag = false;
        transform.position = attackmentPoint[count].points.position;

        // set the game obj for the next track on
        attackmentPoint[count].points.GetChild(0).gameObject.SetActive(true);
        attackmentPoint[count].points.GetComponent<CircleCollider2D>().enabled = false;
    }

    void ResetTrack()
    {
        connectingLine.isConnected = false;
        GameManager.instance.startButton.interactable = false;
        //if (colliderToDestroy == null)
        //    return;
        //Destroy(colliderToDestroy);
    }
}