using UnityEngine;
using System.Collections;

public class RotatingPlatformBehaviour : PlatformBehaviour
{
    private bool m_isRotating;

    [Header("Rotation Movement")]
    [SerializeField] private float m_RotationSpeed;
    [SerializeField] private float m_RotationIncremements; //Determines how much the platform rotates each time

    private Quaternion initialRotation;
    private Quaternion targetRotation;
    private float timeStartedLerping;
    [SerializeField] private AnimationCurve m_RotationCurve;
    
    void Start()
    {
        //InitiateRotation();
    }

    void ActivateBehaviour()
    {
        InitiateRotation();
    }

    void InitiateRotation()
    {
        initialRotation = transform.rotation;

        targetRotation = initialRotation;

        targetRotation *= Quaternion.Euler(0, 0, m_RotationIncremements);

        m_isRotating = true;
        timeStartedLerping = Time.time;
    }

    void Update()
    {
        if (m_isRotating)
            RotatePlatform();
    }

    void RotatePlatform()
    {
        float _timeSinceStarted = Time.time - timeStartedLerping;
        float _percentageComplete = _timeSinceStarted / m_RotationSpeed;

        transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, m_RotationCurve.Evaluate(_percentageComplete));

        if(_percentageComplete >= 1)
        {
            StopRotation();
        }
    }

    void StopRotation()
    {
        m_isRotating = false;
        //transform.rotation = targetRotation;
    }
	
}
