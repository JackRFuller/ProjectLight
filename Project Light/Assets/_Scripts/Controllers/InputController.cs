using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;

public class InputController : C_Singleton<InputController>
{
    [Header("Camera")]
    [SerializeField] private Camera m_MainCamera;

    private Vector3 m_waypointPosition; //Holds the target position at which the player hits the platform
    public Vector3 WaypointPosition
    {
        get { return m_waypointPosition; }
    }

    public static event Action MovementTriggered;

    void Update()
    {
        DetectMouseInput();
    }	

    void DetectMouseInput()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Vector2 _mousePos = Input.mousePosition;            
            SendOutRayCast(_mousePos);
        }
    }

    void DetectMobileInput()
    {
        if(Input.touchCount > 0)
        {
            Vector2 _touchPos = Input.GetTouch(0).position;
        }
    }

    void SendOutRayCast(Vector2 _hitPosition)
    {
        Ray _ray = m_MainCamera.ScreenPointToRay(_hitPosition);

        RaycastHit _hit;

        if(Physics.Raycast(_ray, out _hit, Mathf.Infinity))
        {
            if(_hit.collider.tag == "Platform")
            {
                m_waypointPosition = _hit.point;
                Debug.Log(_hit.point);
                m_waypointPosition.z = 0;
                Debug.Log(m_waypointPosition);

                //Start triggering player movement
                if(MovementTriggered != null)
                    MovementTriggered(); 
            }
        }
    }
}
