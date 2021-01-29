using UnityEngine;
namespace KartGame.KartSystems
{
    public class KeyboardInput : BaseInput
    {
        private string Horizontal = "Horizontal";
        private string Vertical = "Vertical";
        public bool isGround = false;
        public static bool isFinish = false;
        public bool isWinner = false;
        private bool isJump = false;
        private int height = 40;
        private int front = 30;
        private float horizontalInput;
        private float verticalInput;
        private float currentSteerAngle;
        private float currentbreakForce;
        private bool isBreaking;
        private float jumptime = 20000;
        public bool isJumped = false;
        ObjectiveManager m_ObjectiveManager;

        Rigidbody rb;
        private GameObject stoneInstance;
        public GameObject stone;
        private bool isStone = false;

        AudioSource ki_audio;
        public AudioClip GetJump;
        public AudioClip GetStone;




        public override Vector2 GenerateInput()
        {
            return new Vector2
            {
                x = Input.GetAxis(Horizontal),
                y = Input.GetAxis(Vertical)
            };
        }

        private void Start()
        {
            m_ObjectiveManager = FindObjectOfType<ObjectiveManager>();
            rb = GetComponent<Rigidbody>();
            stoneInstance = GetComponent<GameObject>();
            ki_audio = GetComponent<AudioSource>();
        }

        private void Update()
        {
            GetInput();

            //jump
            if (isJump) //
            {
                ki_audio.clip = GetJump;
                ki_audio.Play();
                if (isJumped == false) jumptime = Time.time;
                isJumped = true;
                gameObject.transform.position += (Vector3.up * height * Time.deltaTime);
                gameObject.transform.position += (Vector3.forward * front * Time.deltaTime);
                //isJump = false;
            }
            if (Time.time - jumptime > 1f)
            {
                //gameObject.transform.position += (Vector3.right * up * Time.deltaTime);
                Debug.Log("gameObject.transform.position " + gameObject.transform.position);
                isJumped = false;
                isJump = false;
            }

           

            if (isStone)
            {
                ki_audio.clip = GetStone;
                ki_audio.Play();
                Vector3 playerPostion = gameObject.transform.position;
                Vector3 stonePosition = Vector3.zero;
                if (LoadSceneButton.selectedSceneName == "MainScene")
                {
                    stonePosition = playerPostion + new Vector3(0, 1, -50);
                }
                else if (LoadSceneButton.selectedSceneName == "Map1Scene")
                {
                    stonePosition = new Vector3(213, 40, -140);
                    //playerPostion + new Vector3(-40, 10, -50);
                }
                Debug.Log(" stonePosition : " + stonePosition);
                float a = Random.Range(-10f, 10f);
                Quaternion rot = Quaternion.Euler(a, a, a);
                stoneInstance = Instantiate(stone, stonePosition, rot);
                Debug.Log(rot);
                //stoneInstance.transform.Rotate(Vector3.up * 2 * Time.deltaTime);
                //stoneInstance.transform.Rotate(Vector3.left * 2 * Time.deltaTime);
                Rigidbody stoneRigidBody = stoneInstance.AddComponent<Rigidbody>();
                stoneRoll(stoneRigidBody);
                Destroy(stoneInstance, 10);
            }

      
        }

        private void stoneRoll(Rigidbody stoneRigidBody)
        {
            Debug.Log("stoneRoll get");
            if (LoadSceneButton.selectedSceneName == "MainScene")
            {
                stoneRigidBody.AddForce(new Vector3(0, 0, 10) * 40f);
            }
            else if (LoadSceneButton.selectedSceneName == "Map1Scene")
            {
                stoneRigidBody.AddForce(new Vector3(10, 0, 0) * 100f);
                stoneRigidBody.mass = 100;
                //playerPostion + new Vector3(-40, 10, -50);
            }
            isStone = false;

        }
        private void GetInput()
        {
         
            isBreaking = Input.GetKey(KeyCode.A);
        }
      
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "plane")
            {
                isGround = true;
            }
            else
            {
                isGround = false;
            }
            if (collision.gameObject.tag == "jump")
            {
                isJump = true;
                Debug.Log("isJump");
            }
            else
            {
                isJump = false;
            }
        }
        public void OnTriggerEnter(Collider collision)
        {
            if (collision.gameObject.tag == "finish" && !isFinish && m_ObjectiveManager.AreAllObjectivesCompleted())
            {
                isFinish = true;
                isWinner = true;
                Debug.Log("isFinish True");
            }

            if (collision.gameObject.tag == "stone")
            {
                isStone = true;
                Debug.Log("isStone " + isStone);
            }
            else
            {
                isStone = false;
            }
        }
    }
}
