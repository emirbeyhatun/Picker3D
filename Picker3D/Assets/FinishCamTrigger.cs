using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishCamTrigger : MonoBehaviour
{
    public bool isIn = true;
    private bool isActive = true;
    void OnTriggerEnter(Collider collider)
    {
        PickerController picker = collider.GetComponentInParent<PickerController>();
        
        if(picker != null && isActive == true)
        {
            if(isIn == true)
            {
                Camera.main.GetComponent<Animator>().SetBool("GoUp", false);
                Camera.main.GetComponent<Animator>().SetBool("GoDown", true);
                isActive = false;
            }
            else
            {
                Camera.main.GetComponent<Animator>().SetBool("GoDown", false);
                Camera.main.GetComponent<Animator>().SetBool("GoUp", true);
                picker.StopMoving();
                LevelManager.LevelManagerInstance.StartNewLevelUI();
                isActive = false;
            }
        }
    }
}
