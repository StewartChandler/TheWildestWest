using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : ThrowableObject
{

    public override void matchRotation()
    {
        transform.rotation = target.rotation * Quaternion.Euler(90f, 0, 0);
    }

    public override void throwObject(Vector3 dir, float throwingSpeed)
    {
        Debug.Log("Boomerang thrown");
        // lock the boomerang's rotation to only the y-axis and add rotation
        rb.constraints = RigidbodyConstraints.FreezeRotationY;
        float rotationSpeed = 100;
        //rb.angularVelocity = new Vector3(0, 0, 100);
        rb.angularVelocity = rotationSpeed * dir.normalized;

        base.throwObject(dir, throwingSpeed * 2);
    
    }
}
