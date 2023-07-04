using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SelectedObject : MonoBehaviour
{
    public GameObject selectedObject;
    public TMP_Text infoDisplay;
    public Features featuresScript;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0)){
            Ray selectRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(selectRay, out hit, 100)){
                if (hit.transform.gameObject.layer == 7 && selectedObject == null){
                    selectedObject = hit.transform.gameObject;
                    infoDisplay.text = selectedObject.GetComponent<Information>().objectInfo;
                    if (selectedObject.GetComponent<AtomProperties>()!=null){
                        selectedObject.GetComponent<AtomProperties>().atomFeatures.SetActive(true);
                        StartCoroutine("setAddDropDownToNull");
                    }
                    else{
                        selectedObject.GetComponent<BondFeatures>().bondFeaturesUI.SetActive(true);
                        selectedObject.GetComponent<BondFeatures>().addButton.onClick.AddListener(() => featuresScript.addAtom());
                        featuresScript.selectedBond = selectedObject;
                        featuresScript.addDropdown = selectedObject.GetComponent<BondFeatures>().addDropdown;
                    }
                    selectedObject.layer = 6;
                }
                else if (hit.transform.gameObject.layer == 7){
                    StartCoroutine("changeShader", hit);
                }
            }
        }
    }
    IEnumerator setAddDropDownToNull(){
        yield return new WaitForEndOfFrame();
        featuresScript.addDropdown = null;
    }
    IEnumerator changeShader(RaycastHit objHit){
        selectedObject.layer = 7;
        if (selectedObject.GetComponent<AtomProperties>()!=null){
            selectedObject.GetComponent<AtomProperties>().atomFeatures.SetActive(false);
            StartCoroutine("setAddDropDownToNull");
        }
        else{
            selectedObject.GetComponent<BondFeatures>().bondFeaturesUI.SetActive(false);
        }
        yield return new WaitForEndOfFrame();
        selectedObject = objHit.transform.gameObject;
        infoDisplay.text = selectedObject.GetComponent<Information>().objectInfo;
        if (selectedObject.GetComponent<AtomProperties>()!=null){
            selectedObject.GetComponent<AtomProperties>().atomFeatures.SetActive(true);
        }
        else{
            selectedObject.GetComponent<BondFeatures>().bondFeaturesUI.SetActive(true);
            featuresScript.addDropdown = selectedObject.GetComponent<BondFeatures>().addDropdown;
            featuresScript.selectedBond = selectedObject;
        }
        selectedObject.layer = 6;
    }
}
