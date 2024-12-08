using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Ball : MonoBehaviour
{
    #region RuntimeVariables
    PhotonView m_pv;
    [SerializeField] public bool m_hasEnteredTheRedGoal;
    [SerializeField] public bool m_hasEnteredTheBlueGoal;
    #endregion
    void Start()
    {
        m_pv = GetComponent<PhotonView>();    
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("RedGoal"))
        {
            if (m_hasEnteredTheRedGoal)
            {
                return;
            }
            UpdateRedScore();
        }
        else if (other.gameObject.CompareTag("BlueGoal"))
        {
            if (m_hasEnteredTheBlueGoal)
            {
                return;
            }
            UpdateBlueScore();
        }
    }

    void UpdateRedScore()
    {
        LevelManager.instance.updateRedScore();
    }

    void UpdateBlueScore()
    {
        LevelManager.instance.updateBlueScore();
    }

}
