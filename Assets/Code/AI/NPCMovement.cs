using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCMovement : MonoBehaviour
{
    NavMeshAgent m_agent;
    [SerializeField] protected float m_moveRadius;
    [SerializeField] ParticleSystem m_particleSystem;
    PhotonView m_PV;

    // Start is called before the first frame update
    void Start()
    {
        m_PV = GetComponent<PhotonView>();
        m_agent = GetComponent<NavMeshAgent>();
        //m_particleSystem = GetComponent<ParticleSystem>();
        m_particleSystem.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_agent.pathPending && m_agent.remainingDistance < 0.5f)
        {
            MoveRandomPosition();
        }
    }

    void MoveRandomPosition()
    {
        Vector3 m_randomDirection = Random.insideUnitSphere * m_moveRadius;
        m_randomDirection += transform.position;

        NavMeshHit m_hit;

        if(NavMesh.SamplePosition(m_randomDirection, out m_hit, m_moveRadius, NavMesh.AllAreas))
        {
            m_agent.SetDestination(m_hit.position);
        }
    }

    public void DestroyNPC()
    {
        m_agent.speed = 0;
        m_PV.RPC("DestroyCurrentNPC", RpcTarget.All);
    }

    [PunRPC]
    void DestroyCurrentNPC()
    {
        StartCoroutine(WaitForParticleSystem());
    }

    IEnumerator WaitForParticleSystem()
    {
        m_particleSystem.Play();

        yield return new WaitForSeconds(m_particleSystem.main.duration);

        PhotonNetwork.Destroy(gameObject);
    }
}
