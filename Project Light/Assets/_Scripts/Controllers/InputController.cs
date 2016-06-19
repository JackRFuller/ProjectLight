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

    [Header("Input")]
    [SerializeField] private float m_TimeToAllowForDoubleTap;

    private bool m_hasDoubleTapped;
    public bool HasDoubleTapped
    {
        get { return m_hasDoubleTapped; }
    }

    private bool m_singleClick;   
    private float m_TimeForDoubleTap;

    public static event Action MovementTriggered;

    void Update()
    {
        DetectMouseInput();
    }	

    void DetectMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ReigsterDoubleTap();
    
            Vector2 _mousePos = Input.mousePosition;            
            SendOutRayCast(_mousePos);
        }

        if (m_singleClick)
        {
            if((Time.time - m_TimeForDoubleTap) > m_TimeToAllowForDoubleTap)
            {
                m_singleClick = false;
            }
        }
    }

    void DetectMobileInput()
    {
        if(Input.touchCount > 0)
        {
            ReigsterDoubleTap();

            Vector2 _touchPos = Input.GetTouch(0).position;
            SendOutRayCast(_touchPos);
        }

        if (m_singleClick)
        {
            if ((Time.time - m_TimeForDoubleTap) > m_TimeToAllowForDoubleTap)
            {
                m_singleClick = false;
            }
        }
    }

    void ReigsterDoubleTap()
    {
        if (!m_singleClick)
        {
            m_singleClick = true;

            m_TimeForDoubleTap = Time.time;

            m_hasDoubleTapped = false;
        }
        else
        {
            m_singleClick = false; //Has Double Clicked
            m_hasDoubleTapped = true;
        }
    }

    void SendOutRayCast(Vector2 _hitPosition)
    {
        Ray _ray = m_MainCamera.ScreenPointToRay(_hitPosition);

        RaycastHit _hit;

        if (Physics.Raycast(_ray, out _hit, Mathf.Infinity))
        {
            if (_hit.collider.tag == "Platform")
            {
                m_waypointPosition = _hit.point;

                m_waypointPosition.z = 0; //Make sure the target position z is always 0

                //Start triggering player movement
               if(MovementTriggered != null)
                    MovementTriggered();
            }
            else
            {
                _hit.transform.SendMessage("HitByInput", SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    //void RegisterDoubleTap()
    //{
    //    m_hasDoubleTapped = true;

    //    Debug.Log("DoubleClick");
    //}
}
