using UnityEngine;
using UnityEngine.Events;

public class UIDoraEndGameButton : MonoBehaviourBase, ISelectable
{
    [SerializeField] GameObject selected = null;
    [SerializeField] GameObject unselected = null;
    [SerializeField] AudioSource selectionSFX = null;
    [SerializeField] UnityEvent onPressed = null;

    bool isSelected = false;

    #region UNITY AND CORE

    void OnEnable()
    {
        updateSelectionVisuals();
    }

    #endregion

    #region PUBLIC API

    public void TriggerCallback()
    {
        if (false == isSelected) return;
        onPressed?.Invoke();
    }

    #endregion

    #region ISelectable

    public bool IsSelected => isSelected;

    public void Select(bool i_animated)
    {
        if (false == isSelected) playSelectionSFX();

        isSelected = true;
        updateSelectionVisuals();
    }

    public void Unselect(bool i_animated)
    {
        isSelected = false;
        updateSelectionVisuals();
    }

    #endregion

    #region PRIVATE

    void updateSelectionVisuals()
    {
        selected.SetActive(isSelected);
        unselected.SetActive(!isSelected);
    }

    void playSelectionSFX()
    {
        selectionSFX?.Play();
    }


    #endregion

}
