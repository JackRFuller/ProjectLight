using UnityEngine;
using System.Collections;

public class RotatingPlatformBehaviour : PlatformBehaviour
{
    private bool m_isRotating;

    [Header("Rotation Movement")]
    [SerializeField] private float m_RotationSpeed;
    [SerializeField] private float m_RotationIncremements; //Determines how much the platform rotates each time

    private Vector3 initialRotation;
    private Vector3 targetRotation;
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
        initialRotation = transform.localEulerAngles;

        targetRotation = initialRotation;

        targetRotation.z += m_RotationIncremements; 

        //Debug.Log(initialRotation);
        //Debug.Log(targetRotation);

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
        Quaternion _startRotation = Quaternion.Euler(initialRotation);
        Quaternion _targetRotation = Quaternion.Euler(targetRotation);

        float _timeSinceStarted = Time.time - timeStartedLerping;
        float _percentageComplete = _timeSinceStarted / m_RotationSpeed;

        transform.rotation = Quaternion.Slerp(_startRotation, _targetRotation, m_RotationCurve.Evaluate(_percentageComplete));

        if(_percentageComplete >= 1)
        {
            m_isRotating = false;
        }
    }
	
}
