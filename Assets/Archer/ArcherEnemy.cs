using UnityEngine;
using System.Collections;

public class ArcherEnemy : MonoBehaviour
{
    public GameObject volleyPrefab;
    public float shootInterval = 2f;
    public float volleyDelay = 1f;
    public float playerPositionDelay = 0.5f;
    public Animator animator;
    public int health = 100;
    public float volleyHeightOffset = 2f;
    public float detectionRange = 10f; 

    private Transform player;
    private float nextShootTime;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        nextShootTime = Time.time + shootInterval;
    }

    void Update()
    {
        if (Time.time >= nextShootTime && IsPlayerInRange())
        {
            StartCoroutine(ShootVolley());
            nextShootTime = Time.time + shootInterval;
        }
    }

    bool IsPlayerInRange()
    {
        return Vector2.Distance(transform.position, player.position) <= detectionRange;
    }

    IEnumerator ShootVolley()
    {
        animator.SetTrigger("ShootArrows");
        yield return new WaitForSeconds(volleyDelay);

        Vector2 targetPosition = player.position;
        yield return new WaitForSeconds(playerPositionDelay);

        GameObject volley = Instantiate(volleyPrefab, new Vector2(targetPosition.x, targetPosition.y + volleyHeightOffset), Quaternion.identity);
        volley.GetComponent<Volley>().Initialize(targetPosition, volleyHeightOffset);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        animator.SetTrigger("Death");
        Destroy(gameObject, 1.5f); 
    }
}