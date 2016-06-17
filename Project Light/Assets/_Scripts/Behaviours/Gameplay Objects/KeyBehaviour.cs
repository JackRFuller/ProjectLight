using UnityEngine;
using System.Collections;

public class KeyBehaviour : MonoBehaviour
{
    [Header("Key Attribute")]
    [SerializeField] private MeshRenderer mesh;

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            DeactivateKey();
        }
    }

    void DeactivateKey()
    {
        LevelController.Instance.IncrementNumberOfKeys(); //Tell Level Controller that key has been picked up

        mesh.enabled = false;
    }
}
