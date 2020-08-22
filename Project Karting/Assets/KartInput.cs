using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartInput : BaseInput
{
    
    public override Vector2 GenerateInput()
    {
        //print("inputs : " + Input.GetAxisRaw("Horizontal") + " " + Input.GetAxisRaw("Vertical"));
        return new Vector2(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical"));
    }
}
