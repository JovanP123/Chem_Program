using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HydrogenFeatures : MonoBehaviour
{
    public MoleculeManager mlclMngmnt;
    public TMP_Text infoBarText;

    public void deleteSelf(){
        gameObject.GetComponent<AtomProperties>().bonds[0].GetComponent<SpThreeBondOccupation>().complete = false;
        infoBarText.text = "";
        StartCoroutine("deleteSelfAfterFrame");
    }
    IEnumerator deleteSelfAfterFrame(){
        mlclMngmnt.hydrogens.Remove(gameObject);
        yield return new WaitForEndOfFrame();
        Destroy(gameObject);
    }
}
