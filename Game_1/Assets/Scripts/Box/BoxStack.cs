using UnityEngine;

public class BoxStack : Stack<Box>
{
    // public BoxType stackType;

    private void Start()
    {
        elements = GetComponentsInChildren<Box>();
        SetStackTop();
    }

    public void FallBoxesFrom(Box selectedBox)
    {
        int index = 0;
        for (int i = 0; i <= topIndex; i++)
        {
            if (selectedBox.Equals(elements[i]))
            {
                index = i;
                break;
            }
        }
        Rigidbody2D rb;
        for (int i = index; i <= topIndex; i++)
        {
            elements[i].gameObject.layer = 8;
            elements[i].spriteRenderer.sortingLayerName = "Box Animating";
            rb = elements[i].gameObject.AddComponent<Rigidbody2D>();
            rb.angularDrag = 10f;
            rb.AddForce(ExtentionMethods.GetRandomLeftRight(), ForceMode2D.Impulse);
        }
    }

    public void SetStackTop() => topIndex = elements.Length - 1;

    public bool IsEmpty() => topIndex == -1;
}
