using UnityEngine;
using System.Collections;

//Controls the Camera
[RequireComponent(typeof(Camera))]
public class FlyCamera : MonoBehaviour
{
    #region UI

    [Space]

    [SerializeField]
    [Tooltip("The script is currently active")]
    private bool _active = true;

    [Space]

    [SerializeField]
    [Tooltip("Camera rotation by mouse movement is active")]
    private bool _enableRotation = true;

    [SerializeField]
    [Tooltip("Sensitivity of mouse rotation")]
    public float _mouseSense = 1.8f;

    [Space]

    [SerializeField]
    [Tooltip("Camera zooming in/out by 'Mouse Scroll Wheel' is active")]
    private bool _enableTranslation = true;

    [SerializeField]
    [Tooltip("Velocity of camera zooming in/out")]
    private float _translationSpeed = 55f;

    [Space]

    [SerializeField]
    [Tooltip("Camera movement by 'W','A','S','D','Q','E' keys is active")]
    private bool _enableMovement = true;

    [SerializeField]
    [Tooltip("Camera movement speed")]
    public float _movementSpeed = 10f;

    [SerializeField]
    [Tooltip("Speed of the quick camera movement when holding the 'Left Shift' key")]
    public float _boostedSpeed = 50f;

    [SerializeField]
    [Tooltip("Boost speed")]
    private KeyCode _boostSpeed = KeyCode.LeftShift;

    [SerializeField]
    [Tooltip("Move up")]
    private KeyCode _moveUp = KeyCode.E;

    [SerializeField]
    [Tooltip("Move down")]
    private KeyCode _moveDown = KeyCode.Q;

    [Space]

    [SerializeField]
    [Tooltip("Acceleration at camera movement is active")]
    private bool _enableSpeedAcceleration = true;

    [SerializeField]
    [Tooltip("Rate which is applied during camera movement")]
    private float _speedAccelerationFactor = 1.5f;

    [Space]

    [SerializeField]
    [Tooltip("This keypress will move the camera to initialization position")]
    private KeyCode _initPositonButton = KeyCode.R;

    #endregion UI

    public CursorLockMode _wantedMode;

    private float _currentIncrease = 1;
    private float _currentIncreaseMem = 0;

    private Vector3 _initPosition;
    private Vector3 _initRotation;

    public bool inMenu;
    bool isLocked;
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_boostedSpeed < _movementSpeed)
            _boostedSpeed = _movementSpeed;
    }
#endif

    Interactable interactableObject;
    private Vector3 origin;
    Vector3 direction;

    public float sphereRadius;
    public float maxDistance;
    public LayerMask layerMask;
    bool UIactive;
    public GameObject canvas;


    private void Start()
    {
        _initPosition = transform.position;
        _initRotation = transform.eulerAngles;
    }

    private void OnEnable()
    {
        if (_active)
            Cursor.visible = true;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            UIactive = !UIactive;
            canvas.SetActive(UIactive);
        }

        if (!_active)
            return;

        SetCursorState();

        if (Cursor.visible || inMenu)
            return;

        // Translation
        if (_enableTranslation)
        {
            transform.Translate(Vector3.forward * Input.mouseScrollDelta.y * Time.deltaTime * _translationSpeed);
        }

        // Movement
        if (_enableMovement)
        {
            Vector3 deltaPosition = Vector3.zero;
            float currentSpeed = _movementSpeed;

            if (Input.GetKey(_boostSpeed))
                currentSpeed = _boostedSpeed;

            if (Input.GetKey(KeyCode.W))
                deltaPosition += transform.forward;

            if (Input.GetKey(KeyCode.S))
                deltaPosition -= transform.forward;

            if (Input.GetKey(KeyCode.A))
                deltaPosition -= transform.right;

            if (Input.GetKey(KeyCode.D))
                deltaPosition += transform.right;

            if (Input.GetKey(_moveUp))
                deltaPosition += transform.up;

            if (Input.GetKey(_moveDown))
                deltaPosition -= transform.up;

            // Calc acceleration
            CalculateCurrentIncrease(deltaPosition != Vector3.zero);

            transform.position += deltaPosition * currentSpeed * _currentIncrease;
        }

        // Rotation
        if (_enableRotation)
        {
            ArrowKeys();
            // Pitch
            transform.rotation *= Quaternion.AngleAxis(
                -Input.GetAxis("Mouse Y") * _mouseSense,
                Vector3.right
            );

            // Paw
            transform.rotation = Quaternion.Euler(
                transform.eulerAngles.x,
                transform.eulerAngles.y + Input.GetAxis("Mouse X") * _mouseSense,
                transform.eulerAngles.z
            );
        }

        // Return to init position
        if (Input.GetKeyDown(_initPositonButton))
        {
            transform.position = _initPosition;
            transform.eulerAngles = _initRotation;
        }

        CheckForInteractableObject();
    }

    public void CheckForInteractableObject()
    {
        origin = transform.position;
        direction = transform.forward;
        RaycastHit hit;
        if (Physics.SphereCast(origin, sphereRadius, direction, out hit, maxDistance, layerMask, QueryTriggerInteraction.UseGlobal))
        {
            if (hit.collider.tag == "Interactable")
            {
                interactableObject = hit.collider.GetComponent<Interactable>();

                if (interactableObject != null)
                {
                    interactableObject.UIObject.SetActive(true);
                    interactableObject.line.SetActive(true);
                    hit.collider.GetComponent<Interactable>().Interact();
                }
            }
        }
        else if (interactableObject != null)
        {
            interactableObject.UIObject.SetActive(false);
        }
    }

    private void SetCursorState()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isLocked = !isLocked;
            if (isLocked == false)
                Cursor.lockState = _wantedMode = CursorLockMode.None;
            else if(isLocked == true)
                _wantedMode = CursorLockMode.Locked;
        }

        // Apply cursor state
        Cursor.lockState = _wantedMode;
        // Hide cursor when locking
        Cursor.visible = (CursorLockMode.Locked != _wantedMode);
    }

    private void CalculateCurrentIncrease(bool moving)
    {
        _currentIncrease = Time.deltaTime;

        if (!_enableSpeedAcceleration || _enableSpeedAcceleration && !moving)
        {
            _currentIncreaseMem = 0;
            return;
        }

        _currentIncreaseMem += Time.deltaTime * (_speedAccelerationFactor - 1);
        _currentIncrease = Time.deltaTime + Mathf.Pow(_currentIncreaseMem, 3) * Time.deltaTime;
    }

    private void ArrowKeys()
    {

        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(0, 10 * _mouseSense * Time.deltaTime, 0, Space.Self);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(0, -10 * _mouseSense * Time.deltaTime, 0, Space.Self);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Rotate(10 * _mouseSense * Time.deltaTime, 0, 0, Space.Self); 
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Rotate(-10 * _mouseSense * Time.deltaTime, 0, 0, Space.Self);
        }
        if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
        {
            Vector3 v = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(v.x, v.y, 0);
        }
        
    }

    public void ChangeLayerMask(string LayerMasked)
    {

        layerMask.value = 1 << LayerMask.NameToLayer(LayerMasked);
    }
}