using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestController : MonoBehaviour
{
    // Start is called before the first frame update
    public CharacterController cha;
    public Vector3 Dir;
    public float speed;
    public float rotatespeed;
    public float swordspeed;
    public GameObject sword;
    public float swordangle;
    public bool isattack = false;
    void Start()
    {
        cha = this.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            Dir = Vector3.forward;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            Dir = Vector3.left;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            Dir = Vector3.back;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Dir = Vector3.right;
        }
        else
        {
            Dir.x = 0;
            Dir.z = 0;
        }
        if (!cha.isGrounded)
        {
            Dir.y = -10;
        }
        else Dir.y = 0;
        if (Input.GetAxis("Mouse X") != 0)
        {
            this.transform.localEulerAngles += Input.GetAxis("Mouse X") * Time.deltaTime * new Vector3(0, rotatespeed, 0);
        }
        if (Input.GetMouseButtonDown(0) && !isattack)
        {
            isattack = true;
            StartCoroutine(attack());
        }
        cha.Move(this.transform.TransformDirection(Dir * speed * Time.deltaTime));
    }

    IEnumerator attack()
    {
        while(swordangle < 90)
        {
            sword.transform.Rotate(new Vector3(swordspeed, 0, 0) * Time.deltaTime * speed * 5);
            swordangle += swordspeed * Time.deltaTime * speed * 5;
            yield return new WaitForSeconds(0.0005f);
        }
        while(swordangle > 0)
        {
            sword.transform.Rotate(new Vector3(-swordspeed, 0, 0) * Time.deltaTime * speed * 5);
            swordangle -= swordspeed * Time.deltaTime * speed * 5;
            yield return new WaitForSeconds(0.0005f);
        }
        isattack = false;
    }
}
