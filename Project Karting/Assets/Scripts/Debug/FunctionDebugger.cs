using UnityEngine;
using UnityEngine.UI;

public class FunctionDebugger : MonoBehaviour
{
    [SerializeField] private Text functionName;
    private string funcNameString = "";
    public void SetName(string name)
    {
        functionName.text = name;
        funcNameString = name;
    }
    public void SetValue(float value)
    {
        functionName.text = funcNameString + " : " + value;
    }

}
