using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAi : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround;
    public LayerMask whatIsPlayer;
    public float health;

    // Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;
    public float patrolWaitTime = 5f; // Duration to wait after each patrol

    private bool isWaiting = false; // Track if AI is waiting after patrol

    // Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject projectile;

    // States
    public float sightRange;
    public float attackRange;
    public bool playerInSightRange;
    public bool playerInAttackRange;

    Animator animator;

    private void Awake()
    {
        player = GameObject.Find("Character").transform;
        agent = GetComponent<NavMeshAgent>();

        animator = gameObject.GetComponent<Animator>();
        animator.SetBool("walking", true); // Start with walking animation
    }

    // Start is called before the first frame update
    void Start()
    {
        animator.SetBool("walking", true); // Ensure walking animation on start
    }

    // Update is called once per frame
    void Update()
    {
        // Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        // Handle states based on player proximity
        if (!playerInSightRange && !playerInAttackRange && !isWaiting)
            Patroling();
        if (playerInSightRange && !playerInAttackRange)
            ChasePlayer();
        if (playerInAttackRange && playerInSightRange)
            AttackPlayer();
    }

    private void Patroling()
    {
        animator.SetBool("walking", true);  // Set to walking animation

        if (!walkPointSet)
            SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        // Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f && !isWaiting)
        {
            walkPointSet = false;
            StartCoroutine(WaitAfterPatrol());
        }
    }

    private IEnumerator WaitAfterPatrol()
    {
        isWaiting = true;
        agent.isStopped = true;  // Stop the agent's movement
        animator.SetBool("walking", false); // Switch to idle animation

        float elapsedTime = 0f; // Track the elapsed time

        while (elapsedTime < patrolWaitTime)
        {
            // Break out of the wait if the player enters sight or attack range
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

            if (playerInSightRange || playerInAttackRange)
            {
                isWaiting = false; // Reset waiting flag
                agent.isStopped = false; // Resume movement
                yield break; // Exit the coroutine immediately
            }

            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame and re-check
        }

        // Resume patrolling after waiting if the player has not entered range
        agent.isStopped = false; // Resume the agent's movement
        animator.SetBool("walking", true); // Switch back to walking animation

        isWaiting = false; // Allow the AI to continue patrolling
    }

    private void SearchWalkPoint()
    {
        // Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        animator.SetBool("walking", true); // Set to walking animation
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        // Stop movement and switch to idle during attack
        agent.SetDestination(transform.position);
        animator.SetBool("walking", false); // Switch to idle animation

        transform.LookAt(player);

        // Distance in front of the bot where to instantiate the object
        float distanceInFront = 3.0f;

        // Calculate the position in front of the bot
        Vector3 positionInFront = transform.position + transform.forward * distanceInFront + transform.up * distanceInFront;

        if (!alreadyAttacked)
        {
            // Attack code here
            Rigidbody rb = Instantiate(projectile, positionInFront, Quaternion.identity).GetComponent<Rigidbody>();

            rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
            rb.AddForce(transform.up * 8f, ForceMode.Impulse);

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0) Invoke(nameof(DestroyEnemy), 2f);
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
