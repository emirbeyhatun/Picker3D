using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickerController : MonoBehaviour
{
    public float ForwardSpeed = 10f;
    public float SideSpeedWithPhys = 15f;
    public float SideSpeedPhyTargetSpeed = 6f;
    public float SideSpeedNoPhys = 3;
    public float PushForceOnCheckpoint = 1;
    public float XposLimitNoPhys = 1.5f;
    public float XposLimitPhys = 3.6f;
    public Material GreenMat;
    public BoxCollider BoundsCollider;
    public Transform SideMovementParent;
    public Transform PhyTargetTransform;
    public static PickerController PickerInstance{ get {return pickerInstance;}}
    private static PickerController pickerInstance;
    private Rigidbody RgdBody;
    private Vector3 TempVector;
    private bool EnableMoving = true;
    public bool SideMovementWithPhysics = false;
    private Vector3 ForwardDirection;
    private Vector3 FirstTouchPos;
    private float TouchXDiff;
    private float TouchXDiffClamped;
    private bool IsTouching = false;
    private float TouchXpos;
    private float TouchMoveXDirection; 

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

        MoveForward();        
        MoveSide();
    }
    void MoveForward()
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
    }

    void MoveSide()
    {
        UpdateTouchControls();        
        MoveSideWithPhysics();

        // if(Input.GetKey(KeyCode.LeftArrow))
        // {
        //     TempVector = RgdBody.velocity;
        //     TempVector.x = SideSpeedWithPhys;
        //     RgdBody.velocity = TempVector;
        //      //transform.Translate(Vector3.right*SideSpeedWithPhys, Space.Self);
        // }
        // else
        // {
        //     if(Input.GetKey(KeyCode.RightArrow))
        //     {
        //         TempVector = RgdBody.velocity;
        //         TempVector.x = SideSpeedWithPhys*-1;
        //         RgdBody.velocity = TempVector;
        //          //transform.Translate(Vector3.right*SideSpeedWithPhys, Space.Self);
        //     }
        //     else
        //     {
        //         TempVector = RgdBody.velocity;
        //         TempVector.x = 0;
        //         RgdBody.velocity = TempVector;
        //     }
        // }
    }

    private void UpdateTouchControls()
    {
        foreach(Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                TempVector = touch.position;
                TempVector.x += TouchXDiffClamped;
                FirstTouchPos = TempVector;
                IsTouching = true;
            }

            if (touch.phase == TouchPhase.Ended)
            {
                IsTouching = false;
            }

            if(IsTouching == true)
            {
                TouchXDiff = FirstTouchPos.x - touch.position.x;
            }

            TouchXpos = touch.position.x;
        }
    
    }

    private void CheckIfCollectablesInside()
    {
        LevelController lvl = LevelManager.LevelManagerInstance.GetCurrentLevel();
        //print("lvl.Levelnu "+ lvl.LevelNumber);
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
        if(SideMovementParent)
        {
            if(SideMovementWithPhysics == false  )
            {
                if(IsTouching == true)
                {
                    float clampedDif = (TouchXDiff/(Screen.width / 2))*SideSpeedNoPhys;
                    if(Mathf.Abs(clampedDif) <= XposLimitNoPhys)
                    {
                        TouchXDiffClamped = TouchXDiff;
                    }
                    float newXpos = Mathf.Clamp(clampedDif, -XposLimitNoPhys, XposLimitNoPhys);
                    TempVector = SideMovementParent.transform.localPosition;
                    TempVector.x = newXpos;
                    SideMovementParent.localPosition = TempVector;
                }
            }
            else
            {
                TempVector = transform.position;
                TempVector.x = PhyTargetTransform.position.x;
                PhyTargetTransform.position = TempVector;

                if(IsTouching == true)
                {
                    
                    float clampedDif = (TouchXDiff/(Screen.width / 2))*SideSpeedPhyTargetSpeed;
                    if(Mathf.Abs(clampedDif) <= XposLimitPhys)
                    {
                        TouchXDiffClamped = TouchXDiff;
                    }
                    float newXpos = Mathf.Clamp(clampedDif, -XposLimitPhys, +XposLimitPhys);


                    TempVector = PhyTargetTransform.transform.position;
                    TempVector.x = -newXpos;
                    PhyTargetTransform.position = TempVector;

                    if(Mathf.Abs(transform.position.x) <= XposLimitPhys)
                    {
                        float xDir = PhyTargetTransform.position.x - transform.position.x;
                        TempVector = RgdBody.velocity;
                        TempVector.x = xDir*SideSpeedWithPhys;
                        RgdBody.velocity = TempVector;

                    }
                    else
                    {
                        TempVector = RgdBody.velocity;
                        TempVector.x = 0;
                        RgdBody.velocity = TempVector;
                    }
                }
                else
                {
                    TempVector = RgdBody.velocity;
                    TempVector.x = 0;
                    RgdBody.velocity = TempVector;
                }
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
