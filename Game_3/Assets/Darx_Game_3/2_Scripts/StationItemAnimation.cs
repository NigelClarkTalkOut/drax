using TMPro;
using UnityEngine;

public class StationItemAnimation : MonoBehaviour
{
    [SerializeField] Transform spawnPlace;

    [Header("Station 1")]
    [SerializeField] GameObject plam;
    [SerializeField] GameObject station1Item;
    [SerializeField] Transform station1IteSpawnPlace;
    [Header("Station 2")]
    [SerializeField] GameObject station2Item;
    [SerializeField] Transform station2IteSpawnPlace;
    
    [Header("Station 3")]
    [SerializeField] GameObject completeArm;
    [SerializeField] GameObject station3Item;
    [SerializeField] Transform station3IteSpawnPlace;

    [Header("Success Panel")]
    [SerializeField] int station1CompletedItems = 0;
    [SerializeField] TMP_Text station1SuccessText;

    [SerializeField] int station2CompletedItems = 0;
    [SerializeField] TMP_Text station2SuccessText;

    private void OnEnable()
    {
        Item.Station1LastPart += Station1Item;
        Item.Station2LastPart += Station2Item;
        Item.Station3LastPart += Station3Item;
        CompletedItem.PalmReached += ShowPalm;
    }

    private void OnDisable()
    {
        Item.Station1LastPart -= Station1Item;
        Item.Station2LastPart -= Station2Item;
        Item.Station3LastPart -= Station3Item;
        CompletedItem.PalmReached -= ShowPalm;
    }

    void Station1Item ()
    {
        station1CompletedItems++;
        station1SuccessText.text = $"Well done. You made {station1CompletedItems} hands in 20 seconds. Now tap on station 2 to continue the assembly.";
        plam.SetActive(false);
        Instantiate(station1Item, station1IteSpawnPlace.position, Quaternion.identity, spawnPlace);
    }
    void Station2Item ()
    {
        station2CompletedItems++;
        station2SuccessText.text = $"Good work!You made {station2CompletedItems} lower arms in 15 seconds.Did you notice that you couldn’t make as many as in station 1 ? You had to wait for the tug and there are still hands left at station 1 that you could not use.Now tap on station 3 to continue the assembly.";
        Instantiate(station2Item, station2IteSpawnPlace.position, Quaternion.identity, spawnPlace);
    }
    
    void Station3Item ()
    {
        completeArm.SetActive(false);
        Instantiate(station3Item, station3IteSpawnPlace.position, Quaternion.identity, spawnPlace);
    }

    void ShowPalm()
        => plam.SetActive(true);
}