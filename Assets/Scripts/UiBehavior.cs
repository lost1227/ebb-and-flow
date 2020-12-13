using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using TMPro;

public class UiBehavior : MonoBehaviour
{
    private GameState gamestate;
    public TextMeshProUGUI moveCounter;
    public TextMeshProUGUI levelCounter;

    public int levelNo;
    void Start()
    {
        gamestate = Object.FindObjectOfType<GameState>();
        Assert.IsNotNull(gamestate);

        levelCounter.text = string.Format("Level: {0} / 10", levelNo);
    }

    // Update is called once per frame
    void Update()
    {
        moveCounter.text = string.Format("Moves: {0}", gamestate.moves);
    }
}
