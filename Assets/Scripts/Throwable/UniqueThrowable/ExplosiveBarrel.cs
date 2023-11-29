using UnityEngine;

public class ExplosiveBarrel : ThrowableObject
{
    public ParticleSystem explosionEffect;
    public float explosionForce = 10000f;
    public float explosionRadius = 50f;

    protected void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 position = contact.point;

        bool isThrown = state == State.Thrown;

        if (!isThrown) return;

        Explode();

        Instantiate(explosionEffect, position, rotation);

        BreakBarrel();
    }

    private void BreakBarrel()
    {
        Destroy(gameObject);
    }

    private void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (var obj in colliders) {

            Rigidbody rb = obj.GetComponent<Rigidbody>();

            if (rb == null) rb = obj.GetComponentInChildren<Rigidbody>();

            if (obj.transform.CompareTag("Player"))
            {
                Debug.Log("HIT PLAYERRRR");
                obj.transform.GetComponent<PlayerController>().takeDamage(target.position - obj.transform.position);
                //target = null;
            }

            if (rb != null)
            {
                Vector3 position = transform.position + Vector3.down * 1.5f;
                rb.useGravity = true;
                rb.AddExplosionForce(explosionForce, position, explosionRadius);
            }
        }
    }
}
