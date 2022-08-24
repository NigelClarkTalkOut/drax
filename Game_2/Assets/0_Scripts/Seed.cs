using UnityEngine;

[CreateAssetMenu (fileName = "Seed")]
public class Seed : ScriptableObject
{
    [Range (0, 3)]
    public int conveyorPosition;                    // position of packages for lower conveyor 
    [Header ("X: Blue | Y: Green")]
    public Vector2 activatedSubAssembly;            // sub assemblies position 
    [Range (0, 7)]
    public int upperConveyor;                       // position of packages for upper conveyor
    public PackagesDataHolder[] packageData;        // for holding data for types of packages 
    public int optimalIndex;                        // index of the optimal path
    public int improvedOptimalIndex;                // index of the optimal path
    public int phaseOneImprovedOptimalIndex;        // index of the optimal path
    public int phaseTwoImprovedOptimalIndex;        // index of the optimal path

    [Header("Timings")]
    public int normalPhaseTimer;
    public int improvementPhaseTimer;
    public int loadBalancingPhaseOneTimer;
    public int loadBalancingPhaseTwoTimer;
    public int loadBalancingPhaseThreeTimer;
    public int loadBalancingPhaseFourTimer;

    [Header("Distance")]
    public int idelDistanceNormalPhase;
    public int idelDistanceImprovementPhase;
    public int idelDistanceLoadbalancingPhase1G;
    public int idelDistanceLoadbalancingPhase1B;
    public int idelDistanceLoadbalancingPhase2G;
    public int idelDistanceLoadbalancingPhase2B;

    [Header ("Optimal path")]
    public GameObject ompimalPath;                  // get the optimal path image
    public GameObject improvedOptimalPath;          // get the optimal path image
    public GameObject loadBalancingPhaseOne;        // get the optimal path image
    public GameObject loadBalancingPhaseTwo;        // get the optimal path image
    public GameObject loadBalancingPhaseTwoG;        // get the optimal path image
    public GameObject loadBalancingPhaseTwoB;        // get the optimal path image

    [Header("Path Checking")]
    public bool[] normalPhasePath;
    public bool[] improvementPhasePath;
    public bool[] loadBalancingPhase1G;
    public bool[] loadBalancingPhase1B;
    public bool[] loadBalancingPhase2G;
    public bool[] loadBalancingPhase2B;

    public bool rounded;
    public bool nonRounded;
}

[System.Serializable]
public class PackagesDataHolder
{
    //[HideInInspecto]
    public string name;
    [Range (0, 4)]
    public int[] packagePlace;                  // place of where package should rest 
}