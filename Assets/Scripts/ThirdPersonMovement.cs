using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;

    public GameObject cuboVeneno;
    public GameObject cuboAntidoto;
    public GameObject balloff;
    public GameObject barrier;



    public float passForce = 900f;
    public float range = 50f;

    public float speed;
    float speedf;



    bool poison = false;

    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    Vector3 velocity;

    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    bool isGrounded;

    //public GameObject ball;

    public GameObject torso;

    public GameObject RingR;
    public GameObject RingB;

    bool haveBall = false;

    Vector3 moveDir;
    private ThirdPersonMovement[] allOtherPlayers;
    private Ball ball;

    private void Awake()
    {
        allOtherPlayers = FindObjectsOfType<ThirdPersonMovement>().Where(t => t != this).ToArray();
        ball = FindObjectOfType<Ball>();

    }

    private void Start()
    {

        speedf = speed;
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {

        StatePoison();

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        //Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        if (gameObject.name.Equals("Personaje"))
        {
            Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            float targetAngle2 = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle2 = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle2, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle2, 0f);
            Vector3 moveDir2 = Quaternion.Euler(0f, targetAngle2, 0f) * Vector3.forward;

            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

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

            if (IHaveBall())
            {
                var targetPlayer = FindPlayerInDirection(moveDir2);
                if (targetPlayer != null)
                {
                    if (Input.GetButton("Fire1"))
                        PassBallToPlayer(targetPlayer);
                }
            }

        }


    }

    private void PassBallToPlayer(ThirdPersonMovement targetPlayer)
    {
        var direction = DirectionTo(targetPlayer);
        ball.transform.SetParent(null);
        ball.GetComponent<Rigidbody>().isKinematic = false;
        ball.GetComponent<Rigidbody>().AddForce(direction * passForce);
    }

    private Vector3 DirectionTo(ThirdPersonMovement player)
    {
        return Vector3.Normalize(player.transform.position - ball.transform.position);
    }

    private ThirdPersonMovement FindPlayerInDirection(Vector3 direction)
    {
        var closestAngle = allOtherPlayers
            .OrderBy(t => Vector3.Angle(direction, DirectionTo(t)))
            .FirstOrDefault();

        return closestAngle;

    }

    private bool IHaveBall()
    {
        return transform.childCount > 3;
    }

    private void Aim(Vector3 moveDir)
    {
        if (Input.GetMouseButtonDown(0) && haveBall)
        {
            RaycastHit hit;
            ball.GetComponent<Rigidbody>().isKinematic = false;
            if (Physics.Raycast(transform.position, moveDir, out hit, range))
            {
                Debug.Log(hit.collider.name);
                if (hit.collider.name.Equals("colisionAyudaR"))
                {
                    Vector3 dirRing = Vector3.Normalize(RingR.transform.position - ball.transform.position);
                    dirRing.y += 0.05f;
                    ball.transform.parent = null;
                    ball.GetComponent<Rigidbody>().AddForce(dirRing * passForce);
                    haveBall = false;
                }
            }
            else
            {
                moveDir.y = 0.5f;
                ball.transform.parent = null;
                ball.GetComponent<Rigidbody>().AddForce(moveDir * passForce);
                haveBall = false;
            }
        }
    }

    private void StatePoison()
    {

        if (poison)
        {

            speed = speed - 0.01f;
        }
        if (speed <= 0)
        {
            Destroy(this.gameObject);
        }

    }

    void barrierOn()
    {

        barrier.SetActive(true);

        Invoke("barrierOff", 5.0f);

    }

    void barrierOff()
    {
        barrier.SetActive(false);


    }





    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.name.Equals("ball"))
        {
            Debug.Log("Choco");
            haveBall = true;
            ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
            ball.GetComponent<Rigidbody>().isKinematic = true;
            ball.transform.SetParent(transform);

        }

        if (other.gameObject.name.Equals("Poison"))
        {
            Destroy(cuboVeneno);

            poison = true;


        }

        if (other.gameObject.name.Equals("Antidote"))
        {
            Destroy(cuboAntidoto);

            poison = false;
            speed = speedf;

        }

        if (other.gameObject.name.Equals("Balloff"))
        {
            Destroy(balloff);
            barrierOn();
        }

    }


    private void OnTriggerEnter(Collider other)
    {
        Ball ball = other.GetComponent<Ball>();
        if (ball != null)
        {
            ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
            ball.GetComponent<Rigidbody>().isKinematic = true;
            ball.transform.SetParent(transform);
        }
    }
}
