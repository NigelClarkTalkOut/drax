using UnityEngine;
using UnityEngine.UI;


public class SeedSystem : MonoBehaviour
{
    public static SeedSystem instance;
    [SerializeField] Timer timeSystem;
    public Seed[] seed;
    public int seedCount;
    public int optimalPathCount;
    public PathChecker[] pathChecker;
    public bool falsePath = false;

    [SerializeField] Text timeText;
    [SerializeField] Text successText;
    [SerializeField] Text timeOutPath;
    [SerializeField] Button editButton;
    [SerializeField] Transform point1;
    [SerializeField] Transform point2;
    [SerializeField] Transform movingPointHolder;
    [SerializeField] GameObject normalSeed;
    [SerializeField] GameObject loadBalancingSeeds;
    [SerializeField] GameObject loadBalancingOptimalPath;
    [SerializeField] GameObject afterImprovementCleaning;
    [SerializeField] GameObject nonWorkingRackingArea;
    [SerializeField] Collider2D secondPathCollider;

    [Header ("ASSEMBLIES TO SPAWN")]
    [SerializeField] Transform conveyorSystem;              // 4 position from the bottom conveyor
    [SerializeField] Transform subAssembly_Blue;            // for changing blue sub assembly position 
    [SerializeField] Transform subAssembly_Green;           // for changing green sub assembly position 
    [SerializeField] Transform optimalSubAssembly_Green;    // for changing green sub assembly position 
    [SerializeField] Transform upperConveyorAssembly;               // 6 position for the upper conveyor

    [Header ("SPAWN POSITION")]
    [SerializeField] Transform[] converyorPosition;         // storing 4 conveyor position
    [SerializeField] Transform[] subAssemblyPosition;       // storing 6 sub assembly position 
    [SerializeField] GameObject[] upperConveyorBelt;        // storing 6 upper conveyor position 

    [Header ("RACKING AREA AND PACKAGING")]
    [SerializeField] RackingArea[] rackingArea;             // racking area for packages 
    [SerializeField] PackagesType[] packagesType;           // type of packages racking area will hold

    [Header("LOAD BALANCING PHASE")]
    //[SerializeField] GameObject blueSubAssembly;
    [SerializeField] SpriteRenderer orangeSubAssebly;
    [SerializeField] SpriteRenderer greenSubAssembly;
    [SerializeField] SpriteRenderer[] yellowSubAssembly;
    [SerializeField] SpriteRenderer tugSprite;

    [SerializeField] GameObject bluePackage;
    [SerializeField] GameObject greenPackage;
    [SerializeField] GameObject yellowPackage;
    [SerializeField] GameObject orangePackage;

    [SerializeField] Sprite blueOrangeSubAssembl;
    [SerializeField] Sprite blueGreenSubAssembl;
    [SerializeField] Sprite bluePackageSprite;
    [SerializeField] Sprite greenPackageSprite;
    [SerializeField] Sprite greenTugSprite;
    [SerializeField] Sprite blueTugSprite;
    [SerializeField] Color colorForUpperConveyor;
    [SerializeField] Color bluePackageColor;

    private void Awake()
    {
        // get the instance for singleton and selectSeed
        instance = this;
    }

    private void Start()
    {
        seedCount = SelectSeed.instance.seedSleceted;
        postSeedFunctions();
        CheckThePath();
    }

    private void OnEnable()
    {
        GameManager.startTheTug += __CrossCheckpath;
    }

    private void OnDisable()
    {
        GameManager.startTheTug -= __CrossCheckpath;
    }

    // arrangin the sub assemblies and get the optimal path
    private void postSeedFunctions()
    {
        // if the game is in normal state
        if (SelectSeed.instance.phases == SelectSeed.Phases.normalPhase)
        {
            secondPathCollider.enabled = false;
            timeSystem.timeRemaining = seed[seedCount].normalPhaseTimer;
            timeText.text = $"You have {timeSystem.timeRemaining} seconds to complete the level.";
            successText.text = $"Well done, you have managed to collect and deliver all the materials in the allocated time. However, you will have less time in the future due to increasing demand, therefore, we have to make improvements.";
            PlacingTheSubAssembly();
            // disable the edit button and get the optimal path
            editButton.interactable = false;
            // spawn the optimal path
            Instantiate(seed[seedCount].ompimalPath, transform.position, Quaternion.identity);
            optimalPathCount = seed[seedCount].optimalIndex;        // get the normal optimal index
        }

        // if the game is in improvement phase
        else if (SelectSeed.instance.phases == SelectSeed.Phases.improvementPhase)
        {
            timeSystem.timeRemaining = seed[seedCount].improvementPhaseTimer;
            timeText.text = $"You have {timeSystem.timeRemaining} seconds to complete the level.";
            successText.text = $"Well done! You have managed to collect and deliver all of the items around the factory within the timeframe.";
            PlacingTheSubAssembly();
            if (SelectSeed.instance.deleteThepathWay)
                AfterChanginSubassembiles();
            // remove the racking area if prevously removed 
            if (SelectSeed.instance.removedRackings)
                Destroy(nonWorkingRackingArea);

            // get the improved optimal index
            Instantiate(seed[seedCount].improvedOptimalPath, transform.position, Quaternion.identity);
            optimalPathCount = seed[seedCount].improvedOptimalIndex;
        }

        // get turn the load balancing seed on and normal seed off 
        else if (SelectSeed.instance.phases == SelectSeed.Phases.loadBalancingPhaseOne)
        {
            // green tug system
            timeSystem.timeRemaining = seed[seedCount].loadBalancingPhaseOneTimer;
            timeText.text = $"You have {timeSystem.timeRemaining} seconds to complete the level.";
            successText.text = "Well done! You have managed to collect and deliver all of the items around the factory within the timeframe.";
            PlacingTheSubAssembly();
            tugSprite.sprite = greenTugSprite;
            // get the changes for removing the racks and sub-assembly
            SelectSeed.instance.removedRackings = true;
            SelectSeed.instance.movedSubassemblies = true;
            AfterChanginSubassembiles();
            // remove the non working racking area
            Destroy(nonWorkingRackingArea);
            GameManager.instance.packagesLeft = 1;
            // get the improved optimal index
            optimalPathCount = 21;
            // get the load balancing optimal path for the load balancing part
            Instantiate(seed[seedCount].loadBalancingPhaseOne, transform.position, Quaternion.identity);
            ChangingLoadBalancing();    // changes according to load blancing
        }

        else if (SelectSeed.instance.phases == SelectSeed.Phases.loadBalancingPhaseTwo)
        {
            // blue tug system
            timeSystem.timeRemaining = seed[seedCount].loadBalancingPhaseTwoTimer;
            timeText.text = $"You have {timeSystem.timeRemaining} seconds to complete the level.";
            successText.text = $"The green tug driver has be waiting for you to finsih, it would be good to balance the route to reduce the waiting";
            //timeOutPath.text = "You have not managed to collect and deliver all the items in 30 seconds as you had too many items";
            PlacingTheSubAssembly();
            tugSprite.sprite = blueTugSprite;
            // get the changes for removing the racks and sub-assembly
            SelectSeed.instance.removedRackings = true;
            SelectSeed.instance.movedSubassemblies = true;
            AfterChanginSubassembiles();
            // remove the non working racking area
            Destroy(nonWorkingRackingArea);
            GameManager.instance.packagesLeft = 3;
            // get the improved optimal index
            optimalPathCount = seed[seedCount].phaseOneImprovedOptimalIndex;    // get the normal optimal index
            // get the load balancing optimal path for the load balancing part
            Instantiate(seed[seedCount].loadBalancingPhaseTwo, transform.position, Quaternion.identity);
            ChangingLoadBalancing();    // changes according to load blancing 
        }

        else if (SelectSeed.instance.phases == SelectSeed.Phases.loadBalancingPhaseThree)
        {
            // green tug system
            timeSystem.timeRemaining = seed[seedCount].loadBalancingPhaseThreeTimer;
            timeText.text = $"You have {timeSystem.timeRemaining} seconds to complete the level.";
            successText.text = "Well done! You have managed to collect and deliver all of the items around the factory within the timeframe.";
            PlacingTheSubAssembly();
            tugSprite.sprite = greenTugSprite;
            // get the changes for removing the racks and sub-assembly
            SelectSeed.instance.removedRackings = true;
            SelectSeed.instance.movedSubassemblies = true;
            AfterChanginSubassembiles();
            // remove the non working racking area
            Destroy(nonWorkingRackingArea);
            GameManager.instance.packagesLeft = 2;      // how many packages player have to deliver
            // get the improved optimal index
            optimalPathCount = 27;
            // get the load balancing optimal path for the load balancing part
            Instantiate(seed[seedCount].loadBalancingPhaseTwoG, transform.position, Quaternion.identity);
            ChangingLoadBalancingPhaseThree();
        }

        else if (SelectSeed.instance.phases == SelectSeed.Phases.loadBalancingPhaseFour)
        {
            // green tug system
            timeSystem.timeRemaining = seed[seedCount].loadBalancingPhaseFourTimer;
            timeText.text = $"You have {timeSystem.timeRemaining} seconds to complete the level.";
            successText.text = "Well done! there is no waiting and both runs have be completed in the same time making the factory more efficient";
            PlacingTheSubAssembly();
            tugSprite.sprite = blueTugSprite;
            // get the changes for removing the racks and sub-assembly
            SelectSeed.instance.removedRackings = true;
            SelectSeed.instance.movedSubassemblies = true;
            AfterChanginSubassembiles();
            // remove the non working racking area
            Destroy(nonWorkingRackingArea);
            GameManager.instance.packagesLeft = 2;      // how many packages player have to deliver
            // get the improved optimal index
            optimalPathCount = seed[seedCount].phaseTwoImprovedOptimalIndex;
            // get the load balancing optimal path for the load balancing part
            Instantiate(seed[seedCount].loadBalancingPhaseTwoB, transform.position, Quaternion.identity);
            ChangingLoadBalancingPhaseThree();
        }
    }

    void ChangingLoadBalancing ()
    {
        // changing the color of subassemblies 
        greenSubAssembly.sprite = blueGreenSubAssembl;
        orangeSubAssebly.sprite = blueOrangeSubAssembl;
        for (int i = 0; i < yellowSubAssembly.Length; i++)
        {
            yellowSubAssembly[i].color = colorForUpperConveyor;
        }

        // changing the color of packages
        greenPackage.GetComponent<SpriteRenderer>().sprite = bluePackageSprite;
        orangePackage.GetComponent<SpriteRenderer>().sprite = bluePackageSprite;
        yellowPackage.GetComponent<SpriteRenderer>().sprite = greenPackageSprite;

        greenPackage.GetComponent<SpriteRenderer>().color = bluePackageColor;
        orangePackage.GetComponent<SpriteRenderer>().color = bluePackageColor;

        if (SelectSeed.instance.phases == SelectSeed.Phases.loadBalancingPhaseOne)
        {
            Destroy(greenPackage.transform.GetChild(0).gameObject);
            Destroy(orangePackage.transform.GetChild(0).gameObject);
            Destroy(bluePackage.transform.GetChild(0).gameObject);
        }
        if (SelectSeed.instance.phases == SelectSeed.Phases.loadBalancingPhaseTwo)
            Destroy(yellowPackage.transform.GetChild(0).gameObject);
    }

    void ChangingLoadBalancingPhaseThree()
    {
        // the player will not able to pick blue packages
        if (SelectSeed.instance.phases == SelectSeed.Phases.loadBalancingPhaseThree)
        {
            Destroy(orangePackage.transform.GetChild(0).gameObject);
            Destroy(bluePackage.transform.GetChild(0).gameObject);
        }
        
        // the player will not able to pick blue packages
        if (SelectSeed.instance.phases == SelectSeed.Phases.loadBalancingPhaseFour)
        {
            Destroy(yellowPackage.transform.GetChild(0).gameObject);
            Destroy(greenPackage.transform.GetChild(0).gameObject);
        }

        orangeSubAssebly.sprite = blueOrangeSubAssembl;
        for (int i = 0; i < yellowSubAssembly.Length; i++)
        {
            yellowSubAssembly[i].color = colorForUpperConveyor;
        }

        // changing the color of packages
        orangePackage.GetComponent<SpriteRenderer>().sprite = bluePackageSprite;
        yellowPackage.GetComponent<SpriteRenderer>().sprite = greenPackageSprite;

        orangePackage.GetComponent<SpriteRenderer>().color = bluePackageColor;
    }

    // place the sub assemblies at the starting of any phase
    void PlacingTheSubAssembly ()
    {
        // placing the packages in the racking area according to the seed 
        for (int i = 0; i < rackingArea.Length; i++)
        {
            packagesType[i].packages[0].transform.position = rackingArea[i].packageHolder[seed[seedCount].packageData[i].packagePlace[0]].transform.position;
        }

        // conveyor point position
        conveyorSystem.position = converyorPosition[seed[seedCount].conveyorPosition].position;     

        // placing the green and blue sub assembly 
        int blue = (int)seed[seedCount].activatedSubAssembly.x;
        subAssembly_Blue.SetPositionAndRotation(subAssemblyPosition[blue].position, subAssemblyPosition[blue].rotation);
        Destroy(subAssemblyPosition[blue].gameObject);      // delete the non working sub assembly 

        // will set the green assembly position 
        int green = (int)seed[seedCount].activatedSubAssembly.y;
        // move the green subassembly according to the improvement
        if (!SelectSeed.instance.movedSubassemblies)
            subAssembly_Green.SetPositionAndRotation(subAssemblyPosition[green].position, subAssemblyPosition[green].rotation);
        else if (SelectSeed.instance.movedSubassemblies)
            subAssembly_Green.SetPositionAndRotation(optimalSubAssembly_Green.position, optimalSubAssembly_Green.rotation);
        // delete the non working sub assembly
        Destroy(subAssemblyPosition[green].gameObject);

        // get the upper conveyor belt's position
        upperConveyorBelt[seed[seedCount].upperConveyor].SetActive(true);
        upperConveyorAssembly.position = upperConveyorBelt[seed[seedCount].upperConveyor].transform.position;
    }

    // once the sub assemblies position is changed
    public void AfterChanginSubassembiles ()
    {
        // move both of them to the improvment position slot 
        point1.transform.position = movingPointHolder.position;
        point2.transform.position = movingPointHolder.position;
    }

    void CheckThePath()
    {
        if (SelectSeed.instance.phases == SelectSeed.Phases.normalPhase)
        {
            for (int i = 0; i < pathChecker.Length; i++)
            {
                pathChecker[i].correctTrigger = seed[seedCount].normalPhasePath[i];
            }
        }

        else if (SelectSeed.instance.phases == SelectSeed.Phases.improvementPhase)
        {
            for (int i = 0; i < pathChecker.Length; i++)
            {
                pathChecker[i].correctTrigger = seed[seedCount].improvementPhasePath[i];
            }
        }

        else if (SelectSeed.instance.phases == SelectSeed.Phases.loadBalancingPhaseOne)
        {
            for (int i = 0; i < pathChecker.Length; i++)
            {
                pathChecker[i].correctTrigger = seed[seedCount].loadBalancingPhase1G[i];
            }
        }
 
        else if (SelectSeed.instance.phases == SelectSeed.Phases.loadBalancingPhaseTwo)
        {
            for (int i = 0; i < pathChecker.Length; i++)
            {
                pathChecker[i].correctTrigger = seed[seedCount].loadBalancingPhase1B[i];
            }
        }
   
        else if (SelectSeed.instance.phases == SelectSeed.Phases.loadBalancingPhaseThree)
        {
            for (int i = 0; i < pathChecker.Length; i++)
            {
                pathChecker[i].correctTrigger = seed[seedCount].loadBalancingPhase2G[i];
            }
        }
        else if (SelectSeed.instance.phases == SelectSeed.Phases.loadBalancingPhaseFour)
        {
            if (seed[seedCount].rounded)
                timeSystem.timeDifference = 1.2f;
            else if (seed[seedCount].nonRounded)
                timeSystem.timeDifference = 1.6f;

            for (int i = 0; i < pathChecker.Length; i++)
            {
                pathChecker[i].correctTrigger = seed[seedCount].loadBalancingPhase2B[i];
            }
        }
    }

    public void __CrossCheckpath ()
    {

        for (int i = 0; i < pathChecker.Length; i++)
        {
            if (pathChecker[i].checkTrigger != pathChecker[i].correctTrigger)
            {
                TugSystem.instance.tugRunningSpeed = 2.4f;
                TugSystem.instance.tugSpeed = TugSystem.instance.tugRunningSpeed;
                falsePath = true;
                return;
            }
        }
    }

    void StartingTheTug ()
    {
        __CrossCheckpath();
    }
}

// number of racking areas and holders inside it 
[System.Serializable]
public class RackingArea 
{
    public string name;
    public GameObject[] packageHolder;      // places for holding the packages
}

// types of packages and number of them
[System.Serializable]
public class PackagesType
{
    public string name;
    public GameObject[] packages;           // number of packages in the racking area
}
