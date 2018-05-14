using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerController : MonoBehaviour {
    public static PlayerController Instance;
    [SerializeField]
    private Canvas m_uiCanvas;

    [SerializeField]
    private Image[] m_livesUI;

    [SerializeField]
    private Sprite m_lifeFull;

    [SerializeField]
    private Sprite m_lifeEmpty;
    
    public Camera m_head;
    
    
    [SerializeField]
    private float lifeGainInterval = 20f;

    private float timer = 0;

    private int m_currentLives;

    private ParticleSystem EMP;

    [SerializeField]
    private Animator buttonsUI;

    private bool canUseEMP = false;

    [SerializeField]
    private float EMPintervalWrong = 5;

    [SerializeField]
    private float EMPintervalCorrect = 15f;
    
    private float empTimer = 0;

    private float holdTimer = 0;

    void Start ()
    {
        EMP = GetComponentInChildren<ParticleSystem>();
        Instance = this;
        m_livesUI[0].gameObject.GetComponentInParent<CanvasGroup>().alpha = 0f;
        m_currentLives = m_livesUI.Length;
        
        UpdateLives();
        empTimer = EMPintervalCorrect;
    }

    

    void UpdateLives()
    {
        for (int i = 0; i < m_livesUI.Length; i++)
        {
            m_livesUI[i].sprite = m_lifeEmpty;
        }

        for (int i = 0; i < m_currentLives ; i++)
        {
            m_livesUI[i].sprite = m_lifeFull;
        }
    }
    bool done = false;
    bool charging = false;

	void Update ()
    {
        if (GameManager.Instance.m_gameStarted && !canUseEMP)
        {
            empTimer -= Time.unscaledDeltaTime;
            if (empTimer <= 0)
            {
                canUseEMP = true;
                buttonsUI.SetTrigger("On");
                holdTimer = 0;
            }
        }
        if(GameManager.Instance.m_gameStarted)
        {
            if (OVRInput.Get(OVRInput.Button.One) && OVRInput.Get(OVRInput.Button.Three))
            {
                charging = true;
                holdTimer += Time.unscaledDeltaTime;
            }

            if ((OVRInput.GetUp(OVRInput.Button.One) || OVRInput.GetUp(OVRInput.Button.Three)) && charging)
            {
                charging = false;
                canUseEMP = false;
                if (holdTimer >= GameManager.Instance.m_greenInterval.x && holdTimer <= GameManager.Instance.m_greenInterval.y)
                {
                    EMP.Play();
                    GameManager.Instance.CheckForDead();
                    buttonsUI.SetTrigger("Fade");
                    empTimer = EMPintervalCorrect;
                }
                else
                {
                    buttonsUI.SetTrigger("Wrong");
                    empTimer = EMPintervalWrong;
                }
                GameManager.Instance.AddEvent(holdTimer);
            }
        }


        if (Input.GetKeyDown(KeyCode.M))
        {
            EMP.Play();
            GameManager.Instance.CheckForDead();
        }
		if(m_uiCanvas.worldCamera != m_head)
            m_uiCanvas.worldCamera = m_head;

        if(GameManager.Instance.m_gameStarted && !done)
        {
            m_livesUI[0].gameObject.GetComponentInParent<CanvasGroup>().alpha = 1f;
            
            done = true;
        }

        timer += Time.fixedUnscaledDeltaTime;
        if (timer >= lifeGainInterval)
        {
            timer = 0;
            m_currentLives++;
            m_currentLives = Mathf.Clamp(m_currentLives, 0, 5);

        }
    }

    public void ProcessHit()
    {
        m_currentLives--;
        timer = 0;
        // m_livesUI.text = "Lives: " + m_currentLives;
        UpdateLives();
        if (m_currentLives <= 0)
            GameManager.Instance.FinishGame();
    }
}
