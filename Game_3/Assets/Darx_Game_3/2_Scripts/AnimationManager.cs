using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    [SerializeField] GameObject animatedonWorkersAchive;
    [SerializeField] GameObject animatedWorkersNonAchive;
    public Animation animatedWorkers;
    [SerializeField] GameObject staticWorkers;

    private void Start()
    {
        animatedWorkers.Play("0_Worker_Animation_Before");
    }

    #region Events

    private void OnEnable()
    {
        Timer.ChangeStationNumber += _ShowAnimationWorkers;
    }

    private void OnDisable()
    {
        // show static workers when sation one part is ready
        Timer.ChangeStationNumber -= _ShowAnimationWorkers;
    }

    #endregion

    // show animated workers 
    public void _ShowAnimationWorkers (float timeFrame)
    {
        if (StationManager.instance.station == StationManager.Station.A_station1 || StationManager.instance.station == StationManager.Station.A_station2 || StationManager.instance.station == StationManager.Station.A_station3)
            return;
        // show animated workers
        animatedWorkersNonAchive.SetActive(true);
        animatedWorkers.Play("0_Worker_Animation_Before");
        animatedWorkers.GetComponent<Animation>()["0_Worker_Animation_Before"].time = timeFrame;
        // when user is out side of stations and loop animation is running 
        staticWorkers.SetActive(false);
    }

    // when user enters station for producting parts
    public void _ShowStaticWorkers ()
    {
        if (StationManager.instance.station == StationManager.Station.A_station1 || StationManager.instance.station == StationManager.Station.A_station2 || StationManager.instance.station == StationManager.Station.A_station3)
            return;
        // show normal workers
        staticWorkers.SetActive(true);
        animatedWorkersNonAchive.SetActive(false);      // hide normal workers
    }

}