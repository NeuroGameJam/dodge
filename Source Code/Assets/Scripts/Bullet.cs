using UnityEngine;

public class Bullet : MonoBehaviour {

    private Rigidbody   m_rbody;
    private LineRenderer m_line;

    [SerializeField]
    private float       m_speed = 5f;

    [SerializeField]
    private float       m_lifeTime = 5f;
    
	void Start ()
    {
        m_rbody = GetComponent<Rigidbody>();
        Destroy(transform.parent.gameObject, m_lifeTime);
    }
	
	
	void Update ()
    {
        m_rbody.AddForce(transform.parent.forward * m_speed);
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            other.GetComponentInParent<PlayerController>().ProcessHit();
            Destroy(transform.parent.gameObject);
        }
    }
}
