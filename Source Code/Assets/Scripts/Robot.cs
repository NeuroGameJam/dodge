using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Robot : MonoBehaviour
{
    private NavMeshAgent m_agent;

    [SerializeField]
    private Transform m_destination;

    [SerializeField]
    private Transform m_origin;
    
    private Transform m_enemyHead;
    
    private bool m_moving = false;
    Transform nextPoint;

    [SerializeField]
    private Transform m_spawnPoint;
    
    [SerializeField]
    private GameObject m_bulletPrefab;

    private Animator m_anim;

    private Transform m_body;
    
    [SerializeField]
    private float fireRate = 2f;
    
    bool goingToFight = false;
    public bool dead = false;
    
    void Awake ()
    {
        m_enemyHead = PlayerController.Instance.m_head.transform;
        m_agent = GetComponent<NavMeshAgent>();
        m_anim = GetComponent<Animator>();
        m_agent.enabled = false;
        m_body = transform.GetChild(0);
    }
    
    public void SetPositions(Transform origin, Transform destination)
    {
        m_origin = origin;
        m_destination = destination;

        transform.position = m_origin.position;
    }

    void OnParticleCollision(GameObject other)
    {
        if (!dead)
        {
            dead = true;
            CancelInvoke("StartShooting");
            m_anim.SetTrigger("Die");
            GetComponent<Collider>().enabled = false;
            m_agent.SetDestination(transform.position);
        }
        
    }

    void Update ()
    {
        if (!dead)
        {
            if (m_moving)
            {
                m_body.LookAt(nextPoint);
                if (Vector3.Distance(transform.position, nextPoint.position) <= 0.5f)
                {
                    m_moving = false;
                    transform.position = new Vector3(nextPoint.position.x, transform.position.y, nextPoint.position.z);

                    if (goingToFight)
                    {
                       // GameManager.Instance.StartTimer();
                       
                       Invoke("StartShooting",Random.Range(0,2));
                    }
                    else
                    {
                        Destroy(gameObject);
                    }
                }
            }
            else
            {
                m_body.LookAt(m_enemyHead);
            }
        }
        
	}
    
    public void GoToDestination()
    {
        if (dead)
            return;
        m_agent.enabled = true;
        m_moving = true;
        nextPoint = m_destination;
        m_agent.SetDestination(nextPoint.position);
        goingToFight = true;
    }

    public void GoToOrigin()
    {
        if (dead)
            return;
        m_agent.enabled = true;
        m_moving = true;
        nextPoint = m_origin;
        m_agent.SetDestination(nextPoint.position);
        goingToFight = false;
        CancelInvoke("StartShooting");
    }

    void StartShooting()
    {
        m_anim.SetTrigger("Shoot");
    }

    public void SpawnBullet()
    {
        GameObject bullet;

        if (!m_moving && !dead)
            bullet = Instantiate(m_bulletPrefab, m_spawnPoint.position, m_spawnPoint.rotation, GameManager.Instance.bulletHolder);
    }

    public void FinishShooting()
    {
        if(!m_moving && goingToFight)
            Invoke("StartShooting", fireRate);
    }
}
