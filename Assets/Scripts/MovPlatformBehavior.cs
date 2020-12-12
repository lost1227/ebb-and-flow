using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class MovPlatformBehavior : StateTrackedObject
{
    public GameObject waterPrefab;

    protected override void trackedStart()
    {
        Instantiate(waterPrefab, transform.position, Quaternion.identity);
    }
}
