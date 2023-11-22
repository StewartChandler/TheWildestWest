using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StartTrainManager : MonoBehaviour
{

    private bool trainCalled = false;

    // Update is called once per frame
    void Update()
    {

        if (!trainCalled)
        {
            GameObject train = GameObject.FindGameObjectWithTag("Train");
            TrainLogic trainLogic = train.GetComponentInChildren<TrainLogic>();
            trainLogic.InvokeRepeating("TrainEvent", 5.0f, 20.0f);
            trainCalled = true;
        }


    }
}