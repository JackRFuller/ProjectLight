using UnityEngine;
using System.Collections;

public class MovementController : C_Singleton<MovementController>
{
    [SerializeField] private Transform PC;

    void Start()
    {

    }

    void FindClosestSquare()
    {
        PathBehaviour[] nodes = FindObjectsOfType<PathBehaviour>();

        Transform _closestNode = nodes[0].transform;

        float _firstDist = Vector3.Distance(_closestNode.position, PC.transform.position);

        for(int i = 0; i < nodes.Length; i++)
        {
            float _dist = Vector3.Distance(PC.position, nodes[i].transform.position);
            

        }
    }
	
}
