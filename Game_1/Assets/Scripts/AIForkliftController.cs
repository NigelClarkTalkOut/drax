using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIForkliftController : MonoBehaviour
{
    [Header("Parts")]
    [SerializeField] Transform forkliftBody;
    [SerializeField] Transform fork;
    [SerializeField] Transform pole;
    public Transform boxHolder;
    [SerializeField] ForkliftWheel[] wheels;

    [Header("Default Positions")]
    public Vector3 initialPos;

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

    bool isBoxPicked = false;

    bool isReverse = false;
    float sign => isReverse ? -1 : 1;

    WaitForSeconds switchDelay;

    public Box flashingBox;

    private void Start()
    {
        switchDelay = new WaitForSeconds(forkPoleDelay);
        xOffset = transform.position.x - boxHolder.position.x;
        boxHolderInitY = boxHolder.position.y;
        initialPos = initialPos.With(y: boxHolderInitY);
        wheelRotSpeed = moveSpeed * 100;
    }

    public void SetFar()
    {
        transform.localPosition = transform.localPosition.With(x: -16f);
    }

    public void SetNear()
    {
        transform.localPosition = transform.localPosition.With(x: targetPos.x + xOffset - 1.5F);
    }

    public void MoveVehicle()
    {
        if(flashingBox == null)
            return;

        // transform.localPosition = transform.localPosition.With(x: -12f);
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

        isReverse = (targetPos.x == initialPos.x);

        StartCoroutine(nameof(MoveForklift));
    }

    public void SetToStartPoition()
    {
        initialPos.x = -16f;
        targetPos = initialPos;
    }

    private IEnumerator MoveForklift()
    {

        // Move forklift under the box
        if (transform.localPosition.x != forkliftTargetX)
        {
            while (transform.localPosition.x != forkliftTargetX)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, transform.localPosition.With(x: forkliftTargetX), moveSpeed * Time.deltaTime);
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

        // // Pick up / Put down the box based on the condition
        // if (GameManager.instance.selectedPlace != null && GameManager.instance.gameState != GameState.SelectingBox)
        //     GameManager.instance.PutdownBox();

        if (!isBoxPicked && GameManager.instance.flashingBox != null)
        {
            // pickup flashing box
            isBoxPicked = true;
            GameManager.instance.PickupFlashingBox();
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

        if(isBoxPicked)
        {
            // GameManager.instance.flashingBox = null;
            isBoxPicked = false;
            SetToStartPoition();
            MoveVehicle();
        }
        else
        {
            // GameManager.instance.StopFlashingRandomBox();
            flashingBox.StopFlash();
            // SetFar();
        }
    }
}
