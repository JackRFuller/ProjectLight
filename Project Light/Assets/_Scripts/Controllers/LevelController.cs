using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class LevelController : C_Singleton<LevelController>
{
    [SerializeField] private int m_LevelIndex;
    [SerializeField] private LevelData m_CurrentLevelData;

    private int m_NumberOfKeysNeededToProgress;
    private int m_currentNumberOfKeys;

    public static event Action ObtainedAllKeys;// Triggered when the player has obtained all of the keys necessary for the level

    void Start()
    {
        SetLevelAttributes();
    }

    void SetLevelAttributes()
    {
        m_NumberOfKeysNeededToProgress = m_CurrentLevelData.m_numberOfKeysNeeded;
        m_currentNumberOfKeys = 0;
    }

    public void IncrementNumberOfKeys()
    {
        m_currentNumberOfKeys++;

        if(m_currentNumberOfKeys == m_NumberOfKeysNeededToProgress)
        {
            if(ObtainedAllKeys != null)
            {
                ObtainedAllKeys();
            }
        }
    }
	
}
