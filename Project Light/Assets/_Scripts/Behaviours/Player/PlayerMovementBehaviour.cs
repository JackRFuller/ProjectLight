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

        Debug.Log(transform.eulerAngles);

        //Determine Player's Rotation
        bool _ignoreY = IgnoreY();

        if (!_ignoreY)
            m_targetPosition.x = transform.position.x;
        else      
            m_targetPosition.y = transform.position.y;  //Ignore Height of Waypoint

        //Determine if the player is sprinting or not
        float _speed = m_MovementSpeed;
        if (isSprinting())
            _speed *= m_SprintModifier;       

        //Calculate Direction Vector
        Vector3 _direction = (m_targetPosition - transform.position).normalized * _speed;
        lastSquareMagnitude = Mathf.Infinity;

        m_desiredVelocity = _direction;

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

    bool isSprinting()
    {
        if (InputController.Instance.HasDoubleTapped)
            return true;
        else return false;
    }

    void StopMovement()
    {
        m_desiredVelocity = Vector3.zero;
        rb.velocity = m_desiredVelocity;
        m_isMoving = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Platform")
        {
            transform.parent = other.transform;
        }
    }

    bool IgnoreY()
    {
        if (transform.eulerAngles.z == 0 || transform.eulerAngles.z == 180.0f)
        {
            return true;
        }
        else if(transform.eulerAngles.z == 90 || transform.eulerAngles.z == 270)
        {
            return false;
        }
        return true;
    }
}
