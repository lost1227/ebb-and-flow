using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class FlowPlatforms : MonoBehaviour
{
    private enum Direction
    {
        UP,
        DOWN,
        LEFT,
        RIGHT
    }


    private List<Move> moves = new List<Move>();
    private GameState state;


    private class Move
    {
        public Vector2 from;
        public Vector2 to;
        public Transform who;

        private Vector2 pos;
        private Vector2 vel = Vector2.zero;

        private float maxSpd = 0.1f;
        private float acc = 0.01f;

        bool done = false;
        public Move(Vector2 from, Vector2 to, Transform who)
        {
            this.from = from;
            this.to = to;
            this.who = who;

            pos = from;
        }

        public void tick()
        {
            vel += ((to - from).normalized * acc);
            if (vel.magnitude > maxSpd)
            {
                vel = vel.normalized * maxSpd;
            }
            pos += vel;

            done = done || (vel.x > 0 && pos.x > to.x);
            done = done || (vel.x < 0 && pos.x < to.x);
            done = done || (vel.y > 0 && pos.y > to.y);
            done = done || (vel.y < 0 && pos.y < to.y);
            if (done)
            {
                pos = to;
            }

            who.position = pos;
        }

        public bool isDone()
        {
            return done;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        state = Object.FindObjectOfType<GameState>();
        Assert.IsNotNull(state);
    }

    private void checkForMove(StateTrackedObject[,] staticState, StateTrackedObject[,] dynamicState, StateTrackedObject[,] nextState, int x, int y, int dx, int dy)
    {
        StateTrackedObject staticCurr = staticState[x, y];
        StateTrackedObject dynamicCurr = dynamicState[x, y];
        if (staticCurr == null)
        {
            return;
        }
        else if (dynamicCurr == null)
        {
            nextState[x, y] = staticCurr;
            return;
        }
        else if (dynamicCurr is MovPlatformBehavior)
        {
            int nextX = x;
            int nextY = y;
            while (true)
            {
                int testX = nextX + dx;
                int testY = nextY + dy;

                if (testX < 0 || testY < 0
                    || testX >= staticState.GetLength(0) || testY >= staticState.GetLength(1)
                    || nextState[testX, testY] == null
                    || !(nextState[testX, testY] is WaterBehavior))
                {
                    nextState[x, y] = staticCurr;
                    nextState[nextX, nextY] = dynamicCurr;
                    if (x != nextX || y != nextY)
                    {
                        moves.Add(new Move(this.state.gridToGlobal(new Vector2(x, y)), this.state.gridToGlobal(new Vector2(nextX, nextY)), dynamicCurr.transform));
                    }
                    return;
                }

                nextX = testX;
                nextY = testY;
            }
        }
        else
        {
            Assert.IsTrue(false);
        }

    }

    private void flow(Direction direction)
    {
        StateTrackedObject[,] staticState = this.state.buildStaticState();
        StateTrackedObject[,] dynamicState = this.state.buildDynamicState();
        int width = staticState.GetLength(0);
        int height = staticState.GetLength(1);
        StateTrackedObject[,] nextState = new StateTrackedObject[width, height];

        switch (direction)
        {
            case Direction.DOWN:
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        checkForMove(staticState, dynamicState, nextState, x, y, 0, -1);
                    }
                }
                break;
            case Direction.UP:
                for (int y = height - 1; y >= 0; y--)
                {
                    for (int x = 0; x < width; x++)
                    {
                        checkForMove(staticState, dynamicState, nextState, x, y, 0, 1);
                    }
                }
                break;
            case Direction.LEFT:
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        checkForMove(staticState, dynamicState, nextState, x, y, -1, 0);
                    }
                }
                break;
            case Direction.RIGHT:
                for (int x = width - 1; x >= 0; x--)
                {
                    for (int y = 0; y < height; y++)
                    {
                        checkForMove(staticState, dynamicState, nextState, x, y, 1, 0);
                    }
                }
                break;
            default:
                Assert.IsTrue(false);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (moves.Count == 0)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                flow(Direction.UP);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                flow(Direction.DOWN);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                flow(Direction.LEFT);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                flow(Direction.RIGHT);
            }
        }


        List<Move> doneMoves = new List<Move>();
        foreach (Move move in moves)
        {
            move.tick();
            if (move.isDone())
            {
                doneMoves.Add(move);
            }
        }
        foreach (Move move in doneMoves)
        {
            moves.Remove(move);
        }
    }

    
}
