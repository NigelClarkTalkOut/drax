using UnityEngine;

public class Draggable : MonoBehaviour
{
    Camera mainCamera;                      // refrance to main camera
    GameManager gameManagerInstance;
    [SerializeField] GameObject dragableHolder;         // placing the dragable object
    [SerializeField] SpriteRenderer redGrid;            // for indicating player cannot place the sub assembly
    [SerializeField] GameObject deleteThePathWay;       // for indicating player cannot place the sub assembly

    bool canDrag = true;                    // if the object is on corret position or not 
    public bool onHolder = false;           // if the game object is on place or not

    // mouse inputs and outputs 
    Vector2 mousePosition;                  // mouse position 
    Vector2 offsetPosition;                 // offset of mouse position and the game object 
    Vector2 originPosition;                 // the object orogin point
    Quaternion originalRotation;

    private void Awake()
    {
        //dragableHolder.GetComponent<SpriteRenderer>().enabled = false;
        // get the camera instance 
        mainCamera = Camera.main;
        // get the path way button
    }

    private void Start()
    {
        originPosition = transform.position;    // origin position or the object
        originalRotation = transform.rotation;
    }

    public void Update()
    {
        if (deleteThePathWay == null)
            return;
        if (deleteThePathWay != null)
            deleteThePathWay.SetActive(false);
    }

    private void OnMouseDown()
    {
        // get the offset
        if (GameManager.instance.isEditing && canDrag)
        {
            offsetPosition = (Vector2)transform.position - GetMousePosition();
            dragableHolder.GetComponent<SpriteRenderer>().enabled = true;           // hints for placing will be on
            redGrid.enabled = true;
        }
    }

    private void OnMouseDrag()
    {
        // transform the position to mouse position 
        if (GameManager.instance.isEditing && canDrag)
            transform.position = GetMousePosition() + offsetPosition;
    }

    private void OnMouseUp()
    {
        redGrid.enabled = false;
        // get to the original position
        if (GameManager.instance.isEditing && canDrag)
        {
            // hints for placing will be off 
            dragableHolder.GetComponent<SpriteRenderer>().enabled = false;
            // if the object is not on place then replace it to original position 
            if (!onHolder)
            {
                transform.position = originPosition;
                transform.rotation = originalRotation;
            }
            else
            {
                GameManager.instance.subAssemblyMoved++;
                SelectSeed.instance.movedSubassemblies = true;
                // if the object is on place then player cannnot drag anymore and it will be at corret place 
                transform.position = dragableHolder.transform.position;
                canDrag = false;        // player cannot drag it another time
                Destroy(this);
            }
        }
    }

    Vector2 GetMousePosition ()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
        // get mouse position 
        mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        return mousePosition;
    }

    // if the object is on the right place 
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.name == dragableHolder.name)
        {
            onHolder = true;
            // snap the sub assembly to the correct position
            transform.rotation = dragableHolder.transform.rotation;
            redGrid.enabled = false;            // indicate the player with the red grid
        }
    }

    // if the object is not on the right place 
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == dragableHolder.name)
        {
            onHolder = false;
            redGrid.enabled = true;         // indicate the player with red grid 
        }
    }
}