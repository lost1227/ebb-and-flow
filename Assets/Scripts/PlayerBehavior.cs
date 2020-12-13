using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerBehavior : MonoBehaviour
{

    public float playerSpeed = 10f;

    public float colliderWidth = 0.1f;
    public float colliderOffset = 0.1f;

    public LayerMask playerColliderMask;

    public LayerMask movPlatformMask;

    private GameState gamestate;
    private BoxCollider2D theCollider;
    // Start is called before the first frame update
    void Start()
    {
        gamestate = Object.FindObjectOfType<GameState>();
        theCollider = gameObject.GetComponent<BoxCollider2D>();
        Assert.IsNotNull(theCollider);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mov = Vector2.zero;
        if(gamestate.state == GameState.State.IDLE || gamestate.state == GameState.State.CROSSING)
        {
            if(Input.GetKey(KeyCode.W))
            {
                mov += new Vector2(0, 1);
            }
            if(Input.GetKey(KeyCode.A))
            {
                mov += new Vector2(-1, 0);
            }
            if(Input.GetKey(KeyCode.S))
            {
                mov += new Vector2(0, -1);
            }
            if(Input.GetKey(KeyCode.D))
            {
                mov += new Vector2(1, 0);
            }

            Vector2 playerBottomLeft = theCollider.bounds.center - theCollider.bounds.extents;
            Vector2 playerTopRight = theCollider.bounds.center + theCollider.bounds.extents;

            Debug.DrawLine(playerTopRight, playerBottomLeft, Color.cyan);

            float playerTop = playerTopRight.y;
            float playerBottom = playerBottomLeft.y;
            float playerRight = playerTopRight.x;
            float playerLeft = playerBottomLeft.x;

            Vector2 hitbox_up_topLeft = new Vector2(playerLeft + colliderOffset, playerTop + colliderWidth);
            Vector2 hitbox_up_bottomRight = new Vector2(playerRight - colliderOffset, playerTop);
            
            Vector2 hitbox_down_topLeft = new Vector2(playerLeft + colliderOffset, playerBottom);
            Vector2 hitbox_down_bottomRight = new Vector2(playerRight - colliderOffset, playerBottom - colliderWidth);

            Vector2 hitbox_left_topLeft = new Vector2(playerLeft - colliderWidth, playerTop - colliderOffset);
            Vector2 hitbox_left_bottomRight = new Vector2(playerLeft, playerBottom + colliderOffset);

            Vector2 hitbox_right_topLeft = new Vector2(playerRight, playerTop - colliderOffset);
            Vector2 hitbox_right_bottomRight = new Vector2(playerRight + colliderWidth, playerBottom + colliderOffset);

            mov = mov.normalized * playerSpeed * Time.deltaTime;

            if (mov.y > 0)
            {
                Collider2D upColl = Physics2D.OverlapArea(hitbox_up_topLeft, hitbox_up_bottomRight, playerColliderMask);
                if(upColl)
                {
                    mov.y = 0;
                    Debug.DrawLine(hitbox_up_topLeft, hitbox_up_bottomRight, Color.red);
                } else
                {
                    Debug.DrawLine(hitbox_up_topLeft, hitbox_up_bottomRight, Color.green);
                }
            }
            if(mov.y < 0)
            {
                Collider2D downColl = Physics2D.OverlapArea(hitbox_down_topLeft, hitbox_down_bottomRight, playerColliderMask);
                if(downColl)
                {
                    mov.y = 0;
                    Debug.DrawLine(hitbox_down_topLeft, hitbox_down_bottomRight, Color.red);
                } else
                {
                    Debug.DrawLine(hitbox_down_topLeft, hitbox_down_bottomRight, Color.green);
                }
            }
            if(mov.x < 0)
            {
                Collider2D leftColl = Physics2D.OverlapArea(hitbox_left_topLeft, hitbox_left_bottomRight, playerColliderMask);
                if(leftColl)
                {
                    mov.x = 0;
                    Debug.DrawLine(hitbox_left_topLeft, hitbox_left_bottomRight, Color.red);
                } else
                {
                    Debug.DrawLine(hitbox_left_topLeft, hitbox_left_bottomRight, Color.green);
                }
            }
            if (mov.x > 0)
            {
                Collider2D rightColl = Physics2D.OverlapArea(hitbox_right_topLeft, hitbox_right_bottomRight, playerColliderMask);
                if (rightColl)
                {
                    mov.x = 0;
                    Debug.DrawLine(hitbox_right_topLeft, hitbox_right_bottomRight, Color.red);
                }
                else
                {
                    Debug.DrawLine(hitbox_right_topLeft, hitbox_right_bottomRight, Color.green);
                }
            }

            transform.position += (Vector3)mov;
        }

        // gamestate.isCrossing = theCollider.IsTouchingLayers(movPlatformMask);
        bool crossing = Physics2D.OverlapArea(theCollider.bounds.center - theCollider.bounds.extents, theCollider.bounds.center + theCollider.bounds.extents, movPlatformMask);
        switch(gamestate.state)
        {
            case GameState.State.IDLE:
                if(crossing)
                {
                    gamestate.state = GameState.State.CROSSING;
                }
                break;
            case GameState.State.CROSSING:
                if(!crossing)
                {
                    gamestate.state = GameState.State.IDLE;
                }
                break;
            default:
                break;
        }
    }
}
