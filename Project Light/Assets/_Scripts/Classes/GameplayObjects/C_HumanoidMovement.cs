using UnityEngine;
using System.Collections;

public class C_HumanoidMovement : MonoBehaviour
{
    [SerializeField] protected float m_MovementSpeed;  
    [SerializeField] protected float m_SprintModifier; //Is multiplied by the movement speed when actor is sprinting
    [SerializeField] protected float m_DistanceToStopBeforeWaypoint; 
    
}
