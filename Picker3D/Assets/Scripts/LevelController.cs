using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
    public List<Checkpoint> Checkpoints;
    public int LevelNumber = 1;
    public Transform PickerStartTransf;
    private Checkpoint CurrentCheckpoint;
    private int CheckpointIndex = -1;
    [HideInInspector]
    public int ProgressbarCurrentIndex = 0;

    void Awake()
    {
        CheckpointIndex = -1;
        SetCheckpoint();
    }

    public void SetCheckpoint()
    {
        if(Checkpoints.Count > 0 && CheckpointIndex + 1 < Checkpoints.Count)
        {
            CheckpointIndex++;
            CurrentCheckpoint = Checkpoints[CheckpointIndex];
            ProgressbarCurrentIndex = CheckpointIndex;
        }
        else if(CheckpointIndex + 1  == Checkpoints.Count)
        {
            //print("FINISH LEVEL ");
            ProgressbarCurrentIndex = CheckpointIndex  + 1;
            StartCoroutine(DelayLevelSet());
        }
    }

    public void ResetCheckpoints()
    {
        if(Checkpoints.Count > 0 )
        {
            for (int i = 0; i < Checkpoints.Count; i++)
            {
                Checkpoints[i].ResetCheckpoint();
            }
        }
    }

    IEnumerator DelayLevelSet()
    {
        yield return new WaitForSeconds(LevelManager.LevelManagerInstance.LevelSetDelay);
        LevelManager.LevelManagerInstance.EnableProgresbar(false);
        LevelManager.LevelManagerInstance.SetNextLevel();
    }
    IEnumerator ShowProgressbarWithDelay(bool flag)
    {
        yield return new WaitForSeconds(LevelManager.LevelManagerInstance.ProgressbarSetDelay);
        LevelManager.LevelManagerInstance.EnableProgresbar(flag);
    }
    public List<Collectable> GetCheckpointCollectables()
    {
         //print("CheckpointIndex "+ CheckpointIndex);
        if(CheckpointIndex < Checkpoints.Count && CheckpointIndex >= 0)
        {
            if(Checkpoints[CheckpointIndex]!= null)
            {
                return Checkpoints[CheckpointIndex].Collectables;
            }
        }
        return null;
    }

    public void UpdateProgressbar(int checkpointCount, int currentCheckpoint)
    {
        currentCheckpoint = Mathf.Max(currentCheckpoint, 0);
        
        List<Image> checkpointImages = LevelManager.LevelManagerInstance.CheckpointBars;
        if(checkpointImages != null)
        {
            for (int i = 0; i < checkpointImages.Count; i++)
            {
                if(i < currentCheckpoint)
                {
                    checkpointImages[i].material = LevelManager.LevelManagerInstance.FullBarMat;
                }
                else
                {
                    checkpointImages[i].material = LevelManager.LevelManagerInstance.EmptyBarMat;
                }
            }
        }
        //print("currentCheckpoint: "+ currentCheckpoint + "  checkpointCount : "+ checkpointCount);
    }

    public int GetProgressbarIndex()
    {
        return ProgressbarCurrentIndex;
    }

    public void SetProgressbarIndex(int val)
    {
        ProgressbarCurrentIndex = val;
    }

    public void  SetCheckpointIndex(int val)
    {
        CheckpointIndex = val;
    }
}
