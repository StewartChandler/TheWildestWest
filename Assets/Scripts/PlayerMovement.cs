using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    private HatStack hatStack;

    public float playerSpeed = 0.4f;

    public Health _healthScript;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        _healthScript = GameObject.FindObjectOfType<Health>();

        hatStack = GetComponentInChildren<HatStack>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        move *= Time.fixedDeltaTime * playerSpeed;
        controller.Move(move);

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            SceneManager.LoadScene("EndScene");
        }

        if (_healthScript.health <= 0.01f)
        {
            SceneManager.LoadScene("EndScene");
        }
    }

    public void takeDamage()
    {
        if (hatStack.getNumHats() > 0)
        {
            hatStack.popHat();
        } else
        {
            SceneManager.LoadScene("EndScene");
        }
    }
}
