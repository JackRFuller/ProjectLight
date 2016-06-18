using UnityEngine;
using System.Collections;

public class SwitchBehaviour : MonoBehaviour
{
    [SerializeField] private Transform m_target;

	void HitByInput()
    {
        ActivateTargetObject();
    }

    void ActivateTargetObject()
    {
        m_target.SendMessage("ActivateBehaviour", SendMessageOptions.DontRequireReceiver);
    }
}
