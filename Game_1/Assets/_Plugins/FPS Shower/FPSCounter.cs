using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    public Text fpsText;

    int fps = 0;
    float time = 0f;

    private void Update()
    {
        if(time >= 1)
        {
            fpsText.text = $"FPS : {fps}";
            time = 0;
            fps = 0;
        }

        time += Time.deltaTime;
        fps += 1;
    }
}
