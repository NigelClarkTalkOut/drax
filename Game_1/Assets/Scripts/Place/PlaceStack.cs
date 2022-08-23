
public class PlaceStack : Stack<Place>
{
    public Place flashingPlace;
    public int minimumLevelIndex;

    private void Start()
    {
        elements = GetComponentsInChildren<Place>();
        SetStackTop();
    }

    public bool CanPlaceTo(Place place) =>  place.isAvailable && IsTop(place) && place.type == GameManager.instance.selectedBox.type && !IsAnyBoxFlashing();

    public void StartFlashingTopBox()
    {
        flashingPlace = PossibleFlashPlace();
        flashingPlace.StartFlashing();
    }

    public void StopFlashingTopBox()
    {
        flashingPlace.StopFlashing();
    }

    public void CancleFlashingBox()
    {
        flashingPlace.CancleFlashingBox();
    }

    public Place PossibleFlashPlace() => elements[topIndex + 1];

    public bool IsBelowMinimumLevel() => elements[minimumLevelIndex].isAvailable;

    public bool IsAnyBoxFlashing()
    {
        bool value = false;

        for (int i = 0; i < elements.Length; i++)
        {
            if (elements[i].isFlashing)
            {
                value = true;
                break;
            }
        }

        return value;
    }

    public void SetStackTop()
    {
        for (int i = 0; i < elements.Length; i++)
        {
            elements[i].Reset();

            if (elements[i].isAvailable)
                topIndex = i;
        }
    }
}
