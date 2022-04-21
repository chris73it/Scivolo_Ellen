using UnityEngine;
using Cinemachine;

public class FreeLookCameraZoom : MonoBehaviour
{
    [SerializeField] CinemachineFreeLook vcam;
    [SerializeField] float duration; //0.2f
    [SerializeField] float radiusShrinkageFactor; //3f
    
    float maxUpperRadius; //The mid radius is assumed to start at this value.
    float maxMidRadius; //The mid radius is assumed to start at this value.
    float maxLowerRadius; //The mid radius is assumed to start at this value.
    
    float minUpperRadius;
    float minMidRadius;
    float minLowerRadius;

    float startUpperRadius;
    float startMidRadius;
    float startLowerRadius;

    float targetUpperRadius;
    float targetMidRadius;
    float targetLowerRadius;

    float timeElapsed = 0;

    enum ZoomDirection
    {
        ZOOM_IN  = -1,
        NO_ZOOM  =  0,
        ZOOM_OUT = +1,
    }
    ZoomDirection zoomDir = ZoomDirection.NO_ZOOM;

    void Awake()
    {
        maxUpperRadius = vcam.m_Orbits[0].m_Radius; //The initial upper radius is assumed to be at the max radius.
        maxMidRadius = vcam.m_Orbits[1].m_Radius; //The initial mid radius is assumed to be at the max radius.
        maxLowerRadius = vcam.m_Orbits[2].m_Radius; //The initial lower radius is assumed to be at the max radius.

        minUpperRadius = maxUpperRadius / radiusShrinkageFactor;
        minMidRadius = maxMidRadius / radiusShrinkageFactor;
        minLowerRadius = maxLowerRadius / radiusShrinkageFactor;
    }

    public void ZoomIn()
    {
        if (zoomDir != ZoomDirection.ZOOM_IN && vcam.m_Orbits[1].m_Radius > minMidRadius)
        {
            zoomDir = ZoomDirection.ZOOM_IN;
            timeElapsed = (maxMidRadius - vcam.m_Orbits[1].m_Radius) / (maxMidRadius - minMidRadius) * duration;

            startUpperRadius = vcam.m_Orbits[0].m_Radius;
            startMidRadius = vcam.m_Orbits[1].m_Radius;
            startLowerRadius = vcam.m_Orbits[2].m_Radius;
            
            targetUpperRadius = minUpperRadius;
            targetMidRadius = minMidRadius;
            targetLowerRadius = minLowerRadius;
        }
    }

    public void ZoomOut()
    {
        if (zoomDir != ZoomDirection.ZOOM_OUT && vcam.m_Orbits[1].m_Radius < maxMidRadius)
        {
            zoomDir = ZoomDirection.ZOOM_OUT;
            timeElapsed = (vcam.m_Orbits[1].m_Radius - minMidRadius) / (maxMidRadius - minMidRadius) * duration;

            startUpperRadius = vcam.m_Orbits[0].m_Radius;
            startMidRadius = vcam.m_Orbits[1].m_Radius;
            startLowerRadius = vcam.m_Orbits[2].m_Radius;
            
            targetUpperRadius = maxUpperRadius;
            targetMidRadius = maxMidRadius;
            targetLowerRadius = maxLowerRadius;
        }
    }

    void LateUpdate()
    {
        if (zoomDir != 0)
        {
            vcam.m_Orbits[0].m_Radius = Mathf.Lerp(startUpperRadius, targetUpperRadius, timeElapsed / duration);
            vcam.m_Orbits[1].m_Radius = Mathf.Lerp(startMidRadius, targetMidRadius, timeElapsed / duration);
            vcam.m_Orbits[2].m_Radius = Mathf.Lerp(startLowerRadius, targetLowerRadius, timeElapsed / duration);
            timeElapsed += Time.deltaTime;

            if (zoomDir == ZoomDirection.ZOOM_IN && vcam.m_Orbits[1].m_Radius <= 1.01f * minMidRadius)
            {
                zoomDir = ZoomDirection.NO_ZOOM;
                vcam.m_Orbits[0].m_Radius = minUpperRadius;
                vcam.m_Orbits[1].m_Radius = minMidRadius;
                vcam.m_Orbits[2].m_Radius = minLowerRadius;
            }
            else if (zoomDir == ZoomDirection.ZOOM_OUT && vcam.m_Orbits[1].m_Radius >= 0.99f * maxMidRadius)
            {
                zoomDir = ZoomDirection.NO_ZOOM;
                vcam.m_Orbits[0].m_Radius = maxUpperRadius;
                vcam.m_Orbits[1].m_Radius = maxMidRadius;
                vcam.m_Orbits[2].m_Radius = maxLowerRadius;
            }
        }
    }
}