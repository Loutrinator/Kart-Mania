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
        onGroundMovement();
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

    private void onGroundMovement()
    {
        rb.AddForce(onGroundPercent*inputVector.y*stats.acceleration*transform.forward,ForceMode.Acceleration);
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
            Gizmos.DrawRay(wheels[i].position+(transform.up*0.5f),-transform.up * 3f);
        }
    }
    #endregion
    
}
