using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartBehavior : StillPlatformBehavior
{

    public GameObject playerFab;

    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();
        Instantiate(playerFab, transform.position, Quaternion.identity);
    }
}
