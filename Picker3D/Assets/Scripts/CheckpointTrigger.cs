using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    [HideInInspector]
    public bool isActive = true;
    private Checkpoint Checkpoint;

    void Awake()
    {
        Checkpoint = GetComponentInParent<Checkpoint>();
    }
    void OnTriggerEnter(Collider collider)
    {
        if(isActive == false)
            return;

        PickerController picker = collider.GetComponentInParent<PickerController>();
        
        if(picker != null)
        {
            picker.StopMoving();
            picker.PushCollectables();
            isActive = false;
            if(Checkpoint != null)
            {
                StartCoroutine(Checkpoint.StartCheckpointAnimation(LevelManager.LevelManagerInstance.CheckpointTimer));
            }
        }
    }
}
