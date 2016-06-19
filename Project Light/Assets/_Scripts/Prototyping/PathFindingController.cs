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

    void Start()
    {
        SubscribeToEvents();

        AssembleNodeLists();
    }

    void SubscribeToEvents()
    {
        InputController.MovementTriggered += BuildPathToTarget;
    }

    void AssembleNodeLists()
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
    }

    void BuildPathToTarget()
    {
        Transform _startingNode = GetClosestNodeToPlayersPosition(); //Get Starting Node

        if (_startingNode == null) //If there is no starting node
            return;

        m_targetPosition = InputController.Instance.WaypointPosition; //Get Target Position

        //Work out the difference between the PC and the TargetPosition.
        float _differenceBetweenPCAndTarget = GetDifferenceBetweenTargetAndPC();
        Debug.Log("Difference Between PC & Target: " + _differenceBetweenPCAndTarget);

        //Round Difference To The Nearest Whole Number
        _differenceBetweenPCAndTarget = Mathf.Round(_differenceBetweenPCAndTarget);
        Debug.Log("Rounded Difference " +_differenceBetweenPCAndTarget);

        //Identify if we're cycling left or right
        bool _isPositive = GetIfDifferenceIsPositive(_differenceBetweenPCAndTarget);

        //Get Absolute Value of The Difference
        float _numberOfPotentialNodes = Mathf.Abs(_differenceBetweenPCAndTarget);

        float _targetNodeY = _startingNode.position.y; //Ignores the Height of the Nodes

        int _numberOfNodesToTarget = 0;

        Debug.Log("Starting Node " + _startingNode.position);

        //Cycle Through the potential number of nodes
        for (int i = 0; i < _numberOfPotentialNodes; i++)
        {
            float _targetNodeX = 0;

            if (_isPositive)
                _targetNodeX = _startingNode.position.x + (i + 1);
            else _targetNodeX = _startingNode.position.x + (-i - 1);

            Vector3 _targetNodePosition = new Vector3(_targetNodeX, _targetNodeY, 0);        

            Debug.Log("Target Node Position " +_targetNodePosition);

            int _nodeIndex1 = nodeWorldPositions.FindIndex(d => d == _targetNodePosition);
            Debug.Log("Node Index" + _nodeIndex1);

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

    bool GetIfDifferenceIsPositive(float _value) //Get if the player is moving left/right or up/down
    {
        if (_value > 0)
            return true; //Is Positive
        else
            return false; //Is Negative
    }

    float GetDifferenceBetweenTargetAndPC()
    {
        float _difference = 0;
        _difference = m_targetPosition.x - PC.transform.position.x;

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
