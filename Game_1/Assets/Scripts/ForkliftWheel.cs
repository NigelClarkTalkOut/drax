using UnityEngine;

public class ForkliftWheel : MonoBehaviour
{
    float scale;
    Vector3 curRot;

    private void Start()
    {
        curRot = transform.localRotation.eulerAngles;
        scale = transform.localScale.x;
    }

    public void RotateWheel(float speed)
    {
        curRot.z += Time.deltaTime * speed / scale;
        transform.localRotation = Quaternion.Euler(curRot);
    }
}
