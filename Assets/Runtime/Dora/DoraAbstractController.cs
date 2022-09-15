﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DoraAbstractController : MonoBehaviourBase
{
    [SerializeField] protected bool useRangeMarking = false;
    [SerializeField] DoraInputs inputs = null;
    [SerializeField] protected DoraCellSelector cellSelector = null;
    [SerializeField] KernelSpawner kernelSpawner = null;
    [SerializeField] DoraFrenzyController frenzyController = null;
    [SerializeField] DoraGameplayData DoraGameplayData = null;
    [SerializeField] UIDoraBiteAnimation biteAnimation = null;

    [Header("Score")]
    [SerializeField] DoraScoreManager scoreManager = null;
    [SerializeField] UIKernelManager uiKernelManager = null;
    [SerializeField] float animationTime = 0.5f;
    [SerializeField] float animationOffset = 10f;
    [SerializeField] float alphaTime = 0.2f;

    [Header("SFX")]
    [SerializeField] private AudioSource[] bigBiteSFXs = null;
    [SerializeField] private AudioSource smallBiteSFX = null;
    [SerializeField] private AudioSource chewSFX = null;
    [SerializeField] private AudioSource[] burntKernelSFXs = null;

    Coroutine eatingRoutine = null;
    protected Coroutine frenzyRoutine = null;
    AutoRotator autoRotator = null;
    int unburntEatenCount = 0;
    int selectedRadius = 0;

    private bool didGameplayEnd = false;

    protected DoraCellMap cellMap = null;

    #region UNITY AND CORE

    protected virtual void Start()
    {
        enableControllerUI(false);
        listenToInputs();
    }

    #endregion

    #region PUBLIC API

    public int CurrentSelectionRadius => selectedRadius;

    public int MaxSelectionRadius => null == cellSelector ? 1 : cellSelector.MaxSelectionRadius;

    public bool IsSelectingKernel()
    {
        if (null == cellSelector.CurrentOriginCell) return false;
        DoraCellData cell = cellMap.GetCell(cellSelector.CurrentOriginCell.Value, false, false);
        return cell.HasKernel;
    }

    public int UnburntEatenCount => unburntEatenCount;

    [ExposePublicMethod]
    public void EnableController()
    {
        Debug.LogError("Enable Controller");
        inputs.EnableInputs();
        enableControllerUI(true);
    }

    [ExposePublicMethod]
    public void DisableController(bool i_clearSelection = true)
    {
        inputs.DisableInputs();
        enableControllerUI(false);

        stopFrenzyMode();

        if (true == i_clearSelection)
            cellSelector.ClearSelection();
    }

    public void DisableMoveControls()
    {
        inputs.DisableMoveInputs();
    }

    public virtual void StartAutoRotation(bool i_setDefaultSpeed = true)
    {
        if (true == i_setDefaultSpeed) autoRotator?.SetRotationSpeedX(DoraGameplayData.DefaultRotationSpeed);
        autoRotator?.StartAutoRotation();
    }

    public virtual void StopAutoRotation()
    {
        autoRotator?.StopAutoRotation();
    }

    public void SetDoraComponents(DoraCellMap i_cellMap, AutoRotator i_autoRotator)
    {
        autoRotator = i_autoRotator;
        cellMap = i_cellMap;
        if (null == cellMap) DisableController();
        cellSelector.SetCellMap(cellMap);

        KernelSpawner kSpawner = i_cellMap.GetComponentInChildren<KernelSpawner>();
        if (kSpawner != null)
            kernelSpawner = kSpawner;
        else
            Debug.LogError("No kernel spawner attached to the provided cell map!");

        unburntEatenCount = 0;
    }

    public bool IsEating()
    {
        if (null == eatingRoutine) return false;
        else return true;
    }

    public void StopController()
    {
        didGameplayEnd = true;

        stopFrenzyMode();

        unlistenToInputs();
        StopAutoRotation();
        DisableController();
    }

    #endregion

    #region PROTECTED

    protected abstract void enableControllerUI(bool i_enable);

    protected abstract void move(Vector2 i_move);

    protected virtual void onEatStarted()
    {
        if (false == IsSelectingKernel()) return;

        if (frenzyRoutine == null) StopAutoRotation();
        selectedRadius = 0;

        inputs.DisableMoveInputs();
    }

    protected virtual void onEat()
    {
        Vector2Int? currentSelect = cellSelector.CurrentOriginCell;
        if (null == currentSelect) return;

        if (selectedRadius <= cellSelector.MaxSelectionRadius)
        {
            cellSelector.SelectRange(currentSelect.Value, selectedRadius, true, false, false);
            selectedRadius++;
        }
    }

    protected virtual void onEatReleased()
    {
        eatKernels();
    }

    #endregion

    #region PRIVATE

    void listenToInputs()
    {
        inputs.OnMoveStarted += onMoveStarted;
        inputs.OnMove += onMove;
        inputs.OnMoveReleased += onMoveReleased;

        inputs.OnEatStarted += onEatStarted;
        inputs.OnEat += onEat;
        inputs.OnEatReleased += onEatReleased;
    }

    void unlistenToInputs()
    {
        inputs.OnMoveStarted -= onMoveStarted;
        inputs.OnMove -= onMove;
        inputs.OnMoveReleased -= onMoveReleased;

        inputs.OnEatStarted -= onEatStarted;
        inputs.OnEat -= onEat;
        inputs.OnEatReleased -= onEatReleased;
    }

    private void onMoveStarted(Vector2 i_move) { move(i_move); }

    private void onMove(Vector2 i_move) { move(i_move); }

    private void onMoveReleased(Vector2 i_move) { }

    private void eatKernels()
    {
        if (null != eatingRoutine) return;
        if (null == cellSelector.CurrentOriginCell)
        {
            inputs.EnableInputs();
            return;
        }

        inputs.DisableInputs();

        DoraCellData cell = cellMap.GetCell(cellSelector.CurrentOriginCell.Value, false, false);
        if (false == cell.HasKernel) return;

        IReadOnlyList<HashSet<Vector2Int>> selectedKernelsInSteps = cellSelector.SelectedRangeInSteps;
        if (null == selectedKernelsInSteps)
        {
            Debug.LogError("Invalid selected cell steps list! Returning...");
            return;
        }

        int burntKenrelsCount = 0;
        int eatenKernels = 0;

        List<HashSet<DoraKernel>> kernelSets = new List<HashSet<DoraKernel>>();
        HashSet<DoraCellData> cellsToCleanup = new HashSet<DoraCellData>();

        bool startFrenzyMode = false;

        foreach (HashSet<Vector2Int> cellSet in selectedKernelsInSteps)
        {
            HashSet<DoraKernel> newSet = new HashSet<DoraKernel>();
            foreach (Vector2Int coord in cellSet)
            {
                cell = cellMap.GetCell(coord, false, false);

                if (cell.KernelStatus == KernelStatus.Burnt && frenzyRoutine != null)
                    continue;
                if (cell.HasKernel)
                {
                    newSet.Add(cell.Kernel);
                    if (cell.KernelStatus == KernelStatus.Burnt) burntKenrelsCount++;
                    eatenKernels++;
                    cellsToCleanup.Add(cell);

                    if (cell.KernelStatus == KernelStatus.Super)
                    {
                        startFrenzyMode = true;
                    }
                }
            }
            kernelSets.Add(newSet);
        }

        uiKernelManager.EnqueueKernels(getStackInfo(kernelSets));
        eatingRoutine = StartCoroutine(eatingSequence(cellsToCleanup, eatenKernels , burntKenrelsCount, startFrenzyMode));
    }

    private IEnumerator playBiteAnimation()
    {
        if (CurrentSelectionRadius == 0)
        {
            playSmallBiteSFX();
            yield break;
        }
        if (null != frenzyRoutine) yield break;

        biteAnimation?.Play();

        playRandomSoundFromArray(bigBiteSFXs);

        while (true == biteAnimation.IsPlaying)
            yield return null;

        while (bigBiteSFXs[0].isPlaying || bigBiteSFXs[1].isPlaying)
        {
            yield return null;
        }
        chewSFX.Play();
    }

    private void playSmallBiteSFX()
    {
        float randomPitch = Random.Range(1f, 2f);
        smallBiteSFX.pitch = randomPitch;
        Debug.LogError(randomPitch);
        smallBiteSFX.Play();
    }

    private void playRandomSoundFromArray(AudioSource[] i_audioSources)
    {
        int randomSFX = Random.Range(0, bigBiteSFXs.Length);
        i_audioSources[randomSFX]?.Play();
    }

    private IEnumerator eatingSequence(HashSet<DoraCellData> i_cellsToCleanup, int i_eatenKernel,int i_burntKernels, bool i_startFrenzy)
    {
        yield return StartCoroutine(playBiteAnimation());

        if (i_burntKernels > 0) playRandomSoundFromArray(burntKernelSFXs);

        foreach (DoraCellData cell in i_cellsToCleanup)
        {
            // cell.Eat();
            kernelSpawner.RequestKernelDespawn(cell.Kernel, false);
            cell.Reset();
        }

        unburntEatenCount += (i_eatenKernel - i_burntKernels);

        if (false == didGameplayEnd && null == frenzyRoutine)
            StartAutoRotation();

        inputs.EnableInputs();

        selectedRadius = 0;

        if (frenzyRoutine == null && true == i_startFrenzy)
            frenzyRoutine = StartCoroutine(startFrenzyMode());

        this.DisposeCoroutine(ref eatingRoutine);
    }


    private IEnumerator startFrenzyMode()
    {
        Debug.LogError("Start Frenzy Mode");

        inputs.DisableInputs();

        frenzyController.PlayFrenzyMode(autoRotator);

        uiKernelManager.ActivateFrenzy(true);

        yield return frenzyController.PlayFrenzyMode(autoRotator);

        inputs.EnableInputs();

        StartAutoRotation();

        stopFrenzyMode();
    }

    private void stopFrenzyMode()
    {
        this.DisposeCoroutine(ref frenzyRoutine);
        frenzyController.StopFrenzyMode();

        uiKernelManager.ActivateFrenzy(false);
    }

    private Queue<ScoreKernelInfo> getStackInfo(List<HashSet<DoraKernel>> i_eatenKernels)
    {
        Queue<ScoreKernelInfo> scoreKernels = new Queue<ScoreKernelInfo>();
        float multiplier = 1f;
        HashSet<DoraKernel> currSet = null;
        int length = i_eatenKernels.Count;
        for (int i = 0; i < length; i++)
        {
            currSet = i_eatenKernels[i];
            foreach (DoraKernel kernel in currSet)
            {
                scoreKernels.Enqueue(new ScoreKernelInfo(multiplier, kernel.Status));
            }
            multiplier += 1;
        }

        return scoreKernels;
    }
    #endregion
}
