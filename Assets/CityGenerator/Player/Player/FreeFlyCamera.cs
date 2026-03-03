using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CinematicFreeCam : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float fastMult = 3f;
    public float mouseSens = 2f;
    public float scrollSens = 5f;
    public float fovSpeed = 50f;
    public float timeSpeed = 2f;

    public float positionLerpTime = 5f;
    public float rotationLerpTime = 5f;

    private float _currentSpeed;
    private float _rotX;
    private float _rotY;
    private Camera _cam;
    private bool _isCinematic;

    private Transform _target;
    private Vector3 _localHitPoint;
    private Vector3 _localCameraOffset;

    private Vector3 _desiredPosition;
    private Quaternion _desiredRotation;

    private void Start()
    {
        _cam = GetComponent<Camera>();
        _currentSpeed = moveSpeed;

        Vector3 rot = transform.localRotation.eulerAngles;
        _rotY = rot.y;
        _rotX = rot.x;

        _desiredPosition = transform.position;
        _desiredRotation = transform.localRotation;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        float dt = Time.unscaledDeltaTime;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            _isCinematic = !_isCinematic;
            if (!_isCinematic)
            {
                _desiredPosition = transform.position;
                Vector3 r = transform.localRotation.eulerAngles;
                _rotX = r.x;
                _rotY = r.y;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Ray ray = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, ~0, QueryTriggerInteraction.Ignore))
            {
                _target = hit.transform;
                _localHitPoint = _target.InverseTransformPoint(hit.point);
                _localCameraOffset = _target.InverseTransformPoint(transform.position);
            }
            else
            {
                _target = null;
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _target = null;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.001f)
        {
            _currentSpeed += scroll * scrollSens * 10f;
            _currentSpeed = Mathf.Clamp(_currentSpeed, 1f, 200f);
        }

        if (Input.GetKey(KeyCode.Z)) _cam.fieldOfView = Mathf.Clamp(_cam.fieldOfView - (fovSpeed * dt), 5f, 150f);
        if (Input.GetKey(KeyCode.X)) _cam.fieldOfView = Mathf.Clamp(_cam.fieldOfView + (fovSpeed * dt), 5f, 150f);

        if (Input.GetKey(KeyCode.T)) Time.timeScale = Mathf.Clamp(Time.timeScale - (timeSpeed * dt), 0f, 100f);
        if (Input.GetKey(KeyCode.Y)) Time.timeScale = Mathf.Clamp(Time.timeScale + (timeSpeed * dt), 0f, 100f);
        if (Input.GetKeyDown(KeyCode.R)) Time.timeScale = 1f;

        if (_target != null)
        {
            _desiredPosition = _target.TransformPoint(_localCameraOffset);

            Vector3 worldHitPoint = _target.TransformPoint(_localHitPoint);
            Vector3 dir = (worldHitPoint - _desiredPosition).normalized;

            if (dir != Vector3.zero)
            {
                _desiredRotation = Quaternion.LookRotation(dir);
                Vector3 euler = _desiredRotation.eulerAngles;
                _rotX = euler.x;
                _rotY = euler.y;
            }
        }
        else
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                _rotY += Input.GetAxis("Mouse X") * mouseSens;
                _rotX -= Input.GetAxis("Mouse Y") * mouseSens;
                _rotX = Mathf.Clamp(_rotX, -90f, 90f);
                _desiredRotation = Quaternion.Euler(_rotX, _rotY, 0f);
            }

            float finalSpeed = Input.GetKey(KeyCode.LeftShift) ? _currentSpeed * fastMult : _currentSpeed;
            float step = finalSpeed * dt;

            Vector3 moveDir = Vector3.zero;
            moveDir += transform.forward * Input.GetAxis("Vertical");
            moveDir += transform.right * Input.GetAxis("Horizontal");

            if (Input.GetKey(KeyCode.E)) moveDir += Vector3.up;
            if (Input.GetKey(KeyCode.LeftControl)) moveDir += Vector3.down;

            _desiredPosition += moveDir * step;
        }

        if (_isCinematic)
        {
            transform.position = Vector3.Lerp(transform.position, _desiredPosition, positionLerpTime * dt);
            transform.rotation = Quaternion.Slerp(transform.rotation, _desiredRotation, rotationLerpTime * dt);
        }
        else
        {
            transform.position = _desiredPosition;
            transform.rotation = _desiredRotation;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (Input.GetMouseButtonDown(0) && Cursor.lockState == CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}