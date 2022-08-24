using System.Collections;
using UnityEngine;

public class ForkliftController : MonoBehaviour
{
    [Header("Parts")]
    [SerializeField] Transform forkliftBody;
    [SerializeField] Transform fork;
    [SerializeField] Transform pole;
    public Transform boxHolder;
    [SerializeField] ForkliftWheel[] wheels;
    
    [Header("Default Positions")]
    public Vector3 selectBoxPos;
    public Vector3 selectPlacePos;

    [Header("Speeds")]
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] float liftingSpeed = 2f;
    [SerializeField] float wheelRotSpeed = 2f;

    [Header("Max Values")]
    [SerializeField] float forkMaxY = .67f;
    [SerializeField] float poleMaxY = .96f;
    [SerializeField] float forkPoleDelay = .5f;

    [Header("Set at runtime")]
    public float xOffset;
    public float boxHolderInitY;
    public Vector3 targetPos = new Vector3();

    float forkliftTargetX;
    float forkTargetY;
    float poleTargetY;

    bool isReverse = false;
    float sign => isReverse ? -1 : 1;

    public float curSpeed;

    WaitForSeconds switchDelay;

    private void Start()
    {
        SetupSpeeds(.70f);
        switchDelay = new WaitForSeconds(forkPoleDelay);
        xOffset = transform.position.x - boxHolder.position.x;
        boxHolderInitY = boxHolder.position.y;
        selectBoxPos = selectBoxPos.With(y: boxHolderInitY);
        selectPlacePos = selectPlacePos.With(y: boxHolderInitY);
    }

    public void SetupSpeeds(float ratio)
    {
        curSpeed = moveSpeed * ratio;
        wheelRotSpeed = curSpeed * 100;
    }

    public void MoveVehicle()
    {
        forkliftTargetX = targetPos.x + xOffset;
        float targetY = targetPos.y - boxHolderInitY;

        if (targetY > forkMaxY)
        {
            forkTargetY = forkMaxY;
            poleTargetY = targetY - forkMaxY;
        }
        else
        {
            forkTargetY = targetY;
            poleTargetY = 0f;
        }

        isReverse = (targetPos.x == selectBoxPos.x);

        StartCoroutine(nameof(MoveForklift));
    }

    public void StopVehicle()
    {
        StopCoroutine(nameof(MoveForklift));
    }

    public void SetToStartPoition()
    {
        selectBoxPos.x = 0.608f;
        targetPos = selectBoxPos;
    }

    private IEnumerator MoveForklift()
    {

        // Move forklift under the box
        if (transform.localPosition.x != forkliftTargetX)
        {
            while (transform.localPosition.x != forkliftTargetX)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, transform.localPosition.With(x: forkliftTargetX), curSpeed * Time.deltaTime);
                // rotate wheels
                for (int i = 0; i < wheels.Length; i++)
                {
                    wheels[i].RotateWheel(wheelRotSpeed * sign);
                }
                // Debug.Log("Moving Lift");
                yield return null;
            }
        }

        if (fork.localPosition.y != forkTargetY)
        {
            // Move fork up to the box
            yield return switchDelay;
            while (fork.localPosition.y != forkTargetY)
            {
                fork.localPosition = Vector3.MoveTowards(fork.localPosition, fork.localPosition.With(y: forkTargetY), liftingSpeed * Time.deltaTime);
                // Debug.Log("Moving Fork");
                yield return null;
            }
        }

        if (pole.localPosition.y != poleTargetY)
        {
            // Move pole up to the box
            yield return switchDelay;
            while (pole.localPosition.y != poleTargetY)
            {
                pole.localPosition = Vector3.MoveTowards(pole.localPosition, pole.localPosition.With(y: poleTargetY), liftingSpeed * Time.deltaTime);
                // Debug.Log("Moving Pole");
                yield return null;
            }
        }

        // Pick up / Put down the box based on the condition
        if (GameManager.instance.selectedPlace != null && GameManager.instance.gameState != GameState.SelectingBox)
            GameManager.instance.PutdownBox();

        else if (GameManager.instance.selectedBox != null && GameManager.instance.gameState != GameState.SelectingPlace && !GameManager.instance.isImproving)
        {
            if (!GameManager.instance.CanPickupBox())
                yield break;
        }

        if (pole.localPosition.y != 0f)
        {
            // Move pole down to initial position
            yield return switchDelay;
            while (pole.localPosition.y != 0f)
            {
                pole.localPosition = Vector3.MoveTowards(pole.localPosition, pole.localPosition.With(y: 0f), liftingSpeed * Time.deltaTime);
                // Debug.Log("Moving Pole");
                yield return null;
            }
        }

        if (fork.localPosition.y != 0f)
        {
            // Move fork down to initial position
            yield return switchDelay;
            while (fork.localPosition.y != 0f)
            {
                fork.localPosition = Vector3.MoveTowards(fork.localPosition, fork.localPosition.With(y: 0f), liftingSpeed * Time.deltaTime);
                // Debug.Log("Moving Fork");
                yield return null;
            }
        }

        if (GameManager.instance.gameState == GameState.BoxPicked && !GameManager.instance.isImproving)
        {
            GameManager.instance.gameState = GameState.SelectingPlace;
            // GameManager.instance.HandleBoxFlashing();
            targetPos = selectPlacePos;
            // Debug.Log("Goto select place pos");
            MoveVehicle();
            yield break;
        }
        else if (GameManager.instance.gameState == GameState.BoxPlaced && !GameManager.instance.isImproving)
        {
            GameManager.instance.gameState = GameState.SelectingBox;
            selectBoxPos.x = GameManager.instance.GetSelectBoxPosition();
            targetPos = selectBoxPos;

            GameManager.instance.CheckForBelowMinimumLevel();

            MoveVehicle();
            yield break;
        }
        else
        {
            if (GameStartController.instance.guideRackingArea)
            {
                GameManager.instance.GuideRackingArea();
            }
            else
            {
                GameManager.instance.FlashInitialBoxes();
                
                GameManager.instance.CheckLineStatus();
                GameManager.instance.canInteract = true;
            }
            
            // if(GameManager.instance.isImproving && selectBoxPos.x != 0.608f)
            // {
            //     if(targetPos != selectBoxPos)
            //     {
            //         SetToStartPoition();

            //         MoveVehicle();
            //     }
            // }
        }
    }
}
