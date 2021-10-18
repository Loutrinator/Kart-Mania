using UnityEngine;

public class SphereController : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float speed;
    [SerializeField] private float steering;
    [SerializeField] private Transform controlledRenderer;

    private void Start()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
    }
    
    private void Update()
    {
        float forward = Input.GetAxis("Accelerate");
        float backwards = Input.GetAxis("Brake");
        float verticalMove = forward - backwards;
        rb.AddForce(controlledRenderer.forward * (verticalMove * speed));

        float hMove = Input.GetAxis("Horizontal");
        controlledRenderer.Rotate(controlledRenderer.up * steering * hMove);

        controlledRenderer.position = transform.position;
    }
}
