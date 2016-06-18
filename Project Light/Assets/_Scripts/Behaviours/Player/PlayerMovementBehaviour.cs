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
        

        //Define TargetPosition
        m_targetPosition = InputController.Instance.WaypointPosition;

        if (!CheckOnSamePlane())
            return;

        //Determine Player's Rotation
        bool _ignoreY = IgnoreY();

        if (!_ignoreY)
            m_targetPosition.x = transform.position.x; //Ignore X of Waypoint
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

    //void OnTriggerEnter(Collider other)
    //{
    //    if (other.tag == "Platform")
    //    {
    //        transform.parent = other.transform;
    //    }
    //}



    bool CheckOnSamePlane()
    {
        float _playersRotation = Quaternion.Angle(transform.rotation, Quaternion.identity);
        Debug.Log(_playersRotation);

        if (_playersRotation == 0.0f || _playersRotation == 180.0f)
        {
            float _targetY = Mathf.Abs(m_targetPosition.y);
            float _playerY = Mathf.Abs(transform.position.y);

            if ((_targetY <= _playerY + 0.5f) && (_targetY >= _playerY - 0.5f))
                return true;
        }
        if (_playersRotation == 90.0f || _playersRotation == 270.0f)
        {
            float _targetX = Mathf.Abs(m_targetPosition.x);
            float _playerX = Mathf.Abs(transform.position.x);

            if ((_targetX <= _playerX + 0.5f) && (_targetX >= _playerX - 0.5f))
                return true;
        }

        return false;

    }

    bool IgnoreY()
    {
        float _playersRotation = Quaternion.Angle(transform.rotation, Quaternion.identity);

        if (_playersRotation == 0.0f || _playersRotation == 180.0f)
            return true;
        else return false;
    }
}
