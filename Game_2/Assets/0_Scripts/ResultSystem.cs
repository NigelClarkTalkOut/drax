using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ResultSystem : MonoBehaviour
{
    UIManager uimanager;
    SelectSeed selectSeed;
    GameManager gameManager;
    [SerializeField] PostFunction postInstance;

    [SerializeField] bool sub = true;
    public static Action GameEnded;
    public int optimalCount;
    public GameObject optimalPath;
    public GameObject trackSystem;
    public Text losePanelText;          // losing panel text

    public CreatingPathWays pathWaySystem;
    [SerializeField] GameObject notRequriedStuff;
    [SerializeField] Transform greenSubAssembly;
    [SerializeField] Transform optimalGreenSubAssembly;

    public int tries;

    private void OnEnable()
    {
        if (sub)
            Timer.threeTimes += TimerRanOut;
    }

    private void OnDisable()
    {
        if (sub)
            Timer.threeTimes -= TimerRanOut;
    }

    private void Start()
    {
        notRequriedStuff = GameObject.FindGameObjectWithTag("ExtraStuff");
        pathWaySystem = GameObject.FindGameObjectWithTag("PathWaySystem").GetComponent<CreatingPathWays>();
        uimanager = UIManager.instance;
        gameManager = GameManager.instance;
        selectSeed = SelectSeed.instance;
        optimalPath = GameObject.FindGameObjectWithTag("Optimal Path");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag ("CheckTheTug"))
        {
            GameEnded?.Invoke();
            // get the optimal path count 
            optimalCount = SeedSystem.instance.optimalPathCount;

            // if packages are not delivered or not chooses the optimal path
            if (gameManager.packagesLeft > 0 || TugSystem.instance.routeCount > optimalCount)
            {
                // add tries everytime player lose in the particular level
                tries = selectSeed.tries++;
                if (tries < 2)
                {
                    uimanager.ChangePannel(uimanager.losePannel);
                }
                else
                {
                    // player lost for 3 times 
                    //StartCoroutine(nameof(After3Lose));       // show the results after losing 3 times 
                }
            }
            // if all the packages are delivered 
            else if (gameManager.packagesLeft <= 0)
            {
                // show the player won pannel
                SelectSeed.instance.welcomePannelShown = false;
                // if the player passed the load balancing phase
                if (SelectSeed.instance.phases == SelectSeed.Phases.loadBalancingPhaseFour)
                {
                    gameManager.trackSystem.SetActive(false);
                    uimanager.ChangePannel(uimanager.CongratulationPannel);
                    GameManager.instance.gameCompleted = true;
                    postInstance.PostData();
                    return;
                }
                // change the phase of the game
                print("Win change");
                ChangePhase();

                // reset the tries for the game 
                selectSeed.tries = 0;
                // show player won and restart the game
                if (SelectSeed.instance.phases == SelectSeed.Phases.normalPhase)
                    uimanager.ChangePannel(uimanager.successPanelNormal);
                else if (SelectSeed.instance.phases == SelectSeed.Phases.improvementPhase)
                    uimanager.ChangePannel(uimanager.successPanelNormal);
                else if (SelectSeed.instance.phases == SelectSeed.Phases.loadBalancingPhaseOne)
                    uimanager.ChangePannel(uimanager.successPanelImprovement);
                else if (SelectSeed.instance.phases == SelectSeed.Phases.loadBalancingPhaseTwo)
                    uimanager.ChangePannel(uimanager.successPanelLoadbalancing1);
                else if (SelectSeed.instance.phases == SelectSeed.Phases.loadBalancingPhaseThree)
                    uimanager.ChangePannel(uimanager.successPanelLoadbalancing1green);
                else if (SelectSeed.instance.phases == SelectSeed.Phases.loadBalancingPhaseFour)
                    uimanager.ChangePannel(uimanager.successPanelLoadbalancing2);
            }
        }
    }

    public void ShowOptimalPath()
    {
        // search for the optimal path 
        optimalPath = GameObject.FindGameObjectWithTag("Optimal Path");
        optimalPath.transform.GetChild(0).gameObject.SetActive(true);       // and activate the optimal path
    }

    void TimerRanOut ()
        => StartCoroutine(nameof(After3Lose));

    // if player lose for 3 times in a row 
    IEnumerator After3Lose ()
    {
        trackSystem.SetActive(false);
        print("Change");
        // indicater for the correct path 
        SelectSeed.instance.welcomePannelShown = false;         // game will show the welcome panel again for the next phase
        // clear the path and show the optimal path 
        gameManager.__ClearButton();
        
        if (SelectSeed.instance.phases == SelectSeed.Phases.improvementPhase)
        {
            SeedSystem.instance.AfterChanginSubassembiles();
            Destroy(notRequriedStuff);
            pathWaySystem.leftPoint = 1;
            pathWaySystem.rightPoint = 2;
        }
        // reset the tries of the player
        uimanager.ChangePannel(uimanager.losePannel);

        // change the phase of the game 
        ChangePhase();
        yield return null;
    }

    // change the phase of the game 
    void ChangePhase ()
    {
        if (SelectSeed.instance.phases == SelectSeed.Phases.normalPhase)
        {
            SelectSeed.instance.phases = SelectSeed.Phases.improvementPhase;
            return;
        }

        else if (SelectSeed.instance.phases == SelectSeed.Phases.improvementPhase)
        {
            SelectSeed.instance.phases = SelectSeed.Phases.loadBalancingPhaseOne;
            return;
        }

        else if (SelectSeed.instance.phases == SelectSeed.Phases.loadBalancingPhaseOne)
        {
            SelectSeed.instance.phases = SelectSeed.Phases.loadBalancingPhaseTwo;
            return;
        }

        else if (SelectSeed.instance.phases == SelectSeed.Phases.loadBalancingPhaseTwo)
        {
            SelectSeed.instance.phases = SelectSeed.Phases.loadBalancingPhaseThree;
            return;
        }

        else if (SelectSeed.instance.phases == SelectSeed.Phases.loadBalancingPhaseThree)
        {
            SelectSeed.instance.phases = SelectSeed.Phases.loadBalancingPhaseFour;
            return;
        }

        else if (SelectSeed.instance.phases == SelectSeed.Phases.loadBalancingPhaseFour)
            postInstance.PostData();
    }
}

