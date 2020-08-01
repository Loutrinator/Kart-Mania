using System;
using UnityEngine;

public class KartController : MonoBehaviour
{
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
        /*
        if (Vector3.Dot(toPlayerDirection, movingDirection) < 0) // if the spider is not facing the player
        {
            transform.rotation = Quaternion.LookRotation(toUp.normalized, movingDirection.normalized);
        }else
        {
            transform.rotation = Quaternion.LookRotation(toUp.normalized, -movingDirection.normalized);
        }*/
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
    /*
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
    */
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
    
    /*
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
