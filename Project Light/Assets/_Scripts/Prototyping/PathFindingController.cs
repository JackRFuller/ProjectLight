using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;

public class PathFindingController : C_Singleton<PathFindingController>
{
    [SerializeField] private Transform PC;
    private PathBehaviour[] nodes;
    public List<Vector3> nodeLocalPositions = new List<Vector3>();
    public List<Vector3> nodeWorldPositions = new List<Vector3>();

    private Vector3 m_targetPosition;

    public static event Action FoundValidPath;
    public static event Action HasPathNodes; //Called when the controller has all of the nodes

    void Start()
    {
        SubscribeToEvents();

        AssembleNodeLists();
    }

    void SubscribeToEvents()
    {
        InputController.MovementTriggered += BuildPathToTarget;
    }

    public void AssembleNodeLists()
    {
        //Clear Lists
        nodeLocalPositions.Clear();
        nodeWorldPositions.Clear();

        nodes = FindObjectsOfType<PathBehaviour>();

        for (int i = 0; i < nodes.Length; i++)
        {
            nodeLocalPositions.Add(nodes[i].transform.localPosition);
            nodeWorldPositions.Add(nodes[i].transform.position);
        }

        //Stop Any Floating Point Errors
        for (int i = 0; i < nodeWorldPositions.Count; i++)
        {
            nodeWorldPositions[i] = new Vector3(Mathf.Round(nodeWorldPositions[i].x),
                                                Mathf.Round(nodeWorldPositions[i].y),
                                                Mathf.Round(nodeWorldPositions[i].z));
        }

        if (HasPathNodes != null)
            HasPathNodes();
    }

    void BuildPathToTarget()
    {
        Transform _startingNode = GetClosestNodeToPlayersPosition(); //Get Starting Node

        if (_startingNode == null) //If there is no starting node
            return;

        m_targetPosition = InputController.Instance.WaypointPosition; //Get Target Position

        //Check that Target Position is on the same plane as the PC
        if (!isTargetOnTheSamePlaneAsThePlayer())
            return;

        //Work out the difference between the PC and the TargetPosition.
        float _differenceBetweenPCAndTarget = GetDifferenceBetweenTargetAndPC();
        Debug.Log("Difference Between PC & Target: " + _differenceBetweenPCAndTarget);

        //Round Difference To The Nearest Whole Number
        _differenceBetweenPCAndTarget = Mathf.Round(_differenceBetweenPCAndTarget);
        Debug.Log("Rounded Difference " +_differenceBetweenPCAndTarget);

        //Identify if we're cycling left or right/up or down
        bool _isPositive = GetIfDifferenceIsPositive(_differenceBetweenPCAndTarget);

        //Get Absolute Value of The Difference
        float _numberOfPotentialNodes = Mathf.Abs(_differenceBetweenPCAndTarget);

        

        int _numberOfNodesToTarget = 0;

        Debug.Log("Starting Node " + _startingNode.position);

        //Cycle Through the potential number of nodes
        for (int i = 0; i < _numberOfPotentialNodes; i++)
        {
            float _targetNodeX = 0;
            float _targetNodeY = 0;

            //Determine the player's rounded orientation
            float _playersOrientation = Mathf.Round(GetPlayerRotation());

            if (_playersOrientation == 0 || _playersOrientation == 180)
            {
                _targetNodeY = _startingNode.position.y; //Ignores the Height of the Nodes

                if (_isPositive)
                    _targetNodeX = _startingNode.position.x + (i + 1);
                else _targetNodeX = _startingNode.position.x + (-i - 1);
            }
            else
            {
                _targetNodeX = _startingNode.position.x; //Ignores the left & right of the Nodes

                if (_isPositive) //If we're going up
                    _targetNodeY = _startingNode.position.y + (i + 1);
                else //if we're going down
                    _targetNodeY = _startingNode.position.y + (-i - 1);
            }           

            Vector3 _targetNodePosition = new Vector3(_targetNodeX, _targetNodeY, 0);        

            Debug.Log("Target Node Position " +_targetNodePosition);
           
            //Check if node is present
            if (nodeWorldPositions.Exists(d => d == _targetNodePosition))
            {
                //Find Node Relating To Position
                int _nodeIndex = nodeWorldPositions.FindIndex(d => d == _targetNodePosition);
                nodes[_nodeIndex].EnablePath();
                _numberOfNodesToTarget++;
                Debug.Log("Found Node");
            }
            else
            {
                //No Valid Path Present
                Debug.Log("No Valid Path");
                return;
            }
        }

        if(_numberOfNodesToTarget == _numberOfPotentialNodes)
        {
            Debug.Log("Valid Path");
            //Initiate Character Movement
            if (FoundValidPath != null)
                FoundValidPath();
        }

    }

    bool isTargetOnTheSamePlaneAsThePlayer()
    {
        float _playerRotation = Mathf.Round(GetPlayerRotation());        
        Debug.Log("Player's Rotation " + _playerRotation);

        if(_playerRotation == 90 || _playerRotation == 270)
        {
            float _playerX = Mathf.Abs(PC.transform.position.x);
            float _targetX = Mathf.Abs(m_targetPosition.x);

            if(_targetX <= _playerX + 1.0f && _targetX >= _playerX - 1.0f)
            {
                return true;
            }            
        }
        else if(_playerRotation == 0 || _playerRotation == 180)
        {
            float _playerY = Mathf.Abs(PC.transform.position.y);
            float _targetY = Mathf.Abs(m_targetPosition.y);

            if(_targetY <= _playerY + 1.0f && _targetY >= _playerY - 1.0f)
            {
                return true;
            }            
        }

        Debug.Log("Not on Same Plane");

        return false;
    }

    float GetPlayerRotation()
    {
        float _playerRotation = 0;
        _playerRotation = Mathf.Abs(Quaternion.Angle(PC.rotation, Quaternion.identity));
        return _playerRotation;
    }

    bool GetIfDifferenceIsPositive(float _value) //Get if the player is moving left/right or up/down
    {
        if (_value > 0)
            return true; //Is Positive
        else
            return false; //Is Negative
    }

    float GetDifferenceBetweenTargetAndPC()
    {
        float _playerRotation = Mathf.Round(GetPlayerRotation());

        float _difference = 0;

        if (_playerRotation == 90 || _playerRotation == 270)
        {
            _difference = m_targetPosition.y - PC.transform.position.y;
        }
        else
        {
            _difference = m_targetPosition.x - PC.transform.position.x;
        }

        return _difference;
    }    

    Transform GetClosestNodeToPlayersPosition()
    {
        Transform _closestNode = null;

        int _nodeIndex = 0;

        _closestNode = nodes[0].transform;

        float _distanceToNode = Vector3.Distance(PC.transform.position, _closestNode.position);

        for(int i = 0; i < nodes.Length; i++)
        {
            Transform _newNode = nodes[i].transform;

            float _newDistance = Vector3.Distance(PC.transform.position, _newNode.position);

            if(_newDistance < _distanceToNode)
            {
                _nodeIndex = i;
                _closestNode = _newNode;
                _distanceToNode = _newDistance;
            }
        }

        nodes[_nodeIndex].FirstInPath();
        return _closestNode;
    }
}
