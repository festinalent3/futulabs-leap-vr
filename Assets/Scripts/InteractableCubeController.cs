﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity;
using Leap.Unity.Interaction;

namespace Futulabs
{

    public class InteractableCubeController : InteractableObjectControllerBase
    {
        private bool _isSticky = false;
        private bool _isStuck = false;
        [SerializeField]
        private ImpactLightController LightControllerPrefab;

        public override float WallImpactLightIntensityMultiplier
        {
            get
            {
                return _isSticky ? 12.0f : 1.0f;
            }
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public override void Materialize()
        {
            base.Materialize();
            _isSticky = GameManager.Instance.StickyCubes;
            if (_isSticky)
                _outlineMeshes[0].sharedMaterial = SettingsManager.Instance.StickyOutlineMaterial;
        }


        protected override void OnCollisionEnter(Collision collision)
        {
            if (!_isSticky)
                base.OnCollisionEnter(collision);
            else
                if (collision.gameObject.tag.Equals("Wall"))
                Stick();
        }

        private void Stick()
        {
            _isStuck = true;
            transform.rotation = Quaternion.identity;
            Rigidbodies[0].isKinematic = true;
            gameObject.tag = "Wall";
            EffectAudioSource.PlayOneShot(AudioManager.Instance.GetAudioClip(GameAudioClipType.INTERACTABLE_OBJECT_STICK));
            gameObject.layer = LayerMask.NameToLayer("Environment");
            StartCoroutine(DelayAddScript());
        }

        IEnumerator DelayAddScript()
        {
            yield return new WaitForEndOfFrame();
            LightWallController wallScript = gameObject.AddComponent<LightWallController>();
            wallScript.LightPrefab = LightControllerPrefab;
        }
    }

}