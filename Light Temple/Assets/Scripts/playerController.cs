using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour {

    [Header("GameObjects")]
    public GameObject cameraPivot;

    [Header("Restrictions")]
    public bool allowJumping;
    public bool allowAttack;

    [Header("Speed Configuration")]
    public float verticalSpeed;
    public float horizontalSpeed;
    public float rotatingSpeed;


    //PRIVATE
    private Rigidbody rb;
    private Animator anim;
    private CameraController cameraController;
    private Vector3 movementDirection;
    private Vector3 currentLookAtPosition;


    void Start ()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        cameraController = cameraPivot.GetComponent<CameraController>();
        currentLookAtPosition = transform.forward;
    }

	void FixedUpdate ()
    {
        Walk();
        Rotate();
        if(allowJumping)
            Jump();
        if (allowAttack)
            Attack();
    }

    private void Walk()
    {
        float verticalMovement = Input.GetAxis("Vertical") * verticalSpeed * Time.fixedDeltaTime;
        float horizontalMovement = Input.GetAxis("Horizontal") * horizontalSpeed * Time.fixedDeltaTime;

        Vector3 movementDirection = new Vector3(
            horizontalMovement,
            0.0f,
            verticalMovement);

        //The float gets a value between -1 and 1 depending if he is walking forwards or backwards.
        anim.SetFloat("WalkVerticalSpeed", PositiveNegativeMagnitude(Vector3.Scale(new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical")), transform.forward)));

        if (movementDirection != Vector3.zero)
        {
            anim.SetBool("Walk", true);
        }
        else
        {
            anim.SetBool("Walk", false);
        }

        transform.position += cameraController.GetCurrentRotation() * movementDirection;
    }

    private void Rotate()
    {
        //Getting mouse position
        Vector3 auxPosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);   
        Vector3 mousePosition = new Vector3(auxPosition.x - 0.5f, transform.position.y, auxPosition.y - 0.5f);
        
        //Rotating the player towards the mouse
        currentLookAtPosition = Vector3.Lerp(currentLookAtPosition, transform.position + cameraController.GetCurrentRotation() * mousePosition, rotatingSpeed * Time.fixedDeltaTime);
        transform.LookAt(currentLookAtPosition);

        //Freezing x & z rotations to 0.
        transform.rotation = Quaternion.Euler(0.0f, transform.rotation.eulerAngles.y, 0.0f);
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            anim.SetTrigger("Jump");
        }
    }

    private void Attack()
    {
        if (Input.GetButtonDown("Attack1"))
        {
            anim.SetTrigger("Attack01");
        }
        else
        {
            if (Input.GetButtonDown("Attack2"))
            {
                anim.SetTrigger("Attack02");
            }
        }
    }

    /// <summary>
    /// Returns the vector3 lenght in a positive/negative form to indicate the direction the vector is going.
    /// </summary>
    /// <param name="vector3"></param>
    /// <returns></returns>
    private float PositiveNegativeMagnitude(Vector3 vector3)
    {
        float result = (Mathf.Sqrt(Mathf.Pow(vector3.x, 2) + 0.0f + Mathf.Pow(vector3.z, 2)));
        if(vector3.x < 0 && Mathf.Abs(vector3.x) > Mathf.Abs(vector3.z))
        {
            result *= -1;
        }
        if(vector3.z < 0 && Mathf.Abs(vector3.z) > Mathf.Abs(vector3.x))
        {
            result *= -1;
        }
        return result;
    }
}
