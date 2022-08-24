using UnityEngine;

public class Stack<T> : MonoBehaviour
{
    public T[] elements;

    public int topIndex = 0;

    public void Push(T element)
    {
        topIndex += 1;
        elements[topIndex] = element;
    }
    public void Pop() => topIndex -=1;
    public bool IsTop(T element) => element.Equals(Top);
    public T Top => elements[topIndex];
}
