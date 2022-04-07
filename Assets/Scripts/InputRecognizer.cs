using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputRecognizer : MonoBehaviour
{
    [SerializeField] PlayerController PLAYER;
    [SerializeField] [Range (50, 600)] int radius;
    [SerializeField] [Range (50, 200)] int pressSensitivity;
    [SerializeField] [Range (0.3F, 2)] float pressInterval;
    [SerializeField] [Range (50, 200)] int doubleTapSensitivity;
    [SerializeField] [Range (0.15F, 1)] float doubleTapInterval;
    Vector2 start, end, swipe, doubleTapStart;
    float pressTime;
    float doubleTapTime;
    bool doubleTapClock;
    bool IsTapped; // For instant events.
    public bool isTapped { get {return IsTapped;} }
    bool IsDragging;
    public bool isDragging { get {return IsDragging;} }
    bool IsPressing;
    bool IsPressed;
    public bool isPressed { get {return IsPressed;} }
    bool IsDoubleTapped;
    public bool isDoubleTapped { get {return IsDoubleTapped;} }
    int SwipeMode;
    /*
        0: Idle  |  1: Up  |  2: Right  |  3: Down  |  4: Left
    */
    public int swipeMode { get {return SwipeMode;} }
    void Start()
    {
        if (radius == 0) radius = 200;
        if (pressSensitivity == 0) pressSensitivity = 100;
        if (pressInterval == 0) pressInterval = 1;
        if (doubleTapSensitivity == 0) doubleTapSensitivity = 100;
        if (doubleTapInterval == 0) doubleTapInterval = 0.5F;
        Application.targetFrameRate = 60;
    }
    void Update()
    {
        IsTapped = false;
        IsPressed = false;
        SwipeMode = 0;

        if (doubleTapTime > doubleTapInterval)
        {
            doubleTapTime = 0;
            doubleTapClock = false;
        }

        if (Input.GetMouseButtonDown(0))
        {
            IsTapped = true;
            IsDragging = true;
            IsPressing = true;
            start = Input.mousePosition;

            
            if (Vector2.Distance(doubleTapStart, start) > doubleTapSensitivity)
            {
                doubleTapStart = start;
                doubleTapClock = true;
                doubleTapTime = 0;
            }
            else if (doubleTapTime <= doubleTapInterval && doubleTapClock)
            {
                doubleTapTime = 0;
                doubleTapClock = false;
            }
            else doubleTapClock = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            end = Input.mousePosition;
            IsDragging = false;
            IsPressing = false;
            pressTime = 0;
            start = swipe = Vector2.zero;
        }

        if (IsDragging)
        {
            swipe = (Vector2) Input.mousePosition - start;

            if (swipe.magnitude > radius)
            {
                float x = swipe.x;
                float y = swipe.y;

                if (Mathf.Abs(x) >= Mathf.Abs(y))
                {
                    if (x > 0) PLAYER.SwipeRight();
                    else PLAYER.SwipeLeft();
                }
                else
                {
                    if (y > 0) PLAYER.Jump();
                    else if (!PLAYER.isRolling)
                    {
                        PLAYER.isRolling = true;
                        PLAYER.Invoke("RollStop", 0.7F);
                    }
                }

                IsDragging = false;
                start = swipe = Vector2.zero;
            }

            if (IsPressing)
            {
                pressTime += Time.deltaTime;

                if (Vector2.Distance(start, Input.mousePosition) > pressSensitivity)
                {
                    IsPressing = false;
                    pressTime = 0;
                }
                if (pressTime > pressInterval)
                {
                    IsPressing = false;
                    IsPressed = true;
                    pressTime = 0;
                }
            }
        }

        if(doubleTapClock) doubleTapTime += Time.deltaTime;
    }
}