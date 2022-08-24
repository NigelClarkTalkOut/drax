using UnityEngine;

public class ConveyorObjSpawner : MonoBehaviour
{
    [SerializeField] GameObject objectParts;
    [SerializeField] Vector3 objDirection;
    [SerializeField] bool rightAngle = false;

    private void Start()
    {
        InvokeRepeating(nameof(SpawnParts), 0.3f, Random.Range(1.3f, 2f));
    }

    void SpawnParts ()
    {
        if (rightAngle)
        {
            GameObject obj = Instantiate(objectParts, transform.position, Quaternion.Euler(0, 0, 90));
            obj.GetComponent<Rigidbody2D>().velocity = objDirection;
        }
        else
        {
            GameObject obj = Instantiate(objectParts, transform.position, Quaternion.identity);
            obj.GetComponent<Rigidbody2D>().velocity = objDirection;
        }

    }
}