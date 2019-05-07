using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReleaseJump : MonoBehaviour {

    PlayerPlatformerController controller;
    Animator anim;

    private void Awake()
    {
        controller = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPlatformerController>();
        anim = GameObject.FindGameObjectWithTag("Jump").GetComponent<Animator>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //unlock jump
        if (collision.gameObject.tag == "Player" && !anim.GetBool("exit"))
        {
            anim.SetTrigger("Popup");
            controller.setJump(true);

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {

            anim.SetTrigger("Popdown");
            anim.SetBool("exit", true);
            //anim = GameObject.FindGameObjectWithTag("Popup").GetComponent<Animator>();

        }
    }
}
