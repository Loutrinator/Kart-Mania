using UnityEngine;

[RequireComponent(typeof(Rigidbody),typeof(KartInput))]
public class KartController : MonoBehaviour
{
    #region PUBLIC VARIABLES
    
    public KartStats stats;
    
    #endregion
    
    #region PRIVATE VARIABLES
    
    private Rigidbody rb;
    private BaseInput inputs;

    private Vector2 inputVector;
    //Physics
    private Vector3 gravity;
    private float gravityLerpSpeed = 10f;
    [SerializeField,Range(0f,1f)]
    private float onGroundPercent = 1f;
    
    #endregion
    
    //Temporary variables
    public Transform gravityObject;
    
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
        computeGravity();
        getInputs();
    }
    private void FixedUpdate()
    {
        OnGroundMovement();
        InAirMovement();
    }

    #region PHYSICS
    private void computeGravity()
    {
        gravity = Vector3.Lerp(gravity, gravityObject.forward, gravityLerpSpeed*Time.deltaTime);
    }

    private void OnGroundMovement()
    {
        rb.AddForce(onGroundPercent*transform.forward*inputVector.y);
    }

    private void InAirMovement()
    {
        float inAirPercent = 1f - onGroundPercent;
        rb.AddForce(inAirPercent*gravity,ForceMode.Acceleration);
    }
    #endregion
    
    private void getInputs()
    {
        inputVector = inputs.GenerateInput();
    }

    #endregion

    #region DEBUG
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position,gravity);
    }
    #endregion
    
}
