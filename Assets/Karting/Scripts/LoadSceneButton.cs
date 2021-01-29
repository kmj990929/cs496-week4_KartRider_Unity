using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class LoadSceneButton : MonoBehaviourPun
{
    [Tooltip("What is the name of the scene we want to load when clicking the button?")]
    public string SceneName;//play 버튼 클릭시 main scene 으로 이동 
    public static string selectedSceneName;

    public void LoadTargetScene()
    {
        selectedSceneName = SceneName;
        PhotonNetwork.LoadLevel("IntroMenu");
    }
}
