using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SphereKart : MonoBehaviour
{
    [SerializeField] private Rigidbody RB;
    [SerializeField] private Transform kartModel;
    public Transform[] Wheels;
    public Stats baseStats = new Stats
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
    
    [SerializeField] private float raycastDist = 0.3f;
    
    List<StatPowerup> activePowerupList = new List<StatPowerup>();
    Stats finalStats;

    private Vector2 movingAxis = new Vector2(0,0);
    
    private Vector3 currentVelocity = Vector3.zero;
    //those two variables represent how much the kart is in mid air or not depending on the amount of wheels touching ground
    private float inAirPercent;
    private float onGroundPercent;
    
    private void FixedUpdate()
    {
        ApplyPowerups();
        GetGroundedState();
        MoveKart();
        //FixKartRotation();
        GravityPhysics();
        FixKartPlacement();

    }

    private void FixKartPlacement()
    {
        /*
        RaycastHit hit;
        if (Physics.Raycast(RB.position, Vector3.down, out hit, 0.38f))
        {
            Vector3 offset = hit.point - transform.position; 
            RB.transform.position += offset; 
        }
        RB.transform.position += Vector3.up*0.38f; */
    }

    private void GetGroundedState()
    {
        // calcule le pourcentage de "en l'air" ou "au sol" du vehicule
        int groundedCount = CountGroundedWheels(out float minHeight);
        onGroundPercent = (float)groundedCount / Wheels.Length;
        inAirPercent = 1 - onGroundPercent;

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
    void GravityPhysics()
    {
        // si toutes les roues sont dans les air
        if (inAirPercent >= 1)
        {
            //alors on accélère la gravité subie par le kart
            RB.velocity += Time.deltaTime * finalStats.addedGravity * Physics.gravity;
        }
        //TODO: A VOIR POUR APPLIQUER MANUELLEMENT LA GRAVITE EN PLUS COMME ON NE PRENDS PAS EN COMPTE LA GRAVITE NORMALE
        
    }
    private void MoveKart()
    {
        
        // coefficient scalaire de la courbe manuelle d'accélération
        float accelerationCurveCoeff = 5;
        //on convertis la vélocité du véhicule en vélocité locale
        Vector3 localVel = transform.InverseTransformVector(RB.velocity);
        
        //indique si le joueur veut faire avancer le véhicule vers l'avant
        bool accelDirectionIsFwd = movingAxis.y >= 0;
        //indique si le véhicule avance vers l'avant
        bool localVelDirectionIsFwd = localVel.z >= 0;

        // calcule la vitesse maximum en fonction de si le joueur veut aller en avant ou en arrière
        float maxSpeed = accelDirectionIsFwd ? finalStats.topSpeed : finalStats.reverseSpeed;
        // calcule l'accélération en fonction de si le joueur veut aller en avant ou en arrière
        float accelPower = accelDirectionIsFwd ? finalStats.acceleration : finalStats.reverseAcceleration;
        
        //JE CAPTE R
        float accelRampT = RB.velocity.magnitude / maxSpeed;
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
        float turningPower = movingAxis.x * finalStats.steer;
        
        //on calcule le quaternion qui représente la rotation du kart voulue
        Quaternion turnAngle = Quaternion.AngleAxis(turningPower, RB.transform.up);
        //on fait tourner le vecteur forward en multipliant le quaternion de rotation avec le forward de notre kart
        Vector3 fwd = turnAngle * RB.transform.forward;
        
        //calcule le mouvement du kart :
        // onGroundPercent permet de changer la force de déplacement en fonction du nombre de roues au sol
        // accelInput c'est l'input utilisateur donc ca permet d'avancer plus ou moins vite en fonction du joystick
        // finalAcceleration c'est l'accélération actuelle
        // fwd c'est la direction du mouvement
        Vector3 movement = movingAxis.y * finalAcceleration * fwd;
        
        // simple suspension allows us to thrust forward even when on bumpy terrain
        fwd.y = Mathf.Lerp(fwd.y, 0, finalStats.suspension);
        
        // calcule si on est au dessus de la vitesse max
        float currentSpeed = RB.velocity.magnitude;
        bool wasOverMaxSpeed = currentSpeed >= maxSpeed;

        // si on a dépassé la vitesse max alors on n'accélèrera pas pendant cette update
        if (wasOverMaxSpeed && !isBraking) movement *= 0;
        //On obtient donc une nouvelle vélocité
        Vector3 adjustedVelocity = RB.velocity + movement * (Time.deltaTime * onGroundPercent);

        adjustedVelocity.y = RB.velocity.y;
        //TODO: TROUVER UN EQUIVALENT AVEC ROUTE INCLINEE
        
        //si on était pas au dessus de la vitesse max a la frame précédente et que la nouvelle vélocité est au dessus de la vitesse max
        if (adjustedVelocity.magnitude > maxSpeed && !wasOverMaxSpeed)
        {
            //alors on la limite
            adjustedVelocity = Vector3.ClampMagnitude(adjustedVelocity, maxSpeed);
        }
        
        
        // le coasting c'est le fait de rouler en roue libre
        //on calcule donc si l'accélération est proche de 0 : [-0.01f,0.01f]
        bool isCoasting = Mathf.Abs(movingAxis.y) < .01f;
        //si on est en roue libre
        if (isCoasting)
        {   
            //alors on calcule une vélocité qui n'inclue que la gravité
            Vector3 restVelocity = new Vector3(0, RB.velocity.y, 0);
            //TODO: A REVOIR
            adjustedVelocity = Vector3.MoveTowards(adjustedVelocity, restVelocity, Time.deltaTime * finalStats.coastingDrag);
        }

        RB.velocity = adjustedVelocity;
        // si on est au sol
        if (onGroundPercent > 0)
        {
            // manual angular velocity coefficient
            float angularVelocitySteering = .4f;
            float angularVelocitySmoothSpeed = 20f;

            // On tourne à l'envers si on recule
            if (!localVelDirectionIsFwd && !accelDirectionIsFwd) angularVelocitySteering *= -1;
            //on récupère la vitesse angulaire actuelle
            var angularVel = RB.angularVelocity;

            //  on fait tourner notre kart vers la direction voulue par le joueur
            angularVel.y = Mathf.MoveTowards(angularVel.y, turningPower * angularVelocitySteering, Time.deltaTime * angularVelocitySmoothSpeed);
            //TODO: CHANGER TOUT CA POUR QUE CA TOURNE AUTOUR D'UN AUTRE AXE
            // on applique la nouvelle rotation
            RB.angularVelocity = angularVel;

            // On va faire tourner aussi la vélocité pour que la rotation de notre kart ai un effet immédiat sur son déplacement
            // manual velocity steering coefficient
            float velocitySteering = 25f;
            // on fait tourner la vélocité en se basant sur la vélocité de rotation
            RB.velocity = Quaternion.Euler(0f, turningPower * velocitySteering * Time.deltaTime, 0f) * RB.velocity;
            //TODO: faire tourner autrement que sur y
        }
    }

    void ApplyPowerups()
    {
        // on supprime tout powerup qui a dépassé son temps d'activation
        activePowerupList.RemoveAll((p) => { return p.elapsedTime > p.maxTime;});
        var powerups = new Stats();// on initialise des stats vierges pour nos powerups
        // on ajoute à 'powerups' les modifiers de chaque powerup
        for (int i = 0; i < activePowerupList.Count; i++)
        {
            var p = activePowerupList[i];
            // on met a jour le compteur de temps écoulé depuis l'obtention du powerup
            p.elapsedTime += Time.deltaTime;
            // on additionne les modifications des stats de notre powerup à 'powerups'
            powerups += p.modifiers;
        }

        // on ajoute tous nos powerups cumulés à nos stats de base du véhicule
        finalStats = baseStats + powerups;

        // on clamp toutes les valeurs des stats qui nécessitent de pas dépasser [0,1]
        finalStats.grip = Mathf.Clamp(finalStats.grip, 0, 1);
        finalStats.suspension = Mathf.Clamp(finalStats.suspension, 0, 1);
    }

    void Update()
    {
        Vector2 newMovingAxis = Vector2.zero;
        newMovingAxis.y = Input.GetAxis("Vertical")*baseStats.acceleration;
        newMovingAxis.x = Input.GetAxis("Horizontal")*baseStats.steer;
        movingAxis = newMovingAxis; //Vector2.Lerp(movingAxis, newMovingAxis, Time.deltaTime * 1f);

    }


    private void OnDrawGizmos()
    {
        Vector3 dir = RB.velocity;
        Gizmos.color = Color.green;
        
        Gizmos.DrawRay(RB.transform.position, dir);
    }
}
