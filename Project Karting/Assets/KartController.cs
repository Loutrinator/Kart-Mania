using UnityEngine;

[RequireComponent(typeof(Rigidbody),typeof(KartInput))]
public class KartController : MonoBehaviour
{
    #region PUBLIC VARIABLES
    
    public KartStats stats = new KartStats
    {
        topSpeed            = 10f,
        acceleration        = 5f,
        accelerationCurve   = .4f,
        braking             = 10f,
        reverseAcceleration = 5f,
        reverseSpeed        = 5f,
        steer               = 5f,
        coastingDrag        = 4f,
        grip                = .95f,
        addedGravity        = 1f,
        suspension          = .2f
    };
    
    public Transform[] wheels = new Transform[0];
    public float groundDetectionDist = 0.2f;
    public float groundingThreshold = 0.1f;
    
    #endregion
    
    #region PRIVATE VARIABLES
    
    private Rigidbody rb;
    private BaseInput inputs;

    private Vector2 inputVector;
    //Physics
    private Vector3 gravity;
    public float gravityLerpSpeed = 10f;
    [SerializeField,Range(0f,1f)]
    private float onGroundPercent = 1f;
    
    #endregion
    
    #region TEMPORARY VARIABLES
    public Transform gravityObject;
    [Range(0,100)] public float maxSteerVelocity = 25;
    #endregion
    
    #region SETUP

    private void Awake()
    {
        setupRigidbody();
        setupInputs();
    }

    private void Start()
    {
        gravity = Vector3.down;
        inputVector = Vector2.zero;
    }
    
    private void setupRigidbody()
    {
        if (rb) return;
        rb = this.GetComponent<Rigidbody>();
        if (rb) return;
        rb = this.gameObject.AddComponent<Rigidbody>();
    }
    private void setupInputs()
    {
        if (inputs) return;
        inputs = this.GetComponent<BaseInput>();
        if (inputs) return;
        inputs = this.gameObject.AddComponent<KartInput>();
    }
    
    #endregion
    
    #region UPDATE
    private void Update()
    {
        getInputs();
    }
    private void FixedUpdate()
    {
        computeGravity();
        // calcule le pourcentage de "en l'air" ou "au sol" du vehicule
        var groundedCount = countGroundedWheels(out var minHeight);
        onGroundPercent = (float)groundedCount / wheels.Length;

        groundVehicle(minHeight);
        
        moveVehicle();
        gravityPhysics();
    }

    private int countGroundedWheels(out float minHeight)
    {
        //compte le nombre de roue au sol
        int groundedCount = 0;
        //on initialise minheight à la valeur la plus haute
        minHeight = float.MaxValue;
        //pour chaque roue
        for (var i = 0; i < wheels.Length; ++i)
        {
            //la roue actuelle
            Transform current = wheels[i];
            //condition ternaire qui ajoute 1 à groundedCount si on a touché le sol
            groundedCount += Physics.Raycast(current.position, -transform.up, out RaycastHit hit, groundDetectionDist) ? 1 : 0;
            
            if (hit.distance > 0)
            {
                //On stocke la plus petite distance avec le sol
                minHeight = Mathf.Min(hit.distance, minHeight);
            }
        }
        return groundedCount;
    }

    private void getInputs()
    {
        inputVector = inputs.GenerateInput();
    }

    #endregion

    #region PHYSICS
    private void computeGravity()
    {
        Vector3 normalSum = Vector3.zero;
        int didHit = 0;
        for(int i = 0; i<wheels.Length; ++i)
        {
            if (Physics.Raycast(wheels[i].position+(transform.up*0.5f), -transform.up, out var hit, 3f))
            {
                normalSum += hit.normal;
                didHit += 1;
            }
        }

        normalSum /= didHit;
        if (didHit > 0 )
        {
            Vector3 newGravity =  -normalSum;
            gravity = Vector3.Lerp(gravity, newGravity, gravityLerpSpeed*Time.fixedDeltaTime);
        }
    }
    void groundVehicle(float minHeight)
    {
        //si toutes les roues sont au sol
        if (onGroundPercent >= 1f)
        {   
            //si la distance avec le sol est plus petite que la hauteur à laquelle on veut garder le vehicule
            if (minHeight < groundingThreshold)
            {
                //alors on calcule la différence exacte entre les deux
                float diff = groundingThreshold - minHeight;
                //et on lui corrige sa position pour que le vehicule soit exactement à "minHeightThreshold" du sol
                var t = transform;
                t.position += diff * t.up;
            }
        }
    }
    private void moveVehicle()
    {
        // coefficient de proportionnalité de la courbe manuelle d'accélération
        const float accelerationCurveCoeff = 5;
        
        //on convertis la vélocité du véhicule en vélocité locale
        //permet d'ignorer la rotation du véhicule et de réaligner le z avec le forward du véhicule entre autre
        var localVel = transform.InverseTransformVector(rb.velocity);
        
        
        var accelDirectionIsFwd = inputVector.y >= 0;//indique si le joueur veut faire avancer le véhicule vers l'avant
        var localVelDirectionIsFwd = localVel.z >= 0;//indique si le véhicule avance vers l'avant
        var isBraking = accelDirectionIsFwd != localVelDirectionIsFwd;//si l'utilisateur veut reculer mais qu'on va en avant, alors on freine

        
        // calcule la vitesse maximum en fonction de si le joueur veut aller en avant ou en arrière
        float maxSpeed = accelDirectionIsFwd ? stats.topSpeed : stats.reverseSpeed;
        // calcule l'accélération en fonction de si le joueur veut aller en avant ou en arrière
        float accelPower = accelDirectionIsFwd ? stats.acceleration : stats.reverseAcceleration;
        
        // pourcentage de vitesse max exprimée par la vitesse actuelle
        var accelRampT = rb.velocity.magnitude / maxSpeed;
        // calcul de l'accélération maximale (atteinte à une vitesse 0)
        var multipliedAccelerationCurve = stats.accelerationCurve * accelerationCurveCoeff;
        //on fait un lerp inversé. si accelRampT² est à 0 ca veut dire qu'on avance pas et donc on a en sortie la plus haute accélération possible. en revanche si accelRampT est à 1 on obtient une accélération de 1
        // si on calcule accelRampT² et pas accelRampT tout seul c'est pour arrondir la courbe
        var accelRamp = Mathf.Lerp(multipliedAccelerationCurve, 1, accelRampT * accelRampT);
        //la courbe ressemble à la moitié droite d''une parabole inversée
        
        // on prend en compte l'accélération de freinage et non l'accélération normale si on freine
        var finalAccelPower = isBraking ? stats.braking : accelPower;
        
        //on applique accelRamp à finalAccelPower pour augmenter ou non l'accélération
        var finalAcceleration = finalAccelPower * accelRamp;
        
        
        var movement = onGroundPercent * inputVector.y * finalAcceleration * Vector3.forward;
        
        
        float currentSpeed = rb.velocity.magnitude;
        bool wasOverMaxSpeed = currentSpeed >= maxSpeed;
        // si on a dépassé la vitesse max alors on n'accélèrera pas pendant cette update
        if (wasOverMaxSpeed && !isBraking) movement *= 0;
        
        
        //On obtient donc une nouvelle vélocité
        Vector3 adjustedVelocity = localVel + movement * Time.fixedDeltaTime;
        //on passe la vélocité de local à world
        adjustedVelocity = transform.TransformVector(adjustedVelocity);
        
        if (onGroundPercent > 0) //permet d'autoriser une vitesse accrue quand c'est la gravité qui fait chuter le vehicule
        {   //si on était pas au dessus de la vitesse max a la frame précédente et que la nouvelle vélocité est au dessus de la vitesse max
            if (adjustedVelocity.magnitude > maxSpeed && !wasOverMaxSpeed)
            {
                //alors on la limite
                adjustedVelocity = Vector3.ClampMagnitude(adjustedVelocity, maxSpeed);
            }
        }
        
        rb.velocity = adjustedVelocity;
        
        if (onGroundPercent > 0) //si les roues touchent le sol on fait tourner le vehicule
        {   
            // on calcule la force de rotation du Kart
            float turningPower = inputVector.x * stats.steer;
            float percentTurn = Mathf.Abs(inputVector.x);
            float speedLossCoeff = 1f - percentTurn/25f;
            rb.velocity *= speedLossCoeff;
            var angularVel = transform.InverseTransformVector(rb.angularVelocity);

            angularVel += turningPower * Time.fixedDeltaTime * rb.transform.up;
            if (angularVel.y > maxSteerVelocity)
            {
                angularVel.y = maxSteerVelocity;
            }
            
            rb.angularVelocity = transform.TransformVector(angularVel);
            Debug.Log( rb.angularVelocity);
        }
        /*
            
        //si on a au moins une roue au sol 
        
        
        rb.velocity = adjustedVelocity;
        
        if (onGroundPercent > 0)
        {
            // manual angular velocity coefficient
            float angularVelocitySteering = .4f;
            float angularVelocitySmoothSpeed = 20f;

            // On tourne à l'envers si on recule
            if (!localVelDirectionIsFwd && !accelDirectionIsFwd) angularVelocitySteering *= -1;
            //on récupère la vitesse angulaire actuelle
            var angularVel = rb.angularVelocity;
            
            angularVel = transform.InverseTransformVector(angularVel);
            

            //  on fait tourner notre kart vers la direction voulue par le joueur
            angularVel.y = Mathf.MoveTowards(angularVel.y, turningPower * angularVelocitySteering, Time.deltaTime * angularVelocitySmoothSpeed);
            //TODO: CHANGER TOUT CA POUR QUE CA TOURNE AUTOUR D'UN AUTRE AXE
            // on applique la nouvelle rotation
            rb.angularVelocity = transform.TransformVector(angularVel);

            // On va faire tourner aussi la vélocité pour que la rotation de notre kart ai un effet immédiat sur son déplacement
            // manual velocity steering coefficient
            float velocitySteering = 25f;
            // on fait tourner la vélocité en se basant sur la vélocité de rotation
            var rotatedVelocity = Quaternion.Euler(0f, turningPower * velocitySteering * Time.deltaTime, 0f) * transform.InverseTransformVector(rb.velocity);
            rb.velocity = transform.TransformVector(rotatedVelocity);
            /*
            // si on est au sol on applique une friction lattérale
            if (onGroundPercent > 0f)
            {
                // manual grip coefficient scalar
                float gripCoeff = 30f;
                //on récupère la friction latérale en utilisant la regle de la main droite, pouce = a, index = b et majeur = sortie
                Vector3 latFrictionDirection = Vector3.Cross(fwd, transform.up);
                // on récupère le dotproduct de la vélocité et le vecteur directionnel de notre friction latérale
                float latSpeed = Vector3.Dot(rb.velocity, latFrictionDirection);
                // on vient amortir la friction latérale en fonction du grip
                Vector3 latFrictionDampedVelocity = rb.velocity - latSpeed * stats.grip * gripCoeff * Time.deltaTime * latFrictionDirection;

                // on applique la nouvelle vélocité avec friction latérale
                rb.velocity = latFrictionDampedVelocity;
            }
        }
*/
        /*
 //ancien code qui marche pas
//on calcule le quaternion qui représente la rotation du kart voulue
var turnAngle = Quaternion.AngleAxis(turningPower, rb.transform.up);
//on fait tourner le vecteur forward en multipliant le quaternion de rotation avec le forward de notre kart
var fwd = turnAngle * rb.transform.forward;

//calcule le mouvement du kart :
// onGroundPercent permet de changer la force de déplacement en fonction du nombre de roues au sol
// accelInput c'est l'input utilisateur donc ca permet d'avancer plus ou moins vite en fonction du joystick
// finalAcceleration c'est l'accélération actuelle
// fwd c'est la direction du mouvement
var movement = onGroundPercent * inputVector.y * finalAcceleration * fwd;


// simple suspension allows us to thrust forward even when on bumpy terrain
fwd.y = Mathf.Lerp(fwd.y, 0, stats.suspension);

// calcule si on est au dessus de la vitesse max
float currentSpeed = rb.velocity.magnitude;
bool wasOverMaxSpeed = currentSpeed >= maxSpeed;

// si on a dépassé la vitesse max alors on n'accélèrera pas pendant cette update
if (wasOverMaxSpeed && !isBraking) movement *= 0;

var velocity = rb.velocity;
//On obtient donc une nouvelle vélocité
Vector3 adjustedVelocity = velocity + movement * Time.deltaTime;

//adjustedVelocity.y = transform.InverseTransformVector(velocity).y;

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
bool isCoasting = Mathf.Abs(inputVector.y) < .01f;
//si on est en roue libre
if (isCoasting)
{   
    //alors on calcule une vélocité qui n'inclue que la gravité
    Vector3 restVelocity = new Vector3(0, rb.velocity.y, 0);
    //TODO: A REVOIR
    adjustedVelocity = Vector3.MoveTowards(adjustedVelocity, restVelocity, Time.deltaTime * stats.coastingDrag);
}

rb.velocity = adjustedVelocity;

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
    var angularVel = rb.angularVelocity;

    //  on fait tourner notre kart vers la direction voulue par le joueur
    angularVel.y = Mathf.MoveTowards(angularVel.y, turningPower * angularVelocitySteering, Time.deltaTime * angularVelocitySmoothSpeed);
    //TODO: CHANGER TOUT CA POUR QUE CA TOURNE AUTOUR D'UN AUTRE AXE
    // on applique la nouvelle rotation
    rb.angularVelocity = angularVel;

    // On va faire tourner aussi la vélocité pour que la rotation de notre kart ai un effet immédiat sur son déplacement
    // manual velocity steering coefficient
    float velocitySteering = 25f;
    // on fait tourner la vélocité en se basant sur la vélocité de rotation
    rb.velocity = Quaternion.Euler(0f, turningPower * velocitySteering * Time.deltaTime, 0f) * rb.velocity;
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
    float latSpeed = Vector3.Dot(rb.velocity, latFrictionDirection);
    // on vient amortir la friction latérale en fonction du grip
    Vector3 latFrictionDampedVelocity = rb.velocity - latSpeed * stats.grip * gripCoeff * Time.deltaTime * latFrictionDirection;

    // on applique la nouvelle vélocité avec friction latérale
    rb.velocity = latFrictionDampedVelocity;
}
*/
        //rb.AddForce(onGroundPercent*inputVector.y*stats.acceleration*transform.forward,ForceMode.Acceleration);
    }

    private void gravityPhysics()
    {
        float inAirPercent = 1f - onGroundPercent;
        rb.AddForce(gravity,ForceMode.Acceleration);
        rb.AddForce(stats.addedGravity*inAirPercent*gravity,ForceMode.Acceleration);//add gravity in mid air
    }
    #endregion

    #region DEBUG
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position,gravity);

        Gizmos.color = Color.green;
        for(int i = 0; i<wheels.Length; ++i)
        {
            var up = transform.up;
            Gizmos.DrawRay(wheels[i].position+(up*0.5f),-up * 3f);
        }

        if (rb)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position,rb.velocity);
            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(transform.position,transform.InverseTransformVector(rb.velocity));
        } 

    }
    #endregion

}
