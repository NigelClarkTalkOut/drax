using UnityEngine;

public class TurnColliderOn : MonoBehaviour
{
    [SerializeField]
    CircleCollider2D pointCollider;

    private void Start()
        => pointCollider = gameObject.GetComponent<CircleCollider2D>();

    private void OnEnable()
        => GameManager.OnClear += GetColliderBack;


    private void OnDisable()
        => GameManager.OnClear -= GetColliderBack;

    void GetColliderBack()
        => pointCollider.enabled = true;
}