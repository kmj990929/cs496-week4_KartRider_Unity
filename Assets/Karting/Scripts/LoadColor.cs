using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KartGame.KartSystems;
using Photon.Pun;

public class LoadColor : MonoBehaviourPun
{
    public GameObject kartBody;
    public GameObject Human;
    public GameObject minimapPlayer;

    public string kartColor;
    public string playerColor;

    public static Color originKartColor = new Color(0.86f, 0.04f, 0.06f);
    public static Color originHumanColor = new Color(0.45f, 0.9f, 0.9f);

    public static Color red = new Color(1f, 0f, 0f);
    public static Color orange = new Color(1f, 0.37f, 0.5f);
    public static Color yellow = new Color(1f, 0.9f, 0.5f);
    public static Color green = new Color(0.11f, 0.86f, 0.11f);
    public static Color skyblue = new Color(0f, 0.85f, 1f);
    public static Color blue = new Color(0f, 0.35f, 1f);
    public static Color purple = new Color(0.37f, 0f, 1f);
    public static Color pink = new Color(1f, 0f, 0.87f);

    // Start is called before the first frame update
    void Start()
    {
        kartColor = ChangeColor.KartColorString;
        playerColor = ChangeColor.HumanColorString;

        if (photonView.IsMine)
        {
            minimapPlayer.GetComponent<MeshRenderer>().material.color = new Color(1f, 0f, 0f);
        }
        else
        {
            minimapPlayer.GetComponent<MeshRenderer>().material.color = new Color(0f, 0.35f, 1f);
        }
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("changeKartColor", RpcTarget.All, kartColor);
            photonView.RPC("changePlayerColor", RpcTarget.All, playerColor);
        }
    }

    [PunRPC]
    void changeKartColor(string name)
    {
        Color color = originKartColor;
        if (name == "RED") color = red;
        else if (name == "ORANGE") color = orange;
        else if (name == "YELLOW") color = yellow;
        else if (name == "GREEN") color = green;
        else if (name == "SKYBLUE") color = skyblue;
        else if (name == "BLUE") color = blue;
        else if (name == "PURPLE") color = purple;
        else if (name == "PINK") color = pink;
        else if (name == "origin") color = originKartColor;
        kartBody.GetComponent<SkinnedMeshRenderer>().material.color = color;
    }

    [PunRPC]
    void changePlayerColor(string name)
    {
        Color color = originHumanColor;
        if (name == "RED") color = red;
        else if (name == "ORANGE") color = orange;
        else if (name == "YELLOW") color = yellow;
        else if (name == "GREEN") color = green;
        else if (name == "SKYBLUE") color = skyblue;
        else if (name == "BLUE") color = blue;
        else if (name == "PURPLE") color = purple;
        else if (name == "PINK") color = pink;
        else if (name == "origin") color = originHumanColor;
        Human.GetComponent<SkinnedMeshRenderer>().material.color = color;
    }
}
