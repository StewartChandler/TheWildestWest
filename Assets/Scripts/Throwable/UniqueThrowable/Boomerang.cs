using UnityEngine;

public class Boomerang : ThrowableObject

{
    Vector3 initialPosition;


    public override void matchRotation()
    {
        transform.rotation = target.rotation * Quaternion.Euler(90f, 0, 0);
    }

    public override void throwObject(Vector3 dir, float throwingSpeed)
    {
        Debug.Log("Boomerang thrown");
        // lock the boomerang's rotation to only the y-axis and add rotation
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
        float rotationSpeed = 100;
        //rb.angularVelocity = new Vector3(0, 0, 100);
        rb.angularVelocity =  rotationSpeed * dir.normalized;

        base.throwObject(dir, throwingSpeed - 15);
        initialPosition = transform.position;
        Invoke("ReturnBoomerang", 1.0f);
    }

    // Method to make the boomerang come back
    private void ReturnBoomerang()
    {
        Debug.Log("Boomerang returning");
        Vector3 returnDirection = -(transform.position - initialPosition).normalized;
        rb.velocity = (returnDirection * 15); // Adjust the throw force as needed.
        throwtime = 0;

    }

    // Override the resetState method to handle boomerang-specific behavior
    public override void resetState()
    {
        base.resetState();

        // Additional boomerang-specific behavior on reset
    }

    // Add any other boomerang-specific methods or overrides as needed
}
