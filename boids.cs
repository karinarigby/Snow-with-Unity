using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boids : MonoBehaviour
{

    /*separation - steer to avoid crowding local flockmates
     *  Search to find other boid in neighborhood
     *  for each close boid
     *      **NORMALIZE and SCALE***
     *      comput repulse force += ((their.pos - this.pos).normalize * 1/radius)) --- position offset vector scaled by 1/r*2?
            //note ^ all summed up
            A^aij = -((ka)/dij)*normalizedDist.IJ ** between single other boid
            


    alignment - steer toward avg heading of local flockmates - head in same direction and/or speed as flockmates
     *  Search to find other boid in neighborhood
     *  for each close boid
     *  ***NORMALIZE and SCALE*** 
     *      desired avg.velocity =  compute avg velocit(or unit forward vector) of near boids
     *      steering vector = this.currVeloc (or unit forward vector) - desired.avg.velocity
     *      a^vij = kv(Vj-Vi)
     * 
    cohesion - steer to move toward avg pos of local flockmates
     *  Search to find other boid in neighborhood
     *  for each close boid
     *      ***NORMALIZE and SCALE***
     *      compute avg position (or centre of gravity) of all close boids
     *      force = ((avg.pos.of.all - this.pos).normalize * 1/radius)) OR steering force can equal target for *seek* steering behavior 
            acij = kcXij

    
        // then sum all togehter after normalizing and scaling all of them? but slide says "flocking boids defined by nine params,  
        weight, dist, angle for all three parts

     * what about edge cases? when there's only one boid, or if boids are on outside, or if theres an object in its way?
     * is this flock caring about "following the leader?" do I need to program that?     (slide 10)
  
    crowd path  following
    leader following
    unaligned collision avoidance
    queuing * bonus? 

        every boid has
     * -origin
     * -radius
     * angle
     * weight
     * 
     * 
     * 
     * watch out for setting initial velocity
     * watch out for settingg initial positions

    so for each boid: calculate all accelerations, and also calculate damper



    */


    float thetaOne = 30.0f;
    float thetaTwo = 340.0f;
    Vector3 velocity = new Vector3(0.0f,0.01f,0.0f);
    Vector3 oldPosition = new Vector3(0, 0, 0);
    Vector3 relativeOldPosition = new Vector3(0, 0.1f, 0.0f);



  //  float threshold1 = 1.0f;
   // float threshold2 = 0.5f;
    // private Random random = new Random(); //random.


    void Start()
    {
        
        oldPosition = transform.position - relativeOldPosition;
        velocity.Set(0.0f, 0.1f, 0.0f);

        //initialize positions
        //   oldPosition = transform.position - Vector3 (transform.position.x, transform.position.y, transform.position.z-0.2f);//******
        // theta = 
        //relativeOldPosition.Set(Random.Range(0.1f, 0.2f), Random.Range(0.1f, 0.2f), Random.Range(0.1f, 0.2f));
       // velocity.Set(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
    }

    public float GetThetaOne(){
        return thetaOne;
    }

    public float GetThetaTwo(){
        return thetaTwo;
    }

    public Vector3 getVelocity(){
        return velocity;
    }
   
    public void setVelocity(Vector3 newVelocity){
        velocity=newVelocity;

    }
    public Vector3 getOldPosition(){
        return oldPosition;
    }

    public void setOldPosition(Vector3 newOldPosition)
    {
        oldPosition = newOldPosition;

    }
}
