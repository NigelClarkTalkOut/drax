using UnityEngine;
using System.Collections;

public class GameStartController : MonoBehaviour
{
    public static GameStartController instance;
    public bool guideLoadingArea = true;
    public bool guideRackingArea = true;
    public int seedNumber;
    public bool isSameSeed = false;

    [Header("Audio")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip bellAudioClip;
    // [SerializeField] AudioSource backgroundAudioSource;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
    }

    // public void StartBackgrounAudio()
    // {
    //     StartCoroutine(nameof(StartAudio));
    // }

    // IEnumerator StartAudio()
    // {
    //     yield return new WaitForEndOfFrame();
        
    //     if(!backgroundAudioSource.isPlaying)
    //         backgroundAudioSource.Play();
    // }

    public int GetSeedNumber(int max)
    {
        int temp = Random.Range(0, max);
        if (temp == max)
            temp -= 1;

        // if same as previous seed then Get other seed
        if (temp == seedNumber)
            GetSeedNumber(max);
        else
            seedNumber = temp;

        return seedNumber;
    }

    public void PlayBellSound()
    {
        audioSource.PlayOneShot(bellAudioClip);
    }
}
