using UnityEngine;
using System.Collections;

public class PlayerWaypointBehaviour : MonoBehaviour
{
    void Start()
    {
        Init();
    }

    void Init()
    {
        //Subscribe to Input Controller
        InputController.MovementTriggered += MoveWaypoint;
    }

    void MoveWaypoint()
    {
        Vector3 _targetPosition = InputController.Instance.WaypointPosition;
        _targetPosition.z = 0;

        transform.position = _targetPosition;
    }

    
	
}
