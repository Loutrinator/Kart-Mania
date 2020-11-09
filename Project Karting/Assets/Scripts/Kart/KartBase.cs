using UnityEngine;

public class KartBase : MonoBehaviour {
    public Rigidbody rigidBody;
    public Vector3 roadDirection = Vector3.up;
    public Transform raycastCenter;    // Use for gravity & ground check
    public Transform rotationAxis;
    
    public Stats vehicleStats = new Stats {
        topSpeed = 10f,    //
        acceleration = 5f,    //
        braking = 10f,
        reverseAcceleration = 5f,    //
        reverseSpeed = 5f,
        steer = 5f,
        addedGravity = 1f,
        suspension = .2f
    };
    
    protected float hMove;
    protected int forwardMove;    // -1; 0; 1

    private Vector3 _firstPos;
    private float _firstPosTime;
    private float _currentSpeed;
    private float _currentAngularSpeed;

    private void Awake() {
        _firstPos = transform.position;
        _firstPosTime = Time.time;
    }

    protected void move(float direction) {
        if (direction > 0) {
            _currentSpeed += vehicleStats.acceleration * Time.fixedDeltaTime;
            _currentSpeed = Mathf.Min(vehicleStats.topSpeed, _currentSpeed);
        }
        else if (direction < 0) {
            _currentSpeed -= vehicleStats.reverseAcceleration * Time.fixedDeltaTime;
            _currentSpeed = Mathf.Max(-vehicleStats.reverseSpeed, _currentSpeed);
        }
        else _currentSpeed = 0;

        var t = transform;
        t.position += t.forward * (_currentSpeed * Time.fixedDeltaTime);
    }

    protected void rotate(float angle) {
        Vector3 currentRotation = transform.rotation.eulerAngles;
        currentRotation.y += angle * Time.fixedDeltaTime;
        transform.RotateAround(rotationAxis.position, rotationAxis.up, angle);
    }
    
    private void FixedUpdate() {
        move(forwardMove);
        rotate(hMove);
    }

    public float currentSpeed() {
        return _currentSpeed;
    }
    
    public Vector3 getRoadDirection()
    {
        return roadDirection;
    }
}