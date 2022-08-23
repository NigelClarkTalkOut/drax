using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instane;

    [SerializeField] AudioClip sucessSound;
    [SerializeField] AudioSource sucessSource;

    private void Awake()
    {
        instane = this;
    }

    private void OnEnable()
    {
        Item.Itemplaced += PlaySucessSound;
    }

    private void OnDisable()
    {
        Item.Itemplaced -= PlaySucessSound;
    }

    void PlaySucessSound()
    {
        sucessSource.PlayOneShot(sucessSound);
    }
}