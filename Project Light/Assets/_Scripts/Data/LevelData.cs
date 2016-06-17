using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Level Data", menuName = "Data Files/Levels", order = 1)]
public class LevelData : ScriptableObject
{
    public int m_numberOfKeysNeeded;
}
