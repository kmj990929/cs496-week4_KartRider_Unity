using UnityEngine;
using Photon.Pun;

namespace KartGame.KartSystems
{
    public class KartCollisionManager : MonoBehaviourPun
    {
        public bool isItemCollision = false;

        public bool useBanana = false;
        public bool bananaEffect = false;
        public float bananaEffectTime;

        public bool useWaterBalloon = false;
        public bool WaterBalloonEffect = false;
        public float WaterBalloonEffectTime;

        public static bool localPlayerCloudEffect = false;
        public static float localPlayerCloudEffectTime;

        public int itemKind;
        public bool hasItem = false;

        public static int localPlayerItemStates;
        public static bool updateItemBox = false;

        public static bool localPlayerUseRocket = false;

        public bool rocketEffect = false;
        public bool hitRocket = false;
        public float hitRocketTime;

        public KeyboardInput keyboardInput;
        public ArcadeKart arcadeKart;

        public static bool localPlayerIsWinner = false;
        public static bool checkWinner = false;

        AudioSource k_audio;

        public AudioClip itemGet;
        public AudioClip ItemSpace;
        public AudioClip GetBanana;
        public AudioClip GetWaterball;
        public AudioClip GetRespawn;
        public AudioClip GetRocket;

        private void Start()
        {
            keyboardInput = GetComponent<KeyboardInput>();
            arcadeKart = GetComponent<ArcadeKart>();
            k_audio = GetComponent<AudioSource>();
        }
        void Update()
        {
            //아이템 쏘기 - 스페이스바를 누르고 선택된 아이템 번호가 0, 1, 2 중 하나이면 
            bool space = Input.GetKeyDown(KeyCode.Space);
            if (space & hasItem & photonView.IsMine)
            {
                photonView.RPC("useItem", RpcTarget.All, 1);
            }

            //item 획득
            if (isItemCollision)
            {
                if (photonView.IsMine)
                {
                    Debug.Log("item collision");
                    int rand = UnityEngine.Random.Range(1, 5); //Item test를 위한 임시 수정
                    //int rand = 3;
                    photonView.RPC("getItem", RpcTarget.All, rand);

                    // 로컬 아이템 박스 변경
                    localPlayerItemStates = rand;
                    updateItemBox = true;
                }
                else
                {
                    isItemCollision = false;
                }
            }
            if (!hasItem & photonView.IsMine)
            {
                localPlayerItemStates = 0;
                updateItemBox = true;
            }

            //떨어지면 
            if (transform.position.y < -10)
            {
                k_audio.clip = GetRespawn;
                k_audio.Play();
                if (LoadSceneButton.selectedSceneName == "Map1Scene")
                {
                    transform.position = new Vector3(230, 15, 10);
                    Debug.Log("Map1Scene");
                }
                else if (LoadSceneButton.selectedSceneName == "MainScene")
                {
                    transform.position = new Vector3(12, 1, 1);
                    Debug.Log("MainScene");
                }
            }
            //밖으로 나가면 
            if (keyboardInput.isGround)
            {
                k_audio.clip = GetRespawn;
                k_audio.Play();
                Debug.Log("isGround true");
                if (LoadSceneButton.selectedSceneName == "Map1Scene")
                {
                    transform.position = new Vector3(230, 15, 10);
                    Debug.Log("Map1Scene");
                }
                else if (LoadSceneButton.selectedSceneName == "MainScene")
                {
                    transform.position = new Vector3(12, 1, 1);
                    Debug.Log("MainScene");
                }
                keyboardInput.isGround = false;
            }

            //rocket 맞으면
            if (photonManager.shootRocket & !photonView.IsMine)
            {
                k_audio.clip = GetRocket;
                k_audio.Play();
                photonView.RPC("hitRocketFunction", RpcTarget.All, 1);
            }
            if (map1PhotonManager.shootRocket & !photonView.IsMine)
            {
                k_audio.clip = GetRocket;
                k_audio.Play();
                photonView.RPC("hitRocketFunction", RpcTarget.All, 1);
            }


            //우승자 판별
            if (keyboardInput.isWinner)
            {
                Debug.Log("isWinner Test");
                if (photonView.IsMine)
                {
                    localPlayerIsWinner = true;
                    //Debug.Log("localPlayerIsWinner == True");

                }
                else
                {
                    localPlayerIsWinner = false;
                    //Debug.Log("localPlayerIsWinner == False");
                }
                checkWinner = true;
            }
        }
     
        public void OnTriggerEnter(Collider collision)
        {
            if (collision.gameObject.tag == "item")
            {
                Destroy(collision.gameObject);
                k_audio.clip = itemGet;
                k_audio.Play();
                isItemCollision = true;
            }
            else if (collision.gameObject.tag == "checkpoint" && photonView.IsMine)
            {
                if (LoadSceneButton.selectedSceneName == "MainScene")
                {
                    photonManager.localPlayerPassCheckpoint = true;
                }
                else if (LoadSceneButton.selectedSceneName == "Map1Scene")
                {
                    map1PhotonManager.localPlayerPassCheckpoint = true;
                }
                Debug.Log("checkpoint log");
            }
            else if (collision.gameObject.tag == "speedPad")
            {
                speedUp();
            }
        }

        public void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.gameObject.tag == "Player")
            {
                Debug.Log("conflict with player");
            }
            else if (collision.collider.gameObject.tag == "Banana")
            {
                k_audio.clip = GetBanana;
                k_audio.Play();
                Debug.Log("mine");
                PhotonNetwork.Destroy(collision.collider.gameObject);
                bananaEffect = true;
                bananaEffectTime = Time.time; 
            }
            else if (collision.collider.gameObject.tag == "waterBall")
            {
                Debug.Log("get waterball");
                k_audio.clip = GetWaterball;
                k_audio.Play();
                PhotonNetwork.Destroy(collision.collider.gameObject);
                WaterBalloonEffect = true;
                WaterBalloonEffectTime = Time.time;
            }
        }
        
        [PunRPC]
        public void useItem(int arg)
        {
            //banana
            if (itemKind == 1)
            {
                useBanana = true;
                hasItem = false;
                itemKind = 0;
            }
            //cloud
            else if (itemKind == 2)
            {
                Debug.Log("use cloud");
                hasItem = false;
                itemKind = 0;
                //cloud Effect 실행
                Debug.Log(photonView.IsMine);
                if (!photonView.IsMine)
                {
                    Debug.Log("local player cloud effect");
                    localPlayerCloudEffect = true;
                    localPlayerCloudEffectTime = Time.time;
                }
            }
            //rocket
            else if (itemKind == 3)
            {
                Debug.Log("use rocket");
                hasItem = false;
                itemKind = 0;
                //rocket bar 띄우기
                if (photonView.IsMine)
                {
                    localPlayerUseRocket = true;
                }
            }
            else if (itemKind == 4)
            {
                Debug.Log("itemkind waterballoon");
                useWaterBalloon = true; // banana animation 에서 처리 
                hasItem = false;
                itemKind = 0;
            }
        }

        [PunRPC]
        public void getItem(int rand)
        {
            Debug.Log("getItem"); 
            itemKind = rand;
            hasItem = true;
            isItemCollision = false;
        }

        [PunRPC]
        public void hitRocketFunction(int arg)
        {
            hitRocket = true;
            hitRocketTime = Time.time;
            photonManager.shootRocket = false;
            map1PhotonManager.shootRocket = false;
        }

        void speedUp()
        {
            arcadeKart.speedPadActivate = true;
        }

    }


}
