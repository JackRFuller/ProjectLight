using UnityEngine;
using System.Collections;

public class DoorBehaviour : MonoBehaviour
{
    [SerializeField]
    private Collider col;  

    void Start()
    {
        Init();
    }

    void Init()
    {
        //Subscribe to player obtaining all three keys - Level Controller
        LevelController.ObtainedAllKeys += ActivateDoor;
    }

    void ActivateDoor()
    {
        col.enabled = true;
    }

    //Triggered when the player gets to an activated exit
    void PlayerAtExit()
    {
        Debug.Log("Success");
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            PlayerAtExit();
        }
    }

    
}
