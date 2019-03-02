using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Method was derived from "Foundations of Physically Based
 * Modeling and Animation" by Donald House
*/
public class Vortex : MonoBehaviour {
    

    float axisLength = 3f;

    // pointing from origin to 3
    Vector3 l; 
    Vector3 li;
    float radius = 13f;
    Vector3 center = new Vector3(25f, -20f, 25f);
    Vector3 centerToPoint;
    float projLength;
    float torque = 2f;
    float RRotFreq = 1f;
    float fMax = 30f;


    void Start()
    {
        l = new Vector3(0f, axisLength, 0f).normalized;
    }

    public Vector3 BaseToParticle(Vector3 flakePosition, Vector3 center){
        centerToPoint = flakePosition - center;
        return centerToPoint;
    }

    public float ProjectionLengthAtParticle(Vector3 baseToParticle)
    {

        return Vector3.Dot(l, baseToParticle);
    }

    public Vector3 OrthoVFromAxisToParticle(Vector3 baseToParticle, float projLengthAtParticle)
    {
        return baseToParticle - projLengthAtParticle * l;
    }

    public float RadiusDistFromParticleToAxis(Vector3 radius){

        return radius.magnitude; //i think
    }

    public float Frequency(float ri)
    {
        float frequency = Mathf.Pow((radius / ri), torque) * RRotFreq;
        frequency = Mathf.Min(fMax, frequency);
        return frequency;
    }

    public float GetOmega(float frequencyAtI)
    {
        return 2f * Mathf.PI * frequencyAtI;
    }
   
    public bool InVortexCylinder(float li, float ri)
    {
        if (li>=0 && li<= axisLength && ri < radius){
            return true;
        }
        return false;
    }

    public Vector3 GetCenter() 
    { 
        return center; 
    }

    public Vector3 GetAxis()
    { 
        return l; 
    }

    public void SetAxis(Vector3 newAxis)
    {
        l = newAxis;
    }
}
