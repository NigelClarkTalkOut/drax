using System;
using UnityEngine;

public class Boxes : MonoBehaviour
{
    Camera mainCamera;
    Vector2 mousePosition;

    Vector3 defaultPosition;
    [SerializeField] GameObject correctPosition;
    [SerializeField] Collider2D nextCollider;
    [SerializeField] bool onPosition = false;

    private void Start()
    {
        defaultPosition = transform.position;
    }

    private void OnMouseDrag()
    {
        transform.position = GetMousePosition();
    }

    private void OnMouseUp()
    {
        if (onPosition)
        {
            transform.position = correctPosition.transform.position;
            GetComponent<Collider2D>().enabled = false;
            return;
        }
        // if not on correct position, return to the default position 
        transform.position = defaultPosition;
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

    #region Position Check
    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.name == correctPosition.name)
        {
            onPosition = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name == correctPosition.name)
        {
            onPosition = false;
        }
    }
    #endregion
}