using System;
using System.Collections.Generic;
using UnityEngine;

public class Kart : CameraTarget{
    /// <summary>
    /// Contains parameters that can adjust the kart's behaviors temporarily.
    /// </summary>
    [System.Serializable]
    public class StatPowerup
    {
        public Kart.Stats modifiers;
        public float elapsedTime;
        public float maxTime;
    }

    /// <summary>
    /// Contains a series tunable parameters to tweak various karts for unique driving mechanics.
    /// </summary>
    [System.Serializable]
    public struct Stats
    {
        [Header("Movement Settings")]
        [Tooltip("The maximum speed forwards")]
        public float topSpeed;

        [Tooltip("How quickly the Kart reaches top speed.")]
        public float acceleration;

        [Tooltip("The maximum speed backward.")]
        public float reverseSpeed;

        [Tooltip("The rate at which the kart increases its backward speed.")]
        public float reverseAcceleration;

        [Tooltip("How quickly the Kart starts accelerating from 0. A higher number means it accelerates faster sooner.")]
        [Range(0.2f, 1)]
        public float accelerationCurve;

        [Tooltip("How quickly the Kart slows down when going in the opposite direction.")]
        public float braking;

        [Tooltip("How quickly to slow down when neither acceleration or reverse is held.")]
        public float coastingDrag;

        [Range(0, 1)]
        [Tooltip("The amount of side-to-side friction.")]
        public float grip;

        [Tooltip("How quickly the Kart can turn left and right.")]
        public float steer;

        [Tooltip("Additional gravity for when the Kart is in the air.")]
        public float addedGravity;

        [Tooltip("How much the Kart tries to keep going forward when on bumpy terrain.")]
        [Range(0, 1)]
        public float suspension;

        // allow for stat adding for powerups.
        public static Stats operator +(Stats a, Stats b)
        {
            return new Stats
            {
                acceleration        = a.acceleration + b.acceleration,
                accelerationCurve   = a.accelerationCurve + b.accelerationCurve,
                braking             = a.braking + b.braking,
                coastingDrag        = a.coastingDrag + b.coastingDrag,
                addedGravity        = a.addedGravity + b.addedGravity,
                grip                = a.grip + b.grip,
                reverseAcceleration = a.reverseAcceleration + b.reverseAcceleration,
                reverseSpeed        = a.reverseSpeed + b.reverseSpeed,
                topSpeed            = a.topSpeed + b.topSpeed,
                steer               = a.steer + b.steer,
                suspension          = a.suspension + b.suspension
            };
        }
    }

    public Rigidbody Rigidbody { get; private set; }
    public Vector2 Inputs       { get; private set; }
    public float inAirPercent    { get; private set; }
    public float onGroundPercent { get; private set; }

    public Kart.Stats baseStats = new Kart.Stats
    {
        topSpeed            = 10f,
        acceleration        = 5f,
        accelerationCurve   = 4f,
        braking             = 10f,
        reverseAcceleration = 5f,
        reverseSpeed        = 5f,
        steer               = 5f,
        coastingDrag        = 4f,
        grip                = .95f,
        addedGravity        = 1f,
        suspension          = .2f
    };

    
    [Header("Vehicle Physics")]
    [Tooltip("The transform that determines the position of the Kart's mass.")]
    public Transform centerOfMass;

    [Tooltip("The physical representations of the Kart's wheels.")]
    public Transform[] Wheels;

    [Tooltip("Which layers the wheels will detect.")]
    public LayerMask groundLayers = Physics.DefaultRaycastLayers;

    [Tooltip("How far to raycast when checking for ground.")]
    public float raycastDist = 0.3f;

    [Tooltip("How high to keep the kart above the ground.")]
    public float minHeightThreshold = 0.02f;

    //public Transform SuspensionBody;

    // saved transforms of where the suspension's neutral positions are
    //Vector3 suspensionNeutralPos;
    //Quaternion suspensionNeutralRot;

    // the input sources that can control the kart
    IInput[] m_Inputs;

    // can the kart move?
    bool canMove = true;
    List<StatPowerup> activePowerupList = new List<StatPowerup>();
    GameObject lastGroundCollided = null;
    Kart.Stats finalStats;
    private Vector3 roadDirection;

    void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
        m_Inputs = GetComponents<IInput>();
        roadDirection = Vector3.down;
        //suspensionNeutralPos = SuspensionBody.transform.localPosition;
        //suspensionNeutralRot = SuspensionBody.transform.localRotation;
    }

    void FixedUpdate()
    {
        ResetIfStuck();

        GatherInputs();

        // on applique nos powerups
        ApplyPowerups();

        // Convertit la position du centre de gravité en référentiel world space vers local space
        Rigidbody.centerOfMass = Rigidbody.transform.InverseTransformPoint(centerOfMass.position);

        // calcule le pourcentage de "en l'air" ou "au sol" du vehicule
        int groundedCount = CountGroundedWheels(out float minHeight);
        onGroundPercent = (float)groundedCount / Wheels.Length;
        inAirPercent = 1 - onGroundPercent;

        // gather inputs
        float accel = Inputs.y;
        float turn = Inputs.x;

        // On colle le vehicule au sol
        GroundVehicle(minHeight);
        if (canMove)
        {
            MoveVehicle(accel, turn);
        }
        GravityPhysics();

        // animation
        //AnimateSuspension();
    }
    
    void MoveVehicle(float accelInput, float turnInput)
    {
        // coefficient scalaire de la courbe manuelle d'accélération
        float accelerationCurveCoeff = 5;
        //on convertis la vélocité du véhicule en vélocité locale
        Vector3 localVel = transform.InverseTransformVector(Rigidbody.velocity);
        
        //indique si le joueur veut faire avancer le véhicule vers l'avant
        bool accelDirectionIsFwd = accelInput >= 0;
        //indique si le véhicule avance vers l'avant
        bool localVelDirectionIsFwd = localVel.z >= 0;

        // calcule la vitesse maximum en fonction de si le joueur veut aller en avant ou en arrière
        float maxSpeed = accelDirectionIsFwd ? finalStats.topSpeed : finalStats.reverseSpeed;
        // calcule l'accélération en fonction de si le joueur veut aller en avant ou en arrière
        float accelPower = accelDirectionIsFwd ? finalStats.acceleration : finalStats.reverseAcceleration;
        
        //JE CAPTE R
        float accelRampT = Rigidbody.velocity.magnitude / maxSpeed;
        float multipliedAccelerationCurve = finalStats.accelerationCurve * accelerationCurveCoeff;
        float accelRamp = Mathf.Lerp(multipliedAccelerationCurve, 1, accelRampT * accelRampT);
        
        //si l'utilisateur veut reculer mais qu'on va en avant, alors on freine
        bool isBraking = accelDirectionIsFwd != localVelDirectionIsFwd;

        // si on freine
        // on prend en compte l'accélération de freinage et non l'accélération normale
        float finalAccelPower = isBraking ? finalStats.braking : accelPower;
        
        //on applique accelRamp à finalAccelPower pour augmenter ou non l'accélération
        float finalAcceleration = finalAccelPower * accelRamp;

        // on calcule la force de rotation du Kart
        float turningPower = turnInput * finalStats.steer;
        
        //on calcule le quaternion qui représente la rotation du kart voulue
        Quaternion turnAngle = Quaternion.AngleAxis(turningPower, Rigidbody.transform.up);
        //on fait tourner le vecteur forward en multipliant le quaternion de rotation avec le forward de notre kart
        Vector3 fwd = turnAngle * Rigidbody.transform.forward;
        
        //calcule le mouvement du kart :
        // onGroundPercent permet de changer la force de déplacement en fonction du nombre de roues au sol
        // accelInput c'est l'input utilisateur donc ca permet d'avancer plus ou moins vite en fonction du joystick
        // finalAcceleration c'est l'accélération actuelle
        // fwd c'est la direction du mouvement
        Vector3 movement = onGroundPercent * accelInput * finalAcceleration * fwd;

        // simple suspension allows us to thrust forward even when on bumpy terrain
        fwd.y = Mathf.Lerp(fwd.y, 0, finalStats.suspension);

        // calcule si on est au dessus de la vitesse max
        float currentSpeed = Rigidbody.velocity.magnitude;
        bool wasOverMaxSpeed = currentSpeed >= maxSpeed;

        // si on a dépassé la vitesse max alors on n'accélèrera pas pendant cette update
        if (wasOverMaxSpeed && !isBraking) movement *= 0;
        //On obtient donc une nouvelle vélocité
        Vector3 adjustedVelocity = Rigidbody.velocity + movement * Time.deltaTime;

        adjustedVelocity.y = Rigidbody.velocity.y;
        //TODO: TROUVER UN EQUIVALENT AVEC ROUTE INCLINEE
        
        //si on a au moins une roue au sol 
        if (onGroundPercent > 0)
        {   //si on était pas au dessus de la vitesse max a la frame précédente et que la nouvelle vélocité est au dessus de la vitesse max
            if (adjustedVelocity.magnitude > maxSpeed && !wasOverMaxSpeed)
            {
                //alors on la limite
                adjustedVelocity = Vector3.ClampMagnitude(adjustedVelocity, maxSpeed);
            }
        }

        // le coasting c'est le fait de rouler en roue libre
        //on calcule donc si l'accélération est proche de 0 : [-0.01f,0.01f]
        bool isCoasting = Mathf.Abs(accelInput) < .01f;
        //si on est en roue libre
        if (isCoasting)
        {   
            //alors on calcule une vélocité qui n'inclue que la gravité
            Vector3 restVelocity = new Vector3(0, Rigidbody.velocity.y, 0);
            //TODO: A REVOIR
            //
            adjustedVelocity = Vector3.MoveTowards(adjustedVelocity, restVelocity, Time.deltaTime * finalStats.coastingDrag);
        }

        Rigidbody.velocity = adjustedVelocity;

        //ApplyAngularSuspension();
        
        // si on est au sol
        if (onGroundPercent > 0)
        {
            // manual angular velocity coefficient
            float angularVelocitySteering = .4f;
            float angularVelocitySmoothSpeed = 20f;

            // On tourne à l'envers si on recule
            if (!localVelDirectionIsFwd && !accelDirectionIsFwd) angularVelocitySteering *= -1;
            //on récupère la vitesse angulaire actuelle
            var angularVel = Rigidbody.angularVelocity;

            //  on fait tourner notre kart vers la direction voulue par le joueur
            angularVel.y = Mathf.MoveTowards(angularVel.y, turningPower * angularVelocitySteering, Time.deltaTime * angularVelocitySmoothSpeed);
            //TODO: CHANGER TOUT CA POUR QUE CA TOURNE AUTOUR D'UN AUTRE AXE
            // on applique la nouvelle rotation
            Rigidbody.angularVelocity = angularVel;

            // On va faire tourner aussi la vélocité pour que la rotation de notre kart ai un effet immédiat sur son déplacement
            // manual velocity steering coefficient
            float velocitySteering = 25f;
            // on fait tourner la vélocité en se basant sur la vélocité de rotation
            Rigidbody.velocity = Quaternion.Euler(0f, turningPower * velocitySteering * Time.deltaTime, 0f) * Rigidbody.velocity;
            //TODO: faire tourner autrement que sur y
        }

        // si on est au sol on applique une friction lattérale
        if (onGroundPercent > 0f)
        {
            // manual grip coefficient scalar
            float gripCoeff = 30f;
            //on récupère la friction latérale en utilisant la regle de la main droite, pouce = a, index = b et majeur = sortie
            Vector3 latFrictionDirection = Vector3.Cross(fwd, transform.up);
            // on récupère le dotproduct de la vélocité et le vecteur directionnel de notre friction latérale
            float latSpeed = Vector3.Dot(Rigidbody.velocity, latFrictionDirection);
            // on vient amortir la friction latérale en fonction du grip
            Vector3 latFrictionDampedVelocity = Rigidbody.velocity - latFrictionDirection * latSpeed * finalStats.grip * gripCoeff * Time.deltaTime;

            // on applique la nouvelle vélocité avec friction latérale
            Rigidbody.velocity = latFrictionDampedVelocity;
        }
    }
    
    //a lire et réécrire
    void ApplyAngularSuspension()
    {
        // simple suspension dampens x and z angular velocity while on the ground
        Vector3 suspendedX = transform.right;
        Vector3 suspendedZ = transform.forward;
        suspendedX.y *= 0f;
        suspendedZ.y *= 0f;
        var sX = Vector3.Dot(Rigidbody.angularVelocity, suspendedX) * suspendedX;
        var sZ = Vector3.Dot(Rigidbody.angularVelocity, suspendedZ) * suspendedZ;
        var sXZ = sX + sZ;
        var sCoeff = 10f;

        Vector3 suspensionRotation;
        float minimumSuspension = 0.5f;
        if (onGroundPercent > 0.5f || finalStats.suspension < minimumSuspension)
        {
            suspensionRotation = sXZ * finalStats.suspension * sCoeff * Time.deltaTime;
        }
        else
        {
            suspensionRotation = sXZ * minimumSuspension * sCoeff * Time.deltaTime;
        }

        Vector3 suspendedAngular = Rigidbody.angularVelocity - suspensionRotation;

        // apply the adjusted angularvelocity
        Rigidbody.angularVelocity = suspendedAngular;
    }
    
    void GroundVehicle(float minHeight)
    {
        //si toutes les roues sont au sol
        if (onGroundPercent >= 1f)
        {   
            //si la distance avec le sol est plus petite que la hauteur à laquelle on veut garder le vehicule
            if (minHeight < minHeightThreshold)
            {
                //alors on calcule la différence exacte entre les deux
                float diff = minHeightThreshold - minHeight;
                //et on lui corrige sa position pour que le vehicule soit exactement à "minHeightThreshold" du sol
                transform.position += diff * transform.up;
            }
        }
    }
    void GravityPhysics()
    {
        // si toutes les roues sont dans les air
        if (inAirPercent >= 1)
        {
            //alors on accélère la gravité subie par le kart
            Rigidbody.velocity += Physics.gravity * Time.deltaTime * finalStats.addedGravity;
        }
        //TODO: A VOIR POUR APPLIQUER MANUELLEMENT LA GRAVITE EN PLUS COMME ON NE PRENDS PAS EN COMPTE LA GRAVITE NORMALE
        
    }
    int CountGroundedWheels(out float minHeight)
    {    
        //compte le nombre de roue au sol
        int groundedCount = 0;
        //on initialise minheight à la valeur la plus haute
        minHeight = float.MaxValue;
        //pour chaque roue
        for (int i = 0; i < Wheels.Length; i++)
        {
            //la roue actuelle
            Transform current = Wheels[i];
            //condition ternaire qui ajoute 1 à groundedCount si on a touché le sol
            groundedCount += Physics.Raycast(current.position, Vector3.down, out RaycastHit hit, raycastDist) ? 1 : 0;
            //TODO: CHANGER LE VECTOR3.DOWN
            if (hit.distance > 0)
            {
                //On stocke la plus petite distance avec le sol
                minHeight = Mathf.Min(hit.distance, minHeight);
            }
        }
        return groundedCount;
    }
    void ApplyPowerups()
    {
        // on supprime tout powerup qui a dépassé son temps d'activation
        activePowerupList.RemoveAll((p) => { return p.elapsedTime > p.maxTime; });

        // on initialise des stats vierges pour nos powerups
        var powerups = new Stats();

        // on ajoute à 'powerups' les modifiers de chaque powerup
        for (int i = 0; i < activePowerupList.Count; i++)
        {
            var p = activePowerupList[i];

            // on met a jour le compteur de temps écoulé depuis l'obtention du powerup
            p.elapsedTime += Time.deltaTime;

            // on additionne les modifications des stats de notre powerup à 'powerups'
            powerups += p.modifiers;
        }

        // on ajoute 'powerups' à nos stats de base du véhicule
        finalStats = baseStats + powerups;

        // on clamp toutes les valeurs des stats qui nécessitent de pas dépasser [0,1]
        finalStats.grip = Mathf.Clamp(finalStats.grip, 0, 1);
        finalStats.suspension = Mathf.Clamp(finalStats.suspension, 0, 1);
    }
    
    void GatherInputs()
    {
        // on reset les inputs
        Inputs = Vector2.zero;

        //on récupère les inputs de la part des sources disponibles
        for (int i = 0; i < m_Inputs.Length; i++)
        {
            var inputSource = m_Inputs[i];
            Vector2 current = inputSource.GenerateInput();
            //la dernière input qui est pas inactive devient l'input utilisée
            if (current.sqrMagnitude > 0)
            {
                Inputs = current;
            }
        }
    }
    bool IsStuck()
    {
        float speed = Rigidbody.velocity.magnitude;
        //si le kart n'a aucune roue au sol
        if (onGroundPercent <= 0)
        {   //si sa vitesse "est nulle"
            if (speed < 0.01f)
            {   //et que le joueur essaie d'accélérer
                if (Mathf.Abs(Inputs.y) > 0)
                {   //alors il est coincé
                    return true;
                }
            }
        }
        return false;
    }
    
    void OnCollisionEnter(Collision other)
    {
        if(groundLayers == (groundLayers | (1 << other.collider.gameObject.layer)))
        {
            lastGroundCollided = other.collider.gameObject;
        }
    }

    
    void ResetIfStuck()
    {
        //si le joueur ne peut plus bouger et qu'on a deja touché une route
        if (IsStuck() && lastGroundCollided != null)
        {
            if (lastGroundCollided.TryGetComponent(out Collider groundCollider))
            {
                Bounds bounds = groundCollider.bounds;
                Vector3 pos = new Vector3(
                    bounds.center.x,
                    bounds.max.y,
                    bounds.center.z
                );
                transform.position = pos;
            }
        }
    }
    
    public override Vector3 GetRoadDirection()
    {
        return roadDirection;
    }
    
    /*
    [SerializeField] private Rigidbody kart;

    //[SerializeField] private Vector2 roadDetectionColliderSize;
    //[SerializeField] private Vector3 roadDetectionColliderOffset;
    [SerializeField] private float roadDetectionDistance;
    
    [HideInInspector] public Vector3 gravityDirection = Vector3.down;//the rotation of the vehicle
    [SerializeField] private float gravityForce = 9.81f;//the rotation computed for the current frame
    
    private Vector3 roadNormal;
    private Vector3 roadPosition;
    private Vector3 direction;
    private bool isOnRoad = true;
    
    public float speed;
    public float steerMaxAngle;
    public float steerSpeed;
    
    private float velocity;

    //private Vector3[] roadDetectionPoints;

    void Start()
    {
        //roadDetectionPoints = new Vector3[4];
        if (!kart)
        {
            kart = this.GetComponent<Rigidbody>();
        }
        //setupRoadDetection();
    }


    void Update()
    {
        kart.velocity = Vector3.zero;
        Move();
        Steer();
        SetKartOnRoad();
        AlignKartToRoad();
    }

    private void Steer()
    {
        float steerAngle = 0;
        float steerAxis = Input.GetAxis("Horizontal");
        if (Math.Abs(steerAxis) > 0)
        {
            steerAngle = steerAxis * steerMaxAngle;
        }
        transform.Rotate(transform.up,steerAngle*Time.deltaTime*steerSpeed);
        //direction = direction + transform.right
        //    movingDirection = (sine * transform.right * directionNoiseCoeff + transform.forward).normalized;
    }

    private void Move()
    {
        float newVelocity = 0;
        float speedAxis = Input.GetAxis("Vertical");
        if (Mathf.Abs(speedAxis) > 0)
        {
            newVelocity = speedAxis * speed;
        }

        velocity = Mathf.Lerp(velocity, newVelocity, 0.01f);
        kart.velocity += Time.deltaTime * velocity * transform.forward;
    }

    private void AlignKartToRoad()
    {
        //if (Physics.SphereCast(transform.position, positionHeightOffset*1.2f, Vector3.up, out hit, 0, 1 << LayerMask.NameToLayer("Ground"))) //Doesnt Work
        //if (Physics.Raycast(transform.position + transform.up*0.1f, -transform.up, out hit, detectionDistance, 1 << LayerMask.NameToLayer("Ground")))

        if (isOnRoad)
        {
            gravityDirection =-roadNormal;
            transform.rotation = Quaternion.LookRotation(roadNormal.normalized, -transform.forward.normalized);
            transform.Rotate(Vector3.right, 90f, Space.Self);
        }
        else
        {
            kart.velocity += Time.deltaTime * gravityForce * gravityDirection;
        }
        //transform.Rotate(Vector3.up, 180f, Space.Self);
        
        if (Vector3.Dot(toPlayerDirection, movingDirection) < 0) // if the spider is not facing the player
        {
            transform.rotation = Quaternion.LookRotation(toUp.normalized, movingDirection.normalized);
        }else
        {
            transform.rotation = Quaternion.LookRotation(toUp.normalized, -movingDirection.normalized);
        }
        //transform.Rotate(Vector3.right, 90f, Space.Self);
    }
    private void SetKartOnRoad()
    {
        if (Physics.Raycast(transform.position, -transform.up, out var hit, roadDetectionDistance)){
        
            isOnRoad = true;
            var kartTransform = transform;
            kartTransform.position = hit.point + hit.normal * 0.5f;
            roadNormal = Vector3.Lerp(kartTransform.up,hit.normal, 0.1f);
            //il faut pouvoir rotate autour de hit.normal
        }
        else {
            isOnRoad = false;
        }

    }
    
    private void setupRoadDetection()
    {
        roadDetectionPoints[0] = transform.position + transform.rotation*(new Vector3(-roadDetectionColliderSize[0] / 2, 0, roadDetectionColliderSize[1] / 2) +
                                roadDetectionColliderOffset);
        roadDetectionPoints[1] = transform.position + transform.rotation*(new Vector3(roadDetectionColliderSize[0] / 2, 0, roadDetectionColliderSize[1] / 2) +
                                roadDetectionColliderOffset);
        roadDetectionPoints[2] = transform.position + transform.rotation*(new Vector3(-roadDetectionColliderSize[0] / 2, 0, -roadDetectionColliderSize[1] / 2) +
                                roadDetectionColliderOffset);
        roadDetectionPoints[3] = transform.position + transform.rotation*(new Vector3(roadDetectionColliderSize[0] / 2, 0, -roadDetectionColliderSize[1] / 2) +
                                roadDetectionColliderOffset);
    }

    private void getRoadNormal()
    {
        int countHits = 0;
        roadNormal = Vector3.zero;
        RaycastHit hit;
        Vector3 direction = -transform.up;
        for (int i = 0; i < 4; i++)
        {
            if (Physics.Raycast(roadDetectionPoints[i], direction,out hit, roadDetectionDistance))
            {
                roadNormal += hit.normal;
                //roadPosition += hit.point;
                countHits++;
            }
        }
        roadNormal /= countHits > 0 ? (float)countHits : 1f;
        //roadPosition /= countHits > 0 ? (float)countHits : 1f
        isOnRoad = countHits > 0;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        //setupRoadDetection();
        
        var kartTransform = transform;
        var kartPosition = kartTransform.position;
        Vector3 rayDirection = kartPosition-kartTransform.up * roadDetectionDistance;
        Gizmos.DrawLine(kartPosition, rayDirection);
        
        
        Gizmos.color = Color.red;
        Gizmos.DrawLine(kartPosition, kartPosition+roadNormal);
        
    }
    
    
     *PSEUDO CODE
     *
     * START
     * la gravité doit etre de base vers le bas
     * 
     * lancer 4 raycasts
     * faire la moyenne pour obtenir une vecteur moyen représentant la normale de la route sous le vehicule
     * set la gravité à la meme valeur que la normale du sol avec un lerp
     * appliquer la gravité sur le vehicule
     *
     * cette partie est nécessaire uniquement si on veut coller le vehicule au sol sans gravité
     * 
     * faire un raycast depuis le centre du véhicule avec pour direction l'inverse du vecteur normal
     * le point d'impact correspond à la position du vehicule sur la route
     *
     * une fois que le player a changé de position, on peut
     */
}
