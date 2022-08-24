using System;
using UnityEngine;

public class LineRendererPosition : MonoBehaviour
{
    [Header("COMPONENTS")]
    Dragger pointDraggerInstance;                   //point script 
    [SerializeField] LineRenderer lineRenderer;     // line renderer for the track
    public static Action SubTime;
    public bool isConnected = false;      // line renderer for the track
    [SerializeField] bool gotCollider = false;      // line renderer for the track
    bool addDistance = true;      // line renderer for the track

    [Header("TRANSFORM POINTS")]
    public Transform point1;                        // origin for the track 
    [SerializeField] Transform point2;              // end point to connect the other point

    [SerializeField] LayerMask layer;               // layer to check on
    [SerializeField] Vector3 direction;             // direction for raycast
    RaycastHit2D hit;

    private void Awake()
    {
        // get the component 
        pointDraggerInstance = transform.GetChild(2).GetComponent<Dragger>();
    }
    
    private void OnEnable()
    {
        // start observing the event 
        GameManager.OnClear += TrackReset;
        GameManager.startTheTug+= StopShootingRay;
    }

    private void OnDisable()
    {
        // stop observing the event 
        GameManager.OnClear -= TrackReset;
        GameManager.startTheTug+= StopShootingRay;
    }

    // updating line renderer point's position
    private void Update()
    {
        lineRenderer.SetPosition(0, point1.position);
        lineRenderer.SetPosition(1, point2.position);
        direction = point2.position - point1.position;
        if (isConnected && !gotCollider)
        {
            hit = Physics2D.Raycast(point1.position, direction.normalized, direction.magnitude, layer);
            if (addDistance)
            {
                GameManager.instance.pathLenght += (int)direction.magnitude;
                addDistance = false;
            }
            if (hit)
            {
                PathChecker checkerInstance = hit.collider.GetComponent<PathChecker>();
                checkerInstance.checkTrigger = true;
                if (checkerInstance.triggerToSub)
                    SubTime?.Invoke();
                hit.collider.enabled = false;
            }
        }
    }

    void StopShootingRay ()
        => gotCollider = false;

    // reset the track system 
    void TrackReset()
    {
        // reset the points of track
        point2.position = point1.position;
        pointDraggerInstance.canDrag = true;        // the player can drag again

        // disable the dragging points
        gameObject.SetActive(false);
        addDistance = true;
        gotCollider = false;
    }
}