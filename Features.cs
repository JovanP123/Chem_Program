using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Features : MonoBehaviour
{
    
    public GameObject addDropdown;
    public GameObject selectedBond;
    public GameObject carbon_Sp3;
    public GameObject carbon_Sp2;
    public GameObject carbon_Sp;
    public TMP_Text informationText;
    public MoleculeManager mlclMngr;
    bool canGo = true;


    public void addAtom(){
        if (canGo){
            int selection = addDropdown.GetComponent<TMP_Dropdown>().value;
            if (selection==0 && selectedBond.GetComponent<SpThreeBondOccupation>()!=null && !selectedBond.GetComponent<SpThreeBondOccupation>().complete){
                addCSp3(selectedBond);
            }
            else if (selection==1 && !selectedBond.GetComponent<SpThreeBondOccupation>().partOfAnSp2Bond){
                addCSp2ToCSp3(selectedBond);
            }
            else if (selection==0 && selectedBond.GetComponent<SpThreeBondOccupation>().partOfAnSp2Bond){
                addCSp2ToCSp2(selectedBond);
            }
            else if (selection==2 && selectedBond.GetComponent<SpThreeBondOccupation>() != null && !selectedBond.GetComponent<SpThreeBondOccupation>().partOfAnSpBond && !selectedBond.GetComponent<SpThreeBondOccupation>().partOfAnSpBond){
                addCSpToCSp3(selectedBond);
            }
            else if (selection==0 && selectedBond.GetComponentInParent<SpBondOccupation>()!=null){
                addCSpToCSp(selectedBond);
            }
            canGo = false;
            StartCoroutine("wait");
        }
    }
    IEnumerator wait(){
        yield return new WaitForSeconds(0.1f);
        canGo = true;
    }
    //Add an SP3 hybridized carbon atom to the molecule
    void addCSp3(GameObject connectingBond){
        connectingBond.GetComponent<SpThreeBondOccupation>().outerPoint.transform.Translate(0f,(carbon_Sp3.transform.localScale.x/2)-carbon_Sp3.GetComponent<AtomProperties>().bondOffsetRadius,0f);
        GameObject currentCarbon = connectingBond.GetComponentInParent<AtomProperties>().gameObject;
        GameObject newCarbonSp3 = Instantiate(carbon_Sp3,connectingBond.GetComponent<SpThreeBondOccupation>().outerPoint.transform.position,Quaternion.Euler(connectingBond.GetComponent<SpThreeBondOccupation>().outerPoint.transform.eulerAngles));
        newCarbonSp3.transform.Rotate(new Vector3(180f,0f,0f));
        newCarbonSp3.GetComponent<AtomProperties>().chainPoint.GetComponent<SpThreeBondOccupation>().complete = true;
        newCarbonSp3.GetComponent<AtomProperties>().chainPoint.GetComponent<BondFeatures>().connectedTo = connectingBond.GetComponentInParent<AtomProperties>().gameObject;
        if (currentCarbon.GetComponent<AtomProperties>().atomType=="C(sp3)"){
            for (int i=0;i<4;i++){
                if (connectingBond == connectingBond.GetComponentInParent<AtomProperties>().bonds[i]){
                    connectingBond.GetComponentInParent<AtomProperties>().bonds[i] = newCarbonSp3.GetComponent<AtomProperties>().chainPoint;
                    connectingBond.GetComponentInParent<AtomProperties>().outerPoints[i] = newCarbonSp3.GetComponent<AtomProperties>().chainPoint.GetComponent<SpThreeBondOccupation>().innerPoint;
                    connectingBond.GetComponentInParent<AtomProperties>().innerPoints[i] = newCarbonSp3.GetComponent<AtomProperties>().chainPoint.GetComponent<SpThreeBondOccupation>().outerPoint;
                }
            }
        }
        else if (currentCarbon.GetComponent<AtomProperties>().atomType=="C(sp2)"){
            for (int i=1;i<=2;i++){
                if (connectingBond == connectingBond.GetComponentInParent<AtomProperties>().bonds[i]){
                    connectingBond.GetComponentInParent<AtomProperties>().bonds[i] = newCarbonSp3.GetComponent<AtomProperties>().chainPoint;
                    connectingBond.GetComponentInParent<AtomProperties>().outerPoints[i] = newCarbonSp3.GetComponent<AtomProperties>().chainPoint.GetComponent<SpThreeBondOccupation>().innerPoint;
                    connectingBond.GetComponentInParent<AtomProperties>().innerPoints[i] = newCarbonSp3.GetComponent<AtomProperties>().chainPoint.GetComponent<SpThreeBondOccupation>().outerPoint;
                }
            }
        }
        mlclMngr.carbons.Add(newCarbonSp3);
        selectedBond.GetComponent<BondFeatures>().bondFeaturesUI.SetActive(false);
        informationText.text = "";
        newCarbonSp3.SetActive(true);
        StartCoroutine("resetOuterPointOnBondAfterAddingCSp3", connectingBond.GetComponent<SpThreeBondOccupation>().outerPoint);

    }
    IEnumerator resetOuterPointOnBondAfterAddingCSp3(GameObject point){
        yield return new WaitForEndOfFrame();
        point.transform.Translate(0f,-((carbon_Sp3.transform.localScale.x/2)-carbon_Sp3.GetComponent<AtomProperties>().bondOffsetRadius),0f);
        yield return new WaitForEndOfFrame();
        selectedBond.SetActive(false);
    }
    //Add an sp2 hybrydized
    void addCSp2ToCSp3(GameObject selectedBond){
        GameObject currentCarbon = selectedBond.GetComponentInParent<AtomProperties>().gameObject;
        GameObject selectionOuterPoint = selectedBond.GetComponentInParent<SpThreeBondOccupation>().outerPoint;
        selectionOuterPoint.transform.Translate(0f,(carbon_Sp2.transform.localScale.x/2)-carbon_Sp2.GetComponent<AtomProperties>().bondOffsetRadius,0f);
        GameObject newCarbonSp2 = Instantiate(carbon_Sp2, selectionOuterPoint.transform.position, Quaternion.Euler(selectionOuterPoint.transform.eulerAngles));
        int randomNum = Random.Range(0,1);
        if (randomNum==0){
            newCarbonSp2.transform.Rotate(0f,0f,-60f);
        }
        else{
            newCarbonSp2.transform.Rotate(0,0f,60f);
        }
        newCarbonSp2.GetComponent<AtomProperties>().bonds[randomNum+1].GetComponent<SpThreeBondOccupation>().complete = true;
        newCarbonSp2.GetComponent<AtomProperties>().bonds[randomNum+1].GetComponent<BondFeatures>().connectedTo = currentCarbon;
        newCarbonSp2.GetComponent<AtomProperties>().bonds[randomNum+1].GetComponent<SpThreeBondOccupation>().complete = true;
        int selectedBondIndex = currentCarbon.GetComponent<AtomProperties>().bonds.IndexOf(selectedBond);
        currentCarbon.GetComponent<AtomProperties>().bonds[selectedBondIndex] = newCarbonSp2.GetComponent<AtomProperties>().bonds[randomNum+1];
        currentCarbon.GetComponent<AtomProperties>().innerPoints[selectedBondIndex] = newCarbonSp2.GetComponent<AtomProperties>().bonds[randomNum+1].GetComponent<SpThreeBondOccupation>().outerPoint;
        currentCarbon.GetComponent<AtomProperties>().outerPoints[selectedBondIndex] = newCarbonSp2.GetComponent<AtomProperties>().bonds[randomNum+1].GetComponent<SpThreeBondOccupation>().innerPoint;
        mlclMngr.carbons.Add(newCarbonSp2);
        newCarbonSp2.SetActive(true);
        StartCoroutine(afterFrameAddCSp2ToCSp3(selectionOuterPoint,selectedBond));
    }
    IEnumerator afterFrameAddCSp2ToCSp3(GameObject outerPoint, GameObject selectedBond){
        yield return new WaitForEndOfFrame();
        outerPoint.transform.Translate(0f,-((carbon_Sp2.transform.localScale.x/2)-carbon_Sp2.GetComponent<AtomProperties>().bondOffsetRadius),0f);
        yield return new WaitForEndOfFrame();
        selectedBond.SetActive(false);
    }
    void addCSp2ToCSp2(GameObject selectedBond){
        GameObject currentCarbon = selectedBond.GetComponentInParent<SpTwoBondOccupation>().GetComponentInParent<AtomProperties>().gameObject;
        GameObject doubleBond = selectedBond.GetComponentInParent<SpTwoBondOccupation>().gameObject;
        GameObject outerPoint = doubleBond.GetComponent<SpTwoBondOccupation>().outerPoint;
        outerPoint.transform.Translate(0f,(carbon_Sp2.transform.localScale.x/2)-carbon_Sp2.GetComponent<AtomProperties>().bondOffsetRadius,0f);
        GameObject newCarbonSp2 = Instantiate(carbon_Sp2, outerPoint.transform.position, Quaternion.Euler(outerPoint.transform.eulerAngles));
        newCarbonSp2.transform.Rotate(180f,0f,0f);
        currentCarbon.GetComponent<AtomProperties>().bonds[0] = newCarbonSp2.GetComponent<AtomProperties>().chainPoint;
        currentCarbon.GetComponent<AtomProperties>().chainPoint = newCarbonSp2.GetComponent<AtomProperties>().chainPoint;
        newCarbonSp2.SetActive(true);
        mlclMngr.carbons.Add(newCarbonSp2);
        StartCoroutine(afterFrameAddCSp2ToCSp2(outerPoint,selectedBond));
    }
    IEnumerator afterFrameAddCSp2ToCSp2(GameObject outerPoint, GameObject selectedBond){
        yield return new WaitForEndOfFrame();
        outerPoint.transform.Translate(0f,-((carbon_Sp2.transform.localScale.x/2)-carbon_Sp2.GetComponent<AtomProperties>().bondOffsetRadius),-0f);
        yield return new WaitForEndOfFrame();
        selectedBond.SetActive(false);
    }
    void addCSpToCSp3(GameObject selectedBond){
        GameObject currentCarbon = selectedBond.GetComponentInParent<SpThreeBondOccupation>().GetComponentInParent<AtomProperties>().gameObject;
        GameObject selectedBondOuterPoint = selectedBond.GetComponent<SpThreeBondOccupation>().outerPoint;
        selectedBondOuterPoint.transform.Translate(0f,(carbon_Sp.transform.localScale.x/2)-carbon_Sp.GetComponent<AtomProperties>().bondOffsetRadius,0f);
        GameObject newCarbonSp = Instantiate(carbon_Sp,selectedBondOuterPoint.transform.position,Quaternion.Euler(selectedBond.transform.eulerAngles));
        mlclMngr.carbons.Add(newCarbonSp);
        int selectedBondIndex = currentCarbon.GetComponent<AtomProperties>().bonds.IndexOf(selectedBond);
        currentCarbon.GetComponent<AtomProperties>().bonds[selectedBondIndex] = newCarbonSp.GetComponent<AtomProperties>().chainPoint;
        newCarbonSp.GetComponent<AtomProperties>().chainPoint.GetComponent<SpThreeBondOccupation>().complete = true;
        newCarbonSp.SetActive(true);
        StartCoroutine(afterFrameAddCSpToCSp3OrCSp2(selectedBond,selectedBondOuterPoint));
    }
    IEnumerator afterFrameAddCSpToCSp3OrCSp2(GameObject selectedBond,GameObject outerPoint){
        yield return new WaitForEndOfFrame();
        outerPoint.transform.Translate(0f,-((carbon_Sp.transform.localScale.x/2)-carbon_Sp.GetComponent<AtomProperties>().bondOffsetRadius),0f);
        yield return new WaitForEndOfFrame();
        selectedBond.SetActive(false);
    }
    void addCSpToCSp(GameObject selectedBond){
        GameObject currentCarbon = selectedBond.GetComponentInParent<SpBondOccupation>().GetComponentInParent<AtomProperties>().gameObject;
        GameObject tripleBondOuterPoint = selectedBond.GetComponentInParent<SpBondOccupation>().outerPoint;
        tripleBondOuterPoint.transform.Translate(0f,(carbon_Sp.transform.localScale.x/2)-carbon_Sp.GetComponent<AtomProperties>().bondOffsetRadius,0f);
        GameObject newCarbonSp = Instantiate(carbon_Sp,tripleBondOuterPoint.transform.position,Quaternion.Euler(tripleBondOuterPoint.transform.eulerAngles));
        newCarbonSp.transform.Rotate(180f,0f,0f);
        mlclMngr.carbons.Add(newCarbonSp);
        currentCarbon.GetComponent<AtomProperties>().bonds[0] = newCarbonSp.GetComponent<AtomProperties>().bonds[0];
        currentCarbon.GetComponent<AtomProperties>().innerPoints[0] = newCarbonSp.GetComponent<AtomProperties>().bonds[0].GetComponent<SpBondOccupation>().outerPoint;
        currentCarbon.GetComponent<AtomProperties>().outerPoints[0] = newCarbonSp.GetComponent<AtomProperties>().bonds[0].GetComponent<SpBondOccupation>().innerPoint;
        newCarbonSp.SetActive(true);
        newCarbonSp.GetComponent<AtomProperties>().bonds[0].GetComponent<SpBondOccupation>().complete = true;
        StartCoroutine(afterFrameAddCSpToCSp(selectedBond,tripleBondOuterPoint));
    }
    IEnumerator afterFrameAddCSpToCSp(GameObject selectedBond, GameObject tripleBondOuterPoint){
        yield return new WaitForEndOfFrame();
        tripleBondOuterPoint.transform.Translate(0f,-((carbon_Sp.transform.localScale.x/2)-carbon_Sp.GetComponent<AtomProperties>().bondOffsetRadius),0f);
        selectedBond.GetComponentInParent<SpBondOccupation>().gameObject.SetActive(false);
    }

}
