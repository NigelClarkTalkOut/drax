using UnityEngine;

public class Robot : MonoBehaviour
{
    public DefectManager defectManaeger;
    [SerializeField] Animator pulsingAnimation;
    [SerializeField] GameObject greyRobot, redRobot, wrongRobot;
    [SerializeField] int robotIndex;                        // what form of robot it is

    private void Awake()
    {
        pulsingAnimation = GetComponent<Animator>();
        defectManaeger = GameObject.FindGameObjectWithTag("Defect Manager").GetComponent<DefectManager>();
    }

    private void Start()
    {
        // start game with red robots
        greyRobot.SetActive(true);
        if (redRobot != null)               // turn the red robot off 
            redRobot.SetActive(false);
        ChangeForDefect();
        
    }

    private void OnEnable()
    {
        // spawn red robots once task is done 
        DefectManager.OrderTaskDone += SpawnRedRobot;
        DefectManager.taskDone += DoneFirstTask;            
        DefectManager.healthTaskDone += DoneSecondTask;            
        DefectManager.GameCompleted += TurnGrey;            
        RobotSpawner.changeRobots += ChangeForDefect;
    }

    private void OnDisable()
    {
        DefectManager.OrderTaskDone -= SpawnRedRobot;
        DefectManager.taskDone -= DoneFirstTask;
        DefectManager.healthTaskDone -= DoneSecondTask;
        DefectManager.GameCompleted -= TurnGrey;
        RobotSpawner.changeRobots -= ChangeForDefect;
    }

    void ChangeForDefect()
    {
        if (defectManaeger.defects == DefectManager.Defects.taskOrderIncorrect)
        {
            if (robotIndex >= 2)
            {
                // start game with red robots
                if (redRobot == null) return;

                redRobot.SetActive(true);
                greyRobot.SetActive(false);
                SpawnWrongRobot();
            }
        }

        else if (defectManaeger.defects == DefectManager.Defects.taskOrder)
        {
            if (robotIndex >= 2)
            {
                // start game with red robots
                if (redRobot == null) return;

                redRobot.SetActive(true);
                greyRobot.SetActive(false);
            }
        }

        else if (defectManaeger.defects == DefectManager.Defects.heathTask)
        {
            if (robotIndex >= 4)
            {
                // start game with red robots
                if (redRobot == null) return;

                redRobot.SetActive(true);
                greyRobot.SetActive(false);
            }
        }

        else if (defectManaeger.defects == DefectManager.Defects.skillTask)
        {
            if (robotIndex >= 6)
            {
                // start game with red robots
                if (redRobot == null) return;

                redRobot.SetActive(true);
                greyRobot.SetActive(false);
            }
        }
    }

    void DoneFirstTask ()
    {
        if (robotIndex < 4)
        {
            print("Change done");
            // start game with red robots
            if (redRobot == null) return;

            redRobot.SetActive(false);
            greyRobot.SetActive(true);
        }

        if (robotIndex == 4)
        {
            pulsingAnimation.SetTrigger("Pulsing");
        }
    }

    void DoneSecondTask ()
    {
        if (robotIndex < 6)
        {
            print("Change done");
            // start game with red robots
            if (redRobot == null) return;

            redRobot.SetActive(false);
            greyRobot.SetActive(true);
        }

        if (robotIndex == 6)
        {
            pulsingAnimation.SetTrigger("Pulsing");
        }
        
    }

    public void TurnGrey ()
    {
        if (redRobot == null) return;

        redRobot.SetActive(false);
        greyRobot.SetActive(true); 
    }

    // spawn wrong robot for solving incorrect task
    void SpawnWrongRobot ()
    {
        if (robotIndex == 2)
        {
            redRobot.SetActive(false);
            greyRobot.SetActive(false);

            wrongRobot.SetActive(true);
        }
    }

    void SpawnRedRobot()
    {
        if (robotIndex == 2)
        {
            wrongRobot.SetActive(false);
            greyRobot.SetActive(false);

            redRobot.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        

        if (defectManaeger.defects == DefectManager.Defects.taskOrderIncorrect)
        {
            if (collision.gameObject.name == "Station 2")
            {
                pulsingAnimation.SetTrigger("Pulsing");
            }
        }

        else if (defectManaeger.defects == DefectManager.Defects.taskOrder)
        {
            if (collision.gameObject.name == "Station 2")
            {
                pulsingAnimation.SetTrigger("Pulsing");
            }
        }

        else if (defectManaeger.defects == DefectManager.Defects.heathTask)
        {
            if (collision.gameObject.name == "Station 4")
            {
                pulsingAnimation.SetTrigger("Pulsing");
            }
        }

        else if (defectManaeger.defects == DefectManager.Defects.skillTask)
        {
            if (collision.gameObject.name == "Station 6")
            {
                pulsingAnimation.SetTrigger("Pulsing");
            }
        }
    }
}