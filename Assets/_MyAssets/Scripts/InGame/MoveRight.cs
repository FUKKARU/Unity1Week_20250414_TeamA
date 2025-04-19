using UnityEngine;

public class MoveRight : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        transform.position += Vector3.right * speed * Time.deltaTime;
    }
}