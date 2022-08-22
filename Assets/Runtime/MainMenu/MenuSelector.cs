using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSelector : MonoBehaviourBase
{
    enum MiniGame { Harankash =1 , Dora=2 }

    #region Serialized Fields
        [SerializeField] Controls controls;
        [SerializeField] MiniGame miniGame;
        [SerializeField] Transform[] gameNames;
        [SerializeField] Transform indicator;
    #endregion

    #region Enable/Disable controls
        private void OnEnable()
        {
            controls.MoveStarted += MoveSelector;
            controls.JumpPressed += SelectGame;
        }


        private void OnDisable()
        {
            controls.MoveStarted -= MoveSelector;
            controls.JumpPressed -= SelectGame;
        }
    #endregion

    #region Main Methods
        private void MoveSelector()
        {
            switch (miniGame)
            {
                case MiniGame.Harankash:
                    miniGame = MiniGame.Dora;
                    MoveIndicator(gameNames[1]);
                    break;
                case MiniGame.Dora:
                    miniGame = MiniGame.Harankash;
                    MoveIndicator(gameNames[0]);
                    break;
                default:
                    break;
            }
        }

        private void SelectGame()
        {
            SelectScene((int)miniGame);
        }
    #endregion

    #region Sub-methods
        void MoveIndicator(Transform i_selectedGame)
        {
            
            indicator.position = new Vector3(i_selectedGame.position.x, indicator.position.y, indicator.position.z);
        }
        void SelectScene(int i_sceneIndex)
        {
            SceneManager.LoadScene(i_sceneIndex);
        }
    #endregion
}
