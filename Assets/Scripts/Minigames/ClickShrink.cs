﻿using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using Utils;

namespace Minigames
{
    /// <summary>
    /// Tap items to make them shrink, shrink them completely to collect.
    /// </summary>
    public class ClickShrink : IMinigame
    {
        public Vector3 Epicentre { get; set; }
        public MinigameState State { get; set; }
        public MinigameState LastState { get; set; }
        public TextManager TextManager { get; set; }
        public GestureInfoManager GestureInfoManager { get; set; }
        public List<GameObject> Objects { get; set; }
        public GameObject AreaHighlight { get; set; }
        public GameObject Floor { get; set; }
        public int CollectedAmount { get; set; }
        public int Amount { get; set; }
        public int UniqueID { get; set; }
        public int TimeLeft { get; set; }
        public string ResourceName { get; set; }
        public string FileName { get; set; }
        public InteractSoundType SoundType { get; set; }
        public float GrowAmount { get; set; }

        /// <summary>
        /// Spawn in the resources, start the timer and show helpful text.
        /// </summary>
        void IMinigame.OnStart()
        {
            // Spawn in loads of trees and make them click to shrink.
            for (int i = 0; i < Amount; i++)
            {
                GameObject collectableObject = MonoBehaviour.Instantiate(Resources.Load("Objects/" + FileName, typeof(GameObject))) as GameObject;
                float distance = 2.5f;
                float xDist = Random.Range(-distance, distance);
                float zDist = Mathf.Sqrt(Mathf.Pow(distance, 2) - Mathf.Pow(xDist, 2.0f)) * (Random.Range(0, 2) * 2 - 1);
                collectableObject.transform.position = Epicentre + new Vector3(xDist, CameraCache.Main.transform.position.y - 0.3f, zDist);
                MyAnimation animation = collectableObject.AddComponent<MyAnimation>() as MyAnimation;
                animation.StartAnimation(Anims.grow, Vector3.zero, GrowAmount);
                HoloInteractive holoInteractive = collectableObject.AddComponent<HoloInteractive>() as HoloInteractive;
                holoInteractive.SetAttributes(InteractType.ClickShrink, 3, false, GrowAmount, SoundType);
                Objects.Add(collectableObject);
            }
            GestureInfoManager.RequestShowTapInfo();

            SoundManager soundManager = GameObject.Find("SoundManager").GetComponent(typeof(SoundManager)) as SoundManager;

            switch (SoundType)
            {
                case InteractSoundType.Chop:
                    soundManager.PlayChopSound();
                    break;
                case InteractSoundType.Mine:
                    soundManager.PlayShovelSound();
                    break;
                case InteractSoundType.Shovel:
                    soundManager.PlayShovelSound();
                    break;
                case InteractSoundType.Drip:
                    soundManager.PlayDripSound();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Check for updated state of objects.
        /// </summary>
        void IMinigame.OnUpdate()
        {
            foreach (var item in Objects)
            {
                HoloInteractive holoInteractive = item.GetComponent<HoloInteractive>();
                if (holoInteractive.interactState == InteractState.Hidden)
                {
                    Objects.Remove(item);
                    MonoBehaviour.Destroy(item);
                    CollectedAmount += 1;
                    return;
                }
                if (holoInteractive.interactState == InteractState.Touched)
                {
                    GestureInfoManager.RequestHide();
                }
            }
        }

        /// <summary>
        /// Any completion handling custom for the minigame.
        /// </summary>
        void IMinigame.OnComplete()
        {
        }
    }
}
