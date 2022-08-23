using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // text changes
    [SerializeField] Text textChanges;

    public static GameManager instance;
    public static Action OnClear;                       // clearning the track system
    public static Action isInEditingMode;               // if the player is editing 
    public static Action startTheTug;                   // if the player is editing 
    public SelectSeed selectSeed;
    public GameObject trackSystem;
    public static Action isOutOfEditingMode;                    // if the player is out of editing mode 
    public GameObject congrulationpannel;
    public int pathLenght;
    [SerializeField] GameObject deletePathWayButton;

    public int packagesLeft = 3;
    public int subAssemblyMoved = 1;
    // starting point and rot of the game object 
    Quaternion tugRotation;                             // tug's origin rotation 
    [SerializeField] Vector3 tugOrigin;                 // tug's origin point afer reset
    [SerializeField] GameObject startingPoint;          // starting point of the track system
    [SerializeField] Transform tugObj;                  // tug main game obj
    [SerializeField] Color normalColor, editingColor;   // for indicating user is editing or not 

    [Header("FOR BUTTONS")]
    public bool isEditing = false;                  // is user editing or not
    public bool gameCompleted = false;              // is game comepleted or not 
    public Button startButton;                      // for starting the tug movement

    Image editButtonSprite;                         // changin color for button;
    [SerializeField] Button clearButton;            // clear button for disabling for editing
    [SerializeField] Button editButton;             // user can start editing the environment


    private void Awake()
    {
        //deletePathWayButton = GameObject.FindGameObjectWithTag("DeletePathway");
        tugOrigin = tugObj.position;
        // for performance 
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

        instance = this;
        selectSeed = SelectSeed.instance;
        startButton.interactable = false;       // turning of the start button
        editButtonSprite = editButton.GetComponent<Image>();    // get the button's image for color
        isOutOfEditingMode?.Invoke();
    }

    public void __Start()
    {
        startTheTug?.Invoke();
        // if the tug is never moved in the game 
        if (!SelectSeed.instance.tugMoved)
        {
            SelectSeed.instance.tugMoved = true;
            // show the tip for tug 
            UIManager.instance.ChangePannel(UIManager.instance.startingTugTip);
        }
        // start button and edit button cannot work after tug is moving 
        startButton.interactable = false;
        editButton.interactable = false;        // edit button
        clearButton.interactable = false;
    }

    // clean button 
    public void __ClearButton() => ClearButtonFunction();

    // clear button function 
    void ClearButtonFunction ()
    {
        pathLenght = 0;
        // check of the listners
        OnClear?.Invoke();
        // activate the starting point 
        startingPoint.SetActive(true);
        if (SelectSeed.instance.phases == SelectSeed.Phases.improvementPhase)
            editButton.interactable = true;     // player can edit the track on clearing the track 
        tugObj.SetPositionAndRotation(tugOrigin, tugRotation);
    }

    // for editing tracks 
    public void __EditButton ()
    {
        ClearButtonFunction();      // clear the track before editing

        // if already in edit mode 
        if (isEditing)
        {
            if (!SelectSeed.instance.removedRackings || !SelectSeed.instance.movedSubassemblies || !SelectSeed.instance.deleteThepathWay)
            {
                UIManager.instance.ChangePannel(UIManager.instance.removeRackings);
                return;
            }

            if (!SelectSeed.instance.firstTimeEdit)
            {
                UIManager.instance.ChangePannel(UIManager.instance.station2TimePanel);
                SelectSeed.instance.firstTimeEdit = true;
            }
             
            trackSystem.SetActive(true);
            // hide all he pathway points
            isOutOfEditingMode?.Invoke();
            // user can edit the tracks 
            isEditing = false;
            editButtonSprite.color = normalColor;       // change the color
            clearButton.interactable = true;            // user can clear the track 
            startingPoint.SetActive(true);              // get the track starting point on 
        }
        // if not in edit mode 
        else if (!isEditing)
        {
            trackSystem.SetActive(false);
            isInEditingMode?.Invoke();
            // user cannot edit the track 
            isEditing = true;
            editButtonSprite.color = editingColor;      // change the color
            clearButton.interactable = false;           // cannot clear the tracks 
            startingPoint.SetActive(false);             // get tge track starting point off 
        }
        else return;
    }

    public void Update()
    {
        //if (SelectSeed.instance.removedRackings && SelectSeed.instance.movedSubassemblies && SelectSeed.instance.deleteThepathWay)
        //{
        //    if (!SelectSeed.instance.changeDoneCheck)
        //    {
        //        UIManager.instance.ChangePannel(UIManager.instance.allChangesAreDone);
        //        SelectSeed.instance.changeDoneCheck = true;
        //    }
        //}
            // the game will restart 
            if (Input.GetKey(KeyCode.Space))
            __RestartScene();
        if (deletePathWayButton == null)
        {
            return;
        }
        if (subAssemblyMoved > 2)
        {
            deletePathWayButton.SetActive(true);
        }
    }

    public void __RestartScene ()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    [Obsolete]
    public void __ReloadPage()
    {
        Application.ExternalEval("document.location.reload(true)");
    }
}