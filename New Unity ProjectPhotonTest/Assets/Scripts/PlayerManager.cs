using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;
using System.Collections;
using Photon.Pun.Demo.PunBasics;

public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] private GameObject beams;
    [SerializeField] private GameObject PlayerUIPrefab;

    private bool isFiring = false;

    public static GameObject LocalPlayerInstance;

    public float Health = 1f;

    private void Awake()
    {
        if (beams == null)
            Debug.LogError("<Color=Red><a>Missing</a></Color> Beams Reference.", this);
        else
            beams.SetActive(false);

        if (photonView.IsMine)
            PlayerManager.LocalPlayerInstance = this.gameObject;

        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        CameraWork cameraWork = this.gameObject.GetComponent<CameraWork>();

        if (PlayerUIPrefab != null)
        {
            GameObject uiGo = Instantiate(PlayerUIPrefab);
            uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
        }
        else
            Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);

        if (cameraWork != null)
        {
            if (photonView.IsMine)
                cameraWork.OnStartFollowing();
        }
        else
            Debug.LogError("<Color=Red><a>Missing</a></Color> CameraWork Component on playerPrefab.", this);

#if UNITY_5_4_OR_NEWER
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += onSceneLoaded;
#endif
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            processInputs();

            if (Health <= 0f)
                GameManager.Instance.LeaveRoom();
        }

        if (beams != null && isFiring != beams.activeInHierarchy)
        {
            beams.SetActive(isFiring);
        }
    }

#if UNITY_5_4_OR_NEWER
    private void onSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadingMode)
    {
        this.CalledOnLevelWasLoaded(scene.buildIndex);
    }

    private void OnLevelWasLoaded(int level)
    {
        this.CalledOnLevelWasLoaded(level);
    }
#endif

    private void CalledOnLevelWasLoaded(int level)
    {
        if (Physics.Raycast(transform.position, -Vector3.up, 5f))
            transform.position = new Vector3(0f, 5f, 0f);

        GameObject uiGo = Instantiate(this.PlayerUIPrefab);
        uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
    }

    private void processInputs()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && !isFiring)
            isFiring = true;
        if (Input.GetKeyUp(KeyCode.Mouse0) && isFiring)
            isFiring = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine)
            return;

        if (!other.name.Contains("Beam"))
            return;

        Health -= 0.1f;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!photonView.IsMine)
            return;

        if (!other.name.Contains("Beam"))
            return;

        Health -= 0.1f * Time.deltaTime;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isFiring);
            stream.SendNext(Health);
        }
        else
        {
            this.isFiring = (bool)stream.ReceiveNext();
            this.Health = (float)stream.ReceiveNext();
        }
    }

#if UNITY_5_4_OR_NEWER
    public override void OnDisable()
    {
        base.OnDisable();
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= onSceneLoaded;
    }
#endif
}
