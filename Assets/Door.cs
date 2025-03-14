using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Door : MonoBehaviour
{
    public Animator animator; 
    public bool playAnimation = true; 

    private bool playerInRange = false; 

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.V)) 
        {
            if (playAnimation && animator != null)
            {
                StartCoroutine(OpenDoorWithAnimation());
            }
            else
            {
                OpenDoorWithoutAnimation();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    private IEnumerator OpenDoorWithAnimation()
    {
        animator.SetTrigger("Open"); 
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(stateInfo.length); 
        LoadNextScene(); 
    }

    private void OpenDoorWithoutAnimation()
    {
        LoadNextScene(); 
    }

    private void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = (currentSceneIndex + 1) % SceneManager.sceneCountInBuildSettings;
        SceneManager.LoadScene(nextSceneIndex);
    }
}