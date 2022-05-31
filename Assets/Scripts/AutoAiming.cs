using UnityEngine;
using UnityEngine.UI;

public class AutoAiming : MonoBehaviour
{
    [SerializeField] Transform muzzle;
    [SerializeField] Image crosshairIdle; //Notice that this is also used as an "empty hit"
    [SerializeField] Image crosshairNoHit;
    [SerializeField] Image crosshairRedHit;
    [SerializeField] Image crosshairWhiteHit;
    [SerializeField] Image avatarCrosshairX;
    [SerializeField] Image avatarCrosshairO;

    const float debugDrawLineDuration = 0.1f;
    const float minCrosshairDistance = 0.01f;

    bool shouldFire = false;
    Ray ray1;
    Ray ray2;
    RaycastHit hitInfo1;
    RaycastHit hitInfo2;
    Camera cam;
    Target target1;
    Target target2;

    private void Awake()
    {
        shouldFire = false;
        crosshairIdle.transform.gameObject.SetActive(false);
        crosshairNoHit.transform.gameObject.SetActive(false);
        crosshairRedHit.transform.gameObject.SetActive(false);
        crosshairWhiteHit.transform.gameObject.SetActive(false);
        avatarCrosshairX.transform.gameObject.SetActive(false);
        avatarCrosshairO.transform.gameObject.SetActive(false);
    }

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    public bool StartAiming()
    {
        shouldFire = false;
        crosshairIdle.transform.gameObject.SetActive(true);
        crosshairNoHit.transform.gameObject.SetActive(false);
        crosshairRedHit.transform.gameObject.SetActive(false);
        crosshairWhiteHit.transform.gameObject.SetActive(false);
        avatarCrosshairX.transform.gameObject.SetActive(false);
        avatarCrosshairO.transform.gameObject.SetActive(false);

        ray1.origin = transform.position;
        ray1.direction = transform.forward;
        if (Physics.Raycast(ray1, out hitInfo1))//, Mathf.Infinity, layerMask))
        {
            Debug.DrawLine(ray1.origin, hitInfo1.point, Color.white, debugDrawLineDuration);

            target1 = hitInfo1.transform.gameObject.GetComponent<Target>();
            if (target1 == null)
            {
                //Perform another raycast parallel to the first raycast starting from the muzzle of the gun
                ray2.origin = muzzle.position;
                ray2.direction = hitInfo1.point - muzzle.position;
                if (Physics.Raycast(ray2, out hitInfo2))
                {
                    Debug.DrawLine(ray2.origin, hitInfo2.point, Color.red, debugDrawLineDuration);

                    crosshairIdle.transform.gameObject.SetActive(false);
                    crosshairNoHit.transform.gameObject.SetActive(true);
                    target2 = hitInfo2.transform.gameObject.GetComponent<Target>();
                    if (target2 != null)
                    {
                        shouldFire = true;
                        avatarCrosshairO.transform.gameObject.SetActive(true);
                        Vector3 screenPos = cam.WorldToScreenPoint(hitInfo2.point);
                        avatarCrosshairO.transform.position = screenPos;
                    }
                }
            }
            else // target1 != null
            {
                //Perform another raycast from the muzzle of the gun to the hitInfo.point
                ray2.origin = muzzle.position;
                ray2.direction = hitInfo1.point - muzzle.position;
                if (Physics.Raycast(ray2, out hitInfo2))
                {
                    if (Vector3.Distance(hitInfo1.point, hitInfo2.point) < minCrosshairDistance)
                    {
                        Debug.DrawLine(ray2.origin, hitInfo2.point, Color.green, debugDrawLineDuration);
                    }
                    else
                    {
                        Debug.DrawLine(ray2.origin, hitInfo2.point, Color.red, debugDrawLineDuration);
                    }

                    crosshairIdle.transform.gameObject.SetActive(false);
                    target2 = hitInfo2.transform.gameObject.GetComponent<Target>();
                    if (target2 != null) // and target1 != null
                    {
                        shouldFire = true;
                        //Since both target1 and target2 are not null, the camera crosshair is a hit.
                        if (Vector3.Distance(hitInfo1.point, hitInfo2.point) < minCrosshairDistance)
                        {
                            crosshairRedHit.transform.gameObject.SetActive(true);
                        }
                        else
                        {
                            crosshairWhiteHit.transform.gameObject.SetActive(true);
                            avatarCrosshairO.transform.gameObject.SetActive(true);
                            Vector3 screenPos = cam.WorldToScreenPoint(hitInfo2.point);
                            avatarCrosshairO.transform.position = screenPos;
                        }
                    }
                    else // target2 == null (and target1 != null)
                    {
                        //Since target1 is not null and target2 is null, the camera crosshair is an empty hit,
                        //  because the avatar's crosshair doesn't help hitting it.
                        crosshairIdle.transform.gameObject.SetActive(true);
                        if (Vector3.Distance(hitInfo1.point, hitInfo2.point) >= minCrosshairDistance)
                        {
                            avatarCrosshairX.transform.gameObject.SetActive(true);
                            Vector3 screenPos = cam.WorldToScreenPoint(hitInfo2.point);
                            avatarCrosshairX.transform.position = screenPos;
                        }
                    }
                }
            }
        }
        else
        {
            target1 = null;
            //Perform another raycast parallel to the first raycast starting from the muzzle of the gun
            ray2.origin = muzzle.position;
            ray2.direction = ray1.direction;
            if (Physics.Raycast(ray2, out hitInfo2))
            {
                target2 = hitInfo2.transform.gameObject.GetComponent<Target>();
                if (target2 != null)
                {
                    Debug.DrawLine(ray2.origin, hitInfo2.point, Color.green, debugDrawLineDuration);

                    shouldFire = true;
                    avatarCrosshairO.transform.gameObject.SetActive(true);
                    Vector3 screenPos = cam.WorldToScreenPoint(hitInfo2.point);
                    avatarCrosshairO.transform.position = screenPos;
                }
                else
                {
                    Debug.DrawLine(ray2.origin, hitInfo2.point, Color.red, debugDrawLineDuration);
                }
            }
        }

        return shouldFire;
    }

    public void StopAiming()
    {
        shouldFire = false;
        crosshairIdle.transform.gameObject.SetActive(false);
        crosshairNoHit.transform.gameObject.SetActive(false);
        crosshairRedHit.transform.gameObject.SetActive(false);
        crosshairWhiteHit.transform.gameObject.SetActive(false);
        avatarCrosshairX.transform.gameObject.SetActive(false);
        avatarCrosshairO.transform.gameObject.SetActive(false);
    }

    public void StartFiring()
    {

    }
}
