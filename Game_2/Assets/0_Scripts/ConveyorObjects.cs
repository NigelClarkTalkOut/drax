using UnityEngine;

public class ConveyorObjects : MonoBehaviour
{

    [SerializeField] SpriteRenderer partRenderer;
    [SerializeField] Sprite[] part;

    private void Start()
    {
        partRenderer.sprite = part[Random.Range(0, part.Length)];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag ("Delete"))
        {
            Destroy(gameObject);
        }
    }
}