using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using TMPro;

public class UiBehavior : MonoBehaviour
{
    private GameState gamestate;
    public TextMeshProUGUI moveCounter;
    void Start()
    {
        gamestate = Object.FindObjectOfType<GameState>();
        Assert.IsNotNull(gamestate);
    }

    // Update is called once per frame
    void Update()
    {
        moveCounter.text = string.Format("Moves: {0}", gamestate.moves);
    }
}
