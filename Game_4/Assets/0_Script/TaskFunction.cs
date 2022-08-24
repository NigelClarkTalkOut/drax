using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// will enable the player to drag and drop task in correct order.
/// </summary>
public class TaskFunction : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    RectTransform uiTransform;
    CanvasGroup canvasGroup;

    public bool itemSet = false;
    public bool correctItem = false;
    public int correctTaskIndex = 0;
    public GameObject correctTaskIndicater;
    public Transform pickedUpHolder;
    public Transform defaultListHolder;

    [SerializeField] Transform defaultParent;
    [SerializeField] Canvas rearrangingCanvas;
    [SerializeField] Vector3 defaultPosition;

    private void Awake()
    {
        uiTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        UIManager.OnReset += ResetFunction;
    }

    private void OnDisable()
    {
        UIManager.OnReset -= ResetFunction;
    }

    private void Start()
    {
        correctTaskIndicater.SetActive(false);
        defaultPosition = uiTransform.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.SetParent(pickedUpHolder);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (itemSet)
            return;
        uiTransform.anchoredPosition += eventData.delta / rearrangingCanvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!itemSet)
        {
            transform.SetParent(defaultListHolder);
            uiTransform.anchoredPosition = defaultPosition;
        }
        canvasGroup.blocksRaycasts = true;
    }

    public void GetDefaultPosition()
        => uiTransform.anchoredPosition = defaultPosition;


    void ResetFunction ()
    {
        itemSet = false;
        correctItem = false;
        transform.SetParent(defaultParent);
        correctTaskIndicater.SetActive(false);
        uiTransform.anchoredPosition = defaultPosition;
    }
}