using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


namespace KartGame.KartSystems
{
    public class RocketAnimation : MonoBehaviourPun
    {
        private KartCollisionManager collisionManager;
        public float degreePerSecond;

        // Start is called before the first frame update
        void Start()
        {
            collisionManager = GetComponent<KartCollisionManager>();
        }

        // Update is called once per frame
        void Update()
        {
            if (collisionManager.hitRocket)
            {
                if (Time.time - collisionManager.hitRocketTime < 1.0f)
                {
                    int height = 12;
                    gameObject.transform.position += (Vector3.up * height * Time.deltaTime);
                    transform.Rotate(Vector3.up * Time.deltaTime * degreePerSecond);
                    transform.Rotate(Vector3.right * Time.deltaTime * degreePerSecond);
                }
                else
                {
                    Debug.Log("end hit rocket");
                    collisionManager.hitRocket = false;
                }
            }
        }
    }
}
