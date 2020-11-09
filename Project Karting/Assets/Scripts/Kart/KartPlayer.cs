using UnityEngine;

public class KartPlayer : KartBase {
    private void Update() {
        float forwardInput = Input.GetAxis("Vertical");
        if (forwardInput > 0.001f)
            forwardMove = 1;
        else if (forwardInput < -0.001f)
            forwardMove = -1;
        else forwardMove = 0;
        
        hMove = Input.GetAxis("Horizontal");
    }
}
