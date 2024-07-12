using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeControls : MonoBehaviour
{
    public Vector2 swipeStart;
    public Vector2 swipeEnd;

    float minimumDistance = 10f;

    public static event System.Action<SwipeDirections> OnSwipe = delegate { };
    public enum SwipeDirections
    {
        UP,
        DOWN,
        LEFT,
        RIGHT
    };

    private void Update()
    {
        foreach(Touch touch in Input.touches) //Input.touches are an array of all touched in the last frame from Input class.
        {
            if(touch.phase==TouchPhase.Began)
            {
                swipeStart = touch.position;
            }
            else if(touch.phase==TouchPhase.Ended)
            {
                swipeEnd = touch.position;
                ProcessSwipe();
            }
        }

        if(Input.GetMouseButtonDown(0))
        {
            swipeStart = Input.mousePosition;
        }
        else if(Input.GetMouseButtonUp(0))
        {
            swipeEnd = Input.mousePosition;
            ProcessSwipe();
        }
    }

    private void ProcessSwipe()
    {
        float distance = Vector2.Distance(swipeStart, swipeEnd);
        if(distance>minimumDistance)
        {
            if(IsVerticalSwipe())
            {
                if(swipeStart.y>swipeEnd.y)
                {
                    OnSwipe(SwipeDirections.DOWN);//down
                }
                else
                {
                    OnSwipe(SwipeDirections.UP);//up
                }
            }
            else
            {
                if(swipeStart.x>swipeEnd.x)
                {
                    OnSwipe(SwipeDirections.LEFT);//left
                }
                else
                {
                    OnSwipe(SwipeDirections.RIGHT);//right
                }
            }    
        }
    }

    private bool IsVerticalSwipe()
    {
        float vertical = Mathf.Abs(swipeEnd.y - swipeStart.y);
        float horizontal = Mathf.Abs(swipeEnd.x - swipeStart.x);
        if (vertical > horizontal)
            return true;
        else
            return false;
    }
}
