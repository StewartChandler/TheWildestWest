using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float bulletSpeed = 3f;

    private Transform _playerTransform;

    private float _timeInterval = 3f;
    private float _timer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        _timer = Random.Range(0f, 3f);
    }

    // Update is called once per frame
    void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= _timeInterval)
        {
            ShootAtPlayer();
            _timer = 0f; 
        }

    }


    void ShootAtPlayer()
    {
        Vector3 playerPosition = _playerTransform.position;
        Vector3 enemyPosition = transform.position;

        Vector3 directionToPlayer = (playerPosition - enemyPosition).normalized;

        float angleToPlayer = Mathf.Atan2(directionToPlayer.x, directionToPlayer.z) * Mathf.Rad2Deg;

        Quaternion bulletRotation = Quaternion.Euler(90f, angleToPlayer, 0f);

        GameObject bullet = Instantiate(bulletPrefab, enemyPosition + directionToPlayer , bulletRotation);

        Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();

        bulletRigidbody.velocity = directionToPlayer * bulletSpeed;
    }
}
