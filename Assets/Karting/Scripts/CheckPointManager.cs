using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckPointManager : MonoBehaviour
{
    public static bool passCheckpoint = false;
    public static GameObject currentCheckpoint;
    public static int currentCheckpointIndex;

    // Start is called before the first frame update
    public GameObject[] checkPointList = new GameObject[10];
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //현재 통과한 checkpoint index 불러오기
        if (passCheckpoint)
        {
            for (int i=0; i<checkPointList.Length; i++)
            {
                if (checkPointList[i] == currentCheckpoint)
                {
                    currentCheckpointIndex = i;
                    passCheckpoint = false;
                }
            }
        }
    }
}
