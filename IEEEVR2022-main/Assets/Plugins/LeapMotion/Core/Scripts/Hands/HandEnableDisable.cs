/******************************************************************************
 * Copyright (C) Ultraleap, Inc. 2011-2020.                                   *
 *                                                                            *
 * Use subject to the terms of the Apache License 2.0 available at            *
 * http://www.apache.org/licenses/LICENSE-2.0, or another agreement           *
 * between Ultraleap and you, your company or other organization.             *
 ******************************************************************************/

using UnityEngine;
using System.Collections;
using System;
using Leap;
using UnityEngine.WSA;

namespace Leap.Unity
{
    public class HandEnableDisable : HandTransitionBehavior
    {

        [HideInInspector]
        public bool activated = true;
        public bool displayed = false;

        protected override void Awake()
        {
            // Suppress Warnings Related to Kinematic Rigidbodies not supporting Continuous Collision Detection
#if UNITY_2018_3_OR_NEWER
            Rigidbody[] bodies = GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody body in bodies)
            {
                if (body.isKinematic && body.collisionDetectionMode == CollisionDetectionMode.Continuous)
                {
                    body.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                }
            }
#endif

            base.Awake();
            gameObject.SetActive(false);
        }

        protected override void HandReset()
        {
            displayed = true;
            if (activated)
                gameObject.SetActive(true);
        }

        protected override void HandFinish()
        {
            displayed = false;
            gameObject.SetActive(false);
        }

        public void activate(bool v)
        {
            activated = v;
            gameObject.SetActive(v && displayed);
        }
    }
}