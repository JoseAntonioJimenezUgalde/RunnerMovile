using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.UI;



public class Movement : MonoBehaviour
{
    PlayerInput _playerInput;
    CharacterController _controller;
    Animator _animatorController;

    [Header("Shield")] 
    [SerializeField] private GameObject shield;
    [SerializeField] private Image imageShieldFillAmount;
    [SerializeField] private float shieldFillAmountTimer = 0f;
    [Tooltip("Velocidad de Descarga mientras más bajo más durará el efecto"), Range(0.001f, 0.2f)]
    [SerializeField] private float shieldFillSpeed = 0.5f;
    [Tooltip("Velocidad de LLenado de circleShieldFillAmount mientrás más alto más rápido se cargará"), Range(0.01f, 0.1f)]
    [SerializeField] private float shieldSumFillSpeed = 0.1f;
    [SerializeField] private Animator shieldAnimator;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private bool isMovement;
    [SerializeField] private Vector2 movement;
    [SerializeField] private bool isRunning;
    [SerializeField] private Image imageFillAmount;
    [SerializeField] private float runTimer = 10f;
    [SerializeField] private float currentSpeed;
    [SerializeField] private float time = 0;
    [SerializeField] private float fillAmountTimer = 0f;
    [SerializeField] private float fillSpeed = 0.5f; // Velocidad de llenado y descarga de imageFillAmount
    [SerializeField] private float sumFillSpeed = 0.1f; // Velocidad de llenado y descarga de imageFillAmount
    
    [SerializeField] private float limitX = 44f;
    [SerializeField] private float limitZ = 44f;



    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.5f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private bool isGrounded;


    [Header("Jump Settings")]
    [SerializeField] private float jumpHeight = 2;
    [SerializeField] private float maxJumpHeight;
    [SerializeField] private Vector3 jumpDirection; // Almacena la dirección del salto



    [Header("Enemy Detection")]
    [SerializeField] private float detectionRadius = 5f; // Radio de detección del área.
    [SerializeField] private LayerMask enemyLayer; // Máscara de capas que define los enemigos.
    [SerializeField] public Transform targetEnemy; // Referencia al enemigo detectado.

    [Header("Rotation Settings")]
    public float desiredRotationSpeed = 0.1f;
    private Vector3 velocity;

    [Header("Comunication Scripts")] 
    //private Shoot shootScript;
    [SerializeField] private int currentAmmo;


    [Header("Audio")] 
    private AudioSource jumpAudioSource;
    private AudioSource movementAudioSource;
    private AudioSource runAudioSource;
    [Range(0,1),Tooltip("Rango del Audio de 2D a 3D")]
    [SerializeField]private float spatialBlend;
    [SerializeField] private AudioClip jump; 
    [SerializeField] private AudioClip movementAudioClip;
    [SerializeField] private AudioClip runAudioClip;
    public InputActionReference movementAction;
    public AudioMixerGroup mixerGroup;

    [Header("Particle")] 
    [SerializeField] private ParticleSystem jumpParticle;
    [SerializeField] private ParticleSystem runningParticle;
    [SerializeField] private ParticleSystem levelUp;


    private bool isMoving;
    

    public bool IsMoving
    {
        get => isMoving;
    }

    public bool IsGround
    {
        get => isGrounded;
    }

    private void OnEnable()
    {
        movementAction.action.started += OnMovementStarted;
        movementAction.action.canceled += OnMovementCanceled;
    }

    private void OnDisable()
    {
        movementAction.action.started -= OnMovementStarted;
        movementAction.action.canceled -= OnMovementCanceled;
        
    }
  

    private void OnMovementStarted(InputAction.CallbackContext context)
    {
        isMoving = true;
    }
    
    private void OnMovementCanceled(InputAction.CallbackContext context)
    {
        isMoving = false;
        movementAudioSource.Stop();
        runAudioSource.Stop();


    }

    void Start()
    {
        _controller = GetComponent<CharacterController>();
        _playerInput = GetComponent<PlayerInput>();
        _animatorController = GetComponent<Animator>();

        AudioAssigation();
        
        

    }

    void Update()
    {
        
        
        HandleJump(); //Salto
        HandleGroundMovement(); //Movimiento
        Audio();
        //Distance();
        //HandleEnemyDetection(); //Detectar Enemy
        //ActivateShield(); // Animaciones y función del escudo

    }

    void Distance()
    {
        Vector3 newPosition = transform.position;
        newPosition.x = Mathf.Clamp(newPosition.x, -limitX, limitX);
        newPosition.z = Mathf.Clamp(newPosition.z, -limitZ, limitZ);
        transform.position = newPosition;
    }

    void AudioAssigation()
    {
        jumpAudioSource = gameObject.AddComponent<AudioSource>();
        jumpAudioSource.clip = jump;
        jumpAudioSource.spatialBlend = spatialBlend;
        jumpAudioSource.outputAudioMixerGroup = mixerGroup;

        movementAudioSource = gameObject.AddComponent<AudioSource>();
        movementAudioSource.clip = movementAudioClip;
        movementAudioSource.loop = true;
        movementAudioSource.spatialBlend = spatialBlend;
        movementAudioSource.outputAudioMixerGroup = mixerGroup;


        runAudioSource = gameObject.AddComponent<AudioSource>();
        runAudioSource.clip = runAudioClip;
        runAudioSource.loop = true;
        runAudioSource.spatialBlend = spatialBlend;
        runAudioSource.outputAudioMixerGroup = mixerGroup;

    }

    void Audio()
    {
        if (isMoving && isGrounded)
        {
            if (isRunning)
            {
                if (!runAudioSource.isPlaying)
                {
                    runAudioSource.Play();
                }
                movementAudioSource.Stop();
            }
            else
            {
                if (!movementAudioSource.isPlaying)
                {
                    movementAudioSource.Play();
                }
                runAudioSource.Stop();
            }

        }
        else
        {
            movementAudioSource.Stop();
            runAudioSource.Stop();
        }
    }
    
    void HandleGroundMovement()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask); // Comprobar si el personaje está en el suelo mediante un Raycast esférico.
        
        _animatorController.SetBool("isGround", isGrounded);

        // Si el personaje está en el suelo, reiniciar la velocidad vertical (eje Y) a un valor negativo pequeño para mantenerlo en el suelo.
        velocity.y = isGrounded && velocity.y < 0 ? -2 : velocity.y;

        // Leer la entrada de movimiento (eje X e Y) del jugador.
        movement = _playerInput.actions["Movement"].ReadValue<Vector2>();
        Vector3 moveDirection = new Vector3(movement.x, 0f, movement.y);

        // Cambiar la velocidad actual según si el jugador está corriendo o no.
        float speed = isRunning ? runSpeed : moveSpeed;

        // Mover el personaje verticalmente según el vector de velocidad (gravedad y salto), ajustado por el tiempo.
        _controller.Move(moveDirection * speed * Time.deltaTime);

        // Actualizar la variable de animación "isMovement" en el Animator.
         isMovement = movement.magnitude != 0f;

       _animatorController.SetBool("isRunn",isRunning && isMovement);
       _animatorController.SetBool("isMovement", isMovement);
       /*if (isGrounded && isRunning)
       {
           runningParticle.Play();
       }
       else
       {
           runningParticle.Stop();

       }*/
       
        // Almacenar la dirección del movimiento si el jugador está en movimiento y salta.
        if (isGrounded && movement.magnitude != 0f && _playerInput.actions["Jump"].WasPressedThisFrame())
        {
            jumpDirection = moveDirection.normalized;
        }


       /* if (_playerInput.actions["RunSpeed"].WasPressedThisFrame() && isGrounded && imageFillAmount.fillAmount == 1f)
        {
            isRunning = true;
            currentSpeed = runSpeed;

        }*/
        
    }

    private void ActivateShield()
    {
        if (_playerInput.actions["ActivateShield"].WasPressedThisFrame() && imageShieldFillAmount.fillAmount == 1f)
        {
            shield.SetActive(true);
            shieldAnimator.Play("ActivateShield");
        }
        
    }



   



    void HandleJump()
    {
        // Aplicar la gravedad al vector de velocidad (eje Y) para simular la caída y el salto.
        velocity.y += gravity * Time.deltaTime;
        
        // Almacenar la altura actual del personaje para verificar si está en el suelo.
        float currentHeight = transform.position.y;
        
        // Si el personaje está en el suelo, resetear la variable de altura máxima.
        if (isGrounded)
        {
            maxJumpHeight = currentHeight;
        }

        // Mover el personaje verticalmente según el vector de velocidad (gravedad y salto), ajustado por el tiempo.
        _controller.Move(velocity * Time.deltaTime);
        
        if (_playerInput.actions["Jump"].WasPressedThisFrame() && isGrounded)
        {
            // Aplicar la fuerza de salto en la dirección almacenada.
            if (movement.magnitude != 0f)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
                _controller.Move(jumpDirection * jumpHeight * 0.5f * Time.deltaTime);
               
            }
            else
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
            }
            _animatorController.SetBool("Jump", true);
        }
        
        
        if ( maxJumpHeight <= 8f ) 
        {
            _animatorController.SetBool("JumpDistance", true);
        }
        
        // Si el personaje ya no está en el suelo y ha alcanzado una altura mayor a la máxima registrada, actualizar la altura máxima.
        if (!isGrounded && currentHeight > maxJumpHeight)
        {
            maxJumpHeight = currentHeight;
        }

        // Si el personaje vuelve a tocar el suelo, restablecer la animación "JumpDistance" a false.
        if (isGrounded)
        {
            _animatorController.SetBool("JumpDistance", false);
        }
        
        if (_animatorController.GetBool("Jump"))
        {
            jumpAudioSource.Play();
        }
        

    }
    


  
    

    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRadius* 2);

    }

   

}
