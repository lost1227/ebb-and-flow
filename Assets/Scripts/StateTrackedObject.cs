using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class StateTrackedObject : MonoBehaviour
{
    private GameState state;
    // Start is called before the first frame update
    void Start()
    {
        state = Object.FindObjectOfType<GameState>();
        Assert.IsNotNull(state);

        state.register(this);

        trackedStart();
    }

    protected virtual void trackedStart() { }

    // Update is called once per frame
    void Update()
    {
        
    }
}
