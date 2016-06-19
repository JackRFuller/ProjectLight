using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;

public class MovementController : C_Singleton<MovementController>
{
    [SerializeField] private Transform PC;
    [SerializeField] private PlayerMovementBehaviour m_PMBScript;
    private Transform m_currentNode;

    //Path Finding Lists
    private PathBehaviour[] nodes;
    public List<Vector3> nodePositions = new List<Vector3>();

    private int m_stepsTowardsTarget;

    public static event Action GeneratedActivePath;
    public static event Action GenerateNewNodes;

    void Start()
    {
        GenerateNewNodes += AssembleNodes;

        AssembleNodes();
                
        InputController.MovementTriggered += DeterminePathForPlayer;
    }

    public void AssembleNodes()
    {
        nodePositions.Clear();

        nodes = FindObjectsOfType<PathBehaviour>();

        for (int i = 0; i < nodes.Length; i++)
        {
            nodePositions.Add(nodes[i].transform.localPosition);
        }

        m_currentNode = FindClosestNode();
        m_currentNode.GetComponent<PathBehaviour>().FirstInPath();
    }

    void DeterminePathForPlayer()
    {
        if (!CheckOnSamePlaneAsPC(InputController.Instance.WaypointPosition))
        {
            Debug.Log("Wrong Axis");
            return;
        }
            

        if (ValidPath())
        {
            Debug.Log("Valid Path");
            if (GeneratedActivePath != null)
                GeneratedActivePath();
        }
    }

    bool ValidPath()
    {
        m_stepsTowardsTarget = 0;
        bool _validPath = false;

        if (DetermineIfPlayerIsHorizontal())
        {
            Debug.Log("Horizontal");
            _validPath = DetermineHorizontalPath();            
            return _validPath;
        }
        else
        {
            Debug.Log("Vertical");
            _validPath = DetermineVerticalPath();
            return _validPath;
        }
    }

    bool DetermineVerticalPath()
    {
        Vector3 _targetPos = InputController.Instance.WaypointPosition;

        float _MaxNumberOfNodesBetween = _targetPos.y - m_currentNode.position.y;
        _MaxNumberOfNodesBetween = Mathf.Round(_MaxNumberOfNodesBetween);
        Debug.Log(_MaxNumberOfNodesBetween);

        bool _isNegative = CheckForNegative(_MaxNumberOfNodesBetween); //Determine if the number is positive or negative

        float _targetY = m_currentNode.localPosition.y;
        Debug.Log(_targetY);

        float _numberOfNodesBetween = Mathf.Abs(_MaxNumberOfNodesBetween);

        for (int i = 0; i < _numberOfNodesBetween; i++)
        {
            float _targetX = 0;

            if (!_isNegative) //If Positive cycle to the right
                _targetX = m_currentNode.localPosition.x+ (i + 1);
            else
                _targetX = m_currentNode.localPosition.x + (-i - 1);

            Debug.Log(_targetX + "," + _targetY);

            for (int j = 0; j < nodePositions.Count; j++)
            {
                if (nodePositions.Contains(new Vector3(_targetX, _targetY, 0)))
                {
                    if (nodePositions[j] == new Vector3(_targetX, _targetY, 0))
                    {
                        nodes[j].EnablePath();
                        m_stepsTowardsTarget++;
                        break;
                    }
                }
                else
                {
                    Debug.Log("No Path Avilable");
                    return false;
                }
            }
        }

        if (_numberOfNodesBetween == m_stepsTowardsTarget)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool DetermineHorizontalPath()
    {
        Vector3 _targetPos = InputController.Instance.WaypointPosition;        

        float _MaxNumberOfNodesBetween = _targetPos.x - m_currentNode.position.x;

        _MaxNumberOfNodesBetween = Mathf.Round(_MaxNumberOfNodesBetween);
        Debug.Log(_MaxNumberOfNodesBetween);

        bool _isNegative = CheckForNegative(_MaxNumberOfNodesBetween); //Determine if the number is positive or negative

        float _targetY = m_currentNode.localPosition.y; //Ignore Height

        float _numberOfNodesBetween = Mathf.Abs(_MaxNumberOfNodesBetween);
        Debug.Log("Positive Number of Nodes To Check: " + _numberOfNodesBetween);

        for (int i = 0; i < _numberOfNodesBetween; i++)
        {
            float _targetX = 0;

            if (!_isNegative) //If Positive cycle to the right
                _targetX = m_currentNode.localPosition.x + (i + 1);
            else
                _targetX = m_currentNode.localPosition.x + (-i - 1);

            Debug.Log(_targetX);

            Debug.Log(_targetX + "," + _targetY);

            for (int j = 0; j < nodePositions.Count; j++)
            {
                if (nodePositions.Contains(new Vector3(_targetX, _targetY, 0)))
                {
                    if (nodePositions[j] == new Vector3(_targetX, _targetY, 0))
                    {
                        nodes[j].EnablePath();
                        m_stepsTowardsTarget++;
                        break;
                    }
                }
                else
                {
                    Debug.Log("No Path Avilable");
                    return false;
                }
            }
        }

        if (_numberOfNodesBetween == m_stepsTowardsTarget)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    Transform FindClosestNode()
    {   
        Transform _closestNode = nodes[0].transform;

        float _firstDist = Vector3.Distance(_closestNode.position, PC.transform.position);

        for(int i = 0; i < nodes.Length; i++)
        {
            float _dist = Vector3.Distance(PC.position, nodes[i].transform.position);
            
            if(_dist < _firstDist)
            {
                _firstDist = _dist;
                _closestNode = nodes[i].transform;
            }
        }

        return _closestNode;        
    }

    bool DetermineIfPlayerIsHorizontal()
    {
        float _playersRotation = m_PMBScript.GetPlayersRotation();
        Debug.Log(_playersRotation);

        if(_playersRotation == 90 || _playersRotation == 270)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    bool CheckOnSamePlaneAsPC(Vector3 _targetPos)
    {
        Debug.Log(DetermineIfPlayerIsHorizontal());

        if (DetermineIfPlayerIsHorizontal())
        {
            float _targetY = Mathf.Abs(_targetPos.y);
            float _playerY = Mathf.Abs(PC.transform.position.y);

            if ((_targetY <= _playerY + 0.5f) && (_targetY >= _playerY - 0.5f))
                return true;
        }
        else
        {
            float _targetX = Mathf.Abs(_targetPos.x);
            float _playerX = Mathf.Abs(transform.position.x);

            if ((_targetX <= _playerX + 0.5f) && (_targetX >= _playerX - 0.5f))
                return true;
        }

        return false;
    }

    bool CheckForNegative(float _value)
    {
        if (_value < 0)
            return true; //Is Negative
        else return false; //Is Positive;
    }
	
}
