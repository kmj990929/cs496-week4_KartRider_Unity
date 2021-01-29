using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeColor : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject kartBody;
    public GameObject Human;
    public static string KartColorString = "origin";
    public static string HumanColorString = "origin";
    public static Color KartColor = new Color(0.86f, 0.04f, 0.06f);
    public static Color HumanColor = new Color(0.45f, 0.9f, 0.9f);

    void Start()
    {
    }

    public void ChangeColorKart(string name)
    {
        KartColorString = name;
        if (name == "RED") kartBody.GetComponent<SkinnedMeshRenderer>().material.color = new Color(1f, 0f, 0f);
        else if (name == "ORANGE") kartBody.GetComponent<SkinnedMeshRenderer>().material.color = new Color(1f, 0.37f, 0.5f);
        else if (name == "YELLOW") kartBody.GetComponent<SkinnedMeshRenderer>().material.color = new Color(1f, 0.9f, 0.5f);
        else if (name == "GREEN") kartBody.GetComponent<SkinnedMeshRenderer>().material.color = new Color(0.11f, 0.86f, 0.11f);
        else if (name == "SKYBLUE") kartBody.GetComponent<SkinnedMeshRenderer>().material.color = new Color(0f, 0.85f, 1f);
        else if (name == "BLUE") kartBody.GetComponent<SkinnedMeshRenderer>().material.color = new Color(0f, 0.35f, 1f);
        else if (name == "PURPLE") kartBody.GetComponent<SkinnedMeshRenderer>().material.color = new Color(0.37f, 0f, 1f);
        else if (name == "PINK") kartBody.GetComponent<SkinnedMeshRenderer>().material.color = new Color(1f, 0f, 0.87f);
        KartColor = kartBody.GetComponent<SkinnedMeshRenderer>().material.color;


    }
    public void ChangeColorHuman(string name)
    {
        HumanColorString = name;
        if (name == "RED") Human.GetComponent<SkinnedMeshRenderer>().material.color = new Color(1f, 0f, 0f);
        else if (name == "ORANGE") Human.GetComponent<SkinnedMeshRenderer>().material.color = new Color(1f, 0.37f, 0.5f);
        else if (name == "YELLOW") Human.GetComponent<SkinnedMeshRenderer>().material.color = new Color(1f, 0.9f, 0.5f);
        else if (name == "GREEN") Human.GetComponent<SkinnedMeshRenderer>().material.color = new Color(0.11f, 0.86f, 0.11f);
        else if (name == "SKYBLUE") Human.GetComponent<SkinnedMeshRenderer>().material.color = new Color(0f, 0.85f, 1f);
        else if (name == "BLUE") Human.GetComponent<SkinnedMeshRenderer>().material.color = new Color(0f, 0.35f, 1f);
        else if (name == "PURPLE") Human.GetComponent<SkinnedMeshRenderer>().material.color = new Color(0.37f, 0f, 1f);
        else if (name == "PINK") Human.GetComponent<SkinnedMeshRenderer>().material.color = new Color(1f, 0f, 0.87f);
        HumanColor = Human.GetComponent<SkinnedMeshRenderer>().material.color;
    }

    public void Back()
    {
        SceneManager.LoadScene("IntroMenu");
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
