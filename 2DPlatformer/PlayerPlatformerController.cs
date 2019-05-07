using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlatformerController : PhysicsObject
{
    public float maxSpeed = 6;
    public float jumpTakeOffSpeed = 3;
    public float hurtForce = 5f;
    public bool dying;
    // protected bool hasJumped;
    public bool facingRight = true;
    protected bool canJump;

    private Animator animator;

    void Awake()
    {
       //the Animator component controls what animation cycles are playing 
        animator = GetComponent<Animator>();
        canJump = false;
    }

    protected override void ComputeVelocity()
    {
        Vector2 move = Vector2.zero;
        move.x = Input.GetAxis("Horizontal");

        //only allow movement control if not currently dying
        if (canJump)
        {
            if ((Input.GetButtonDown("X") || Input.GetKeyDown("space")) && grounded && animator.GetBool("grounded"))
            {
                velocity.y = jumpTakeOffSpeed;
                // hasJumped = true;
            }

            //Double Jump Logic for later
            /*else if (Input.GetButtonUp("X") && !hasJumped){
              // Debug.Log("X button up and hasnt jumped");
              ///  hasJumped = true;
               if (velocity.y > 0)
                {
                    velocity.y = velocity.y * 0.5f;
                }
            }else if (Input.GetButtonDown("X") && hasJumped){
                //  hasJumped = false;
              //  Debug.Log("X down and has jumped");
                velocity.y = jumpTakeOffSpeed;

            }*/
            else if (Input.GetButtonUp("X"))
            {
                // Debug.Log("X is released /n");

                // if (!hasJumped) { hasJumped = true; }
                //because no matter what, we want to switch the state from whatever it was before.
                /// hasJumped = !hasJumped;

                if (velocity.y > 0)
                {
                    velocity.y = velocity.y * 0.5f;
                }
            }
        }

        if (velocity.x > 0 && !facingRight)
        {
            Flip();
        }else if (velocity.x < 0 && facingRight)
        {
            Flip();
        }

        animator.SetBool("grounded", grounded);

        //the jump animation will be triggered based on the velocity.y amount
        animator.SetFloat("velocityY", velocity.y / maxSpeed);

        //the character will either stay still, walk, or run based on velocity.x
        animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);


        targetVelocity = move * maxSpeed;
    }

    //Flip which way the sprite is facing
    private void Flip()
    {
        facingRight = !facingRight;

        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    public void setJump(bool set) { canJump = set; }

}

