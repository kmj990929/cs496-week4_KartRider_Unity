using System.Collections;
using System.Collections.Generic;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using KartGame.KartSystems;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using System;

public enum GameState { Play, Won, Lost }

public class map1PhotonManager : MonoBehaviourPunCallbacks
{
    public float endSceneLoadDelay = 3f;
    public CanvasGroup endGameFadeCanvasGroup;
    public string winSceneName = "WinScene";
    public string loseSceneName = "LoseScene";
    public float delayBeforeFadeToBlack = 2f;
    public float delayBeforeWinMessage = 1f;
    public AudioClip victorySound;
    public AudioClip defeatSound;
    public DisplayMessage winDisplayMessage; //All objectives completed
    public PlayableDirector raceCountdownTrigger; //countdown 3,2,1,go
    public DisplayMessage loseDisplayMessage; //time out

    AudioSource mainAudio;
    AudioSource mm_audio;
    public AudioClip GetCloud;
    public AudioClip bazzi;
    public GameState gameState { get; private set; }

    public static photonManager Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<photonManager>();
            Debug.Log("cannot found instance");

            return instance;
        }
    }

    private static photonManager instance;
    public ArcadeKart playerPrefab;
    public Slider boosterBar;
    public RawImage boosterItem1;
    public RawImage boosterItem2;
    public RawImage boosterItem3;
    public RawImage boosterState;
    public float boosterNum = 0.0f;
    public float boostTime = 0.0f;
    public static float driftTime = 0.0f;
    public static bool isBoost = false;
    public static bool isDrift = false;
    public TextMeshProUGUI velocityBar;
    public RawImage cloudItem;

    public GameObject rocketBar;
    public Image rocketMatchingBar;
    public bool barDirection = true;
    public static bool shootRocket = false;

    public Sprite[] itemSprite = new Sprite[4];
    public Sprite emptyItem;

    public Image itemBox;
    public Button startButton;

    public bool autoFindKarts = true;
    ArcadeKart[] karts;
    ObjectiveManager m_ObjectiveManager;
    TimeManager m_TimeManager;
    float m_TimeLoadEndGameScene;
    string m_SceneToLoad;
    float elapsedTimeBeforeEndScene = 0;

    public static string[] nameList = new string[4];
    public Text namePrintingArea;
    public static bool localPlayerPassCheckpoint = false;

    public bool map1BoostCanStart = false;

    void Start()
    {
        mainAudio = GetComponent<AudioSource>();
        mm_audio = GetComponent<AudioSource>();
        mainAudio.clip = bazzi;
        mainAudio.volume = 0.5F;
        mainAudio.loop = true;
        mainAudio.Play();
       
        Player();
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("IsMasterClient");
        }
        boosterItem1.enabled = false;
        boosterItem2.enabled = false;
        boosterItem3.enabled = false;
        boosterState.enabled = false;
        cloudItem.enabled = false;
        rocketBar.SetActive(false);

        itemBox.sprite = emptyItem;

        if (autoFindKarts)
        {
            karts = FindObjectsOfType<ArcadeKart>();
            if (karts.Length > 0)
            {
                if (!playerPrefab) playerPrefab = karts[0];
            }
            DebugUtility.HandleErrorIfNullFindObject<ArcadeKart, photonManager>(playerPrefab, this);
        }
        m_ObjectiveManager = FindObjectOfType<ObjectiveManager>();
        DebugUtility.HandleErrorIfNullFindObject<ObjectiveManager, photonManager>(m_ObjectiveManager, this);
        m_TimeManager = FindObjectOfType<TimeManager>();
        DebugUtility.HandleErrorIfNullFindObject<TimeManager, photonManager>(m_TimeManager, this);
        AudioUtility.SetMasterVolume(1);
        winDisplayMessage.gameObject.SetActive(false);
        loseDisplayMessage.gameObject.SetActive(false);
        m_TimeManager.StopRace();
        foreach (ArcadeKart k in karts)
        {
            Debug.Log("StopRace()");
            k.SetCanMove(false); // 못 움직이이게 함 
            map1BoostCanStart = false;
        }

        //startButton
        if (!PhotonNetwork.IsMasterClient)
        {
            startButton.gameObject.SetActive(false);
        }
        startButton.onClick.AddListener(clickStartButton);
    }


    private void Player()
    {
        Debug.Log("player Number");
        //int num = PhotonNetwork.CountOfPlayersInRooms;
        int num = PhotonNetwork.CurrentRoom.PlayerCount;
        Debug.Log(num);
        Vector3 playerInitLocation = new Vector3(219.5f + num * 4, 15, 10);
        PhotonNetwork.Instantiate(playerPrefab.name, playerInitLocation, Quaternion.identity);
    }

    IEnumerator CountdownThenStartRaceRoutine()
    {
        yield return new WaitForSeconds(3f);//3초 기다린 뒤 
        StartRace();
    }
    void StartRace()
    {
        foreach (ArcadeKart k in karts)
        {
            k.SetCanMove(true); //이제 움직일 수 있게끔 함 
            map1BoostCanStart = true;
        }
        m_TimeManager.StartRace();
    }
    void ShowRaceCountdownAnimation()
    {
        raceCountdownTrigger.Play(); //3 -> 2 -> 1 go
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Lobby");
    }

    IEnumerator ShowObjectivesRoutine()
    {
        while (m_ObjectiveManager.Objectives.Count == 0)
            yield return null;
        yield return new WaitForSecondsRealtime(0.2f);
        for (int i = 0; i < m_ObjectiveManager.Objectives.Count; i++)
        {
            if (m_ObjectiveManager.Objectives[i].displayMessage) m_ObjectiveManager.Objectives[i].displayMessage.Display();
            yield return new WaitForSecondsRealtime(1f);
        }
    }

    void Update()
    {
        VelocityUpdate();

        //booster
        if (Input.GetKey(KeyCode.UpArrow) && !isBoost && map1BoostCanStart)
        {
            boosterBar.value += 0.1f;
        }
        if (boosterBar.value == 50.0f && !isBoost)
        {
            Debug.Log("booster bar full!");
            addBooster();
            boosterBar.value = 0.0f;
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (!isBoost && boosterNum > 0.0f)
            {
                boostTime = Time.time;
                isBoost = true;
                deleteBooster();
            }
        }

        if (isBoost)
        {
            if (Time.time - boostTime > 3f)
            {
                isBoost = false;
                boosterBar.value = 0.0f;
                boosterState.enabled = false;
            }
            else
            {
                boosterBar.value = 50.0f;
                boosterState.enabled = true;
            }
        }

        //drift
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (!isDrift)
            {
                isDrift = true;
                driftTime = Time.time;
            }
        }
        else
        {
            isDrift = false;
        }


        if (gameState != GameState.Play)
        {
            elapsedTimeBeforeEndScene += Time.deltaTime;
            if (elapsedTimeBeforeEndScene >= endSceneLoadDelay)
            {
                float timeRatio = 1 - (m_TimeLoadEndGameScene - Time.time) / endSceneLoadDelay;
                endGameFadeCanvasGroup.alpha = timeRatio;
                float volumeRatio = Mathf.Abs(timeRatio);
                float volume = Mathf.Clamp(1 - volumeRatio, 0, 1);
                AudioUtility.SetMasterVolume(volume);
                // See if it's time to load the end scene (after the delay)
                if (Time.time >= m_TimeLoadEndGameScene)
                {
                    SceneManager.LoadScene(m_SceneToLoad);
                    gameState = GameState.Play;
                }
            }
        }
        else
        {
            if (KeyboardInput.isFinish & KartCollisionManager.checkWinner)//모든 것을 다 통과하고 + 시간 내이며 // gameobject가 finish 선 밟았을 때 
            {
                if (KartCollisionManager.localPlayerIsWinner)
                {
                    EndGame(true); //이겼을 때 
                }
                else
                {
                    EndGame(false);
                }

            }
            //if (m_TimeManager.IsFinite && m_TimeManager.IsOver)
            //{
            //    EndGame(false); //졌을 때 
            //    Debug.Log("EndGame(false)");
            //}
        }

        //item 처리
        if (KartCollisionManager.updateItemBox)
        {
            changeImage(KartCollisionManager.localPlayerItemStates);
        }

        //cloud 처리
        if (KartCollisionManager.localPlayerCloudEffect)
        {
            mm_audio.clip = GetCloud;
            mm_audio.Play();
            Debug.Log("Local player cloud effect start");
            if (Time.time - KartCollisionManager.localPlayerCloudEffectTime < 5.0f)
            {
                cloudItem.enabled = true;
            }
            else
            {
                cloudItem.enabled = false;
                KartCollisionManager.localPlayerCloudEffect = false;
            }
        }

        //rocket 조준 처리
        if (KartCollisionManager.localPlayerUseRocket)
        {
            activeRocketBar();
        }

        //nameList Update
        if (PhotonNetwork.IsMasterClient)
        {
            string resultText = "";
            ArcadeKart[] currentKarts;
            currentKarts = FindObjectsOfType<ArcadeKart>();

            List<Tuple<int, string>> rankingDict = new List<Tuple<int, string>>();
            List<int> keyList = new List<int>();
            foreach (ArcadeKart kart in currentKarts)
            {
                rankingDict.Add(new Tuple<int, string>(kart.currentCheckpointIndex, kart.nameText));
                keyList.Add(kart.currentCheckpointIndex);
            }
            keyList.Sort();
            keyList.Reverse();

            foreach (var key in keyList)
            {
                foreach (var ranking in rankingDict)
                {
                    if (ranking.Item1 == key)
                    {
                        resultText += ranking.Item2;
                        rankingDict.Remove(ranking);
                        break;
                    }
                }
                resultText += "\n";
            }
            photonView.RPC("updateNameList", RpcTarget.All, resultText);
        }

        //ranking update
        if (localPlayerPassCheckpoint)
        {
            ArcadeKart.localPlayerPassCheckpointIndex = CheckPointManager.currentCheckpointIndex;
            ArcadeKart.updateCheckpoint = true;
            localPlayerPassCheckpoint = false;

        }

    }

    void addBooster()
    {
        if (boosterNum == 0.0f)
        {
            boosterItem1.enabled = true;
            boosterNum = 1.0f;
        }
        else if (boosterNum == 1.0f)
        {
            boosterItem2.enabled = true;
            boosterNum = 2.0f;
        }
        else if (boosterNum == 2.0f)
        {
            boosterItem3.enabled = true;
            boosterNum = 3.0f;
        }
    }

    void deleteBooster()
    {
        if (boosterNum == 1.0f)
        {
            boosterItem1.enabled = false;
            boosterNum = 0.0f;
            return;
        }
        else if (boosterNum == 2.0f)
        {
            boosterItem2.enabled = false;
            boosterNum = 1.0f;
            return;
        }
        else if (boosterNum == 3.0f)
        {
            boosterItem3.enabled = false;
            boosterNum = 2.0f;
            return;
        }
    }

    void VelocityUpdate()
    {
        var velocity = Math.Round(ArcadeKart.velocityBarData);
        if (velocity < 3) velocityBar.text = ("0" + " km/h");
        else velocityBar.text = (Math.Round(ArcadeKart.velocityBarData * 20)).ToString() + " km/h";
    }

    public void changeImage(int rand)
    {
        if (rand == 1)
        {
            itemBox.sprite = itemSprite[0];
        }
        else if (rand == 2)
        {
            itemBox.sprite = itemSprite[1];
        }
        else if (rand == 3)
        {
            itemBox.sprite = itemSprite[2];
        }
        else if (rand == 4)
        {
            itemBox.sprite = itemSprite[3];
        }

        else if (rand == 0)
        {
            itemBox.sprite = emptyItem;
        }
        KartCollisionManager.updateItemBox = false; //업데이트 완료
    }

    void EndGame(bool win)
    {
        mainAudio.Stop();
        Debug.Log("EndGame");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        m_TimeManager.StopRace();
        //Remember that we need to load the appropriate end scene after a delay
        gameState = win ? GameState.Won : GameState.Lost;
        endGameFadeCanvasGroup.gameObject.SetActive(true);
        if (win) //이겼을 때 
        {
            m_SceneToLoad = winSceneName;
            m_TimeLoadEndGameScene = Time.time + endSceneLoadDelay + delayBeforeFadeToBlack; //씬 딜레이 시간
            // play a sound on win                                                                                 // play a sound on win
            var audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = victorySound;
            audioSource.playOnAwake = false;
            audioSource.outputAudioMixerGroup = AudioUtility.GetAudioGroup(AudioUtility.AudioGroups.HUDVictory);
            audioSource.PlayScheduled(AudioSettings.dspTime + delayBeforeWinMessage);
            // create a game message
            winDisplayMessage.delayBeforeShowing = delayBeforeWinMessage;
            winDisplayMessage.gameObject.SetActive(true); //이겼다는 메세지 true
        }
        else
        {
            m_SceneToLoad = loseSceneName;
            m_TimeLoadEndGameScene = Time.time + endSceneLoadDelay + delayBeforeFadeToBlack;
            var deFeataudioSource = gameObject.AddComponent<AudioSource>();
            deFeataudioSource.clip = defeatSound;
            deFeataudioSource.playOnAwake = false;
            deFeataudioSource.outputAudioMixerGroup = AudioUtility.GetAudioGroup(AudioUtility.AudioGroups.HUDVictory);
            deFeataudioSource.PlayScheduled(AudioSettings.dspTime + delayBeforeWinMessage);
            // create a game message
            loseDisplayMessage.delayBeforeShowing = delayBeforeWinMessage;
            loseDisplayMessage.gameObject.SetActive(true);
        }

    }

    void activeRocketBar()
    {
        rocketBar.SetActive(true);

        int matchingBarSpeed = 5;
        float rocketBarStartpoint = 690f;
        float rocketBarEndpoint = 1230f;
        float beforeX = rocketMatchingBar.transform.position.x;

        Vector3 position;
        if (barDirection)
        {
            position = new Vector3(beforeX + matchingBarSpeed, 740f, 0);
            if (position.x > rocketBarEndpoint)
            {
                barDirection = false;
            }
        }
        else
        {
            position = new Vector3(beforeX - matchingBarSpeed, 740f, 0);
            if (position.x < rocketBarStartpoint)
            {
                barDirection = true;
            }
        }
        rocketMatchingBar.transform.position = position;

        //matching check
        float targetRegionStartPoint = 920f;
        float targetRegionEndPoint = 995f;
        if (Input.GetKey(KeyCode.Z))
        {
            if ((targetRegionStartPoint < position.x) & (position.x < targetRegionEndPoint))
            {
                Debug.Log("rocket success");
                shootRocket = true;
            }
            else
            {
                Debug.Log("rocket false");
            }
            rocketBar.SetActive(false);
            KartCollisionManager.localPlayerUseRocket = false;
        }
    }

    [PunRPC]
    void startGame(int arg)
    {
        ShowRaceCountdownAnimation();
        StartCoroutine(ShowObjectivesRoutine()); //startCoroutine 마치 쓰레드와 같은 역할
        StartCoroutine(CountdownThenStartRaceRoutine());
    }

    void clickStartButton()
    {
        photonView.RPC("startGame", RpcTarget.All, 1);
        startButton.gameObject.SetActive(false);
    }

    [PunRPC]
    void updateNameList(string resultText)
    {
        namePrintingArea.text = resultText;
    }
}
