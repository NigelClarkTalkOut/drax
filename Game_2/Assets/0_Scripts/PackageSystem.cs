using System;
using UnityEngine;
using System.Collections;

public class PackageSystem : MonoBehaviour
{
    public AudioSource deliverySoundSource;
    public AudioClip deliverySoundClip;
    public static PackageSystem instance;
    public static Action TugArrived;                // event for tug arrived
    [SerializeField]PackageSystem recivigEndPackageSystem;


    Vector2 packageOriginPosition;
    [SerializeField] Transform package;             // the package to diliver
    [SerializeField] Transform rackingArea;         // package's original place
    [SerializeField] Transform loadingArea;         // package spawn point
    [SerializeField] Transform packageHolder;       // tug to hold the package 

    public bool waitingPackage = false;             // package is availabe or not, and for null refrence error 
    [SerializeField] float packageSpeed;            // speed of package from loading area to tug
    [SerializeField] bool recivingEnd = false;      // will decide package to recive to deliver

    public WaitForSeconds waitForSeconds = new WaitForSeconds(1);

    private void Awake()
    {
        // get the origin position 
        packageOriginPosition = transform.position;
    }

    private void Start()
    {
        // where package should be loaded on the tug
        packageHolder = GameObject.FindGameObjectWithTag("Package Holder").transform;
        packageSpeed = 4;       // speed of the package from the racking area to tug 
    }

    // will stop the coroutine
    private void OnDisable()
    {
        StopCoroutine(nameof(UpdatingLoop));
    }

    IEnumerator UpdatingLoop ()
    {
        // if its a loading deck
        if (!recivingEnd)
        {
            while (true)
            {
                LoadingAreaToTug();     // make the package move to tug
                // if the package is on tug stop the coroutine
                if (package.position == packageHolder.position)
                {
                    Destroy(gameObject);
                    StopCoroutine(nameof(UpdatingLoop));
                }
                yield return null;      // wait for next frame
            }
        }
        // if its  the delivering deck
        else if (recivingEnd)
        {
            while (true)
            {
                TugToLoadingArea();     // make the package load to loading area
                // if the package is on the loading area stop the coroutine 
                if (package.position == loadingArea.position)
                {
                    deliverySoundSource.PlayOneShot(deliverySoundClip);
                    GameManager.instance.packagesLeft--;
                    Destroy(package.gameObject);
                    StopCoroutine(nameof(UpdatingLoop));
                }
                yield return null;      // wait for the next frame 
            }
        }
    }

    // package from loading deck to tug
    void LoadingAreaToTug()    
    {
        // once the pagake is on tug, there will be no waiting for tug
        package.position = Vector3.MoveTowards(package.transform.position, packageHolder.position, packageSpeed * Time.deltaTime);
        package.SetParent(packageHolder);          // set the tug as the parent
    }

    // package from tug to loading deck 
    void TugToLoadingArea()    
    {
        if (recivingEnd && waitingPackage)
        {
            // package from tug to loading area
            package.position = Vector3.MoveTowards(package.transform.position, loadingArea.position, packageSpeed * Time.deltaTime);
            package.SetParent(loadingArea);     // set the loading area as the parent
            StartCoroutine(nameof(WaitToLoad));
        }
    }

    IEnumerator WaitToLoad ()
    {
        // wait for 1 second 
        yield return waitForSeconds;
        waitingPackage = false;         // the package is dilivered so the tug will not stop second time 
    }

    // starting the coroutine when triggered 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && waitingPackage)
        {
            // the package will move to tug
            if (!recivingEnd)
                recivigEndPackageSystem.waitingPackage = true;
                        
            TugArrived?.Invoke();                   // call the action
            // start the coroutine
            StartCoroutine(nameof(UpdatingLoop));
        }
    }
}