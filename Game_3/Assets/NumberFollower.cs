using UnityEngine;

public class NumberFollower : MonoBehaviour
{
    [SerializeField] GameObject numberHolder;

    [Header ("Numbers")]
    [SerializeField] Transform number1;
    [SerializeField] Transform number2;
    [SerializeField] Transform number3;

    [Header ("Numbers Holder")]
    [SerializeField] Transform number1Holder;
    [SerializeField] Transform number2Holder;
    [SerializeField] Transform number3Holder;

    private void OnEnable()
    {
        numberHolder.SetActive(true);
    }

    private void OnDisable()
    {
        numberHolder.SetActive(false);
    }

    private void Update()
    {
        number1.position = number1Holder.position;
        number2.position = number2Holder.position;
        number3.position = number3Holder.position;
    }
}