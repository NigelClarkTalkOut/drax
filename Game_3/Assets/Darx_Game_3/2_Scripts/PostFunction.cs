using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Runtime.InteropServices;
using System.Web;
using System.Text;

[System.Serializable]
public struct PostData
{
    public MetaData metadata;
}

[System.Serializable]
public struct MetaData
{
    public bool isGameComplete;
}

public class PostFunction : MonoBehaviour
{
    public static PostFunction instance;
    [DllImport("__Internal")]
    private static extern string GetURLFromPage();

    [SerializeField] GameObject openPanelButton;
    [SerializeField] GameObject closePanelButton;

    //[SerializeField] Text outputArea;
    string postUrl;
    private void Start()
    {
#if UNITY_WEBGL
        postUrl = GetURLFromPage();
        // Debug.Log(postUrl);
        postUrl = postUrl.Split('?')[1];
        // Debug.Log(postUrl);
        postUrl = postUrl.Split('=')[1];
        // Debug.Log(postUrl);
        postUrl = HttpUtility.UrlDecode(postUrl);
#endif
    }

    private void Awake()
    {
        instance = this;
    }

    public void PostData()
    {
        StartCoroutine(PostDate_Coroutine());
    }

    IEnumerator PostDate_Coroutine()
    {
        PostData postData;
        postData.metadata.isGameComplete = true;

        string jsonData = JsonUtility.ToJson(postData);
        Debug.Log($"Post URL : {postUrl}");
        Debug.Log($"Raw Data : {jsonData}");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest request = new UnityWebRequest(postUrl, "POST");

        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

        //outputArea.text = "Processing...";

        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.DataProcessingError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log("FAIL");
            //outputArea.text = request.error;
        }
        else if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("SUCESS");
            //outputArea.text = jsonData;
        }

    }

    public void __OpenPanel()
    {
        openPanelButton.SetActive(false);
        closePanelButton.SetActive(true);

        PostData();
    }

    public void __ClosePanel()
    {
        closePanelButton.SetActive(false);
        openPanelButton.SetActive(true);
    }
}