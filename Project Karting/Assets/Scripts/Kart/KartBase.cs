using System;
using UnityEngine;

public class KartBase : MonoBehaviour {
    public Rigidbody rigidBody;

    public Stats vehicleStats = new Stats {
        topSpeed = 10f,
        acceleration = 5f,
        accelerationCurve = .4f,
        braking = 10f,
        reverseAcceleration = 5f,
        reverseSpeed = 5f,
        steer = 5f,
        coastingDrag = 4f,
        grip = .95f,
        addedGravity = 1f,
        suspension = .2f
    };

    protected Vector2 CurrentMoveInput;

    private Vector3 _firstPos;
    private float _firstPosTime;
    private float _currentSpeed;
    private float _currentAngularSpeed; // current input for player / movement for ia

    private void Awake() {
        _firstPos = transform.position;
        _firstPosTime = Time.time;
    }

    protected void Move(Vector3 directionP) {
        transform.position += directionP * Time.deltaTime;
    }

    protected void Rotate(float angle) {
        Vector3 currentRotation = transform.rotation.eulerAngles;
        currentRotation.y += _currentAngularSpeed * angle * Time.deltaTime;
        transform.rotation = Quaternion.Euler(currentRotation);
    }


    private void FixedUpdate() {
        // CALCUL CURRENT SPEED
        float dist = (transform.position - _firstPos).magnitude;
        float timeDiff = Time.time - _firstPosTime;
        if (timeDiff != 0)
            _currentSpeed = dist / timeDiff;
        _firstPos = transform.position;
        _firstPosTime = Time.time;
        
        // ***** CALCUL ACCELERATION *****
        // coefficient scalaire de la courbe manuelle d'accélération
        float accelerationCurveCoeff = 5;
        //on convertis la vélocité du véhicule en vélocité locale
        Vector3 localVel = transform.InverseTransformVector(rigidBody.velocity);

        //indique si le joueur veut faire avancer le véhicule vers l'avant
        bool accelDirectionIsFwd = CurrentMoveInput.y >= 0;
        //indique si le véhicule avance vers l'avant
        bool localVelDirectionIsFwd = localVel.z >= 0;

        // calcule la vitesse maximum en fonction de si le joueur veut aller en avant ou en arrière
        float maxSpeed = accelDirectionIsFwd ? vehicleStats.topSpeed : vehicleStats.reverseSpeed;
        // calcule l'accélération en fonction de si le joueur veut aller en avant ou en arrière
        float accelPower = accelDirectionIsFwd ? vehicleStats.acceleration : vehicleStats.reverseAcceleration;

        //JE CAPTE R
        float accelRampT = _currentSpeed / maxSpeed;
        float multipliedAccelerationCurve = vehicleStats.accelerationCurve * accelerationCurveCoeff;
        float accelRamp = Mathf.Lerp(multipliedAccelerationCurve, 1, accelRampT * accelRampT);

        //si l'utilisateur veut reculer mais qu'on va en avant, alors on freine
        bool isBraking = accelDirectionIsFwd != localVelDirectionIsFwd;

        // si on freine
        // on prend en compte l'accélération de freinage et non l'accélération normale
        float finalAccelPower = isBraking ? vehicleStats.braking : accelPower;

        //on applique accelRamp à finalAccelPower pour augmenter ou non l'accélération
        float finalAcceleration = finalAccelPower * accelRamp;
        // ***** FIN CALCUL ACCELERATION *****
        // ***** CALCUL ANGLE *****
        // on calcule la force de rotation du Kart
        float turningPower = CurrentMoveInput.x * vehicleStats.steer;
        //on calcule le quaternion qui représente la rotation du kart voulue
        Quaternion turnAngle = Quaternion.AngleAxis(turningPower, transform.up);
        //on fait tourner le vecteur forward en multipliant le quaternion de rotation avec le forward de notre kart
        Vector3 fwd = turnAngle * transform.forward;
        // ***** FIN CALCUL ANGLE *****

        Vector3 movement = CurrentMoveInput.y * finalAcceleration * fwd;
        Move(movement);
        // TODO DIFF between transform.forward & fwd
    }

    public float CurrentSpeed() {
        return _currentSpeed;
    }
}