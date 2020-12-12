using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class MovPlatformBehavior : StateTrackedObject
{
    public GameObject waterPrefab;

    protected override void Start()
    {
        base.Start();
        Instantiate(waterPrefab, transform.position, Quaternion.identity);
    }
}
