using SplineEditor.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpawnPoints : MonoBehaviour
{
    [SerializeField] private BezierSpline spline;
    [SerializeField] private ObjectAlongSpline flag;
    private List<ObjectAlongSpline> obj;

    public float distanceOffset;
    public float offset;
    public float horizontalOffset;

    public void SetSpawners()
    {
        obj = transform.GetComponentsInChildren<ObjectAlongSpline>().ToList();
        
        for(int i = 0; i < obj.Count; i++)
        {
            obj[i].spline = spline;
            if (i % 2 == 0) obj[i].horizontalOffset = -horizontalOffset;
            else obj[i].horizontalOffset = horizontalOffset;

            obj[i].distance = flag.distance - (i * distanceOffset) - offset;
            obj[i].computeDistanceFromPosition = false;
            obj[i].doLerp = true;
            obj[i].OnValidate();
        }
    }
}
