using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KartGame.KartSystems
{
    public class BananaAnimation : MonoBehaviour
    {
        private KartCollisionManager collisionManager;
        public float degreePerSecond;

        private float beforeMass;

        // Start is called before the first frame update

        public GameObject waterBall;
        Rigidbody rb;
        void Start()
        {
            collisionManager = GetComponent<KartCollisionManager>();
            rb = GetComponent<Rigidbody>();
            beforeMass = rb.mass;
        }

        // Update is called once per frame
        void Update()
        {
            if (collisionManager.bananaEffect)
            {
                if (Time.time - collisionManager.bananaEffectTime < 3.0f)
                {
                    Debug.Log("bananaEffect");
                    transform.Rotate(Vector3.up * Time.deltaTime * degreePerSecond);
                }
                else
                {
                    collisionManager.bananaEffect = false;
                }
            
            }

            if (collisionManager.WaterBalloonEffect)
            {
                if (Time.time - collisionManager.WaterBalloonEffectTime < 3.0f)
                {
                    //물풍선 이미지에 사람 공중에 가두기
                    //rb.useGravity = false;
                    //rb.mass = 0f;
                    Vector3 currentPosition = transform.position;
                    transform.position = currentPosition + (Vector3.up * 5.0F * Time.deltaTime);
                    //transform.position += (Vector3.up * 2.0F * Time.deltaTime);
                    Debug.Log(" transform.position : " + transform.position);
                    waterBall.SetActive(true);
                }
                else
                {
                    collisionManager.WaterBalloonEffect = false;
                    waterBall.SetActive(false);
                    //rb.useGravity = true;
                    //rb.mass = beforeMass;
                }
            }
        }
    }
}
