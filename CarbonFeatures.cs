using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarbonFeatures : MonoBehaviour
{
    public GameObject hydrogen_NoBond;
    public MoleculeManager moleculeManagement;

    public void fillEmptyWithH(){
        if (gameObject.GetComponent<AtomProperties>().atomType=="C(sp3)"){
            for (int i=0;i<4;i++){
                if (!gameObject.GetComponent<AtomProperties>().bonds[i].GetComponent<SpThreeBondOccupation>().complete){
                    GameObject outerPoint = gameObject.GetComponent<AtomProperties>().outerPoints[i];
                    outerPoint.transform.Translate(0f, hydrogen_NoBond.transform.localScale.x-hydrogen_NoBond.GetComponent<AtomProperties>().bondOffsetRadius,0f);
                    GameObject newHydrogen = Instantiate(hydrogen_NoBond,new Vector3(outerPoint.transform.position.x,outerPoint.transform.position.y,outerPoint.transform.position.z),Quaternion.Euler(outerPoint.transform.eulerAngles));
                    gameObject.GetComponent<AtomProperties>().bonds[i].GetComponent<SpThreeBondOccupation>().complete = true;
                    newHydrogen.GetComponent<AtomProperties>().chainPoint = gameObject.GetComponent<AtomProperties>().bonds[i];
                    newHydrogen.GetComponent<AtomProperties>().bonds.Insert(0,gameObject.GetComponent<AtomProperties>().bonds[i]);
                    newHydrogen.GetComponent<AtomProperties>().innerPoints.Insert(0,gameObject.GetComponent<AtomProperties>().outerPoints[i]);
                    newHydrogen.GetComponent<AtomProperties>().outerPoints.Insert(0,gameObject.GetComponent<AtomProperties>().innerPoints[i]);
                    moleculeManagement.hydrogens.Add(newHydrogen);
                    newHydrogen.SetActive(true);
                }
            }
            StartCoroutine("returnPointsAfterFillWithHSp3");
        }
        else if (gameObject.GetComponent<AtomProperties>().atomType=="C(sp2)"){
            for (int i=0;i<2;i++){
                if (!gameObject.GetComponent<AtomProperties>().bonds[i+1].GetComponent<SpThreeBondOccupation>().complete){
                    GameObject outerPoint = gameObject.GetComponent<AtomProperties>().outerPoints[i+1];
                    outerPoint.transform.Translate(0f, hydrogen_NoBond.transform.localScale.x-hydrogen_NoBond.GetComponent<AtomProperties>().bondOffsetRadius,0f);
                    GameObject newHydrogen = Instantiate(hydrogen_NoBond,outerPoint.transform.position,Quaternion.Euler(outerPoint.transform.eulerAngles));
                    gameObject.GetComponent<AtomProperties>().bonds[i+1].GetComponent<SpThreeBondOccupation>().complete = true;
                    newHydrogen.GetComponent<AtomProperties>().chainPoint = gameObject.GetComponent<AtomProperties>().bonds[i+1];
                    newHydrogen.GetComponent<AtomProperties>().bonds.Insert(0,gameObject.GetComponent<AtomProperties>().bonds[i+1]);
                    newHydrogen.GetComponent<AtomProperties>().innerPoints.Insert(0,gameObject.GetComponent<AtomProperties>().outerPoints[i+1]);
                    newHydrogen.GetComponent<AtomProperties>().outerPoints.Insert(0,gameObject.GetComponent<AtomProperties>().innerPoints[i+1]);
                    newHydrogen.SetActive(true);
                    moleculeManagement.hydrogens.Add(newHydrogen);
                }
            }
            StartCoroutine("returnPointsAfterFillWithHSp2");
        }
        else if (gameObject.GetComponent<AtomProperties>().atomType=="C(sp)"){
            if (!gameObject.GetComponent<AtomProperties>().chainPoint.GetComponent<SpThreeBondOccupation>().complete){
                GameObject outerPoint = gameObject.GetComponent<AtomProperties>().outerPoints[1];
                outerPoint.transform.Translate(0f,hydrogen_NoBond.transform.localScale.x-hydrogen_NoBond.GetComponent<AtomProperties>().bondOffsetRadius,0f);
                GameObject newHydrogen = Instantiate(hydrogen_NoBond,outerPoint.transform.position,Quaternion.Euler(outerPoint.transform.eulerAngles));
                newHydrogen.GetComponent<AtomProperties>().bonds.Add(gameObject.GetComponent<AtomProperties>().chainPoint);
                newHydrogen.GetComponent<AtomProperties>().chainPoint = gameObject.GetComponent<AtomProperties>().chainPoint;
                newHydrogen.GetComponent<AtomProperties>().innerPoints.Add(outerPoint);
                newHydrogen.GetComponent<AtomProperties>().outerPoints.Add(gameObject.GetComponent<AtomProperties>().innerPoints[1]);
                newHydrogen.SetActive(true);
                moleculeManagement.hydrogens.Add(newHydrogen);
            }
        }

    }

    IEnumerator returnPointsAfterFillWithHSp3(){
        yield return new WaitForEndOfFrame();
        for (int i=0;i<4;i++){
            gameObject.GetComponent<AtomProperties>().outerPoints[i].transform.Translate(0f,-(hydrogen_NoBond.transform.localScale.x-hydrogen_NoBond.GetComponent<AtomProperties>().bondOffsetRadius),0f);
        }
    }
    IEnumerator returnPointsAfterFillWithHSp2(){
        yield return new WaitForEndOfFrame();
        for (int i=0;i<2;i++){
            gameObject.GetComponent<AtomProperties>().outerPoints[i+1].transform.Translate(0f,-(hydrogen_NoBond.transform.localScale.x-hydrogen_NoBond.GetComponent<AtomProperties>().bondOffsetRadius),0f);
        }
    }

    public IEnumerator deleteSelf(){
        moleculeManagement.carbons.Remove(gameObject);
        yield return new WaitForEndOfFrame();
        Destroy(gameObject);
    }
}
