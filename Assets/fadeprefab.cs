using UnityEngine;

public class FadeOut : MonoBehaviour
{
    public float fadeDuration = 0.5f; // Time it takes to fade out
    private SpriteRenderer spriteRenderer;
    private float timer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        timer = fadeDuration;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            Destroy(gameObject); // Destroy the effect when it fades out
        }
        else
        {
            // Fade out the sprite
            Color color = spriteRenderer.color;
            color.a = timer / fadeDuration;
            spriteRenderer.color = color;
        }
    }
}