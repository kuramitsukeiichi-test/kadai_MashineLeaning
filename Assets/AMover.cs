using UnityEngine;
using System.Collections;

public class AMover : MonoBehaviour
{
    public Vector3 startPoint;
    public Vector3 endPoint;

    [Header("ˆÚ“®‘¬“x (m/s)")]
    public float moveSpeed = 10f;

    [Header("ƒ‚ƒfƒ‹‚Ì‘O•ûŒü•â³")]
    public Vector3 modelRotationOffset = new Vector3(90f, 0f, 0f);

    public bool isMoving { get; private set; }

    public void StartMove()
    {
        StopAllCoroutines();
        StartCoroutine(MoveRoutine());
    }

    IEnumerator MoveRoutine()
    {
        isMoving = true;

        transform.position = startPoint;
        Vector3 dir = (endPoint - startPoint).normalized;
        transform.rotation = Quaternion.LookRotation(dir) * Quaternion.Euler(modelRotationOffset);

        float totalDistance = Vector3.Distance(startPoint, endPoint);
        float moved = 0f;

        while (moved < totalDistance)
        {
            float step = moveSpeed * Time.deltaTime;
            transform.position += dir * step;
            moved += step;
            yield return null;
        }

        transform.position = endPoint;
        isMoving = false;
    }

}
