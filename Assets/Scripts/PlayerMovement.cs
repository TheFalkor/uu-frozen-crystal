using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public bool slidingLand = true;
    [Header("Player")]
    [SerializeField, Range(1f, 1000f)]
    float maxSpeed = 10f;
    [SerializeField, Range(1f, 1000f)]
    float maxAcceleration = 10f, maxAirAcceleration = 1f;
    [SerializeField, Range(1f, 3f)]
    float sprintModifier = 1.5f;
    [SerializeField, Range(2f, 10f)]
    float jumpHeight = 2f;
    [SerializeField, Range(0, 5)]
    int maxAirJumps = 1;
    [SerializeField, Range(0f, 90f)]
    float maxGroundAngle = 25f, maxStairsAngle = 50f;
    [SerializeField, Range(0f, 100f)]
    float maxSnapSpeed = 100f;
    [SerializeField, Min(0f)]
    float probeDistance = 1f;
    [SerializeField]
    LayerMask probeMask = -1;
    [SerializeField]
    LayerMask stairsMask = -1;
    [SerializeField]
    Transform playerInputSpace = default;

    public bool receiveInput = true;

    bool desiredJump;
    bool isSprinting;
    int jumpPhase;
    int groundContactCount;
    int stepsSinceLastGrounded;
    int stepsSinceLastJumped;
    int steepContactCount;
    float minGroundDotProduct;
    float minStairsDotProduct;
    Vector3 velocity;
    Vector3 desiredVelocity;
    Vector3 contactNormal;
    Vector3 steepNormal;
    Rigidbody body;
    Animator animator;
    private static bool alreadyExistsDebugShit = false;

    bool OnGround => groundContactCount > 0;
    bool OnSteep => steepContactCount > 0;

    void OnValidate()
    {
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
        minStairsDotProduct = Mathf.Cos(maxStairsAngle * Mathf.Deg2Rad);
    }

    void Awake()
    {
        if (alreadyExistsDebugShit)
        {
            Destroy(transform.parent.gameObject);
            return;
        }
        alreadyExistsDebugShit = true;
        transform.parent.rotation = Quaternion.Euler(0, 0, 0);



        body = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        OnValidate();

        Cursor.visible = false;
        Application.targetFrameRate = 120;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Vector2 playerInput = Vector2.zero;
        if (receiveInput)
        {
            playerInput.x = Input.GetAxis("Horizontal");
            playerInput.y = Input.GetAxis("Vertical");
        }
        playerInput = Vector2.ClampMagnitude(playerInput, 1f);
        Vector3 inputDirection = new Vector3(playerInput.x, 0f, playerInput.y);

        isSprinting = Input.GetButton("Sprint");

        animator.SetFloat("velocity", playerInput.magnitude);
        animator.SetBool("isSprinting", isSprinting);
        animator.SetBool("isJumping", !OnGround);
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (slidingLand)
        {
            if (stateInfo.IsName("BasicMotions@Jump01 - Land"))
            {
                playerInput *= 0.2f;
            }
        }     
        else
        {
            receiveInput = !stateInfo.IsName("BasicMotions@Jump01 - Land");
        }

        if (playerInputSpace)
        {
            Vector3 forward = playerInputSpace.forward;
            forward.y = 0f;
            forward.Normalize();
            Vector3 right = playerInputSpace.right;
            right.y= 0f;
            right.Normalize();
            desiredVelocity = (forward * playerInput.y + right * playerInput.x) * maxSpeed;
            desiredVelocity *= isSprinting ? sprintModifier : 1f;
            inputDirection = forward * inputDirection.z + right * inputDirection.x;
        }
        else
        {
            desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;
            desiredVelocity *= isSprinting ? sprintModifier : 1f;
        }
        desiredJump |= Input.GetButtonDown("Jump");

        if (inputDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(inputDirection);
        }
    }

    void FixedUpdate()
    {
        if (PauseManager.isPaused)
            return;

        UpdateState();
        AdjustVelocity();
        if (desiredJump)
        {
            desiredJump = false;
            Jump();
        }
        body.velocity = velocity;
        ClearState();
    }

    private void UpdateState()
    {
        stepsSinceLastGrounded++;
        stepsSinceLastJumped++;
        velocity = body.velocity;
        if (OnGround || SnapToGround() /*|| CheckSteepContacts()*/)
        {
            if (groundContactCount > 1)
                contactNormal.Normalize();
            stepsSinceLastGrounded = 0;
            if (stepsSinceLastJumped > 1)
                jumpPhase = 0;
        }
    }
    void AdjustVelocity()
    {
        Vector3 xAxis = ProjectOnContactPlane(Vector3.right).normalized;
        Vector3 zAxis = ProjectOnContactPlane(Vector3.forward).normalized;

        float currentX = Vector3.Dot(velocity, xAxis);
        float currentZ = Vector3.Dot(velocity, zAxis);

        float acceleration = OnGround ? maxAcceleration : maxAirAcceleration;
        float maxSpeedChange = isSprinting ? acceleration * Time.deltaTime * sprintModifier : acceleration * Time.deltaTime;
        float newX = Mathf.MoveTowards(currentX, desiredVelocity.x, maxSpeedChange);
        float newZ = Mathf.MoveTowards(currentZ, desiredVelocity.z, maxSpeedChange);

        velocity += xAxis * (newX - currentX) + zAxis * (newZ - currentZ);
    }

    private void ClearState()
    {
        groundContactCount = steepContactCount = 0;
        contactNormal = steepNormal = Vector3.zero;
    }

    private void Jump()
    {
        Vector3 jumpDirection;
        if (OnGround)
            jumpDirection = contactNormal;
        else if (maxAirJumps > 0 && jumpPhase <= maxAirJumps)
        {
            if(jumpPhase == 0)
                jumpPhase = 1;
            jumpDirection = Vector3.up;
        }
        else
            return;

        jumpDirection = (jumpDirection + Vector3.up).normalized;
        stepsSinceLastJumped = 0;
        jumpPhase++;
        float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
        float alignedSpeed = Vector3.Dot(velocity, jumpDirection);
        if (alignedSpeed > 0f)
            jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
        velocity += jumpDirection * jumpSpeed;
    }

    void OnCollisionEnter(Collision collision)
    {
        EvaluateCollision(collision);
    }

    void OnCollisionStay(Collision collision)
    {
        EvaluateCollision(collision);
    }

    void EvaluateCollision(Collision collision)
    {
        float minDot = GetMinDot(collision.gameObject.layer);
        for(int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;
            if(normal.y >= minDot)
            {
                groundContactCount++;
                contactNormal += normal;
            }
            else if(normal.y > -0.01f)
            {
                steepContactCount++;
                steepNormal += normal;
            }
        }
    }

    Vector3 ProjectOnContactPlane(Vector3 vector)
    {
        return vector - contactNormal * Vector3.Dot(vector, contactNormal);
    }

    bool CheckSteepContacts()
    {
        if(steepContactCount > 1)
        {
            steepNormal.Normalize();
            if(steepNormal.y >= minGroundDotProduct)
            {
                groundContactCount = 1;
                contactNormal = steepNormal;
                return true;
            }
        }
        return false;
    }

    bool SnapToGround()
    {
        if (stepsSinceLastGrounded > 1 || stepsSinceLastJumped <= 2)
            return false;
        float speed = velocity.magnitude;
        if (speed > maxSnapSpeed)
            return false;
        if (!Physics.Raycast(body.position, Vector3.down, out RaycastHit hit, probeDistance, probeMask))
            return false;
        if (hit.normal.y < GetMinDot(hit.collider.gameObject.layer))
            return false;

        groundContactCount = 1;
        contactNormal = hit.normal;
        float dot = Vector3.Dot(velocity, hit.normal);
        if(dot > 0f)
            velocity = (velocity - hit.normal * dot).normalized * speed;
        return true;
    }

    float GetMinDot(int layer)
    {
        return (stairsMask & (1 << layer)) == 0 ?
            minGroundDotProduct : minStairsDotProduct;
    }
}
