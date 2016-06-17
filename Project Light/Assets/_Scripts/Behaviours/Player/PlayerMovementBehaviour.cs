using UnityEngine;
using System.Collections;

public class PlayerMovementBehaviour : C_HumanoidMovement
{
    private Rigidbody rb;

    //Movement
    private bool m_isMoving;
    private Vector3 m_targetPosition;
    private Vector3 m_direction;
    private float lastSquareMagnitude;
    private Vector3 m_desiredVelocity;

    void Start()
    {
        Init();
    }

    void Init()
    {
        //Subscribe To Input Controller
        InputController.MovementTriggered += InitiateMovement;

        //Cache Components
        rb = GetComponent<Rigidbody>();
    }

    void InitiateMovement()
    {
        //Reset Speed
        //StopMovement();

        //Define TargetPosition
        m_targetPosition = InputController.Instance.WaypointPosition;

        //Ignore Height of Waypoint
        m_targetPosition.y = transform.position.y;

        //Calculate Direction Vector
        Vector3 _direction = (m_targetPosition - transform.position).normalized * m_MovementSpeed;
        lastSquareMagnitude = Mathf.Infinity;

        m_desiredVelocity =_direction;

        //Start Moving
        m_isMoving = true;
    }

    void Update()
    {
        if (m_isMoving)
            CalculateMovementVector();
    }

    void FixedUpdate()
    {
        if (m_isMoving)
            MoveTowardsWaypoint();
    }

    void CalculateMovementVector()
    {
        float _sqrMag = (m_targetPosition - transform.position).sqrMagnitude;

        if (_sqrMag > lastSquareMagnitude)
            StopMovement();

        lastSquareMagnitude = _sqrMag;
    }

    void MoveTowardsWaypoint()
    { 
        rb.velocity = m_desiredVelocity;
    }

    void StopMovement()
    {
        m_desiredVelocity = Vector3.zero;
        rb.velocity = m_desiredVelocity;
        m_isMoving = false;
    }
}
