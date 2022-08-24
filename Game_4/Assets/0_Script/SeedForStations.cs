using UnityEngine;

public class SeedForStations : MonoBehaviour
{
    [Header ("Order Defect")]
    [SerializeField] RectTransform wireTask;
    [SerializeField] RectTransform shoulderTask;
    [SerializeField] RectTransform armTask;

    [Header("Health Defect")]
    [SerializeField] Transform DAD_UpperBox;
    [SerializeField] Transform DAD_LowerBox;
    [SerializeField] Transform C_UpperBox;
    [SerializeField] Transform C_LowerBox;

    [Header("Health Defect Correct Position")]
    [SerializeField] Transform[] correctPositinUpperBox;
    [SerializeField] Transform[] correctPositinLowerBox;


    private void Start()
    {
        int countOrderDefect = Random.Range(0, 4);

        if (countOrderDefect == 0)
            Seed1();
        else if (countOrderDefect == 1)
            Seed2();
        else if (countOrderDefect == 2)
            Seed3();
        else if (countOrderDefect == 3)
            Seed4();

        int countHealthDefect = Random.Range(0, 2);

        if (countHealthDefect == 0)
            Seed1HealthDefect();
        else if (countHealthDefect == 1)
            Seed2HealthDefect();
    }

    #region Order Defect Seed
    void Seed1 ()
    {
        wireTask.anchoredPosition = new Vector3(0, 140, 0);
        shoulderTask.anchoredPosition = new Vector3(0, -140, 0);
        armTask.anchoredPosition = new Vector3(0, 0, 0);
    }

    void Seed2 ()
    {
        wireTask.anchoredPosition = new Vector3(0, -140, 0);
        shoulderTask.anchoredPosition = new Vector3(0, 0, 0);
        armTask.anchoredPosition = new Vector3(0, 140, 0);
    }

    void Seed3 ()
    {
        wireTask.anchoredPosition = new Vector3(0, 140, 0);
        shoulderTask.anchoredPosition = new Vector3(0, 0, 0);
        armTask.anchoredPosition = new Vector3(0, -140, 0);
    }

    void Seed4 ()
    {
        wireTask.anchoredPosition = new Vector3(0, -140, 0);
        shoulderTask.anchoredPosition = new Vector3(0, 140, 0);
        armTask.anchoredPosition = new Vector3(0, 0, 0);
    }
    #endregion

    #region Health Defect Seed
    void Seed1HealthDefect ()
    {
        // for drang and drop box
        DAD_UpperBox.position = correctPositinUpperBox[0].position;
        DAD_LowerBox.position = correctPositinLowerBox[0].position;

        // for click and follow
        C_UpperBox.position = correctPositinUpperBox[0].position;
        C_LowerBox.position = correctPositinLowerBox[0].position;
    }

    void Seed2HealthDefect ()
    {
        // for drang and drop box
        DAD_UpperBox.position = correctPositinUpperBox[1].position;
        DAD_LowerBox.position = correctPositinLowerBox[1].position;

        // for click and follow
        C_UpperBox.position = correctPositinUpperBox[1].position;
        C_LowerBox.position = correctPositinLowerBox[1].position;
    }
    #endregion
}