using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class ExitManager : MonoBehaviourPun
{
    [Tooltip("What is the name of the scene we want to load when clicking the button?")]
    public string SceneName;

    public void LoadTargetScene()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel(SceneName);
    }
}
