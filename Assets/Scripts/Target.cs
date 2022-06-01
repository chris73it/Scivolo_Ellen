using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Target : MonoBehaviour
{
    public void Die()
    {
        Destroy(gameObject);
    }
}
