using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputActionAsset inputActions;
    private InputAction i_moveAction;
    private InputAction i_aAction;
    private InputAction i_bAction;

    Rigidbody2D rb;
    Animator ani;
    public PanningMiniGame panningGame;

    Vector2 moveInput = new Vector2 (0, 0);
    public float moveSpeed = 5f;

    public bool isPanning = false;
    public bool canPan = false;
    internal GameObject panningSpot;
    internal Coroutine panning;

    private void Awake()
    {
        inputActions.FindActionMap("Gameboy ActionMap").Enable();
        i_moveAction = inputActions.FindAction("Move");
        i_aAction = inputActions.FindAction("A Button");
        i_bAction = inputActions.FindAction("B Button");

        rb = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
    }
    void Start()
    {
        panningGame.gameObject.SetActive(false);
    }

    void Update()
    {
        // Get WASD input, if not panning
        if (!isPanning)
        {
            moveInput = i_moveAction.ReadValue<Vector2>();
        }

        // "A button" action
        if (i_aAction.WasPressedThisFrame())
        {
            if(!isPanning && canPan)
            {
                // Start panning
                isPanning = true;

                //Debug.Log("PlayerController - call start panning");
                panningGame.gameObject.SetActive(true);
                panningGame.StartPanning(panningSpot);
            }
            else if (isPanning)
            {
                if (panning == null)
                {
                    Debug.Log("PlayerController - attempt to pan");
                    panning = StartCoroutine(panningGame.AttemptToPan());
                }
                else
                {
                    Debug.Log("PlayerController - already panning");
                }
            }
        }
        
        // "B button" makes player sprint
        if (i_bAction.WasPressedThisFrame())
        {
            // Stop panning
            isPanning = false;

            moveSpeed = 7f;
        }
        else if (i_bAction.WasReleasedThisFrame())
        {
            moveSpeed = 5f;
        }
    }
    private void FixedUpdate()
    {
        // Apply animations
        ani.SetFloat("moveX", moveInput.x);
        ani.SetFloat("moveY", moveInput.y);

        // Move if player is WASD
        if (rb != null && moveInput != Vector2.zero)
        {
            transform.Translate(moveInput * moveSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PanningSpot"))
        {
            if (collision.GetComponent<Animator>().GetBool("canPan"))
            {
                canPan = true;
                panningSpot = collision.gameObject;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("PanningSpot"))
        {
            canPan = false;
            panningSpot = null;
        }
    }
}
