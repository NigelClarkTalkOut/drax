using System.Collections;
using UnityEngine;

public class HealthWorkerAnimation : MonoBehaviour
{
    [SerializeField] Animator workerAnimation;
    [SerializeField] Transform upperPosition;
    [SerializeField] Transform lowerPosition;
    [SerializeField] Transform conveyorBelt;

    // worker stats
    [SerializeField] float workerSpeed;

    // timings
    WaitForSeconds oneSecond = new WaitForSeconds(1);


    private void OnEnable()
    {
        StartCoroutine(nameof(WorkerMovement));
    }

    IEnumerator WorkerMovement()
    {
        while (true)
        {
            // worker going on position 1
            while (transform.position != upperPosition.position)
            {

                transform.position = Vector3.MoveTowards(transform.position, upperPosition.position, workerSpeed * Time.deltaTime);

                Rotation(upperPosition);

                yield return null;
            }
            // worker animation
            transform.rotation = Quaternion.Euler(0, 0, 180);
            workerAnimation.ResetTrigger("Idel");
            workerAnimation.SetTrigger("Pick_up");
            yield return oneSecond;
            workerAnimation.ResetTrigger("Pick_up");
            workerAnimation.SetTrigger("Idel");

            // worker going to position 2
            while (transform.position != lowerPosition.position)
            {
                transform.position = Vector3.MoveTowards(transform.position, lowerPosition.position, workerSpeed * Time.deltaTime);

                Rotation(lowerPosition);

                yield return null;
            }
            // worker animation
            transform.rotation = Quaternion.Euler(0, 0, 90);
            workerAnimation.ResetTrigger("Idel");
            workerAnimation.SetTrigger("Pick_up_2");
            yield return oneSecond;
            workerAnimation.ResetTrigger("Pick_up_2");
            workerAnimation.SetTrigger("Idel");

            // worker going to comveyor belt
            while (transform.position != conveyorBelt.position)
            {
                transform.position = Vector3.MoveTowards(transform.position, conveyorBelt.position, workerSpeed * Time.deltaTime);

                transform.rotation = Quaternion.Euler(0, 0, 90);
                yield return null;
            }
        }
    }

    void Rotation(Transform nextPosition)
    {
        // tug will look at the next point
        Vector2 direction = new Vector2(nextPosition.position.y - transform.position.y, nextPosition.position.x - transform.position.x);
        // rotate the tug in the next point's direction
        transform.up = -direction;
    }
}