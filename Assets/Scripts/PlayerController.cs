using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Animator characterAnimator;
    public Rigidbody2D rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        characterAnimator.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A))
        {
            if (!characterAnimator.enabled)
            {
                characterAnimator.enabled = true;
            }
        }
        else
        {
            if (characterAnimator.enabled)
            {
                characterAnimator.enabled = false;
            }
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(0.03f, 0, 0);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(-0.03f, 0, 0);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            rigidbody.AddForce(new Vector2(0, 260.0f));
        }
    }
}
