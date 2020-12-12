using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GameState : MonoBehaviour
{

    private enum Direction
    {
        UP,
        DOWN,
        LEFT,
        RIGHT
    }
    
    private Vector2 minpos = new Vector2(0, 0);
    private Vector2 maxpos = new Vector2(0, 0);
    private List<StateTrackedObject> water = new List<StateTrackedObject>();
    private List<StateTrackedObject> stillPlatforms = new List<StateTrackedObject>();
    private List<StateTrackedObject> movPlatforms = new List<StateTrackedObject>();

    private List<Move> moves = new List<Move>();


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
            if(vel.magnitude > maxSpd)
            {
                vel = vel.normalized * maxSpd;
            }
            pos += vel;
            
            done = done || (vel.x > 0 && pos.x > to.x);
            done = done || (vel.x < 0 && pos.x < to.x);
            done = done || (vel.y > 0 && pos.y > to.y);
            done = done || (vel.y < 0 && pos.y < to.y);
            if(done)
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

    private void updateBounds(Transform trans) {
        minpos.x = Math.Min(trans.position.x, minpos.x);
        minpos.y = Math.Min(trans.position.y, minpos.y);
        maxpos.x = Math.Max(trans.position.x, maxpos.x);
        maxpos.y = Math.Max(trans.position.y, maxpos.y);
    }

    public void register(StateTrackedObject obj) {
        if(obj is WaterBehavior) {
            WaterBehavior water = obj.GetComponent<WaterBehavior>();
            Assert.IsNotNull(water);
            this.water.Add(obj);
        } else if(obj is MovPlatformBehavior) {
            MovPlatformBehavior platform = obj.GetComponent<MovPlatformBehavior>();
            Assert.IsNotNull(platform);
            this.movPlatforms.Add(obj);
        } else if(obj is StillPlatformBehavior) {
            StillPlatformBehavior platform = obj.GetComponent<StillPlatformBehavior>();
            Assert.IsNotNull(platform);
            this.stillPlatforms.Add(obj);
        } else {
            // Should never be reached
            Assert.IsTrue(false);
        }
        updateBounds(obj.transform);
    }

    void Start()
    {
        
    }

    

    private StateTrackedObject[,] buildState()
    {
        StateTrackedObject[,] state = new StateTrackedObject[((int)maxpos.x - (int)minpos.x) + 1, ((int)maxpos.y - (int)minpos.y) + 1];
        foreach(StateTrackedObject item in water)
        {
            StateTrackedObject curr = state[(int)(item.transform.position.x - minpos.x), (int)(item.transform.position.y - minpos.y)];
            Assert.IsNull(curr);
            state[(int)(item.transform.position.x - minpos.x), (int)(item.transform.position.y - minpos.y)] = item;
        }
        foreach (StateTrackedObject item in stillPlatforms)
        {
            StateTrackedObject curr = state[(int)(item.transform.position.x - minpos.x), (int)(item.transform.position.y - minpos.y)];
            Assert.IsNull(curr);
            state[(int)(item.transform.position.x - minpos.x), (int)(item.transform.position.y - minpos.y)] = item;
        }
        foreach (StateTrackedObject item in movPlatforms)
        {
            StateTrackedObject curr = state[(int)(item.transform.position.x - minpos.x), (int)(item.transform.position.y - minpos.y)];
            Assert.IsTrue(curr == null || curr is WaterBehavior);
            state[(int)(item.transform.position.x - minpos.x), (int)(item.transform.position.y - minpos.y)] = item;
        }

        return state;
    }

    private void checkForMove(StateTrackedObject[,] state, StateTrackedObject[,] nextState, int x, int y, int dx, int dy)
    {
        StateTrackedObject curr = state[x, y];
        if(curr == null)
        {
            return;
        } else if(curr is WaterBehavior || curr is StillPlatformBehavior)
        {
            nextState[x, y] = curr;
            return;
        } else if(curr is MovPlatformBehavior)
        {
            int nextX = x;
            int nextY = y;
            while(true)
            {
                int testX = nextX + dx;
                int testY = nextY + dy;

                if(testX < 0 || testY < 0
                    || testX >= state.GetLength(0) || testY >= state.GetLength(1)
                    || nextState[testX, testY] == null
                    || !( nextState[testX, testY] is WaterBehavior) )
                {
                    // Yeah, this is a hack
                    // I need to mark that the space previously occupied by the movPlatform is now free
                    // However, I don't have easy access to the StateTrackedObject representing the water tile at that location
                    // As a workaround, I'm using the StateTrackedObject representing the water tile at the space now occupied by the movPlatform
                    nextState[x, y] = state[nextX, nextY];
                    nextState[nextX, nextY] = curr;
                    if(x != nextX || y != nextY)
                    {
                        moves.Add(new Move(new Vector2(x, y) + minpos, new Vector2(nextX, nextY) + minpos, curr.transform));
                    }
                    return;
                }

                nextX = testX;
                nextY = testY;
            }
        } else
        {
            Assert.IsTrue(false);
        }

    }

    private void flow(Direction direction)
    {
        Assert.IsTrue(minpos.x < maxpos.x);
        Assert.IsTrue(minpos.y < maxpos.y);
        StateTrackedObject[,] state = buildState();
        StateTrackedObject[,] nextState = new StateTrackedObject[state.GetLength(0), state.GetLength(1)];
        
        switch(direction)
        {
            case Direction.DOWN:
                for(int y = 0; y < state.GetLength(1); y++)
                {
                    for(int x = 0; x < state.GetLength(0); x++)
                    {
                        checkForMove(state, nextState, x, y, 0, -1);
                    }
                }
                break;
            case Direction.UP:
                for(int y = state.GetLength(1) - 1; y >= 0; y--)
                {
                    for(int x = 0; x < state.GetLength(0); x++)
                    {
                        checkForMove(state, nextState, x, y, 0, 1);
                    }
                }
                break;
            case Direction.LEFT:
                for(int x = 0; x < state.GetLength(0); x++)
                {
                    for(int y = 0; y < state.GetLength(1); y++)
                    {
                        checkForMove(state, nextState, x, y, -1, 0);
                    }
                }
                break;
            case Direction.RIGHT:
                for(int x = state.GetLength(0) - 1; x >= 0; x--)
                {
                    for(int y = 0; y < state.GetLength(1); y++)
                    {
                        checkForMove(state, nextState, x, y, 1, 0);
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
        if(moves.Count == 0)
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
        foreach(Move move in moves)
        {
            move.tick();
            if(move.isDone())
            {
                doneMoves.Add(move);
            }
        }
        foreach(Move move in doneMoves)
        {
            moves.Remove(move);
        }
    }
}
