using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RotateAroundBody : MonoBehaviour
{
    
    public Vector3 axisVector;
    public float angle;
    public GameObject body;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.RotateAround(body.transform.position,axisVector,angle);
    }
}
