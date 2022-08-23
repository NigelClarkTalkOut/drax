using UnityEngine;

public class BackgroundSound : MonoBehaviour
{
    public static BackgroundSound instance;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
            instance = this;
        DontDestroyOnLoad(gameObject);
    }
}