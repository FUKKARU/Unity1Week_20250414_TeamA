using UnityEngine;

public class SpikeMover : MonoBehaviour
{
    public enum MoveDirection { Horizontal, Vertical }
    public MoveDirection moveDirection = MoveDirection.Vertical;

    public float moveDistance = 2f; // ˆÚ“®‹——£
    public float moveSpeed = 2f;    // ˆÚ“®‘¬“x

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool movingToTarget = true;

    void Start()
    {
        startPosition = transform.position;

        if (moveDirection == MoveDirection.Vertical)
            targetPosition = startPosition + Vector3.up * moveDistance;
        else
            targetPosition = startPosition + Vector3.right * moveDistance;
    }

    void Update()
    {
        Vector3 destination = movingToTarget ? targetPosition : startPosition;
        transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, destination) < 0.01f)
        {
            movingToTarget = !movingToTarget;
        }
    }
}
