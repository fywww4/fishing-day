using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_move : MonoBehaviour
{
    float moveForward;//Player's move forward
    float moveSide;//Player's move bay
    float jump;//Player's jump
    public float speed = 5f;//Player's movement speed
    Rigidbody rb;//we need rigibody component when we jump

    public float jumpspeed = 10f;//how much force do you want when you jump
    bool isGrounded;//player is on the ground or not

    void Start()
    {
        rb = GetComponent<Rigidbody>();//where do we get that rigibody component? 
    }

    // Update is called once per frame
    void Update()
    {
        moveForward = Input.GetAxis("Vertical") * speed;//move forward mean character will move on X axis 
        moveSide = Input.GetAxis("Horizontal") * speed;//move forward mean character will move on Y axis 
        jump = Input.GetAxis("Jump") * jumpspeed;//jump mean character will move on Y axis with force from rigibody 
        rb.velocity = (transform.forward * moveForward) + (transform.right * moveSide) + (transform.up * rb.velocity.y);//when we move bay,forward and jump we need velocity that velocity we get from rigibody 
        //that's why we need rigibody ok lar?

        if (isGrounded && jump != 0)//when character jump 
        {
            rb.AddForce(transform.up * jump, ForceMode.VelocityChange);//rigibody will give Force on Y axis like arrrrr.. 
            isGrounded = false;//when character jump he is not on the ground right he is in the air so isG is false 
        }
    }

    private void OnCollisionEnter(Collision collision)//how do we know character is on the ground?
    {
        if (collision.gameObject.tag == "Ground")//if he arrive from jump or something he is on the gameobject with tag (ground) 
            isGrounded = true;// we know he is on the ground 
    }

    private void OnCollisionExit(Collision collision)//hey look closer different with upper line 
    {
        if (collision.gameObject.tag == "Ground")//that line mean is mostly the same with line 33
            isGrounded = false;
    }
}
