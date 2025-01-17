using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeHead : BodyPart
{
    Vector2 movement;

    const float TIMETOADDBODYPARTS = .25f;
    float addTimer = TIMETOADDBODYPARTS;

    public int partsToAdd = 0;

    List<BodyPart> parts = new List<BodyPart>();

    private BodyPart tail = null;

    public AudioSource[] gulpSounds = new AudioSource[3];
    public AudioSource dieSound;
    void Start()
    {
        SwipeControls.OnSwipe += SwipeDetection;
    }

    // Update is called once per frame
    public override void Update()
    {
        if (!GameController.instance.isAlive)
            return;

        base.Update();
        SetMovement(movement* Time.deltaTime);
        UpdateDirection();
        UpdatePosition();

        if(partsToAdd>0)
        {
            addTimer -= Time.deltaTime;
            if(addTimer<=0)
            {
                addTimer = TIMETOADDBODYPARTS;
                AddBodyPart();
                partsToAdd --;
            }
        }
    }
    void AddBodyPart()
    {
        if(tail==null) //if there is no tail.
        {
            Vector3 newPosition = transform.position;
            //newPosition.y = newPosition.y - 3f;
            newPosition.z = newPosition.z + 0.01f;

            BodyPart newPart = Instantiate(GameController.instance.bodyPrefab, newPosition, Quaternion.identity);
            newPart.following = this;
            tail = newPart;
            newPart.TurnIntoTail();

            parts.Add(newPart);
        }
        else
        {
            Vector3 newPosition = tail.transform.position;
            //newPosition.y = newPosition.y - 3f;
            newPosition.z = newPosition.z + 0.01f;

            BodyPart newPart = Instantiate(GameController.instance.bodyPrefab, newPosition, tail.transform.rotation);
            newPart.following = tail;
            newPart.TurnIntoTail();
            tail.TurnIntoBodyPart();
            tail = newPart;

            parts.Add(newPart);
        }
    }
    void SwipeDetection(SwipeControls.SwipeDirections directions)
    {
        switch(directions)
        {
            case SwipeControls.SwipeDirections.UP:
                {
                    MoveUp();
                    break;
                }
            case SwipeControls.SwipeDirections.DOWN:
                {
                    MoveDown();
                    break;
                }
            case SwipeControls.SwipeDirections.RIGHT:
                {
                    MoveRight();
                    break;
                }
            case SwipeControls.SwipeDirections.LEFT:
                {
                    MoveLeft();
                    break;
                }
        }
    }

    void MoveUp()
    {
        movement = Vector2.up * GameController.instance.snakeSpeed;
    }
    void MoveDown()
    {
        movement = Vector2.down * GameController.instance.snakeSpeed;
    }
    void MoveRight()
    {
        movement = Vector2.right * GameController.instance.snakeSpeed;
    }
    void MoveLeft()
    {
        movement = Vector2.left * GameController.instance.snakeSpeed;
    }


    public void ResetSnake()
    {
        foreach (BodyPart part in parts)
        {
            Destroy(part.gameObject);
        }
        parts.Clear();

        tail = null;
        MoveUp();

        gameObject.transform.localEulerAngles = new Vector3(0, 0, 0);
        gameObject.transform.position = new Vector3(0, 0, -8);

        ResetMemory();

        partsToAdd = 5;
        addTimer = TIMETOADDBODYPARTS;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Egg egg = collision.GetComponent<Egg>();
        if (egg)
        {
            EatEgg(egg);
            int rand = UnityEngine.Random.Range(0,3);
            gulpSounds[rand].Play();
            Debug.Log("Hit Egg");
        }
        else
        {
            GameController.instance.GameOver();
            dieSound.Play();
            Debug.Log("Hit obstacle");
        }
    }

    private void EatEgg(Egg egg)
    {
        partsToAdd = 5;
        addTimer = 0;

        GameController.instance.EggEaten(egg);
    }
}
