using UnityEngine;

public class Bullet : MonoBehaviour
{
    Vector3 direction;
    Vector3 destination;
    float speed;
    Vector3 step;

    public void SetUp(Vector3 destination, float speed)
    {
        this.direction = (destination - transform.position).normalized;
        this.destination = destination;
        this.speed = speed;
        Destroy(gameObject, 1f);
    }

    void Update()
    {
        step = direction * speed * Time.deltaTime;
        transform.position += step;
        Vector3.Distance(transform.position, destination);
    }
}
