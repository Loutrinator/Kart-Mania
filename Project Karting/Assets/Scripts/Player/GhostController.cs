using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

/// <summary>
/// A class used by GhostRecorder for the time trial game mode.
/// </summary>
public class GhostController : KartController
{
    /// <summary>
    /// Sets the kart's position
    /// </summary>
    /// <param name="newPosition">The desired position</param>
    public void SetKartPosition(Vector3 newPosition)
    {
        kart.transform.position = newPosition;
    }
    
    /// <summary>
    /// Sets the kart's rotation
    /// </summary>
    /// <param name="newPosition">The desired rotation</param>
    public void SetKartRotation(Quaternion newRotation)
    {
        kart.transform.rotation = newRotation;
    }
    
    /// <summary>
    /// Sets the kart's inputs
    /// </summary>
    /// <param name="kartMovement">The desired inputs for direction (x axis) and acceleration (y axis) controls</param>
    /// <param name="isDrifting">Sets the kart in drifting state</param>
    public void SetKartInputs(Vector2 kartMovement, bool isDrifting)
    {
        Move(kartMovement);
        Drift(isDrifting);
    }
    
    /// <summary>
    /// A getter to get the kart's position
    /// </summary>
    /// <returns>The kart's position</returns>
    public Vector3 GetKartPosition()
    {
        return kart.transform.position;
    }
    
    /// <summary>
    /// A getter to get the kart's rotation
    /// </summary>
    /// <returns>The kart's rotation</returns>
    public Quaternion GetKartRotation()
    {
        return kart.transform.rotation;
    }
    
    /// <summary>
    /// A getter to get the kart's direction and acceleration inputs 
    /// </summary>
    /// <returns>The kart's movement
    ///  - X axis : the direction (going left or right)
    ///  - Y axis : the acceleration (acccelerating or braking)
    /// </returns>
    public Vector2 GetKartMovement()
    {
        return kart.movement;
    }
    
    /// <summary>
    /// A getter that returns if the kart is currently drifting or not
    /// </summary>
    /// <returns>The kart's drifting state </returns>
    public bool GetKartDrift()
    {
        return kart.drift;
    }
}
