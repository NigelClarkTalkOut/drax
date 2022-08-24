using UnityEngine;

public class ConveyorPackageSpawnSystem : MonoBehaviour
{ 
    [SerializeField] Transform rackingArea;                 // area for racking all packages 
    [SerializeField] Transform[] packageSpawnPoints;        // place for all the racking area to change 

    void Start()
        => rackingArea.position = packageSpawnPoints[Random.Range(0, packageSpawnPoints.Length)].position;
}