using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickerController : MonoBehaviour
{
    public float ForwardSpeed = 10f;
    public float SideSpeed = 0.1f;
    public float PushForceOnCheckpoint = 1;
    public Material GreenMat;
    public BoxCollider BoundsCollider;
    public static PickerController PickerInstance{ get {return pickerInstance;}}
    private static PickerController pickerInstance;
    private Rigidbody RgdBody;
    private Vector3 TempVector;
    private bool EnableMoving = true;
    private Vector3 ForwardDirection;

    void Awake()
    {
        ForwardDirection = transform.up;
        RgdBody = GetComponent<Rigidbody>();
        if(pickerInstance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            pickerInstance = this;
        }
    }
    void Update()
    {
        CheckIfCollectablesInside();
    }

    void FixedUpdate()
    {
        if(EnableMoving == true)
        {
            MoveForwardWithPhysics();
        }
        else
        {
            TempVector = RgdBody.velocity;
            TempVector.y = 0;
            TempVector.z = 0;
            RgdBody.velocity = TempVector;
        }

        MoveSideWithPhysics();
    }

    private void CheckIfCollectablesInside()
    {
        LevelController lvl = LevelManager.LevelManagerInstance.GetCurrentLevel();
        print("lvl.Levelnu "+ lvl.LevelNumber);
        if(lvl == null)
        return;
    
        List<Collectable> collectables = lvl.GetCheckpointCollectables();

        if(collectables == null)
        return;

        if(BoundsCollider != null)
        {
            
            for (int i = 0; i < collectables.Count; i++)
            {
                if(BoundsCollider.bounds.Contains(collectables[i].transform.position) == true)
                {
                    MeshRenderer renderer = collectables[i].GetComponent<MeshRenderer>();
                    if(renderer != null && GreenMat != null)
                    {
                        renderer.material = GreenMat;
                    }
                }
            } 
            
        }
    }

    public void PushCollectables()
    {
        LevelController lvl = LevelManager.LevelManagerInstance.GetCurrentLevel();

        if(lvl == null)
        return;
    
        List<Collectable> collectables = lvl.GetCheckpointCollectables();

        if(collectables == null)
        return;

        if(BoundsCollider != null)
        {
            foreach (Collectable item in collectables)
            {
                if(BoundsCollider.bounds.Contains(item.transform.position) == true)
                {
                    Rigidbody itemRgd = item.GetComponent<Rigidbody>();
                    if(itemRgd != null)
                    {
                        itemRgd.AddForce(ForwardDirection*PushForceOnCheckpoint* 0.0001f, ForceMode.Force);
                    }
                }
            }
        }
    }
    void MoveForwardWithPhysics()
    {
        RgdBody.velocity = ForwardDirection*ForwardSpeed;
        //rigidBody.AddForce(transform.up*ForwardSpeed, ForceMode.Force);
        //transform.Translate(Vector3.up*ForwardSpeed, Space.Self);
        
    }

    void MoveSideWithPhysics()
    {
        if(Input.GetKey(KeyCode.LeftArrow))
        {
            TempVector = RgdBody.velocity;
            TempVector.x = SideSpeed;
            RgdBody.velocity = TempVector;
            // transform.Translate(Vector3.right*SideSpeed, Space.Self);
        }
        else
        {
            if(Input.GetKey(KeyCode.RightArrow))
            {
                TempVector = RgdBody.velocity;
                TempVector.x = SideSpeed*-1;
                RgdBody.velocity = TempVector;
                // transform.Translate(Vector3.right*SideSpeed, Space.Self);
            }
            else
            {
                TempVector = RgdBody.velocity;
                TempVector.x = 0;
                RgdBody.velocity = TempVector;
            }
        }
    }

    public void StopMoving()
    {
        EnableMoving = false;
    }

    public void StartMoving()
    {
        EnableMoving = true;
    }
}
