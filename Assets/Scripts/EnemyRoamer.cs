using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyRoamer : MonoBehaviour
{
    private NavMeshAgent agent;
    public float roamRadius = 10f;
    public float roamInterval = 5f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        StartCoroutine(StartRoamingAfterDelay(5f));
    }

    private IEnumerator StartRoamingAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(Roam());
    }

    private IEnumerator Roam()
    {
        while (true)
        {
            Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
            randomDirection += transform.position;
            NavMeshHit hit;

            NavMesh.SamplePosition(randomDirection, out hit, roamRadius, NavMesh.AllAreas);
            Vector3 finalPosition = hit.position;

            agent.SetDestination(finalPosition);

            while (agent.remainingDistance > agent.stoppingDistance)
            {
                yield return null;
            }

            yield return new WaitForSeconds(roamInterval);
        }
    }
}