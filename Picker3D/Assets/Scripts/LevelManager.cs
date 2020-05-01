using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public float CheckpointTimer = 4;
    public float LevelLength = 290;
    public float LevelSetDelay = 1.5f;
    public float ProgressbarSetDelay = 6f;
    public List<LevelController> Levels;
    [Header("UI References")]
    public Text CurrentDiamonAmountText;
    public Text GainedDiamonAmountText;
    public Text CurrentLevelNumberText;
    public Text NextLevelNumberText;
    public List<Image> CheckpointBars;
    public GameObject ProgressBar;
    public GameObject MenuUI;
    public Button CollectBtn;
    public Button ResumeBtn;
    public Material FullBarMat;
    public Material EmptyBarMat;
    
    private LevelController CurrentLevel;
    private int LevelIndex = -1;
    [HideInInspector]
    public int CollectedAmountByLevel = 0;
    [HideInInspector]
    public int CollectedAmountTotal = 0;

    private bool ShowProgressbar = false;
    public GameObject DiamondGameobject;
    private List<GameObject> DiamondAnimList;

    private static LevelManager levelManagerInstance;

    public static LevelManager LevelManagerInstance{ get {return levelManagerInstance;}}

    public float DiamondAnimSpeed = 5f;
    private Camera Cam;

    void Awake()
    {
        Cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();;
        DiamondAnimList = new List<GameObject>();
        LevelIndex = -1;
        if(levelManagerInstance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            levelManagerInstance = this;
        }

        SetNextLevel();
        EnableProgresbar(true);
    }


    void Update()
    {
        if(ShowProgressbar == true && CurrentLevel != null)
        {
            if(CurrentLevel.Checkpoints != null)
                CurrentLevel.UpdateProgressbar(CurrentLevel.Checkpoints.Count, CurrentLevel.GetProgressbarIndex());
        }

        if(CurrentDiamonAmountText)
        {
            CurrentDiamonAmountText.text = CollectedAmountTotal.ToString();
        }
        if(GainedDiamonAmountText)
        {
            GainedDiamonAmountText.text = CollectedAmountByLevel.ToString();
        }

        if(DiamondAnimList.Count > 0)
        {
            PlayDiamondAnim();
        }
    }
    public void SetNextLevel()
    {
        LevelController tempLevel = CurrentLevel;
        if(SetLevel() == true)
        {
            if(tempLevel != null)
            {
                CurrentLevel.transform.position = new Vector3(tempLevel.transform.position.x, tempLevel.transform.position.y, tempLevel.transform.position.z + LevelLength);
            }
            else
            {
                CurrentLevel.transform.position = Vector3.zero;
            }
            CurrentLevel.gameObject.SetActive(true);
        }
    }

    public bool SetLevel()
    {
        if(Levels.Count > 0 && LevelIndex + 1 < Levels.Count)
        {
            if(CurrentLevel != null)
                Destroy(CurrentLevel.gameObject, 20);

            LevelIndex++;
            CurrentLevel = Instantiate(Levels[LevelIndex], Vector3.zero, Levels[LevelIndex].transform.rotation) ;
            CurrentLevel.gameObject.SetActive(false);
            CurrentLevelNumberText.text = CurrentLevel.LevelNumber.ToString();

            if(LevelIndex + 1 < Levels.Count)
            {
                NextLevelNumberText.text = Levels[LevelIndex + 1].LevelNumber.ToString();
            }
            else
            {
                NextLevelNumberText.text = (LevelIndex + 2).ToString();
            }
            return true;
        }
        else if(Levels.Count > 0)
        {
             if(CurrentLevel != null)
                Destroy(CurrentLevel.gameObject, 20);

            LevelIndex = 0;
            CurrentLevel = Instantiate(Levels[LevelIndex], Vector3.zero, Levels[LevelIndex].transform.rotation) ;
            CurrentLevel.gameObject.SetActive(false);
            CurrentLevelNumberText.text = CurrentLevel.LevelNumber.ToString();

            if(LevelIndex + 1 < Levels.Count)
            {
                NextLevelNumberText.text = Levels[LevelIndex + 1].LevelNumber.ToString();
            }
            else
            {
                NextLevelNumberText.text = (LevelIndex + 2).ToString();
            }
            return true;
        }
        else{
            
            return false;
        }

    }

    public LevelController GetCurrentLevel()
    {
        if(CurrentLevel != null)
        {
            return CurrentLevel;
        }
        else
        {
            return null;
        }
    }

    public bool SetNextCheckpointCollectables()
    {
        if(CurrentLevel != null)
        {
            CurrentLevel.SetCheckpoint();
            return true;
        }

        return false;
    }

    public void EnableProgresbar(bool flag)
    {
        ShowProgressbar = flag;
        ProgressBar.SetActive(flag);
    } 
    public void StartNewLevelUI()
    {
        countedNum = 0;
        EnableProgresbar(true);
        MenuUI.SetActive(true);
        CollectBtn.gameObject.SetActive(true);
        GainedDiamonAmountText.text = CollectedAmountByLevel.ToString();
    }

    public void ResumeTheGame()
    {
        StopAllCoroutines();

        CollectedAmountTotal += DiamondAnimList.Count - countedNum + CollectedAmountByLevel;
        CollectedAmountByLevel = 0;

        ClearDiamondList();
        MenuUI.SetActive(false);
        PickerController.PickerInstance.StartMoving();
    }

    public void CollectDiamonds()
    {
        StartCoroutine(CollectDiamondsAnimation(0.05f));
    }

    IEnumerator CollectDiamondsAnimation(float secs)
    {
        CollectBtn.gameObject.SetActive(false);
        int total = CollectedAmountByLevel;
        for (int i = 0; i < total; i++)
        {
            CollectedAmountByLevel--;
            DiamondAnimList.Add(Instantiate(DiamondGameobject, GainedDiamonAmountText.transform.parent.position, Quaternion.identity, GainedDiamonAmountText.transform.parent.parent));
            yield return new WaitForSeconds(secs);
        }
    }

    int countedNum;
    private void PlayDiamondAnim()
    {
        Vector3 rotationVec = new Vector3(0,0,5);
        for (int i = 0; i < DiamondAnimList.Count; i++)
        {
            if(DiamondAnimList[i].gameObject.activeSelf == false)
            continue;

            Vector3 targetPoint = Cam.WorldToScreenPoint(CurrentDiamonAmountText.transform.parent.position);
            Vector3 startPoint = Cam.WorldToScreenPoint(DiamondAnimList[i].transform.position);
            Vector3 dirr = startPoint - targetPoint; 
            dirr.z = DiamondAnimList[i].transform.position.z;

            DiamondAnimList[i].transform.Rotate(rotationVec, Space.Self);
            
            if(dirr.magnitude < 5f)
            {
                countedNum++;
                CollectedAmountTotal++; 
                DiamondAnimList[i].gameObject.SetActive(false);
                continue;
            }
            DiamondAnimList[i].transform.Translate(dirr.normalized*DiamondAnimSpeed*Time.deltaTime, Space.World);
        }
    }
    public void ClearDiamondList()
    {
        for (int i = 0; i < DiamondAnimList.Count; i++)
        {
            Destroy(DiamondAnimList[i]);
        }
        DiamondAnimList.Clear();
    }

}
