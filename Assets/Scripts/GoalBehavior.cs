using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GoalBehavior : StillPlatformBehavior
{
    public LayerMask playerLayerMask;

    private BoxCollider2D theCollider;
    private GameState gamestate;
    protected override void Start()
    {
        base.Start();

        theCollider = gameObject.GetComponent<BoxCollider2D>();
        gamestate = Object.FindObjectOfType<GameState>();
        Assert.IsNotNull(gamestate);
    }

    protected override void Update()
    {
        base.Update();

        bool loading = Physics2D.OverlapArea(theCollider.bounds.center - theCollider.bounds.extents, theCollider.bounds.center + theCollider.bounds.extents, playerLayerMask);
        if(loading)
        {
            gamestate.loadNextScene();
        }
    }
}
