using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SelectSeed : MonoBehaviour
{
    [SerializeField] Text loadingText;
    [SerializeField] string[] gameScene;

    private void Start()
    {
        Invoke(nameof(ChooseSeed), Random.Range(1, 2.8f));
    }

    public void _First()
    {
        loadingText.text = $"Loading";
    }
    public void _Second()
    {
        loadingText.text = $"Loading.";
    }
    public void _Third()
    {
        loadingText.text = $"Loading..";
    }
    public void _Forth()
    {
        loadingText.text = $"Loading...";
    }

    void ChooseSeed ()
    {
        int count = Random.Range(0, gameScene.Length);
        SceneManager.LoadScene(gameScene[count]);
    }
}