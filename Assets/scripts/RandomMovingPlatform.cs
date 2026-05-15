using UnityEngine;
using System.Collections.Generic;

public class MovingPlatform : MonoBehaviour
{
    public bool useRandomMovement = true;
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;
    
    public List<Transform> patrolPoints;
    public float speed = 2f;

    private Vector3 targetPosition;
    private int currentPointIndex = 0;

    void Start()
    {
        SetNextTarget();
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            SetNextTarget();
        }
    }

    void SetNextTarget()
    {
        if (useRandomMovement)
        {
            float randomX = Random.Range(minX, maxX);
            float randomY = Random.Range(minY, maxY);
            targetPosition = new Vector3(randomX, randomY, transform.position.z);
        }
        else
        {
            if (patrolPoints != null && patrolPoints.Count > 0)
            {
                targetPosition = patrolPoints[currentPointIndex].position;
                currentPointIndex = (currentPointIndex + 1) % patrolPoints.Count;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}