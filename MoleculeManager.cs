using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoleculeManager : MonoBehaviour
{
    [SerializeField]
    public List<GameObject> carbons;
    public List<GameObject> hydrogens;
    public int previousNumOfAtoms;
    public bool alreadyDoingTheMoving = false;
    public GameObject moleculeCenter;
    public int durationInFrames;
    public CameraRotator camRotatingScript;
    public float distanceToMoveAfterAddingAtom;
    public List<List<elementOfMolecule>> segments = new List<List<elementOfMolecule>>();
    public TMP_Text nameOfMolecule;
    public List<string> organicPrefixes;
    public List<string> organicSuffixes;

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
    public List<elementOfMolecule> trackSegment(elementOfMolecule originPoint){
        List<elementOfMolecule> segment = new List<elementOfMolecule>();
        segment.Add(originPoint);
        elementOfMolecule currentPoint = originPoint;
        if (currentPoint.numberOfConnections==1){
            segment.Add(currentPoint.pointsTo[0].GetComponent<elementOfMolecule>());
            currentPoint = currentPoint.pointsTo[0].GetComponent<elementOfMolecule>();
        }
        else if (currentPoint.numberOfConnections==0){
            return segment;
        }
        while (true){
            if (currentPoint.numberOfConnections==1){
                segment.Add(currentPoint.pointsTo[0].GetComponent<elementOfMolecule>());
                currentPoint = currentPoint.pointsTo[0].GetComponent<elementOfMolecule>();
            }
            else{
                return segment;
            }
        }
    }
    public void nameMolecule(){
        segments = new List<List<elementOfMolecule>>();
        segments.Add(trackSegment(carbons[0].GetComponent<elementOfMolecule>()));
        if (segments.Count == 1){
            List<GameObject> sp3 = new List<GameObject>();
            List<GameObject> sp2 = new List<GameObject>();
            List<GameObject> sp = new List<GameObject>();
            List<GameObject> h = new List<GameObject>();
            for (int i=0;i<segments[0].Count;i++){
                if (segments[0].Count==1){
                    for (int j=0;j<4;j++){
                        GameObject neighbouringAtom = segments[0][0].element.GetComponent<AtomProperties>().bonds[j].GetComponent<SpThreeBondOccupation>().connectedTo;
                        if (neighbouringAtom==null){
                            StartCoroutine(unableToNameMolecule());
                            return;
                        }
                        else if (neighbouringAtom.GetComponent<AtomProperties>().atomType=="H"){
                            h.Add(neighbouringAtom);
                        }
                    }
                    sp3.Add(segments[0][0].element);
                    break;
                }
                if (segments[0][i].element.GetComponent<AtomProperties>().atomType=="C(sp3)"){
                    sp3.Add(segments[0][i].element);
                    for (int j=0;j<4;j++){
                        GameObject bondConnection = segments[0][i].element.GetComponent<AtomProperties>().bonds[j].GetComponent<SpThreeBondOccupation>().connectedTo;
                        if (bondConnection==null){
                            unableToNameMolecule();
                            return;
                        }
                        else if (bondConnection.GetComponent<AtomProperties>().atomType=="H"){
                            h.Add(bondConnection);
                        }
                    }
                }
                else if (segments[0][i].element.GetComponent<AtomProperties>().atomType=="C(sp2)"){
                    sp2.Add(segments[0][i].element);
                    for (int j=0;j<3;j++){
                        if (j==0){
                            GameObject doubleBondConnection = segments[0][i].element.GetComponent<AtomProperties>().bonds[j].GetComponent<SpTwoBondOccupation>().connectedTo;
                            if (doubleBondConnection==null){
                                unableToNameMolecule();
                                return;
                            }
                            continue;    
                        }
                        GameObject singleBondConnection = segments[0][i].element.GetComponent<AtomProperties>().bonds[j].GetComponent<SpThreeBondOccupation>().connectedTo;
                        if (singleBondConnection==null){
                            unableToNameMolecule();
                            return;
                        }
                        else if (singleBondConnection.GetComponent<AtomProperties>().atomType=="H"){
                            h.Add(singleBondConnection);
                        }
                    }
                }
                else if (segments[0][i].element.GetComponent<AtomProperties>().atomType=="C(sp)"){
                    sp.Add(segments[0][i].element);
                    GameObject tripleBondConnection = segments[0][i].element.GetComponent<AtomProperties>().bonds[0].GetComponent<SpBondOccupation>().connectedTo;
                    GameObject singleBondConnection = segments[0][i].element.GetComponent<AtomProperties>().bonds[0].GetComponent<SpThreeBondOccupation>().connectedTo;
                    if (tripleBondConnection==null || singleBondConnection==null){
                        unableToNameMolecule();
                        return;
                    }
                    else if(tripleBondConnection.GetComponent<AtomProperties>().atomType=="H" && singleBondConnection.GetComponent<AtomProperties>().atomType=="H"){
                        h.Add(tripleBondConnection);
                        h.Add(singleBondConnection);
                    }
                    else if(tripleBondConnection.GetComponent<AtomProperties>().atomType=="H"){
                        h.Add(tripleBondConnection);
                    }
                    else if(singleBondConnection.GetComponent<AtomProperties>().atomType=="H"){
                        h.Add(singleBondConnection);
                    }
                }
            }
            Debug.Log(segments[0].Count);
            //Naming the molecule;
            if (sp2.Count==0 && sp.Count==0){
                nameOfMolecule.text = organicPrefixes[sp3.Count-1]+organicSuffixes[0];
            }
            if (sp2.Count==2 && sp3.Count<=1 && sp.Count==0){
                nameOfMolecule.text = organicPrefixes[sp3.Count+sp2.Count-1]+organicPrefixes[1];
            }
            else if (sp2.Count==2 && sp.Count==0){
                int indexOfFirstSp2 = 0;
                int indexOfSecondSp2 = 0;
                for (int i=0;i<segments[0].Count;i++){
                    if (segments[0][i].element.GetComponent<AtomProperties>().atomType=="C(sp2)"){
                        indexOfFirstSp2 = i;
                        indexOfSecondSp2 = i+1;
                        break;
                    }
                }
                int winningPosition = indexOfFirstSp2+1;
                if (indexOfFirstSp2+1<segments[0].Count-indexOfSecondSp2){
                    winningPosition = indexOfFirstSp2+1;
                }
                else if (indexOfFirstSp2+1>segments[0].Count-indexOfSecondSp2){
                    winningPosition = segments[0].Count-indexOfSecondSp2;
                }
                nameOfMolecule.text = organicPrefixes[sp3.Count+sp2.Count-1]+"-"+winningPosition.ToString()+"-"+organicSuffixes[1];
            }

    }
    IEnumerator unableToNameMolecule(){
        nameOfMolecule.text = "UNABLE TO NAME MOLECULE";
        nameOfMolecule.color = Color.red;
        yield return new WaitForSeconds(2);
        nameOfMolecule.text = "";
        nameOfMolecule.color = Color.white;
    }
}
}