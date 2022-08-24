using UnityEngine;

public enum Sides
{
    Left,
    right,
}

public class PathWayPoints : MonoBehaviour
{
    [SerializeField] int number;
    [SerializeField] CreatingPathWays pathwaySystem;
    [SerializeField] Sides sides;

    private void OnMouseDown()
    {
        if (sides == Sides.Left)
        {
            if (!SelectSeed.instance.pathPointForfirstTime)
            {
                SelectSeed.instance.pathPointForfirstTime = true;
            }
            pathwaySystem.leftPoint = number;
        }
        else if (sides == Sides.right)
        {
            pathwaySystem.rightPoint = number;
        }
    }
}