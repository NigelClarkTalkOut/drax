using UnityEngine;

public class InputController : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] LayerMask rayLayerMask;
    Vector3 origin;
    GameObject hitObject;

    Box box;
    Place place;

    float inactiveTime = 0f;

    private void Update()
    {
        if(!GameManager.instance.CanSelect)
        {
            inactiveTime = 0f;
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            inactiveTime = 0f;

            origin = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Ray ray = new Ray(origin, Vector3.forward);

            RaycastHit2D hit = Physics2D.Raycast(origin, Vector3.forward, 50f, rayLayerMask);

            if (hit)
            {
                hitObject = hit.collider.gameObject;

                if (GameManager.instance.gameState == GameState.SelectingBox && hitObject.TryGetComponent<Box>(out box))
                    GameManager.instance.SelectBox(box);
                // GameManager.instance.MoveForkliftToBox(hitObject.GetComponent<Box>());
                else if (GameManager.instance.gameState == GameState.SelectingPlace && hitObject.TryGetComponent<Place>(out place))
                    GameManager.instance.SelectPlace(place);
                // GameManager.instance.MoveForkliftToPlace(hitObject.GetComponent<Place>());
            }
        }
        else
        {
            // count for 10 seconds
            inactiveTime += Time.deltaTime;

            if(inactiveTime >= 10f)
            {
                GameManager.instance.PauseGame();
            }
        }
    }
}
