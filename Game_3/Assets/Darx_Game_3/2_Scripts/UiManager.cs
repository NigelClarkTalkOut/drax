using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Runtime.InteropServices;

public class UiManager : MonoBehaviour
{
    public static UiManager instance;
    public static Action<float> A_StationComplete;
    public static Action ZoomOutForChangs;

    // triggers for entring perticulart stations
    [Header("Station Box Colliders")]
    [SerializeField] Collider2D[] station1Colliders;
    [SerializeField] Collider2D[] station2Colliders;
    [SerializeField] Collider2D[] station3Colliders;

    // triggers for entring perticulart stations
    [Header("After Improvement Station Box Colliders")]
    [SerializeField] Collider2D[] A_station1Colliders;
    [SerializeField] Collider2D[] A_station2Colliders;
    [SerializeField] Collider2D[] A_station3Colliders;

    [Header("Function Button")]
    [SerializeField] GameObject deletingCanvas;         // canvas for deleting extra parts from the factory
    [SerializeField] GameObject stationsButtons;        // for entring the stations
    [SerializeField] Button editModeButton;             // for entring and exiting edit mode 

    // panels not connected with any stations
    [Header("Ui Panels")]
    [SerializeField] GameObject introductionPanel;
    [SerializeField] GameObject timeOutPanel;           // when player runs out of time
    [SerializeField] GameObject handPosePopup;           // when player runs out of time

    [Header ("Improvement Panel")]
    [SerializeField] GameObject improvementPanel;       // letting player know they are in improvement phase
    [SerializeField] GameObject changeLayoutPanel;      // letting player know they have to change layout 
    [SerializeField] GameObject loadBalancingPanel;     // letting player know they have to change layout 
    [SerializeField] GameObject improvementSuccess;     // letting player know they have to change layout 

    [Header ("Station 1 Panel")]
    [SerializeField] GameObject station1InstructionPanel;
    [SerializeField] GameObject A_station1InstructionPanel;
    [SerializeField] GameObject station1SuccessPanel;
    [SerializeField] GameObject A_station1SuccessPanel;
    
    [Header ("Station 2 Panel")]
    [SerializeField] GameObject station2InstructionPanel;
    [SerializeField] GameObject A_station2InstructionPanel;
    [SerializeField] GameObject station2SuccessPanel;
    [SerializeField] GameObject A_station2SuccessPanel;
    
    [Header ("Station 3 Panel")]
    [SerializeField] GameObject station3InstructionPanel;
    [SerializeField] GameObject A_station3InstructionPanel;
    [SerializeField] GameObject station3SuccessPanel;
    [SerializeField] GameObject A_station3SuccessPanel;

    [Header("Moving Station Collider")]
    [SerializeField] Collider2D M_station1Collider;
    [SerializeField] Collider2D M_station2Collider;
    [SerializeField] Collider2D M_station3Collider;
    
    [Header("Moving Boxes Collider")]
    [SerializeField] Collider2D box1;
    [SerializeField] Collider2D box2;

    [Header("Delete Canvas Function")]
    public int itemDeleted = 0;             // extra item placed 
    public int stationPlaced = 0;           // number of stations placed 
    public int boxesPlaced = 0;             // number of boxes placed 
    public int A_Station1Parts = 0;         // number or parts on station 1

    [Header("Station New Position")]
    [SerializeField] Transform station1;
    [SerializeField] Transform station2;
    [SerializeField] Transform station3;

    [SerializeField] Transform station1Position;
    [SerializeField] Transform station2Position;
    [SerializeField] Transform station3Position;
    [SerializeField] GameObject headerCanvas;

    WaitForSeconds oneSecond = new WaitForSeconds(1);


    [DllImport("__Internal")]
    private static extern float GetOrientation();


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        StartCoroutine(nameof(CheckOrientation));
        ChangePanel(introductionPanel);     // game will always start with introduction panel
        HideStationButtons(true);
    }

    #region Events

    private void OnEnable()
    {
        // close station button after completing station
        Timer.ChangeStationNumber += CloseStationButtons;
        Item.lastStation += HideStations;
        Item.A_Station1Placed += _AddStation1Parts;
        Item.A_Station1Placed += HideStationFunction;
        

        // show success panel
        StationManager.Station1Complete += ShowStation1SuccessPanel;
        StationManager.Station2Complete += ShowStation2SuccessPanel;
        StationManager.Station3Complete += ShowStation3SuccessPanel;
        StationManager.A_Station1Complete += A_ShowStation1SuccessPanel;
        StationManager.A_Station2Complete += A_ShowStation2SuccessPanel;
        StationManager.A_Station3Complete += A_ShowStation3SuccessPanel;

        // time out panel
        Timer.B_Station1TimeOut += ShowTimeOutPanel;
        Timer.B_Station2TimeOut += ShowTimeOutPanel;
        Timer.B_Station3TimeOut += ShowTimeOutPanel;

        // time out panel
        Timer.A_Station1TimeOut += ShowTimeOutPanel;
        Timer.A_Station2TimeOut += ShowTimeOutPanel;
        Timer.A_Station3TimeOut += ShowTimeOutPanel;

        // to check how many station are placed 
        Stations.StationPlaced += StationPlaced;
        Stations.BoxPlaced += _BoxPlaced;
    }

    private void OnDisable()
    {
        Timer.ChangeStationNumber -= CloseStationButtons;
        Item.lastStation -= HideStations;
        Item.A_Station1Placed -= _AddStation1Parts;
        Item.A_Station1Placed -= HideStationFunction;

        // show success panel
        StationManager.Station1Complete -= ShowStation1SuccessPanel;
        StationManager.Station2Complete -= ShowStation2SuccessPanel;
        StationManager.Station3Complete -= ShowStation3SuccessPanel;
        StationManager.A_Station1Complete -= A_ShowStation1SuccessPanel;
        StationManager.A_Station2Complete -= A_ShowStation2SuccessPanel;
        StationManager.A_Station3Complete -= A_ShowStation3SuccessPanel;

        // time out panel
        Timer.B_Station1TimeOut -= ShowTimeOutPanel;
        Timer.B_Station2TimeOut -= ShowTimeOutPanel;
        Timer.B_Station3TimeOut -= ShowTimeOutPanel;

        // time out panel
        Timer.A_Station1TimeOut -= ShowTimeOutPanel;
        Timer.A_Station2TimeOut -= ShowTimeOutPanel;
        Timer.A_Station3TimeOut -= ShowTimeOutPanel;


        // to check how many station are placed 
        Stations.StationPlaced -= StationPlaced;
        Stations.BoxPlaced -= _BoxPlaced;
    }

    #endregion

    // close all ui planels in-game
    public void CloseAllPanels ()       // will be used for okay button
    {
        introductionPanel.SetActive(false);
        timeOutPanel.SetActive(false);  
        improvementPanel.SetActive(false);      // letting player knwo its time for improvement 
        changeLayoutPanel.SetActive(false);     // letting player know they have to change canvas
        loadBalancingPanel.SetActive(false);    // letting player know they have to balancing the load 
        improvementSuccess.SetActive(false);    // letting player know they have to balancing the load 

        // station 1
        station1InstructionPanel.SetActive(false);
        A_station1InstructionPanel.SetActive(false);
        station1SuccessPanel.SetActive(false);
        A_station1SuccessPanel.SetActive(false);

        // station 2
        station2InstructionPanel.SetActive(false);
        A_station2InstructionPanel.SetActive(false);
        station2SuccessPanel.SetActive(false);
        A_station2SuccessPanel.SetActive(false);
   
        // station 3
        station3InstructionPanel.SetActive(false);
        A_station3InstructionPanel.SetActive(false);
        station3SuccessPanel.SetActive(false);
        A_station3SuccessPanel.SetActive(false);
    }

    public void ChangePanel (GameObject activePanel)
    {
        print("Change");
        // close all the panels
        CloseAllPanels();
        // show panel you desire
        activePanel.SetActive(true);
    }

    #region Tunring On Item Box
    public void _Station1Button ()
    {
        for (int i = 0; i < station1Colliders.Length; i++)
        {
            station1Colliders[i].enabled = true;
        }
    }
    public void _Station2Button ()
    {
        for (int i = 0; i < station2Colliders.Length; i++)
        {
            station2Colliders[i].enabled = true;
        }
    }
    public void _Station3Button()
    {
        for (int i = 0; i < station3Colliders.Length; i++)
        {
            station3Colliders[i].enabled = true;
        }
    }

    public void _A_Station1Button()
    {
        for (int i = 0; i < A_station1Colliders.Length; i++)
        {
            A_station1Colliders[i].enabled = true;
        }
    }
    
    public void _A_Station2Button()
    {
        for (int i = 0; i < A_station2Colliders.Length; i++)
        {
            A_station2Colliders[i].enabled = true;
        }
    }
    
    public void _A_Station3Button()
    {
        for (int i = 0; i < A_station3Colliders.Length; i++)
        {
            A_station3Colliders[i].enabled = true;
        }
    }

    #endregion

    // hide station buttons
    public void HideStationButtons(bool hide)
    {
        if (hide)
            stationsButtons.SetActive(false);
        else stationsButtons.SetActive(true);
    }

    public void _Station3SuccessOkayButton ()
    {
        // the user will able to interact with edit button now 
        editModeButton.interactable = true;
    }

    void CloseStationButtons(float i) => HideStationButtons(true);
    void HideStationFunction() => HideStationButtons(true);
    void HideStations() => HideStationButtons(true);                    // with arugments

    // to show success panels
    void ShowStation1SuccessPanel() => Invoke("InvokeShowStation1SuccessPanel", 1);
    void InvokeShowStation1SuccessPanel() => ChangePanel(station1SuccessPanel);

    void ShowStation2SuccessPanel() => Invoke("InvokeShowStation2SuccessPanel", 1);
    void InvokeShowStation2SuccessPanel() => ChangePanel(station2SuccessPanel);

    void ShowStation3SuccessPanel() => Invoke("InvokeShowStation3SuccessPanel", 1);
    void InvokeShowStation3SuccessPanel() => ChangePanel(station3SuccessPanel);

    void A_ShowStation1SuccessPanel() => Invoke("InvokeA_ShowStation1SuccessPanel", 1);
    void InvokeA_ShowStation1SuccessPanel() => ChangePanel(A_station1SuccessPanel);

    void A_ShowStation2SuccessPanel() => Invoke("InvokeA_ShowStation2SuccessPanel", 1);
    void InvokeA_ShowStation2SuccessPanel() => ChangePanel(A_station2SuccessPanel);

    void A_ShowStation3SuccessPanel()
    {
        ChangePanel(A_station3SuccessPanel);
        PostFunction.instance.PostData();
    }

    void ShowTimeOutPanel() => ChangePanel(timeOutPanel);

    public void _DeleteButton(GameObject obj)
    {
        Destroy(obj);
        itemDeleted++;      // how many items are deleted
        if (itemDeleted == 12)
        {
            ZoomOutForChangs?.Invoke();
            ChangeStationPosition();
            ChangePanel(changeLayoutPanel);     // letting player know they have to change the layout of factory
        }
    }

    void ChangeStationPosition()
    {
        station1.position = station1Position.position;
        station1.rotation = Quaternion.Euler(0, 180, 0);
        station2.position = station2Position.position;
        station2.rotation = Quaternion.Euler(0, 180, 0);
        station3.position = station3Position.position;
    }

    public void _ShowObject(GameObject obj) => obj.SetActive(true);
    public void _HideOnject(GameObject obj) => obj.SetActive(false);

    // make stations can be moved or not 
    public void _MoveStations ()
    {
        M_station1Collider.enabled = true;
        M_station2Collider.enabled = true;
        M_station3Collider.enabled = true;
    }

    // check how many stations are placed 
    public void StationPlaced ()
    {
        stationPlaced++;

        if (stationPlaced == 3)
        {
            M_station1Collider.enabled = false;
            M_station2Collider.enabled = false;
            M_station3Collider.enabled = false;
            ChangePanel(loadBalancingPanel);
        }
    }

    // moveing box for loadbalancing 
    public void _MoveBoxes()
    {
        box1.enabled = true;
        box2.enabled = true;
    }

    // check how many stations are placed 
    public void _BoxPlaced()
    {
        boxesPlaced++;

        if (boxesPlaced == 2)
        {
            ChangePanel(improvementSuccess);
        }
    }

    public void _AddStation1Parts ()
    {
        A_Station1Parts++;

        if (A_Station1Parts == 4)
        {
            A_StationComplete?.Invoke(1);
        }
    }

    IEnumerator CheckOrientation()
    {
        while (true)
        {
            // width < height which is portrait 
            if (GetOrientation() < 1f)
            {
                headerCanvas.SetActive(true);
                if (handPosePopup.activeInHierarchy)
                    handPosePopup.SetActive(false);
            }
            // width > height which is landscape
            else if (GetOrientation() > 1f)
            {
                headerCanvas.SetActive(false);
                if (!handPosePopup.activeInHierarchy)
                    handPosePopup.SetActive(true);
            }
            yield return oneSecond;
        }
    }
}