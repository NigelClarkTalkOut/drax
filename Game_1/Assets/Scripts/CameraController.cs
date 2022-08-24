using System.Runtime.InteropServices;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Vector3 offset;
    [SerializeField] float speed;

    [Header("")]
    [SerializeField] SpriteRenderer background;

    Vector3 targetPos;

    Camera MainCamera;

    private void Awake()
    {
        MainCamera = GetComponent<Camera>();
    }

    private void Start()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        SetGameView();
#endif
    }

    private void Update()
    {
        SetGameView();
    }
    private void LateUpdate()
    {
        targetPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPos, speed * Time.deltaTime);
    }

    void SetGameView()
    {
        float orthoSize = (background.bounds.size.y / 2) - .15f;

        MainCamera.orthographicSize = orthoSize;
    }
}
