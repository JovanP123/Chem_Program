using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraRotator : MonoBehaviour
{
    [Range(9f,100f)]
    public float mouseSens = 3f;
    public float rotationX;
    public float rotationY;
    public float distanceFromTarget;
    public GameObject rotationTarget;
    private Vector3 targetPreviousPosition;

    private void Update() {
        if (Input.GetMouseButton(1)){
            float mouseX = Input.GetAxis("Mouse X") * mouseSens;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSens;
            rotationX += mouseY;
            rotationY += mouseX;
            transform.localEulerAngles = new Vector3(rotationX,rotationY,0f);
            transform.position = rotationTarget.transform.position - transform.forward * distanceFromTarget;
        }
        if (rotationTarget.transform.position != targetPreviousPosition){
            transform.position = rotationTarget.transform.position - transform.forward * distanceFromTarget;
        }
        targetPreviousPosition = rotationTarget.transform.position;
    }

}
