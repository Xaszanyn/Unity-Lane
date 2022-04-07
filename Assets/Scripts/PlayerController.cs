using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float left;
    [SerializeField] float middle;
    [SerializeField] float right;
    [SerializeField] float cameraLeft;
    [SerializeField] float cameraMiddle;
    [SerializeField] float cameraRight;
    [SerializeField] float speed;
    [SerializeField] float clingSensitivity;
    int lane;
    int destinationLane;
    bool isMoving;
    Vector3 destination;
    Vector3 cameraDestination;
    Camera C;
    int colorMode;
    [SerializeField] Material red;
    [SerializeField] Material blue;
    [SerializeField] Material green;
    public bool isRolling;
    [SerializeField] int jumpSpeed;
    void Start()
    {
        C = Camera.main;
        Application.targetFrameRate = 60;
        colorMode = 3;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) SwipeLeft(); // Somehow broken (;_;)
        else if (Input.GetKeyDown(KeyCode.D)) SwipeRight();
        else if (Input.GetKeyDown(KeyCode.S) && !isRolling) 
        {
            isRolling = true;
            Invoke("RollStop", 0.7F);
        }
        else if (Input.GetKeyDown(KeyCode.W)) Jump();

        if (isRolling) Roll();

        Move();
        PositionCheck();
    }

    void OnTriggerEnter(Collider other)
    {
        string tag = other.gameObject.tag;

        if (tag == "Obstacle")
        {
            SceneManager.LoadScene("GameOver");
        }
        else if (tag == "Red" && colorMode != 0)
        {
            SceneManager.LoadScene("GameOver");
        }
        else if (tag == "Green" && colorMode != 1)
        {
            SceneManager.LoadScene("GameOver");
        }
        else if (tag == "Blue" && colorMode != 2)
        {
            SceneManager.LoadScene("GameOver");
        } if (tag == "Rollable" && !isRolling)
        {
            SceneManager.LoadScene("GameOver");
        }
    }

    public void Red()
    {
        colorMode = 0;
        GetComponent<MeshRenderer>().material = red;
    }
    public void Green()
    {
        colorMode = 1;
        GetComponent<MeshRenderer>().material = green;
    }
    public void Blue()
    {
        colorMode = 2;
        GetComponent<MeshRenderer>().material = blue;
    }

    public void SwipeLeft()
    {
        isMoving = true;
        destinationLane = Mathf.Clamp(lane - 1, -1, 1);
        Destinator(destinationLane);
    }
    public void SwipeRight()
    {
        isMoving = true;
        destinationLane = Mathf.Clamp(lane + 1, -1, 1);
        Destinator(destinationLane);
    }
    void Destinator(int wantedLane)
    {
        if (wantedLane == -1)
        {
            destination = new Vector3(left, transform.position.y, transform.position.z);
            cameraDestination = new Vector3(cameraLeft, C.transform.position.y, C.transform.position.z);
        }
        else if (wantedLane == 0)
        {
            destination = new Vector3(middle, transform.position.y, transform.position.z);
            cameraDestination = new Vector3(cameraMiddle, C.transform.position.y, C.transform.position.z);
        }
        else if (wantedLane == 1)
        {
            destination = new Vector3(right, transform.position.y, transform.position.z);
            cameraDestination = new Vector3(cameraRight, C.transform.position.y, C.transform.position.z);
        }
    }

    void Move()
    {
        if (isMoving)
        {
            transform.position = Vector3.Lerp(transform.position, destination, speed);
            C.transform.position = Vector3.Lerp(C.transform.position, cameraDestination, speed * .8F);
        }
    }

    void PositionCheck()
    {
        if (isMoving)
        {
            float location = transform.position.x;
            
            if (location < middle) lane = -1;
            else if (location > middle) lane = 1;
            else lane = 0;

            bool middleCheck = (location < middle + clingSensitivity && location > middle - clingSensitivity);

            if(middleCheck) lane = 0;

            if ((location < left + clingSensitivity) ||
                (location > right - clingSensitivity) ||
                middleCheck)
            {
                isMoving = false;
                transform.position = destination;
                C.transform.position = cameraDestination;
            }
        }
    }

    public void Roll()
    {
        transform.Rotate(new Vector3(1, 0 ,0), Time.deltaTime * 500);
    }
    public void RollStop()
    {
        isRolling = false;
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    public void Jump()
    {
        GetComponent<Rigidbody>().AddForce(new Vector3(0, jumpSpeed, 0));
    }



}