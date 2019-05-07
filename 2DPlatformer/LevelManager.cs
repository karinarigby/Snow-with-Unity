
using UnityEngine;
using UnityEngine.SceneManagement;

//in this demo this handles switching from the first level to the second level
public class LevelManager : MonoBehaviour {

    public Animator anim;
    private int levelToLoad = 0;


    private void OnTriggerEnter2D(Collider2D collision)
    {
         FadeToNextLevel(1);
    }

    public void FadeToNextLevel(int levelIndex)
    {
        levelToLoad = levelIndex;

        anim.SetTrigger("FadeOut");
    }

    public void OnFadeComplete()
    {
        SceneManager.LoadScene(1);
    }
}

