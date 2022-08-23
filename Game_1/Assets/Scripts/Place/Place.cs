using UnityEngine;

public class Place : MonoBehaviour
{
    public BoxType type;
    public bool isAvailable
    {
        get
        {
            if (box == null)
            {
                box = GetComponentInChildren<Box>();
            }
            return box == null;
        } 
    }
    public PlaceStack parentStack;

    public Box box = null;

    public bool isFlashing = false;

    private void Start()
    {
        parentStack = GetComponentInParent<PlaceStack>();
    }

    public void Select()
    {
        if (parentStack.CanPlaceTo(this))
        {
            GameManager.instance.MoveForkliftToPlace(this);
        }
        else
        {
            GameManager.instance.ShowSelectPlaceWarning();
        }
    }

    public void StartFlashing() 
    {
        if(!isAvailable)
        {
            isFlashing = true;
            box.StartFlash();
        }
    }

    public void StopFlashing()
    {
        isFlashing = false;
        if(!isAvailable)
            box.CancleFlashing();

        // Reset();
        parentStack.Push(this);
    }

    public void DeleteBox()
    {
        box.StopFlash();
        Reset();
    }

    public void CancleFlashingBox()
    {
        isFlashing = false;
        if(!isAvailable)
            box.CancleFlashing();
    }

    public void Reset()
    {
        box = null;
        isFlashing = false;
    }
}
