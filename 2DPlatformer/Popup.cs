using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//simple class that triggers a popup message once the character passes through the collider
public class Popup : MonoBehaviour
{
    Animator anim;

    private void Awake()
    {
        anim = GameObject.FindGameObjectWithTag("Popup").GetComponent<Animator>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !anim.GetBool("exit"))
        {
            anim.SetTrigger("Popup");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {

            anim.SetTrigger("Popdown");
            anim.SetBool("exit", true);
            //anim = GameObject.FindGameObjectWithTag("Popup").GetComponent<Animator>();
            GetComponent<Animator>().enabled = false;
        }
    }
}