using UnityEngine;
using System;

public class CreatingPathWays : MonoBehaviour
{
    [SerializeField] GameObject pointHolder;

    [Header("PATHS")]
    public GameObject upperPath;
    public GameObject middlePath;
    public GameObject lowerPath;
    public GameObject noPath;

    [Header("NEW PATH'S POINT")]
    public GameObject upperPathPoint;
    public GameObject middlePathPoint;
    public GameObject lowerPathPoint;
    public GameObject noPathPoint;

    [Header ("FOR GLOWING POINTS")]
    [SerializeField] SpriteRenderer leftSidePoint1;
    [SerializeField] SpriteRenderer leftSidePoint2;

    [SerializeField] SpriteRenderer rightSidePoint1;
    [SerializeField] SpriteRenderer rightSidePoint2;
    [SerializeField] SpriteRenderer rightSidePoint3;
    [SerializeField] Color defaultColor;
    [SerializeField] Color selectedColor;


    [Header("DRAWING SYSTEM")]
    public GameObject pathWayHolder;
    public GameObject[] pathWayPoints;

    public int leftPoint;
    public int rightPoint;

    private void OnEnable()
    {
        GameManager.isInEditingMode += ShowThePoints;
        GameManager.isOutOfEditingMode += HideThePoints;

        DeleteButton.pathWayDeleted += ActivatePathwayHolder;
    }

    // path drawing system
    void DrawThepath (GameObject activePath)
    {
        upperPath.SetActive(false);
        middlePath.SetActive(false);
        lowerPath.SetActive(false);
        noPath.SetActive(false);
        // activate the needed path
        activePath.SetActive(true);
    }

    // hide and show the points in edit mode and if the path way is deleted 
    void ShowThePoints ()
    {
        for (int i = 0; i < pathWayPoints.Length; i++)
        {
            pathWayPoints[i].SetActive(true);
        }
    }

    void HideThePoints ()
    {
        for (int i = 0; i < pathWayPoints.Length; i++)
        {
            pathWayPoints[i].SetActive(false);
        }
    }

    void ActivatePathwayHolder()
        => pathWayHolder.SetActive(true);

    // draw the path point according to the points player have selected 
    private void Update()
    {
        if (upperPathPoint == null)
        {
            upperPathPoint = GameObject.FindGameObjectWithTag("upper");
            middlePathPoint = GameObject.FindGameObjectWithTag("Middle");
            lowerPathPoint = GameObject.FindGameObjectWithTag("Lower");
        }
        if (leftPoint == 1 && rightPoint == 1)
        {
            DrawThepath(upperPath);
            upperPathPoint.SetActive(true);

            middlePathPoint.SetActive(false);
            lowerPathPoint.SetActive(false);
        }

        else if (leftPoint == 1 && rightPoint == 2)
        {
            DrawThepath(middlePath);
            middlePathPoint.SetActive(true);

            upperPathPoint.SetActive(false);
            lowerPathPoint.SetActive(false);
        }

        else if (leftPoint == 2 && rightPoint == 3)
        {
            DrawThepath(lowerPath);
            lowerPathPoint.SetActive(true);

            upperPathPoint.SetActive(false);
            middlePathPoint.SetActive(false);
        }

        else
        {
            DrawThepath(noPath);
            upperPathPoint.SetActive(false);
            middlePathPoint.SetActive(false);
            lowerPathPoint.SetActive(false);
        }

        MakePointGlow();
    }

    void MakePointGlow ()
    {
        if (leftPoint == 1)
        {
            leftSidePoint1.color = selectedColor;
            leftSidePoint2.color = defaultColor;
        }

        else if (leftPoint == 2)
        {
            leftSidePoint2.color = selectedColor;
            leftSidePoint1.color = defaultColor;
        }

        if (rightPoint == 1)
        {
            rightSidePoint1.color = selectedColor;

            rightSidePoint2.color = defaultColor;
            rightSidePoint3.color = defaultColor;
        }

        else if (rightPoint == 2)
        {
            rightSidePoint2.color = selectedColor;

            rightSidePoint1.color = defaultColor;
            rightSidePoint3.color = defaultColor;
        }

        else if (rightPoint == 3)
        {
            rightSidePoint3.color = selectedColor;

            rightSidePoint1.color = defaultColor;
            rightSidePoint2.color = defaultColor;
        }
    }

}