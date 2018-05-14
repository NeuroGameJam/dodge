using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Round[] m_rounds;
    [SerializeField]
    private GameObject m_tutorial;
    public static GameManager Instance;

    [SerializeField]
    private Image m_keys;

    [SerializeField]
    private Sprite[] m_keySprites;
    
    public Vector2 m_greenInterval;
    
    public Vector2 m_redInterval;

    private float m_timerX;
    private float m_timerA;
    public bool m_gameStarted = false;

    private int m_currentRound = 1;
    private int m_currentWave = 1;
    
    [SerializeField]
    private Text m_roundInfo;
    bool timerOn = false;
    float timeGoal = 0f;
    float timer = 0;

    [SerializeField]
    private GameObject m_robotPrefab;

    [SerializeField]
    private Transform m_robotsHolder;

    public Transform bulletHolder;

    private float globalTimer;
    void Start()
    {
        robots = new List<Robot>();
        Instance = this;
        m_roundInfo.enabled = false;
    }

    bool charging = false;
    int streak = 0;
    public int neededStreak = 2;
    public ParticleSystem EMP;
    void CheckInputs()
    {
        if (OVRInput.Get(OVRInput.Button.One) && OVRInput.Get(OVRInput.Button.Three))
        {
            charging = true;
            m_timerA += Time.fixedUnscaledDeltaTime;
            if (m_timerA < m_redInterval.x || m_timerA > m_redInterval.y)
                m_keys.sprite = m_keySprites[0];
            else if ((m_timerA >= m_redInterval.x && m_timerA < m_greenInterval.x) || (m_timerA > m_greenInterval.y && m_timerA <= m_redInterval.y))
                m_keys.sprite = m_keySprites[1];
            else if (m_timerA >= m_greenInterval.x && m_timerA <= m_greenInterval.y)
                m_keys.sprite = m_keySprites[2];
        }

        if ((OVRInput.GetUp(OVRInput.Button.One) || OVRInput.GetUp(OVRInput.Button.Three)) && charging)
        {
            if (m_timerA >= m_greenInterval.x && m_timerA <= m_greenInterval.y)
            {
                streak++;
                EMP.Play();
            }
            else
                streak = 0;

            m_timerA = 0;
            charging = false;

            if(streak >= neededStreak)
            {
                m_keys.GetComponentInParent<CanvasGroup>().alpha = 0f;
                m_gameStarted = true;
                m_roundInfo.enabled = true;
                m_roundInfo.text = "Round " + m_currentRound;
                Invoke("StartGame", m_rounds[0].interval);
                m_tutorial.SetActive(false);
            }
        }

        
        /*if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger) && OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
        {
            m_keys.GetComponentInParent<CanvasGroup>().alpha = 0f;
            m_gameStarted = true;
            m_roundInfo.enabled = true;
            m_roundInfo.text = "Round " + m_currentRound;
            Invoke("StartGame", m_rounds[0].interval);
            m_tutorial.SetActive(false);
        }*/
        if(Input.GetKeyDown(KeyCode.Space))
        {
            m_roundInfo.enabled = true;
            m_roundInfo.text = "Round " + m_currentRound;
            Invoke("StartGame", m_rounds[0].interval);
            m_keys.GetComponentInParent<CanvasGroup>().alpha = 0f;
            m_tutorial.SetActive(false);
            m_gameStarted = true;
        }
    }
    public void CheckForDead()
    {
        bool alldead = true;
        foreach (Robot robot in robots)
        {
            if (robot.dead)
                alldead = false;
        }
        if(alldead)
        {
            timerOn = false;
            timer = 0;
            if (m_currentWave < m_rounds[m_currentRound - 1].waves.Length)
            {
                m_currentWave++;
                NextWave();
                skip = false;
            }
            else
            {
                if (m_currentRound < m_rounds.Length)
                {
                    Invoke("NextWave", m_rounds[m_currentRound - 1].interval);
                    m_currentRound++;
                    skip = true;
                    m_roundInfo.enabled = true;
                    m_roundInfo.text = "Round " + m_currentRound;
                    m_currentWave = 1;


                    int realRound = 0;
                    int realWave = 0;
                    if (m_currentWave == 1)
                    {
                        realRound = m_currentRound - 1;
                        realWave = m_rounds[realRound].waves.Length;
                    }
                    else
                    {
                        realRound = m_currentRound;
                        realWave = m_currentWave;
                    }

                    foreach (Robot robot in robots)
                    {

                        robot.GoToOrigin();
                    }
                    robots.Clear();

                }
                else
                {
                    FinishGame();
                }
            }
        }
    }

    List<Robot> robots;

    void Update()
    {
        if (!m_gameStarted)
        {
            CheckInputs();
        }
        else
        {
            globalTimer += Time.unscaledDeltaTime;

            if (Input.GetKeyDown(KeyCode.KeypadMinus))
                Time.timeScale -= 0.1f;
            else if (Input.GetKeyDown(KeyCode.KeypadPlus))
                Time.timeScale += 0.1f;
            else if (Input.GetKeyDown(KeyCode.KeypadDivide))
                Time.timeScale = 1f;
            if (timerOn)
            {
                timer += Time.unscaledDeltaTime;
                print(timer);
                if (timer >= timeGoal)
                {
                    timerOn = false;
                    timer = 0;
                    if (m_currentWave < m_rounds[m_currentRound -1].waves.Length)
                    {
                        m_currentWave++;
                        NextWave();
                        skip = false;
                    }
                    else
                    {
                        if (m_currentRound < m_rounds.Length)
                        {
                            Invoke("NextWave", m_rounds[m_currentRound -1].interval);
                            m_currentRound++;
                            skip = true;
                            m_roundInfo.enabled = true;
                            m_roundInfo.text = "Round " + m_currentRound;
                            m_currentWave = 1;


                            int realRound = 0;
                            int realWave = 0;
                            if (m_currentWave == 1)
                            {
                                realRound = m_currentRound - 1;
                                realWave = m_rounds[realRound].waves.Length;
                            }
                            else
                            {
                                realRound = m_currentRound;
                                realWave = m_currentWave;
                            }

                            foreach (Robot robot in robots)
                            {

                                robot.GoToOrigin();
                            }
                            robots.Clear();

                        }
                        else
                        {
                            FinishGame();
                        }
                    }
                }
            }
        }
    }

    bool skip = false;

    public void FinishGame()
    {
        Statistics.Instance.SaveToFile();
        print("FIM");
    }

    void StartGame()
    {
        m_roundInfo.enabled = false;
        //timerOn = true;
        StartTimer();
        timeGoal = m_rounds[m_currentRound -1].waves[m_currentWave -1].duration;
        for (int i = 0; i < m_rounds[m_currentRound -1 ].waves[m_currentWave -1].robots.Length; i++)
        {
            Robot ro = Instantiate(m_robotPrefab, m_robotsHolder).GetComponent<Robot>();
            ro.SetPositions(m_rounds[m_currentRound - 1].waves[m_currentWave - 1].robots[i].origin,
                m_rounds[m_currentRound - 1].waves[m_currentWave - 1].robots[i].destination);
            ro.GoToDestination();
            robots.Add(ro);
        }
    }

    public void AddEvent(float result)
    {
        Statistics.Instance.AddEvent(globalTimer, robots.Count, Time.timeScale, result, Color.white);
    }

    public void StartTimer()
    {
        if (!timerOn)
            timerOn = true;
    }

    void NextWave()
    {
        StartTimer();
        m_roundInfo.enabled = false;
        //timerOn = true;
        timeGoal = m_rounds[m_currentRound - 1].waves[m_currentWave - 1].duration;
        int realRound = 0;
        int realWave = 0;
        if (m_currentWave == 1)
        {
            realRound = m_currentRound - 1;
            realWave = m_rounds[realRound].waves.Length;
        }
        else
        {
            realRound = m_currentRound;
            realWave = m_currentWave;
        }
        
        if (!skip)
        {
            foreach(Robot robot in robots)
            {
                robot.GoToOrigin();
            }
            robots.Clear();
        }

        for (int i = 0; i < m_rounds[m_currentRound - 1].waves[m_currentWave - 1].robots.Length; i++)
        {
            //Robot ro = m_rounds[m_currentRound - 1].waves[m_currentWave - 1].robots[i];
            //ro.GoToDestination();

            Robot ro = Instantiate(m_robotPrefab, m_robotsHolder).GetComponent<Robot>();
            ro.SetPositions(m_rounds[m_currentRound - 1].waves[m_currentWave - 1].robots[i].origin,
                m_rounds[m_currentRound - 1].waves[m_currentWave - 1].robots[i].destination);
            ro.GoToDestination();
            robots.Add(ro);
        }

        Time.timeScale = m_rounds[m_currentRound - 1].timeScale;
    }

    public void Reset()
    {
        SceneManager.LoadScene(0);
    }
}

[System.Serializable]
public class Round
{
    public Wave[] waves;
    public float interval;
    public float timeScale;
}

[System.Serializable]
public class Wave
{
    public RobotPositions[] robots;
    public float duration;
}

[System.Serializable]
public class RobotPositions
{
    public Transform origin;
    public Transform destination;

}
