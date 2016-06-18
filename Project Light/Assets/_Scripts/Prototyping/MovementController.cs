using UnityEngine;
using System.Collections;

public class MovementController : C_Singleton<MovementController>
{
    [SerializeField] private Transform PC;
    private Transform m_closestNode;

    void Start()
    {
        FindClosestSquare();
    }

    void FindClosestSquare()
    {
        PathBehaviour[] nodes = FindObjectsOfType<PathBehaviour>();

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

        Debug.Log(_closestNode.name);
    }
	
}
