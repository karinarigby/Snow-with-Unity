using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyParticle : MonoBehaviour {

    // Use this for initialization
    float lifespan;
    Vector3 velocityDirection;
    float speed;
    Vector3 relativeOldPosition = new Vector3(-0.00f, -0.03f, 0.0f);
    Vector3 oldPosition = new Vector3(0, 0, 0);
    float angularVelocity;
    int counter;
    Vector3 initPosition;

	void Start () {
        lifespan = Time.deltaTime;
        oldPosition = transform.position - relativeOldPosition;
        velocityDirection = new Vector3 (0f,-1f,0f);
        speed = 1f;
	}

	void Update () {
        lifespan += Time.deltaTime;
	}

    public void setVelocity(Vector3 newVelocity, float newSpeed) { 
        velocityDirection = newVelocity;
        speed = newSpeed; 
    }

    public Vector3 GetVelocity() { return speed * velocityDirection;}

    public float GetLifespan() { return lifespan; }

    public void ResetLifespan() { lifespan = 0f;}

    public Vector3 GetOldPosition()
    {
        return oldPosition;
    }

    public void SetOldPosition(Vector3 newOldPosition)
    {
        oldPosition = newOldPosition;

    }

    public void SetInitUnitPosition(Vector3 position){
        initPosition = position;
    }

    public Vector3 getInitUnitPosition() { return initPosition; }

    public int GetCount() { return counter; }
    public void IncCount() { counter++; }


    public void SetCount(int newCount) { counter = newCount; }
    public Vector3 GetRelativeOldPosition()
    {
        return relativeOldPosition;
    }


    internal void SetActive(bool v)
    {
        throw new NotImplementedException();
    }
}
