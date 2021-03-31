using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;

    public float passForce = 900f;
    public float range = 50f;

    public float speed = 6f;

    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    Vector3 velocity;

    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    bool isGrounded;

    public GameObject ball;

    public GameObject torso;

    public GameObject RingR;
    public GameObject RingB;

    bool haveBall = false;

    Vector3 moveDir;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;


        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);

            Debug.DrawRay(transform.position, moveDir * range, Color.red);

            Aim(moveDir);
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    private void Aim(Vector3 moveDir)
    {
        if (Input.GetMouseButtonDown(0) && haveBall)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, moveDir, out hit, range))
            {
                Debug.Log(hit.collider.name);
                if (hit.collider.name.Equals("colisionAyudaR"))
                {
                    Vector3 dirRing = Vector3.Normalize(RingR.transform.position - ball.transform.position);
                    dirRing.y += 0.05f;
                    ball.GetComponent<Rigidbody>().AddForce(dirRing * passForce);
                    haveBall = false;
                }
            }
            else
            {
                moveDir.y = 0.5f;
                ball.GetComponent<Rigidbody>().AddForce(moveDir * passForce);
                haveBall = false;
            }
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.name.Equals("ball"))
        {
            Debug.Log("Choco");
            haveBall = true;
        }
        else 
        {
            haveBall = false;
        }
    }
}
