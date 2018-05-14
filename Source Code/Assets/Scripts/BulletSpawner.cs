using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour {
    public static BulletSpawner Instance;
    [SerializeField]
    private Transform m_playerHead;
    [SerializeField]
    private GameObject m_bulletPrefab;

    [SerializeField]
    private float m_xRange;
    [SerializeField]
    private float m_zRange;

    [SerializeField]
    private float m_minDistance;

    private float m_minX;
    private float m_maxX;

    private float m_minZ;
    private float m_maxZ;

    [SerializeField]
    private float m_spawnRate = 3f;

    private float m_timer = 0;

    private bool m_spawning = false;

    void Start ()
    {
        Instance = this;
        m_minZ = m_playerHead.position.z + m_minDistance;
        m_maxZ = m_minZ + m_zRange;

        m_minX = m_playerHead.position.x - m_xRange / 2;
        m_maxX = m_minX + m_xRange;

        
    }
	
    public void StartSpawning()
    {
        SpawnBullet();
        m_spawning = true;
    }
	
	void Update ()
    {
        if(m_spawning)
        {
            m_timer += Time.fixedUnscaledDeltaTime;
            if (m_timer >= m_spawnRate)
            {
                SpawnBullet();
                m_timer = 0;
            }
        }
        

    }

    void SpawnBullet()
    {
        Vector3 bulletPosition = new Vector3(Random.Range(m_minX,m_maxX), m_playerHead.position.y ,Random.Range(m_minZ, m_maxZ));
        GameObject bullet = Instantiate(m_bulletPrefab, bulletPosition, Quaternion.identity, transform);

        bullet.transform.LookAt(m_playerHead);
    }
}
