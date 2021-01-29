using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class RestartButton : MonoBehaviourPun
{
    public void LoadTargetScene()
    {
        PhotonNetwork.LoadLevel("SelectMapScene");
    }
}
