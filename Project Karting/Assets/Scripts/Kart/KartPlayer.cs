using UnityEngine;

public class KartPlayer : KartBase {
    private void Update() {
        float hMove = Input.GetAxis("Horizontal");
        float vMove = Input.GetAxis("Vertical");

        CurrentMoveInput.x = hMove;
        CurrentMoveInput.y = vMove;
    }
}
