using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//is attached to main character object and is used mainly for when character dies. In future this will contain health tracking, etc.
public class PlayerHealth : MonoBehaviour {

    bool alive;
    Animator anim;
    PlayerPlatformerController controller;


	void Awake() {
        //animator of the character
        anim = GetComponent<Animator>();
        controller = GetComponent<PlayerPlatformerController>();

        alive = true;
        anim.SetBool("dying", false);
    }

    public void Die(Transform source)
    {
        if (alive)
        {
            alive = false;

            //allows character to pass through anything else that could be near while in die sequence
            Collider2D[] cols = GetComponents<Collider2D>();
            foreach(Collider2D c in cols)
            {
                c.isTrigger = true;
            }

            // Trigger Die animation state
            anim.SetTrigger("Die"); 
           anim.SetBool("dying",true);

            //disable player control script
            GetComponent<PlayerPlatformerController>().enabled = false;

        }
    }

}
