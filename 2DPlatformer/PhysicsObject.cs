using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*From Unity's Live Session on 2D Platformer Character Controller
*  https://unity3d.com/learn/tutorials/topics/2d-game-creation/scripting-gravity?playlist=17093
*/
public class PhysicsObject : MonoBehaviour
{
    //the min angle required between player and ground for ground to be considered ground and not other surface (say wall)
    public float minGroundNormalY = .65f;
    public float gravityModifier = 2f;

    protected Vector2 targetVelocity;
    protected bool grounded;
    protected bool hasJumped;
   

    protected Vector2 groundNormal;
    protected Rigidbody2D rb2d;
    protected Vector2 velocity;

    //which colliders to consider or not
    protected ContactFilter2D contactFilter;
    protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
    protected List<RaycastHit2D> hitBufferList = new List<RaycastHit2D>(16);

    //
    protected const float minMoveDistance = 0.001f;

    //a buffer to ensure that we dont get stuck inside of a collider
    protected const float shellRadius = 0.01f;

    void OnEnable()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        contactFilter.useTriggers = false;
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        contactFilter.useLayerMask = true;
        hasJumped = false;
      
    }

    void Update()
    {
        targetVelocity = Vector2.zero;
        ComputeVelocity();
        //rb2d.AddForce(new Vector3(.3f, 0, 0));
    }

    protected virtual void ComputeVelocity()
    {

    }

    void FixedUpdate()
    {
        velocity += gravityModifier * Physics2D.gravity * Time.deltaTime;
        velocity.x = targetVelocity.x;

        grounded = false;


        Vector2 deltaPosition = velocity * Time.deltaTime;

        //this allows object to keep moving in horizontal direction if character hits the ceiling or something
        Vector2 moveAlongGround = new Vector2(groundNormal.y, -groundNormal.x);

        Vector2 move = moveAlongGround * deltaPosition.x;

        Movement(move, false);

        move = Vector2.up * deltaPosition.y;

        Movement(move, true);
    }

    void Movement(Vector2 move, bool yMovement)
    {
        float distance = move.magnitude;

        if (distance > minMoveDistance)
        {
            int count = rb2d.Cast(move, contactFilter, hitBuffer, distance + shellRadius);
            hitBufferList.Clear();
            for (int i = 0; i < count; i++)
            {
                hitBufferList.Add(hitBuffer[i]);
            }

            for (int i = 0; i < hitBufferList.Count; i++)
            {
                Vector2 currentNormal = hitBufferList[i].normal;
                if (currentNormal.y > minGroundNormalY)
                {
                    grounded = true;
                  //      hasJumped = false;
                    if (yMovement)
                    {
                        groundNormal = currentNormal;
                        currentNormal.x = 0;
                    }
                }

                float projection = Vector2.Dot(velocity, currentNormal);
                if (projection < 0)
                {
                    velocity = velocity - projection * currentNormal;
                }

                float modifiedDistance = hitBufferList[i].distance - shellRadius;
                distance = modifiedDistance < distance ? modifiedDistance : distance;
            }


        }

        rb2d.position = rb2d.position + move.normalized * distance;
    }

}
