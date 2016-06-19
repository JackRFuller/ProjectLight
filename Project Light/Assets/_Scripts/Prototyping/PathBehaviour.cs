using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PathBehaviour : MonoBehaviour
{
    [SerializeField] private Image nodeImage;

    void Start()
    {
        //Debug.Log(transform.localPosition);
        //Debug.Log(transform.position);
    }
    
    public void FirstInPath()
    {
        nodeImage.color = Color.green;
    }
    
    public void EnablePath()
    {
        nodeImage.color = Color.blue;
    }

    public void DisablePath()
    {
        nodeImage.color = Color.white;
    }
}
