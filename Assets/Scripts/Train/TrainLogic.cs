using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainLogic : MonoBehaviour
{
    // Reference Values
    private Vector3 railwayPlacement = new Vector3(0.0f, 0.0f, 2.0f);
    private float startingOffset = -48.0f;
    private float endingOffset = 8.0f;
    private float trainSpeed = 20.0f;
    public GameObject[] trainLoot = new GameObject[2];
    private int[] eventThresholds = { 2, 1 };
    private Vector3 spawnOffset = new Vector3(0.0f, 30.0f);

    // Variables for managing movement
    private bool isMoving = false;
    private float xValToMoveTo = -48.0f;
    private float currSpeed = 0.0f;

    // Call this method to call a randomized train event
    // new events should be added to the train class as functions and thresholds should be added and adjusted
    public void TrainEvent()
    {
        int eventChosen = Random.Range(0, eventThresholds[0] + 1);

        if (eventChosen > eventThresholds[1])
        {
            TrainDeposit();
        }
        else
        {
            SneakyTrain();
        }
    }

    // This event is for when the train pops its head out of the tunnel
    public void SneakyTrain()
    {
        float sneakyOffset = -22.0f;

        // Since Coroutines need to be chained i have created chain architecture for the things that need to happen in sequence
        // like this set of moves. Each Coroutine with the chain thing should have a next that contains another Coroutine that
        // gets called at the end of execution. End the chain with End() which just returns zero.

        // I am so sorry to whoever has to work with this that isn't me
        StartCoroutine(
            MoveToXPos(sneakyOffset, trainSpeed,
            Wait(0.5f,
            MoveToXPos(sneakyOffset - 3, trainSpeed / 12,
            MoveToXPos(startingOffset, trainSpeed,
            End())))));
    }

    // This event is for when the train drops stuff on the field
    private void TrainDeposit()
    {
        StartCoroutine(
            MoveToXPos(endingOffset, trainSpeed,
            SpawnItems(
            Wait(2.0f,
            MoveToXPos(startingOffset, trainSpeed,
            End())))));
    }

    private void ReturnTrain()
    {
        transform.position = railwayPlacement + new Vector3(startingOffset, 0.0f);
    }

    // Calling this will move the train until it gets past the x value provided at a specified speed
    private IEnumerator MoveToXPos(float xpos, float speed, IEnumerator next)
    {
        xValToMoveTo = xpos;
        currSpeed = speed;
        isMoving = true;

        while (isMoving)
        {
            yield return null;
        }

        StartCoroutine(next);
    }

    private IEnumerator Wait(float seconds, IEnumerator next)
    {
        yield return new WaitForSeconds(seconds);

        StartCoroutine(next);
    }

    private IEnumerator End()
    {
        yield return 0;
    }

    private IEnumerator SpawnItems(IEnumerator next)
    {

        while (isMoving)
        {
            yield return null;
        }

        int spawnChosen = Random.Range(0, trainLoot.Length);
        Instantiate(trainLoot[spawnChosen], transform.position + spawnOffset, Quaternion.identity);

        StartCoroutine(next);
    }

    // Update function is what actually moves the train
    private void FixedUpdate()
    {
        if (isMoving)
        {
            float direction = xValToMoveTo - transform.position.x;
            direction = direction / Mathf.Abs(direction);
            var nextPos = transform.position;
            nextPos.x += currSpeed * direction * Time.fixedDeltaTime;
            transform.position = nextPos;

            if ((xValToMoveTo - transform.position.x) * direction <= 0)
            {
                isMoving = false;
            }
        }
    }

    // Allows collision to be triggered by children
    public void HitByTrain(Collision collision)
    {
        if (collision != null && collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().takeDamage(-10 * (transform.position - collision.gameObject.transform.position));
        }
        Debug.Log("Colissy");
    }
}
