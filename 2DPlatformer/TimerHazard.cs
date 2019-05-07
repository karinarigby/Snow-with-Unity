using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//used for the thorn. This class set
public class TimerHazard : MonoBehaviour
{
    PlayerHealth playerHealth;
    bool move;

    //the thorn animator
    Animator anim;
    public Animator levelAnim;
    public float waitTime = 1.2f;

    private void Awake()
    {
       playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        StartCoroutine(Movement());
    }

    //this coroutine will cause the thorn asset to move up and down continuously while the game is running
    IEnumerator Movement()
    {
        while (true)
        {
            anim.SetTrigger("Move");
            move = !move;
            yield return new WaitForSeconds(1.6f);
        }
    }

//if character hits the collider attached
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //trigger player death
        if(collision.gameObject.tag == "Player")
        {
            playerHealth.Die(transform);
            StartCoroutine("ReloadGame");

        }
    }

    IEnumerator ReloadGame()
    {
        //pause briefly
        yield return new WaitForSeconds(waitTime);
    
        //fade screen to black
        levelAnim.SetTrigger("FadeOut"); 

        //reload current level
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}