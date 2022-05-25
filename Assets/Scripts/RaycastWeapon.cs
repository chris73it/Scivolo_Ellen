using UnityEngine;

public class RaycastWeapon : MonoBehaviour
{
    public bool isFiring = false;
    public Transform muzzle;
    public Transform target;

    Ray ray;
    RaycastHit hitInfo;

    public void StartFiring()
    {
        //Debug.Log("StartFiring");
        isFiring = true;
        ray.origin = muzzle.position;
        ray.direction = target.position - muzzle.position;
        if (Physics.Raycast(ray, out hitInfo))
        {
            Debug.DrawLine(ray.origin, hitInfo.point, Color.red, 1.0f);
        }
    }

    public void StopFiring()
    {
        //Debug.Log("StopFiring");
        isFiring = false;
    }
}