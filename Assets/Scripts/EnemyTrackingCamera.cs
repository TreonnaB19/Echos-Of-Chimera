using UnityEngine;

public class EnemyTrackingCamera : MonoBehaviour
{
    public Transform targetEnemy;
    public Vector3 offset = new Vector3(0f, 5f, 10f);
    public float smoothSpeed = 0.125f;

    void LateUpdate()
    {
        if (targetEnemy != null)
        {
            Vector3 desiredPosition = targetEnemy.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

            transform.position = smoothedPosition;
            transform.LookAt(targetEnemy);
        }
    }
}