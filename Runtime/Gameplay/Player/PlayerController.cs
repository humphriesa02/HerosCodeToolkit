using UnityEngine;
using UnityEngine.InputSystem;
/// <summary>
/// Based off a series by Jojik (https://www.youtube.com/@jojikYT)
/// 
/// A generic player controller that utilizes a state machine
/// </summary>
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [Header("Player Control")]
    [SerializeField] private PlayerValues playerValues;

    [Header("References")]
    public Transform focus;
    public Animator animator;

    // Static Values
    [HideInInspector] public CharacterController controller;
    [HideInInspector] public PlayerInput playerInput;
    [HideInInspector] public float gravityValue = -9.81f;
    [HideInInspector] public Vector3 playerVelocity;

    // State Machine
    private StateMachine movementSM;
    public GroundState groundState;
    public AirState jumpState;
    public LandingState landingState;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        if (animator == null)
        {
            // Assumes "visual" is a child
            animator = GetComponentInChildren<Animator>();
        }
        
        movementSM = new StateMachine();
        groundState = new GroundState(this, movementSM, playerValues);
        jumpState = new AirState(this, movementSM, playerValues);
        landingState = new LandingState(this, movementSM, playerValues);
    }

    void Start()
    {
        if (focus == null)
        {
            focus = Camera.main.transform;
        }

        movementSM.Initialize(groundState);
    }

    void Update()
    {
        movementSM.HandleInput();
        movementSM.LogicUpdate();
    }

    void LateUpdate()
    {
        movementSM.LateUpdate();
    }

    void FixedUpdate()
    {
        movementSM.PhysicsUpdate();
    }

    void OnGUI()
    {
        GUI.Label(new Rect(15, 15, 300, 100), movementSM.GetCurrentState().ToString());
    }
}
