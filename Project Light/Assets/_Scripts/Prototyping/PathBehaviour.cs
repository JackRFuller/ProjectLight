using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PathBehaviour : MonoBehaviour
{
    [SerializeField] private Image nodeImage;
    
    public void EnableNode()
    {
        nodeImage.color = Color.green;
    }
}
