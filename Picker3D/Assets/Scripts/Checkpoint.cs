using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Checkpoint : MonoBehaviour
{
    public List<BoxCollider> SideColliders;
    public BoxCollider BoundsCollider;
    public int CollectablesLimit;
    private int CollectablesCurrentCount;
    public Text CollectableInfoText;
    [HideInInspector]
    public List<Collectable> Collectables;
    public GameObject CollectablePrefab;
    public Transform CollectableSpawnPos;
    private Animator GateAnimator;
    public GameObject NewPlatform;
    [HideInInspector]
    public CheckpointTrigger Trigger;

    void Awake()
    {
        Collectables = new List<Collectable>();
        GateAnimator = GetComponentInChildren<Animator>();
        Trigger = GetComponentInChildren<CheckpointTrigger>();
        InitiateCollectables();
    }
    void Update()
    {
        CheckIfCollectablesInside();
        UpdateUI();
    }

    void InitiateCollectables()
    {
        GameObject collectables = Instantiate(CollectablePrefab, CollectableSpawnPos.position, CollectableSpawnPos.rotation, CollectableSpawnPos);
        Collectable[] collectableCollection = collectables.GetComponentsInChildren<Collectable>();
        Collectables.AddRange(collectableCollection);
    }
    void DestroyCollectables()
    {
        foreach (Collectable item in Collectables)
        {
            Destroy(item.gameObject);
        }
        Collectables.Clear();
    }
    private void CheckIfCollectablesInside()
    {
        LevelController lvl = LevelManager.LevelManagerInstance.GetCurrentLevel();

        if(lvl == null)
        return;
    
        List<Collectable> collectables = lvl.GetCheckpointCollectables();

        if(collectables == null)
        return;

        int count = 0;
        if(BoundsCollider != null)
        {
            for (int i = 0; i < collectables.Count; i++)
            {
                if(BoundsCollider.bounds.Contains(collectables[i].transform.position) == true)
                {
                    count ++;
                }
            } 
        }
        CollectablesCurrentCount = count;
    }

    private void UpdateUI()
    {
        if(CollectableInfoText!= null)
        {
            CollectableInfoText.text = CollectablesCurrentCount + " / " + CollectablesLimit;
        }
    }
    public IEnumerator StartCheckpointAnimation(float timer)
    {
        yield return new WaitForSeconds(timer*0.4f);

        if(CollectablesCurrentCount < CollectablesLimit)
        {
            LevelManager.LevelManagerInstance.ResetLevel();
            yield break;
        }
        else
        {
            DisableSideColliders(false);
        }

        yield return new WaitForSeconds(timer*0.25f);
        SetNewPlatformAnimation();
        yield return new WaitForSeconds(timer*0.35f);

        LevelManager.LevelManagerInstance.CollectedAmountByLevel += CollectablesCurrentCount;
        //SetNewPlatformColliders();
        GateAnimator.SetBool("OpenGate", true);
        LevelManager.LevelManagerInstance.SetNextCheckpointCollectables();
        PickerController.PickerInstance.StartMoving();
    }
    public void ResetCheckpoint()
    {
        DestroyCollectables();
        InitiateCollectables();
        DisableSideColliders(true);
        NewPlatform.GetComponent<Animator>().Rebind();
        NewPlatform.SetActive(false);
        GateAnimator.Rebind();
        Trigger.isActive = true;
    } 
    public void DisableSideColliders(bool flag)
    {
        for (int i = 0; i < SideColliders.Count; i++)
        {
            SideColliders[i].enabled = flag;
        }
    }

    void SetNewPlatformAnimation()
    {
        NewPlatform.SetActive(true);
        NewPlatform.GetComponent<Animator>().SetBool("LiftPlatform", true);
    }
    void SetNewPlatformColliders()
    {
        //NewPlatform.transform.localPosition = new Vector3(NewPlatform.transform.localPosition.x, NewPlatform.transform.localPosition.y + 2, NewPlatform.transform.localPosition.z);
        MeshCollider meshCld = NewPlatform.GetComponentInChildren<MeshCollider>();
        if(meshCld)
        {
            meshCld.isTrigger = false;
            meshCld.convex = false;
        }
        BoxCollider[] bxCldrs = NewPlatform.GetComponentsInChildren<BoxCollider>();
        if(bxCldrs != null)
        {
            for (int i = 0; i < bxCldrs.Length; i++)
            {
                bxCldrs[i].isTrigger = false;
            }
        }
    }

}
