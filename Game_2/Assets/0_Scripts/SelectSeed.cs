using UnityEngine;

public class SelectSeed : MonoBehaviour
{
    public enum Phases
    {
        normalPhase,
        improvementPhase,
        loadBalancingPhaseOne,
        loadBalancingPhaseTwo,
        loadBalancingPhaseThree,
        loadBalancingPhaseFour,
    }

    public static SelectSeed instance;

    [SerializeField] GameObject pointHolder;
    public Phases phases;
    public int seedSleceted;
    public int tries;
    [SerializeField] int customSeedCount;

    // if the welcome panel shown yet
    public bool welcomePannelShown = false;

    // checking the first time events
    public bool tugMoved = false;
    public bool removedRackings = false;
    public bool movedSubassemblies = false;
    public bool drawingTutDone = false;
    public bool deleteThepathWay = false;
    public bool pathPointForfirstTime = false;
    public bool firstTimeEdit = false;
    public bool changeDoneCheck = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            seedSleceted = Random.Range(0, 11);
            //seedSleceted = customSeedCount;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void Update()
    {
        if (deleteThepathWay)
            pointHolder.SetActive(true);
        if (phases != Phases.improvementPhase)
            pointHolder.SetActive(false);
    }

}