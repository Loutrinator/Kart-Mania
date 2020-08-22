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
    
    //Physics
    private Vector3 gravity;
    public float gravityLerpSpeed;
    #endregion
    
    //Temporary variables
    public Transform gravityObject;
    
    #region SETUP
    
    private void Start()
    {
        setupRigidbody();
        setupInputs();
        gravity = Vector3.down;
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
    }

    private void computeGravity()
    {
        gravity = Vector3.Lerp(gravity, gravityObject.forward, gravityLerpSpeed*Time.deltaTime);
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
