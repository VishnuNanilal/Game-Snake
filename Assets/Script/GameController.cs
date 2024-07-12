using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public float snakeSpeed = 1f;

    public Sprite tailSprite = null;
    public Sprite bodySprite = null;

    public BodyPart bodyPrefab = null;
    public GameObject rockPrefab;
    public GameObject eggPrefab;
    public GameObject goldenEggPrefab;
    public GameObject spikePrefab;

    public SnakeHead snakeHead = null;

    const float width = 3.7f;
    const float height = 7f;

    public bool isAlive = true;
    public bool waitingToPlay = true;

    List<Egg> eggs = new List<Egg>();
    List<GameObject> spikes = new List<GameObject>();

    int level = 0;
    int noOfEggsForNextLevel = 0;

    int score = 0;
    int highScore = 0;

    public Text scoreText;
    public Text highScoreText;
    public Text gameOverText;
    public Text tapToPlayText;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            Debug.LogError("[GameController] Trying Multiple instantiation.");
        }

        CreateWalls();
        isAlive = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(waitingToPlay)
        {
            foreach(Touch touch in Input.touches)
            {
                if(touch.phase==TouchPhase.Ended)
                {
                    StartGamePlay();
                }

            }

            if(Input.GetMouseButtonDown(0))
            {
                StartGamePlay();
            }
        }
    }

    void CreateWalls()
    {
        float z = -1;
        Vector3 start=new Vector3(-width,-height,z);
        Vector3 finish=new Vector3(-width ,+height, z);
        CreateWall(start, finish);

        start = new Vector3(-width, +height, z);
        finish = new Vector3(+width, +height, z);
        CreateWall(start, finish);

        start = new Vector3(+width, +height, z);
        finish = new Vector3(+width, -height, z);
        CreateWall(start, finish);

        start = new Vector3(-width, -height, z);
        finish = new Vector3(+width, -height, z);
        CreateWall(start, finish);

    }

    void CreateWall(Vector3 start, Vector3 finish)
    {
        float distance = Vector3.Distance(start,finish);
        int noOfRocks = (int)(3 * distance);
        Vector3 delta = (finish-start)/ noOfRocks;

        Vector3 position=start;
        for(int rock=0;rock<=noOfRocks;rock++)
        {
            float rotation = Random.Range(0, 360);
            float scale = Random.Range(1.5f, 2f);
            CreateRock(position, rotation, scale);
            position += delta;
        }
    }

    void CreateRock(Vector3 position, float rotation, float scale)
    {
        GameObject rock = Instantiate(rockPrefab, position, Quaternion.Euler(0,0,rotation));
        rock.transform.localScale=new Vector3(scale, scale, 1);
    }

    public void CreateEgg(bool golden=false)
    {
        Vector3 position;
        position.x=-width+Random.Range(1,(2*width)-2f);
        position.y = -height + Random.Range(1, (2 * height) - 2f);
        position.z = -1 ;

        Egg egg = null;
        if(golden)
            egg=Instantiate(goldenEggPrefab, position, Quaternion.identity).GetComponent<Egg>();
        else
            egg=Instantiate(eggPrefab, position, Quaternion.identity).GetComponent<Egg>();

        eggs.Add(egg);
    }

    public void CreateSpike()
    {
        for(int i=0;i<level;i++)
        {
            Vector3 position;
            position.x = -width + Random.Range(1, (2 * width) - 2f);
            position.y = -height + Random.Range(1, (2 * height) - 2f);
            position.z = -1;

            GameObject spike=Instantiate(spikePrefab, position, Quaternion.identity);
            spikes.Add(spike);
        }
    }
        

    public void GameOver()
    {
        isAlive = false;
        waitingToPlay = true;

        gameOverText.gameObject.SetActive(true);
        tapToPlayText.gameObject.SetActive(true);
    }

    void StartGamePlay()
    {
        score = 0;
        level = 0;

        scoreText.text = "Score = " + score;
        highScoreText.text = "HighScore = " + highScore;

        gameOverText.gameObject.SetActive(false);
        tapToPlayText.gameObject.SetActive(false);
        
        waitingToPlay = false;
        isAlive = true;
        KillOldEggs();
        LevelUp();
        
    }

    void LevelUp()
    {
        level++;
        noOfEggsForNextLevel = 4 + (level * 2);

        snakeSpeed = 1f + (level/4f);
        if (snakeSpeed > 6) snakeSpeed = 6;

        snakeHead.ResetSnake();
        CreateEgg();
        KillOldSpikes();
        CreateSpike();
    }

    public void EggEaten(Egg egg)
    {
        score++;
        Destroy(egg.gameObject);
        eggs.Remove(egg);

        noOfEggsForNextLevel--;
        if (noOfEggsForNextLevel == 0)
        {
            score += 10;
            LevelUp();
        }
        //
        else if (noOfEggsForNextLevel == 1) //last egg
        {
            CreateEgg(true);
        }
        else
        {
            CreateEgg(false);
        }

        scoreText.text = "Score = " + score;

        if (score > highScore)
        {
            highScore = score;
            highScoreText.text = "HighScore = " + highScore;
        }

    }

    void KillOldEggs() //To clear any present egg on scene while resetting game (either by level up or death)
    {
        foreach(Egg egg in eggs)
        {
            Destroy(egg.gameObject);
        }

        eggs.Clear();
    }

    void KillOldSpikes()
    {
        foreach(GameObject spike in spikes)
        {
            Destroy(spike);
        }

        spikes.Clear();
    }
}
