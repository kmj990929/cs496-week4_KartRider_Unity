using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    private readonly string gameVersion = "1"; // 게임 버전

    public TextMeshProUGUI userInfoText; // 사용자 정보를 표시할 텍스트
    public TextMeshProUGUI connectionInfoText; // 네트워크 정보를 표시할 텍스트
    public Button defaultSelection;         //////////////////////
    public static string userEmail;

    public GameObject kartBody;
    public GameObject HumanBody;
    public GameObject PlayerKart;
    public GameObject PlayerHuman;
    public static Color KartColor = new Color(0.86f, 0.04f, 0.06f);
    public static Color HumanColor = new Color(0.45f, 0.9f, 0.9f);



    // 게임 실행과 동시에 마스터 서버 접속 시도
    private void Start()
    {
        Debug.Log("lobby start");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        EventSystem.current.SetSelectedGameObject(null);
        userInfoText.text = AuthManager.User.Email;
        userEmail = AuthManager.User.Email;

        // 접속에 필요한 정보(게임 버전) 설정
        PhotonNetwork.GameVersion = gameVersion;
        // 설정한 정보를 가지고 마스터 서버 접속 시도
        PhotonNetwork.ConnectUsingSettings();

        // 룸 접속 버튼을 잠시 비활성화
        //defaultSelection.interactable = false;
        // 접속을 시도 중임을 텍스트로 표시
        connectionInfoText.text = "Connecting to master server...";

        SetColor();
        //kartBody.GetComponent<SkinnedMeshRenderer>().material.color = ChangeColor.KartColor;
        //HumanBody.GetComponent<SkinnedMeshRenderer>().material.color = ChangeColor.HumanColor;
        //Debug.Log(ChangeColor.kartBody);
        //Debug.Log(ChangeColor.Human);

    }

    private void Update()
    {
        SetColor();
    }

    void LateUpdate()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            if (Input.GetButtonDown(GameConstants.k_ButtonNameSubmit)
                || Input.GetAxisRaw(GameConstants.k_AxisNameHorizontal) != 0
                || Input.GetAxisRaw(GameConstants.k_AxisNameVertical) != 0)
            {
                EventSystem.current.SetSelectedGameObject(defaultSelection.gameObject);
            }
        }
    }


    // 마스터 서버 접속 성공시 자동 실행
    public override void OnConnectedToMaster()
    {
        // 룸 접속 버튼을 활성화
        defaultSelection.interactable = true;
        // 접속 정보 표시
        connectionInfoText.text = "Online : Connected to Master Server";
    }

    // 마스터 서버 접속 실패시 자동 실행
    public override void OnDisconnected(DisconnectCause cause)
    {
        // 룸 접속 버튼을 비활성화
        defaultSelection.interactable = false;
        // 접속 정보 표시
        connectionInfoText.text = $"Offline : Connection Disabled {cause.ToString()}  - Try reconnecting...";

        // 마스터 서버로의 재접속 시도
        PhotonNetwork.ConnectUsingSettings();
    }

    // 룸 접속 시도
    public void Connect()
    {
        Debug.Log("connect");
        // 중복 접속 시도를 막기 위해, 접속 버튼 잠시 비활성화
        defaultSelection.interactable = false;

        // 마스터 서버에 접속중이라면
        if (PhotonNetwork.IsConnected)
        {
            // 룸 접속 실행
            connectionInfoText.text = "Connecting to Random Room...";
            //PhotonNetwork.JoinRandomRoom();
            PhotonNetwork.JoinRoom(LoadSceneButton.selectedSceneName);
        }
        else
        {
            // 마스터 서버에 접속중이 아니라면, 마스터 서버에 접속 시도
            connectionInfoText.text = $"Offline : Connection Disabled - Try reconnecting...";
            // 마스터 서버로의 재접속 시도
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void Myroom()
    {
        SceneManager.LoadScene("Myroom");
    }

    public void SetColor()
    {
        kartBody.GetComponent<SkinnedMeshRenderer>().material.color = ChangeColor.KartColor;
        HumanBody.GetComponent<SkinnedMeshRenderer>().material.color = ChangeColor.HumanColor;
        KartColor = ChangeColor.KartColor;
        HumanColor = ChangeColor.HumanColor;

    }

    // (빈 방이 없어)랜덤 룸 참가에 실패한 경우 자동 실행
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        // 접속 상태 표시
        Debug.Log("make room : " + LoadSceneButton.selectedSceneName);
        connectionInfoText.text = "There are no empty rooms, create new room...";

        // 최대 2명을 수용 가능한 빈방을 생성
        PhotonNetwork.CreateRoom(LoadSceneButton.selectedSceneName, new RoomOptions { MaxPlayers = 4 });
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        // 접속 상태 표시
        Debug.Log("JoinRoom");
        Debug.Log("make room : " + LoadSceneButton.selectedSceneName);
        connectionInfoText.text = "There are no empty rooms, create new room...";
        // 최대 2명을 수용 가능한 빈방을 생성
        PhotonNetwork.CreateRoom(LoadSceneButton.selectedSceneName, new RoomOptions { MaxPlayers = 4 });
    }

    // 룸에 참가 완료된 경우 자동 실행
    public override void OnJoinedRoom()
    {
        Debug.Log("onjoinnedroom : " + LoadSceneButton.selectedSceneName);

        // 접속 상태 표시
        connectionInfoText.text = "Connected with Room.";
        PhotonNetwork.LoadLevel(LoadSceneButton.selectedSceneName); //선택적으로 룸 안에서 로드된 레벨과 동기화 
        // 모든 룸 참가자들이 Main 씬을 로드하게 함
    }
}
