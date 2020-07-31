using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Comparers;
using UnityEngine.PlayerLoop;

public class KartController : MonoBehaviour
{
    [SerializeField] private Transform kart;

    [SerializeField] private Vector2 roadDetectionColliderSize;
    [SerializeField] private Vector3 roadDetectionColliderOffset;
    [SerializeField] private float roadDetectionDistance;
    
    [SerializeField] private float steering;//the steering amount
    [SerializeField] private float acceleration;//the acceleration amount

    [HideInInspector] public Vector3 gravityDirection = Vector3.down;//the rotation of the vehicle
    [SerializeField] private float gravityForce = 9.81f;//the rotation computed for the current frame
    
    private Vector3 roadNormal;
    private Vector3 roadPosition;
    private Vector3 direction;
    private bool isOnRoad = true;
    
    private Vector3[] roadDetectionPoints = new Vector3[4];

    private float velocity;
    public float speed;

    public float steerMaxAngle;
    public float steerSpeed;
    void Start()
    {
        //setupRoadDetection();
    }


    void Update()
    {
        //getRoadNormal();
        move();
        steer();
        setKartOnRoad();
        alignKartToRoad();
    }

    private void steer()
    {
        float steerAngle = 0;
        if (Input.GetAxis("Horizontal") != 0)
        {
            steerAngle = Input.GetAxis("Horizontal") * steerMaxAngle;
        }
        transform.Rotate(transform.up,steerAngle*Time.deltaTime*steerSpeed);
        //direction = direction + transform.right
        //    movingDirection = (sine * transform.right * directionNoiseCoeff + transform.forward).normalized;
    }

    private void move()
    {
        float newVelocity = 0;
        if (Input.GetAxis("Vertical") != 0)
        {
            newVelocity = Input.GetAxis("Vertical") * speed;
        }

        velocity = Mathf.Lerp(velocity, newVelocity, 0.01f);
        transform.position += transform.forward * velocity * Time.deltaTime;
    }

    private void alignKartToRoad()
    {
        //if (Physics.SphereCast(transform.position, positionHeightOffset*1.2f, Vector3.up, out hit, 0, 1 << LayerMask.NameToLayer("Ground"))) //Doesnt Work
        //if (Physics.Raycast(transform.position + transform.up*0.1f, -transform.up, out hit, detectionDistance, 1 << LayerMask.NameToLayer("Ground")))

        if (isOnRoad)
        {
            var forward = transform.forward;
            var Up = gravityDirection =roadNormal;
            transform.rotation = Quaternion.LookRotation(Up.normalized, -forward.normalized);
            transform.Rotate(Vector3.right, 90f, Space.Self);
        }
        else
        {
            transform.position -= gravityDirection * gravityForce * Time.deltaTime / 10f;
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
    private void setKartOnRoad()
    {
        RaycastHit hit;
    
        if (Physics.Raycast(transform.position, -transform.up, out hit, roadDetectionDistance)){
        
            isOnRoad = true;
            transform.position = hit.point + hit.normal * 0.5f;
            roadNormal = Vector3.Lerp(transform.up,hit.normal, 0.1f);
            //il faut pouvoir rotate autour de hit.normal
        }
        else
        {
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
        Vector3 direction = transform.position-transform.up * roadDetectionDistance;
        Gizmos.DrawLine(transform.position, direction);
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
