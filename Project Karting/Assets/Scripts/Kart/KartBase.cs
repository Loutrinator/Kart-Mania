using UnityEngine;

public class KartBase : MonoBehaviour {
    public Rigidbody rigidBody;
    public Vector3 roadDirection = Vector3.up;
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

    private float _currentSpeed;
    private float _currentAngularSpeed;
    private float _yVelocity;

    private void move(float direction) {
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

    private void rotate(float angle) {
        Vector3 currentRotation = transform.rotation.eulerAngles;
        currentRotation.y += angle * Time.fixedDeltaTime;
        transform.RotateAround(rotationAxis.position, rotationAxis.up, angle);
    }
    
    private void applyGravity() {
        Transform t = transform;
        if (Physics.Raycast(t.position + t.up * 1f, -t.up, out var hit, 1.1f,
            1 << LayerMask.NameToLayer("Ground"))) {
            t.position += (hit.point.y - t.position.y) * t.up;
            _yVelocity = 0;
        }
        else {
            _yVelocity += Physics.gravity.y * Time.fixedDeltaTime;
        }
        transform.position += transform.up * (_yVelocity * Time.fixedDeltaTime);
    }
    
    private void FixedUpdate() {
        move(forwardMove);
        rotate(hMove);
        applyGravity();
    }

    public float currentSpeed() {
        return _currentSpeed;
    }
    
    public Vector3 getRoadDirection()
    {
        return roadDirection;
    }
}