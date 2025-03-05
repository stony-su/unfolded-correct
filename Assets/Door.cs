using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Door : MonoBehaviour
{
    public Animator animator; // Reference to the Animator component
    public bool playAnimation = true; // Whether to play the animation or not

    private bool playerInRange = false; // To check if the player is in range

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E)) // Assuming 'E' is the key to interact
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
        animator.SetTrigger("Open"); // Trigger the door open animation
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(stateInfo.length); // Wait for the animation to finish
        LoadNextScene(); // Load the next scene
    }

    private void OpenDoorWithoutAnimation()
    {
        LoadNextScene(); // Load the next scene immediately
    }

    private void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = (currentSceneIndex + 1) % SceneManager.sceneCountInBuildSettings;
        SceneManager.LoadScene(nextSceneIndex);
    }
}