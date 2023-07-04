using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoleculeManager : MonoBehaviour
{
    public List<GameObject> carbons;
    public List<GameObject> hydrogens;
    public int previousNumOfAtoms;
    public bool alreadyDoingTheMoving = false;
    public GameObject moleculeCenter;
    public int durationInFrames;
    public CameraRotator camRotatingScript;
    public float distanceToMoveAfterAddingAtom;

    public void Update(){
        if ((carbons.Count != previousNumOfAtoms) && !alreadyDoingTheMoving){
            StartCoroutine("moveMoleculeCenter");
            alreadyDoingTheMoving = true;
        }
        previousNumOfAtoms = carbons.Count;
    }
    IEnumerator moveMoleculeCenter(){
        Vector3 newPosition;
        Vector3 sumOfXyz = Vector3.zero;
        for (int i=0;i<carbons.Count;i++){
            sumOfXyz.x += carbons[i].transform.position.x;
            sumOfXyz.y += carbons[i].transform.position.y;
            sumOfXyz.z += carbons[i].transform.position.z;
        }
        Vector3 oldPosition = moleculeCenter.transform.position;
        newPosition = new Vector3(sumOfXyz.x/carbons.Count,sumOfXyz.y/carbons.Count,sumOfXyz.z/carbons.Count);
        float distanceBetweenOldAndNewPoint = Mathf.Sqrt(Mathf.Pow(newPosition.x-oldPosition.x,2)+Mathf.Pow(newPosition.y-oldPosition.y,2)+Mathf.Pow(newPosition.z-oldPosition.z,2));
        for (int i=0;i<durationInFrames;i++){
            moleculeCenter.transform.position = Vector3.MoveTowards(moleculeCenter.transform.position,newPosition,distanceBetweenOldAndNewPoint/durationInFrames);
            camRotatingScript.distanceFromTarget += distanceToMoveAfterAddingAtom/durationInFrames;
            yield return new WaitForEndOfFrame();
        }
        alreadyDoingTheMoving = false;
    }

}
