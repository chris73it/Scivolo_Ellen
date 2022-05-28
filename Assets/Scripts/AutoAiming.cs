using UnityEngine;

public class AutoAiming : MonoBehaviour
{
    public bool isFiring = false;
    [SerializeField] Camera _camera;
    [SerializeField] Transform muzzle;
    [SerializeField] GameObject whiteTarget;
    [SerializeField] GameObject yellowTarget;
    [SerializeField] GameObject redTarget;

    Ray ray;
    Ray ray2;
    RaycastHit hitInfo;
    RaycastHit hitInfo2;

    float whiteTargetDistance = 9f;

    public void StartFiring()
    {
        isFiring = true;

        //Perform a raycast from the center of the camera, and
        //  mark it with a white sphere if it hits something, otherwise
        //  mark it with a yellow sphere if it hits nothing in particular, like the sky.
        // NOTE The distance of the yellow sphere should be the same as the last distance of the white sphere.
        ray.origin = transform.position;
        ray.direction = transform.forward;
        if (Physics.Raycast(ray, out hitInfo))
        {
            whiteTarget.SetActive(true);
            yellowTarget.SetActive(false);
            whiteTarget.transform.position = hitInfo.point;
            whiteTargetDistance = Vector3.Distance(ray.origin, hitInfo.point);
            Debug.DrawLine(ray.origin, hitInfo.point, Color.white, 0.1f);

            //Perform another raycast from the muzzle of the gun to the hitInfo.point (white target.)
            ray2.origin = muzzle.position;
            ray2.direction = hitInfo.point - muzzle.position;
            if (Physics.Raycast(ray2, out hitInfo2))
            {
                redTarget.SetActive(true);
                redTarget.transform.position = hitInfo2.point;
                if (Vector3.Distance(whiteTarget.transform.position, redTarget.transform.position) < 0.1f)
                {
                    Debug.DrawLine(ray2.origin, hitInfo2.point, Color.green, 0.1f);
                }
                else
                {
                    Debug.DrawLine(ray2.origin, hitInfo2.point, Color.red, 0.1f);
                }
            }
        }
        else
        {
            whiteTarget.SetActive(false);
            yellowTarget.SetActive(true);
            redTarget.SetActive(false);
            yellowTarget.transform.position = ray.GetPoint(whiteTargetDistance);
        }
    }

    public void StopFiring()
    {
        //Debug.Log("StopFiring");
        isFiring = false;
        whiteTarget.SetActive(false);
        yellowTarget.SetActive(false);
        redTarget.SetActive(false);
    }
}
