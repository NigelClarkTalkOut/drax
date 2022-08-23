using UnityEngine;

public class TimeChanger : MonoBehaviour
{
    [SerializeField] SeedSystem seedInstance;

    public float idelTime;
    public float currentTime;

    public float idelDistance;
    public float currentDistance;
    public float timeToSub = 8;

    [SerializeField] PathChecker[] checkerInstance;

    public float timeToAdd;

    private void OnEnable()
    {
        GameManager.OnClear += OnClear;
        LineRendererPosition.SubTime += subTimer;
    }

    public void OnDisable()
    {
        LineRendererPosition.SubTime -= subTimer;
        GameManager.OnClear -= OnClear;
    }

    private void Start()
    {
        // if the game is in normal state
        if (SelectSeed.instance.phases == SelectSeed.Phases.normalPhase)
        {
            idelTime = seedInstance.seed[seedInstance.seedCount].normalPhaseTimer - timeToSub;
            idelDistance = seedInstance.seed[seedInstance.seedCount].idelDistanceNormalPhase;
            checkerInstance[0].triggerToSub = true;
            checkerInstance[1].triggerToSub = true;
            checkerInstance[2].triggerToSub = true;
            checkerInstance[3].triggerToSub = true;
        }

        // if the game is in improvement phase
        else if (SelectSeed.instance.phases == SelectSeed.Phases.improvementPhase)
        {
            idelTime = seedInstance.seed[seedInstance.seedCount].improvementPhaseTimer - timeToSub;
            idelDistance = seedInstance.seed[seedInstance.seedCount].idelDistanceImprovementPhase;

            checkerInstance[1].triggerToSub = true;
            checkerInstance[2].triggerToSub = true;
            checkerInstance[3].triggerToSub = true;
        }

        // get turn the load balancing seed on and normal seed off 
        else if (SelectSeed.instance.phases == SelectSeed.Phases.loadBalancingPhaseOne)
        {
            idelTime = (seedInstance.seed[seedInstance.seedCount].loadBalancingPhaseOneTimer - timeToSub) - 8;
            idelDistance = seedInstance.seed[seedInstance.seedCount].idelDistanceLoadbalancingPhase1G;

            checkerInstance[3].triggerToSub = true;
        }

        else if (SelectSeed.instance.phases == SelectSeed.Phases.loadBalancingPhaseTwo)
        {
            idelTime = seedInstance.seed[seedInstance.seedCount].loadBalancingPhaseTwoTimer - timeToSub;
            idelDistance = seedInstance.seed[seedInstance.seedCount].idelDistanceLoadbalancingPhase1B;

            checkerInstance[1].triggerToSub = true;
            checkerInstance[2].triggerToSub = true;
        }

        else if (SelectSeed.instance.phases == SelectSeed.Phases.loadBalancingPhaseThree)
        {
            idelTime = seedInstance.seed[seedInstance.seedCount].loadBalancingPhaseThreeTimer - timeToSub;
            idelDistance = seedInstance.seed[seedInstance.seedCount].idelDistanceLoadbalancingPhase2G;

            checkerInstance[2].triggerToSub = true;
            checkerInstance[3].triggerToSub = true;
        }
        else if (SelectSeed.instance.phases == SelectSeed.Phases.loadBalancingPhaseFour)
        {
            idelTime = seedInstance.seed[seedInstance.seedCount].loadBalancingPhaseFourTimer - timeToSub;
            idelDistance = seedInstance.seed[seedInstance.seedCount].idelDistanceLoadbalancingPhase2B;

            checkerInstance[1].triggerToSub = true;
            checkerInstance[2].triggerToSub = true;
        }
    }

    public void GetDifference ()
    {
        currentDistance = GameManager.instance.pathLenght * .66f;
        currentTime += ((currentDistance * idelTime) / idelDistance);

        timeToAdd = (idelTime - currentTime) / idelTime;
        if (currentTime < idelTime)
            timeToAdd = idelTime / (currentTime);
        else if (currentTime > idelTime)
            timeToAdd = idelTime / (currentTime);
        else if (currentTime == idelTime)
            timeToAdd = 1;
    }

    void subTimer()
    {
        timeToSub--;
        currentTime += 2f;
        idelTime = seedInstance.seed[seedInstance.seedCount].normalPhaseTimer - timeToSub;
    }

    void OnClear()
    {
        currentTime = 0;
        timeToSub = 8;
    }
}