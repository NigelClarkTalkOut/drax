using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioClip sucessSound;
    [SerializeField] AudioSource sucessSource;

    private void OnEnable()
    {
        // task one and two
        TaskSlot.playSound += PlaySucessSound;
        Item.playSound += PlaySucessSound;

        // task three
        Worker.workerPlaced += PlaySucessSound;
        Worker.workerPlacedIncorrect += PlaySucessSound;
    }

    private void OnDisable()
    {
        // task one and two
        TaskSlot.playSound -= PlaySucessSound;
        Item.playSound -= PlaySucessSound;

        // task three
        Worker.workerPlaced -= PlaySucessSound;
        Worker.workerPlacedIncorrect -= PlaySucessSound;
    }

    void PlaySucessSound()
    {
        sucessSource.PlayOneShot(sucessSound);
    }
}