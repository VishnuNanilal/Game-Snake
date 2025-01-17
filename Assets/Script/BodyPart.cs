using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPart : MonoBehaviour
{
    Vector2 dPosition;
    public BodyPart following=null;

    SpriteRenderer spriteRenderer = null;

    const int POSITIONSREMEMBERED = 10;
    public Vector3[] previousPositions = new Vector3[POSITIONSREMEMBERED];

    public int setIndex = 0;
    public int getIndex = -(POSITIONSREMEMBERED - 1);


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (!GameController.instance.isAlive)
            return;

        Vector3 followPosition;
        if(following!=null)//For Body and not Head.
        {
            if(following.getIndex>-1)
            {
                followPosition = following.previousPositions[following.getIndex];
            }
            else
            {
                followPosition = following.transform.position;
            }
        }
        else //For Head.
        {
            followPosition = gameObject.transform.position;
        }

        previousPositions[setIndex].x = gameObject.transform.position.x;
        previousPositions[setIndex].y = gameObject.transform.position.y;
        previousPositions[setIndex].z = gameObject.transform.position.z;

        setIndex++;
        if (setIndex >= POSITIONSREMEMBERED)
            setIndex = 0;

        getIndex++;
        if (getIndex >= POSITIONSREMEMBERED)
            getIndex = 0;

        if (following != null)
        {
            Vector3 newPosition;
            if (following.getIndex > -1)
            {
                newPosition = followPosition;
            }
            else
            {
                newPosition = following.transform.position;
            }

            newPosition.z = newPosition.z + .01f;

            SetMovement(newPosition - gameObject.transform.position);
            UpdateDirection();
            UpdatePosition();

        }
    }



    public void SetMovement(Vector2 movement)
    {
        dPosition = movement;
    }
    
    public void UpdatePosition()
    {
        gameObject.transform.position += (Vector3)dPosition;
    }

    public void UpdateDirection()
    {
        if(dPosition.y>0)//up
        {
            gameObject.transform.localEulerAngles = new Vector3(0, 0, 0);
        }
        else if(dPosition.y<0)//down
        {
            gameObject.transform.localEulerAngles = new Vector3(0, 0, 180);
        }
        else if(dPosition.x>0)//right
        {
            gameObject.transform.localEulerAngles = new Vector3(0, 0, -90);
        }
        else if(dPosition.x<0)//left
        {
            gameObject.transform.localEulerAngles = new Vector3(0, 0, 90);
        }
    }

    public void TurnIntoTail()
    {
        spriteRenderer.sprite = GameController.instance.tailSprite;
    }
    public void TurnIntoBodyPart()
    {
        spriteRenderer.sprite = GameController.instance.bodySprite;
    }

    public void ResetMemory()
    {
        setIndex = 0;
        getIndex = -(POSITIONSREMEMBERED - 1);
    }
}
