using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Linq;
using UnityEngine.UI;
using Cinemachine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviourPunCallbacks, IOnEventCallback {
    #region References

    [Header("Cinemachine")]
    [SerializeField] protected CinemachineImpulseSource m_cmImpulseSource;
    [SerializeField] protected Camera _camera;

    [Header("Player Componentes")]
    [SerializeField] protected Rigidbody rb;
    //[SerializeField] TextMeshPro m_nickname;
    [SerializeField] protected Animator m_myAnim;

    [Header("UI Player")]
    [SerializeField] TextMeshProUGUI m_currentTeamText;
    [SerializeField] TextMeshPro m_nicknameTMP;

    [SerializeField] protected string m_currentTeamName;

    [Header("Ball")]
    [SerializeField] protected Transform m_ballPosition;
    [SerializeField] protected float m_kickForce;
    [SerializeField] protected bool m_canKick;

    PhotonView m_PV;

    #endregion

    #region Knobs

    [SerializeField] protected float playerSpeed;

    #endregion

    #region RuntimeVariables

    [SerializeField] protected float playerInputHorizontal;
    [SerializeField] protected float playerInputForward;
    Vector3 m_moveDirection;

    Vector3 cameraForward;
    Vector3 cameraRight;

    #endregion

    #region UnityMethods

    private void Start()
    {
        m_PV = GetComponent<PhotonView>();
        //m_PV.Owner.NickName = PhotonNetwork.NickName; // NO PEDIRLO NUNCA MÁS DE UNA VEZ.
        //m_nickname.text = m_PV.Owner.NickName;
        gameObject.name = m_PV.Owner.NickName;
        m_myAnim.SetBool("JOG", false);
        m_myAnim.SetBool("STOP", true);
        PhotonPeer.RegisterType(typeof(Color), (byte)'C', TypeTransformer.SerializeColor, TypeTransformer.DeserializeColor);
        m_currentTeamText.text = "Team...";
        m_nicknameTMP.text = gameObject.name;

        m_cmImpulseSource = FindObjectOfType<CinemachineImpulseSource>();

        //LevelManager.instance.AddPlayerToCinemachineTargetGroup(this.gameObject.transform);
    }

    private void Update()
    {
        m_nicknameTMP.transform.rotation = Quaternion.Euler(0f, 270f, 0f);
        m_nicknameTMP.text = gameObject.name;

        if (!m_PV.IsMine)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            LevelNetworkManager.Instance.disconnectFromCurrentRoom();
        }

        if (Input.GetKeyDown(KeyCode.K) && m_canKick)
        {
            playerSpeed = 0;
            m_myAnim.SetBool("JOG", false);
            m_myAnim.SetBool("STOP", false);
            m_myAnim.SetBool("KICK", true);
            GameObject m_ball = m_ballPosition.GetChild(0).gameObject;
            m_ball.GetComponent<Rigidbody>().isKinematic = false;
            m_ball.gameObject.transform.SetParent(null);
            m_ball.GetComponent<Rigidbody>().AddForce(transform.forward * m_kickForce, ForceMode.Impulse);
            StartCoroutine(cooldownTimer());
        }

        if (Input.GetKey(KeyCode.Tab))
        {
            LevelManager.instance.showPlayersList(true);
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            LevelManager.instance.showPlayersList(false);
        }
    }

    private void FixedUpdate()
    {
        if (!m_PV.IsMine)
        {
            return;
        }
        //m_nickname.transform.position = new Vector3(transform.position.x, transform.position.y + 4.5f, transform.position.z);
        PlayerMov();
    }

    private void OnTriggerStay(Collider p_other)
    {
        if (!m_PV.IsMine)
        {
            return;
        }
        if (p_other.gameObject.CompareTag("Ball") && !m_canKick)
        {
            p_other.GetComponent<Rigidbody>().isKinematic = true;
            p_other.gameObject.transform.position = m_ballPosition.transform.position;
            p_other.gameObject.transform.SetParent(m_ballPosition.transform, true);
            m_canKick = true;
        }
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }
    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    #endregion

    #region PublicMethods

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        switch (eventCode)
        {
            case 1: // Evento de asignación de roles.
                getNewGameplayRole();
                break;
            case 2: // Evento de vibración de la cámara.
                cameraShake();
                break;
            case 3:
                break;
        }
    }

    #endregion

    #region LocalMethods

    void PlayerMov()
    {
        if (!LevelManager.instance.m_playersCanMove)
        {
            return;
        }
        if (m_PV.IsMine)
        {
            playerInputHorizontal = Input.GetAxisRaw("Horizontal");
            playerInputForward = Input.GetAxisRaw("Vertical");

            cameraForward = Camera.main.transform.forward;
            cameraRight = Camera.main.transform.right;

            cameraForward.y = 0;
            cameraRight.y = 0;

            cameraForward.Normalize();
            cameraRight.Normalize();

            m_moveDirection = (cameraForward * playerInputForward + cameraRight * playerInputHorizontal).normalized;

            rb.velocity = m_moveDirection * playerSpeed;

            if (m_moveDirection.magnitude > 0.1f)
            {
                transform.rotation = Quaternion.LookRotation(m_moveDirection);
                m_myAnim.SetBool("JOG", true);
                m_myAnim.SetBool("STOP", false);
                m_myAnim.SetFloat("MovingFloat", m_moveDirection.magnitude);
            }
            else
            {
                m_myAnim.SetBool("JOG", false);
                m_myAnim.SetBool("STOP", true);
            }
        }
    }

    void getNewGameplayRole()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Team", out object team))
        {
            //LevelManager.AssignRole();

            string m_newTeam = team.ToString();

            switch (m_newTeam)
            {
                case "Red":
                    m_currentTeamText.text = "Red Team";
                    m_currentTeamName = "Red Team";
                    m_currentTeamText.color = Color.red;
                    this.gameObject.transform.position = new Vector3(Random.Range(5, 15), 0.0f, Random.Range(-12, 12));
                    break;
                case "Blue":
                    m_currentTeamText.text = "Blue Team";
                    m_currentTeamName = "Blue Team";
                    m_currentTeamText.color = Color.blue;
                    this.gameObject.transform.position = new Vector3(Random.Range(-5, -15), 0.0f, Random.Range(-12, 12));
                    break;
            }
        }
    }

    void cameraShake()
    {
        m_cmImpulseSource.GenerateImpulse();
    }

    protected IEnumerator cooldownTimer()
    {
        yield return new WaitForSeconds(1);
        m_myAnim.SetBool("KICK", false);
        m_myAnim.SetBool("STOP", true);
        playerSpeed = 12;
        m_canKick = false;
    }

    #endregion
}
