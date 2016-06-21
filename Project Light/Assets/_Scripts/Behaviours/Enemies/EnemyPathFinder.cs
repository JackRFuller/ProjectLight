using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class EnemyPathFinder : MonoBehaviour
{
    private List<Vector3> nodes = new List<Vector3>(); //Holds all of the nodes

    private Vector3 m_FirstWaypoint = Vector3.zero;
    public Vector3 FirstWaypoint
    {
        get { return m_FirstWaypoint; }
    }

    private Vector3 m_SecondWaypoint = Vector3.zero;
    public Vector3 SecondWaypoint
    {
        get { return m_SecondWaypoint; }
    }

    public static event Action HasNPCWaypoints;

	// Use this for initialization
	void Start ()
    {
        SubscribeToEvents();
	}

    void SubscribeToEvents()
    {
        PathFindingController.HasPathNodes += GetPathNodes;
    }

    //Gets all of the nodes from the Path Finding Controller
    void GetPathNodes()
    {
        nodes.Clear();

        nodes = PathFindingController.Instance.nodeWorldPositions;

        //Debug.Log("Get Nodes");

        DeterminePath();
    }

    void DeterminePath()
    {
        //Find the starting node
        Vector3 _startingPosition = GetStartingNode();

        Debug.Log("NPC Starting Node: " +_startingPosition);

        //Determine the NPC's Rotation
        float _npcRot = GetObjectsRotation();

        Debug.Log("Player Rotation: " +_npcRot);

        bool _HasFirst = false; 

        if (_npcRot == 0 || _npcRot == 180) //If Player is going from left to right
        {
            m_FirstWaypoint = CalculateHorizontalWaypoints(_startingPosition, _HasFirst);
            _HasFirst = true;
            m_SecondWaypoint = CalculateHorizontalWaypoints(_startingPosition, _HasFirst);
        }
        else
        {
            m_FirstWaypoint = CalculateVerticalWaypoints(_startingPosition, _HasFirst);
            _HasFirst = true;
            m_SecondWaypoint = CalculateVerticalWaypoints(_startingPosition, _HasFirst);
        }

        Debug.Log("First Waypoint: " + m_FirstWaypoint);
        Debug.Log("Second Waypoint: " + m_SecondWaypoint);

        //Let NPC Know that waypoints have been found
        if (HasNPCWaypoints != null)
            HasNPCWaypoints();
    }

    //Finds the Left and Right Path
    Vector3 CalculateHorizontalWaypoints(Vector3 _startingPos, bool HasFirst)
    {
        //Set Target Node Variables
        float _targetNodeX = 0;
        float _targetNodeY = 0;

        //Ignore the Height
        _targetNodeY = _startingPos.y;

        Vector3 _currentWayPoint = new Vector3(_targetNodeX, _targetNodeY, 0);
        Vector3 _targetNodePosition = new Vector3(_targetNodeX, _targetNodeY, 0);

        //Check going from the left
        for (int i = 0; i < nodes.Count; i++)
        {
            if(!HasFirst)
                _targetNodeX = _startingPos.x + (i + 1);
            else
                _targetNodeX = _startingPos.x - (i - 1);

            _targetNodePosition.x = _targetNodeX;

            //Check if node is present
            if (nodes.Exists(d => d == _targetNodePosition))
            {
                _currentWayPoint.x = _targetNodePosition.x;
            }
            else
            {
                return _currentWayPoint;
            }
        }

        return _currentWayPoint;
    }

    //Finds the Left and Right Path
    Vector3 CalculateVerticalWaypoints(Vector3 _startingPos, bool HasFirst)
    {
        //Set Target Node Variables
        float _targetNodeX = 0;
        float _targetNodeY = 0;

        //Ignore the Width
        _targetNodeX = _startingPos.x;

        Vector3 _currentWayPoint = new Vector3(_targetNodeX, _targetNodeY, 0);
        Vector3 _targetNodePosition = new Vector3(_targetNodeX, _targetNodeY, 0);

        //Check going from the left
        for (int i = 0; i < nodes.Count; i++)
        {
            if (!HasFirst)
                _targetNodeY = _startingPos.y + (i + 1);
            else
                _targetNodeY = _startingPos.y - (i - 1);

            _targetNodePosition.y = _targetNodeY;

            //Check if node is present
            if (nodes.Exists(d => d == _targetNodePosition))
            {
                _currentWayPoint.y = _targetNodePosition.y;
            }
            else
            {
                return _currentWayPoint;
            }
        }

        return _currentWayPoint;
    }

    void InitiateMovement()
    {

    }


	
	// Update is called once per frame
	void FixedUpdate ()
    {
	
	}

    //Gets the closest node to the NPC
    Vector3 GetStartingNode()
    {
        Vector3 _startingNodePos = nodes[0];

        float _startDistance = Vector3.Distance(transform.position, _startingNodePos);

        for (int i = 0; i < nodes.Count; i++)
        {
            float _dist = Vector3.Distance(transform.position, nodes[i]);

            if(_dist < _startDistance)
            {
                _startingNodePos = nodes[i];
                _startDistance = _dist;
            }
        }

        return _startingNodePos;
    }

    //Return the positive, rounded rotation of the NPC
    float GetObjectsRotation()
    {
        float _npcRotation = Mathf.Abs(Quaternion.Angle(transform.rotation, Quaternion.identity));

        _npcRotation = Mathf.Round(_npcRotation);

        return _npcRotation;
    }

    void SetParentToPlatform(Transform _target)
    {
        transform.parent = _target.parent.parent;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Platform")
        {
            SetParentToPlatform(other.transform);
        }
    }

    
}
