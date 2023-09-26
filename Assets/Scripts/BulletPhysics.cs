using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPhysics : MonoBehaviour
{
    public float bulletDespawnTime = 10f;
    private float _bulletDespawnTimer;

    private Rigidbody _playerRigidbody;
    private PlayerMovement _playerMovement;
    private Rigidbody _bulletRigidbody;
    public Health _healthScript;
    

    // Start is called before the first frame update
    void Start()
    {
        _bulletDespawnTimer = bulletDespawnTime;
        _playerRigidbody = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
        _playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        _bulletRigidbody = GetComponent<Rigidbody>();
        _healthScript = GameObject.FindObjectOfType<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        _bulletDespawnTimer -= Time.deltaTime;
        // Debug.Log(_bulletDespawnTimer);
       
        if (_bulletDespawnTimer <= 0)
        {
            Destroy(gameObject);
        }

        if (_bulletRigidbody.velocity.magnitude <= 1f)
        {
            _bulletRigidbody.useGravity = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
            // _healthScript.health -= 0.2f;

            _playerMovement.takeDamage();
        }
    }
}
