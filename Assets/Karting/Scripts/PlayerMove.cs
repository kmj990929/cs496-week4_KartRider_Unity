using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerMove : MonoBehaviour
{
    public string Horizontal = "Horizontal";
    public string Vertical = "Vertical";
    public float speed = 10.0F;
    public float rotationSpeed = 100.0F;
    public static bool isCollision = false;
    //public bool isSpacebarClick = false;
    //public ItemImageSet sn;
    //public override Vector2 GenerateInput() {
    //  return new Vector2 {
    //    x = Input.GetAxis(Horizontal),
    //   y = Input.GetAxis(Vertical)
    // };
    //}
    private void Start()
    {
        //GameObject imageObject = GameObject.FindGameObjectWithTag("itemImage");
        //if (imageObject != null)
        //{
        //    myimage = imageObject.GetComponent<Image>();
        //}
    }
    void Update()
    {
        float translation = Input.GetAxis(Vertical) * speed;
        float rotation = Input.GetAxis(Horizontal) * rotationSpeed;
        translation *= Time.deltaTime;
        rotation *= Time.deltaTime;
        transform.Translate(0, 0, translation);
        transform.Rotate(0, rotation, 0);
        //아이템 쏘기 - 스페이스바를 누르고 선택된 아이템 번호가 0, 1, 2 중 하나이면 
        bool space = Input.GetKeyDown(KeyCode.Space);
        if (space)
        {
            Debug.Log("Click");
        }
    }
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.tag == "item")
        {
            Destroy(collision.collider.gameObject);
            isCollision = true; 
        }
        else
        {
            isCollision = false;
        }
    }
}