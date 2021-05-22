/******************************************************************************
 * Copyright (C) Ultraleap, Inc. 2011-2020.                                   *
 *                                                                            *
 * Use subject to the terms of the Apache License 2.0 available at            *
 * http://www.apache.org/licenses/LICENSE-2.0, or another agreement           *
 * between Ultraleap and you, your company or other organization.             *
 ******************************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Leap;
using Leap.Unity.Attributes;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Leap.Unity
{

    /// <summary> A skinned and jointed 3D HandModel. </summary>
    public class RiggedHand : HandModel
    {

        [Header("Custom Parameters")]
        public int handModel = 0;
        int prevHandModel = -1000;
        public GameObject plane;
        public GameObject mesh;
        public GameObject secondHand;
        public bool offset = false;
        public bool shouldUpdateSecond = false;
        public bool main = false;
        public bool example = false;
        public float offsetVal;



        public override ModelType HandModelType { get { return ModelType.Graphics; } }
        public override bool SupportsEditorPersistence() { return true; }

        [SerializeField]
        [FormerlySerializedAs("DeformPositionsInFingers")]
        [OnEditorChange("deformPositionsInFingers")]
        private bool _deformPositionsInFingers = true;
        public bool deformPositionsInFingers
        {
            get { return _deformPositionsInFingers; }
            set
            {
                _deformPositionsInFingers = value;
                updateDeformPositionsInFingers();
            }
        }

        [Tooltip("Because bones only exist at their roots in model rigs, the length " +
          "of the last fingertip bone is lost when placing bones at positions in the " +
          "tracked hand. " +
          "This option scales the last bone along its X axis (length axis) to match " +
          "its bone length to the tracked bone length. This option only has an " +
          "effect if Deform Positions In Fingers is enabled.")]
        [DisableIf("_deformPositionsInFingers", isEqualTo: false)]
        [SerializeField]
        [OnEditorChange("scaleLastFingerBones")]
        private bool _scaleLastFingerBones = true;
        public bool scaleLastFingerBones
        {
            get { return _scaleLastFingerBones; }
            set
            {
                _scaleLastFingerBones = value;
                updateScaleLastFingerBoneInFingers();
            }
        }

        [Tooltip("Hands are typically rigged in 3D packages with the palm transform near the wrist. Uncheck this if your model's palm transform is at the center of the palm similar to Leap API hands.")]
        [FormerlySerializedAs("ModelPalmAtLeapWrist")]
        public bool modelPalmAtLeapWrist = true;

        [Tooltip("Set to True if each finger has an extra trasform between palm and base of the finger.")]
        [FormerlySerializedAs("UseMetaCarpals")]
        public bool useMetaCarpals;

#pragma warning disable 0414
        // TODO: DELETEME these
        [Header("Values for Stored Start Pose")]
        [SerializeField]
        private List<Transform> jointList = new List<Transform>();
        [SerializeField]
        private List<Quaternion> localRotations = new List<Quaternion>();
        [SerializeField]
        private List<Vector3> localPositions = new List<Vector3>();
#pragma warning restore 0414

        [Tooltip("If non-zero, this vector and the modelPalmFacing vector " +
          "will be used to re-orient the Transform bones in the hand rig, to " +
          "compensate for bone axis discrepancies between Leap Bones and model " +
          "bones.")]
        public Vector3 modelFingerPointing = new Vector3(0, 0, 0);
        [Tooltip("If non-zero, this vector and the modelFingerPointing vector " +
          "will be used to re-orient the Transform bones in the hand rig, to " +
          "compensate for bone axis discrepancies between Leap Bones and model " +
          "bones.")]
        public Vector3 modelPalmFacing = new Vector3(0, 0, 0);

        /// <summary> Rotation derived from the `modelFingerPointing` and
        /// `modelPalmFacing` vectors in the RiggedHand inspector. </summary>
        public Quaternion userBoneRotation
        {
            get
            {
                if (modelFingerPointing == Vector3.zero || modelPalmFacing == Vector3.zero)
                {
                    return Quaternion.identity;
                }
                return Quaternion.Inverse(
                  Quaternion.LookRotation(modelFingerPointing, -modelPalmFacing));
            }
        }

        public override void InitHand()
        {
            UpdateHand();
            updateDeformPositionsInFingers();
            updateScaleLastFingerBoneInFingers();
        }

        public void SecondUpdateIterration()
        {


            if (palm != null)
            {
                if (modelPalmAtLeapWrist)
                {
                    palm.position = GetWristPosition();
                }
                else
                {
                    palm.position = GetPalmPosition();
                    if (wristJoint)
                    {
                        wristJoint.position = GetWristPosition();
                    }
                }
                palm.rotation = getRiggedPalmRotation() * userBoneRotation;
            }

            if (forearm != null)
            {

                forearm.rotation = GetArmRotation() * userBoneRotation;
            }

            for (int i = 0; i < fingers.Length; ++i)
            {
                FingerModel finger = fingers[i];
                if (fingers[i] != null && fingers[i].fingerType == Finger.FingerType.TYPE_RING)
                {
                    fingers[i].fingerType = (Finger.FingerType)i;
                    fingers[i].UpdateFinger();
                }
            }
        }

        public override void UpdateHand()
        {

            if (shouldUpdateSecond) return;

            if (example && main && Input.GetKeyDown("space"))
            {
                handModel = (handModel + 1) % 4;
            }

            if (palm != null)
            {
                if (modelPalmAtLeapWrist)
                {
                    palm.position = GetWristPosition();
                }
                else
                {
                    palm.position = GetPalmPosition();
                    if (wristJoint)
                    {
                        wristJoint.position = GetWristPosition();
                    }
                }
                palm.rotation = getRiggedPalmRotation() * userBoneRotation;
            }
            if (forearm != null)
            {
                forearm.rotation = GetArmRotation() * userBoneRotation;
            }


            for (int i = 0; i < fingers.Length; ++i)
            {
                if (fingers[i] != null)
                {
                    fingers[i].fingerType = (Finger.FingerType)i;
                    fingers[i].UpdateFinger();
                }
            }

            if (mesh != null) mesh.SetActive(true);
            if (plane != null && handModel != 3) { plane.SetActive(false); }
            if (secondHand != null && handModel != 2) { secondHand.SetActive(false); }

            bool changeAppearance = (handModel == prevHandModel);
            prevHandModel = handModel;

            //reset hand proportions 
            foreach (RiggedFinger finger in fingers)
            {
                finger.transform.localScale = new Vector3(1, 1, 1);
            }

            palm.transform.localScale = new Vector3(1, 1, 1f);

            if (offset)
            {
                transform.position += new Vector3(0.15f, 0, 0f);
                if (secondHand) secondHand.transform.position += new Vector3(0.15f, 0, 0f);
            }

            if (!main) return;
            if (!gameObject.active) return;

            if (handModel == 1) foreach (RiggedFinger finger in fingers)
                {


                    if (finger.fingerType != Finger.FingerType.TYPE_THUMB)
                    {
                        Transform[] t = finger.bones;

                        Vector3 v = (t[2].position - t[1].position).normalized;
                        t[2].position += v * 0.02f;

                        v = (t[3].position - t[2].position).normalized;
                        t[3].position += v * 0.02f;

                        t[3].localScale = new Vector3(1.2f, 1, 1);

                    }// else {

                    //Transform[] t = finger.bones;

                    //Vector3 v = (t[1].position - t[0].position).normalized;
                    //t[1].position += v * 0.02f;

                    //Vector3 v = (t[2].position - t[1].position).normalized;
                    //t[2].position += v * 0.02f;

                    //v = (t[3].position - t[2].position).normalized;
                    //t[3].position += v * 0.02f;

                    //t[3].localScale = new Vector3(1.2f, 1, 1);

                    //if (finger.fingerType == Finger.FingerType.TYPE_THUMB) for (int i = 1; i < 3; i++)
                    //{
                    //Vector3 vect = finger.bones[i].transform.localRotation.eulerAngles;
                    //finger.bones[i].transform.localRotation = Quaternion.Euler(vect - new Vector3(0,0,50));
                    //finger.bones[i].transform.localRotation = Quaternion.Euler(vect - new Vector3(0,0,vect.z/8));

                    //}
                    //}
                }
            else if (handModel == 2)
            {
                secondHand.SetActive(true);

                secondHand.GetComponent<RiggedHand>().SecondUpdateIterration();
                Transform[] pinky = new Transform[0];
                Transform[] ring = new Transform[0];

                // turn to 6 fingers hand

                //update first hand proportion
                palm.transform.localScale = new Vector3(1, 1, 1.5f);

                foreach (RiggedFinger finger in fingers)
                {
                    if (finger.fingerType != Finger.FingerType.TYPE_THUMB)
                    {
                        finger.transform.localScale = new Vector3(1, 1, 0.67f);
                    }

                    if (finger.fingerType == Finger.FingerType.TYPE_PINKY)
                    {
                        finger.transform.localPosition += new Vector3(0.0f, 0f, 0.01f);
                        finger.bones[1].transform.localPosition += new Vector3(0.008f, 0f, 0.01f);
                    }
                }

                //update second hand proportions (5th finger)
                foreach (RiggedFinger finger in secondHand.GetComponent<RiggedHand>().fingers)
                {
                    if (finger.fingerType == Finger.FingerType.TYPE_RING)
                    {
                        finger.transform.localScale = new Vector3(0.8f, 0.5f, 1);
                        //finger.bones[1].localScale = new Vector3(1, 3f, 1);


                        for (int i = 1; i < 4; i++)
                        {
                            finger.bones[i].parent = null;
                            finger.bones[i].localScale = new Vector3(1f, 1.2f, 1);
                            finger.bones[i].parent = finger.bones[i - 1].transform;
                        }

                        Vector3 v = (finger.bones[2].position - finger.bones[1].position).normalized;
                        finger.bones[2].position -= v * 0.006f;

                        v = (finger.bones[3].position - finger.bones[2].position).normalized;
                        finger.bones[3].position -= v * 0.006f;

                        finger.bones[3].localScale = new Vector3(0.9f, 1, 1);


                        //finger.bones[1].localScale = new Vector3(1.1f, 1.15f / 0.5f, 1);
                        //
                        //finger.bones[1].localScale = new Vector3(1.1f, 1.15f/0.5f, 1);
                        //finger.bones[2].localScale = new Vector3(1, 1f, 1);
                        finger.transform.Rotate(0, 5, 0);
                        //finger.transform.localPosition += new Vector3(0.015f, 0.00f, 0.021f);
                        finger.transform.localPosition += new Vector3(0.006f, 0.00f, 0.015f);

                    }
                    else
                    {
                        finger.transform.localScale = new Vector3(0, 0, 0);
                    }
                }

                //get pinky and ring objects 
                foreach (RiggedFinger finger in fingers)
                {

                    if (finger.fingerType == Finger.FingerType.TYPE_PINKY)
                    {

                        pinky = finger.bones;

                    }
                    if (finger.fingerType == Finger.FingerType.TYPE_RING)
                    {
                        ring = finger.bones;
                    }
                }

                //update the 5th finger position and movement as the average of 4th and 6th finger 

                foreach (RiggedFinger finger in secondHand.GetComponent<RiggedHand>().fingers)
                {
                    if (finger.fingerType == Finger.FingerType.TYPE_RING)
                    {

                        //Transform[] t = finger.bones;

                        //Vector3 v = (t[2].position - t[1].position).normalized;
                        //t[2].position += v * 0.005f;

                        //v = (t[3].position - t[2].position).normalized;
                        //t[3].position += v * 0.005f;

                        //t[1].localScale = new Vector3(0.95f, 1, 1);

                        for (int i = 1; i < finger.bones.Length; i++)
                        {

                            float zPinky = (pinky[i].transform.localRotation.eulerAngles.z);
                            //float zPinky = (pinky[i].localRotation.z*180 > 0)? -(pinky[i].localRotation.z * 180) : (pinky[i].localRotation.z * 180);
                            float zRing = (ring[i].transform.localRotation.eulerAngles.z);
                            //float zRing = (ring[i].localRotation.z * 180 > 0) ? -(ring[i].localRotation.z * 180) : (ring[i].localRotation.z * 180);

                            //if (Mathf.Abs(zPinky - 180 - zRing) < Mathf.Abs(zPinky - zRing)) zPinky = zPinky - 180;
                            //else if (Mathf.Abs(zPinky + 180 - zRing) < Mathf.Abs(zPinky - zRing)) zPinky = zPinky + 180;

                            if (zPinky - zRing > 180) zPinky -= 360;
                            if (zPinky - zRing < -180) zPinky += 360;

                            float zAverage = (zPinky + zRing) / 2;


                            float xPinky = pinky[i].localRotation.y * 180;
                            //float xPinky = (pinky[i].localRotation.y * 180 > 0) ? -(pinky[i].localRotation.y * 180) : (pinky[i].localRotation.y * 180);
                            float xRing = ring[i].localRotation.y * 180;
                            //float xRing = (ring[i].localRotation.y * 180 > 0) ? -(ring[i].localRotation.y * 180) : (ring[i].localRotation.y * 180);

                            //if (Mathf.Abs(xPinky - 180 - xRing) < Mathf.Abs(xPinky - xRing)) xPinky = xPinky - 180;
                            //else if (Mathf.Abs(xPinky + 180 - xRing) < Mathf.Abs(xPinky - xRing)) xPinky = xPinky + 180;

                            if (xPinky - xRing > 180) xPinky -= 360;
                            if (xPinky - xRing < -180) xPinky += 360;


                            float xAverage = (xPinky + xRing) / 2;

                            //add some interpolation for some positions
                            float openingRatioX = (xAverage + 8.5f) / (16 + 8.5f); //1 = hand open; 0 = hand closed 
                            float interpolatedXrotation = openingRatioX * -6.26f + (1 - openingRatioX) * 3.18f;




                            finger.bones[i].localRotation = Quaternion.Euler(new Vector3(-2, xAverage + ((i == 1) ? interpolatedXrotation : 0), zAverage));

                        }
                    }
                }
            }
            else if (handModel == 3)
            {
                mesh.SetActive(false);
                plane.SetActive(true);

                int cpt = 0;
                float rotation_add = 0;

                foreach (RiggedFinger finger in fingers)
                {


                    if (finger.fingerType != Finger.FingerType.TYPE_THUMB)
                    {
                        cpt = cpt + 2;
                        float val1 = finger.bones[1].transform.localRotation.z;
                        float val2 = finger.bones[2].transform.localRotation.z;

                        rotation_add += val1 > 0 ? val1 : val1 * -1;
                        rotation_add += val2 > 0 ? val2 : val2 * -1;

                    }

                }
                plane.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -90 * rotation_add / (cpt)));

            }



            //transform.position += new Vector3(0,0,-0.3f);           


        }


        /**Sets up the rigged hand by finding base of each finger by name */
        [ContextMenu("Setup Rigged Hand")]
        public void SetupRiggedHand()
        {
            Debug.Log("Using transform naming to setup RiggedHand on " + transform.name);
            modelFingerPointing = new Vector3(0, 0, 0);
            modelPalmFacing = new Vector3(0, 0, 0);
            assignRiggedFingersByName();
            setupRiggedFingers();
            modelPalmFacing = calculateModelPalmFacing(palm, fingers[2].transform, fingers[1].transform);
            modelFingerPointing = calculateModelFingerPointing();
            setFingerPalmFacing();
        }

        /**Sets up the rigged hand if RiggedFinger scripts have already been assigned using Mecanim values.*/
        public void AutoRigRiggedHand(Transform palm, Transform finger1, Transform finger2)
        {
            Debug.Log("Using Mecanim mapping to setup RiggedHand on " + transform.name);
            modelFingerPointing = new Vector3(0, 0, 0);
            modelPalmFacing = new Vector3(0, 0, 0);
            setupRiggedFingers();
            modelPalmFacing = calculateModelPalmFacing(palm, finger1, finger2);
            modelFingerPointing = calculateModelFingerPointing();
            setFingerPalmFacing();
        }

        /**Finds palm and finds root of each finger by name and assigns RiggedFinger scripts */
        private void assignRiggedFingersByName()
        {
            List<string> palmStrings = new List<string> { "palm" };
            List<string> thumbStrings = new List<string> { "thumb", "tmb" };
            List<string> indexStrings = new List<string> { "index", "idx" };
            List<string> middleStrings = new List<string> { "middle", "mid" };
            List<string> ringStrings = new List<string> { "ring" };
            List<string> pinkyStrings = new List<string> { "pinky", "pin" };
            //find palm by name
            //Transform palm = null;
            Transform thumb = null;
            Transform index = null;
            Transform middle = null;
            Transform ring = null;
            Transform pinky = null;
            Transform[] children = transform.GetComponentsInChildren<Transform>();
            if (palmStrings.Any(w => transform.name.ToLower().Contains(w)))
            {
                base.palm = transform;
            }
            else
            {
                foreach (Transform t in children)
                {
                    if (palmStrings.Any(w => t.name.ToLower().Contains(w)) == true)
                    {
                        base.palm = t;

                    }
                }
            }
            if (!palm)
            {
                palm = transform;
            }
            if (palm)
            {
                foreach (Transform t in children)
                {
                    RiggedFinger preExistingRiggedFinger;
                    preExistingRiggedFinger = t.GetComponent<RiggedFinger>();
                    string lowercaseName = t.name.ToLower();
                    if (!preExistingRiggedFinger)
                    {
                        if (thumbStrings.Any(w => lowercaseName.Contains(w)) && t.parent == palm)
                        {
                            thumb = t;
                            RiggedFinger newRiggedFinger = thumb.gameObject.AddComponent<RiggedFinger>();
                            newRiggedFinger.fingerType = Finger.FingerType.TYPE_THUMB;
                        }
                        if (indexStrings.Any(w => lowercaseName.Contains(w)) && t.parent == palm)
                        {
                            index = t;
                            RiggedFinger newRiggedFinger = index.gameObject.AddComponent<RiggedFinger>();
                            newRiggedFinger.fingerType = Finger.FingerType.TYPE_INDEX;
                        }
                        if (middleStrings.Any(w => lowercaseName.Contains(w)) && t.parent == palm)
                        {
                            middle = t;
                            RiggedFinger newRiggedFinger = middle.gameObject.AddComponent<RiggedFinger>();
                            newRiggedFinger.fingerType = Finger.FingerType.TYPE_MIDDLE;
                        }
                        if (ringStrings.Any(w => lowercaseName.Contains(w)) && t.parent == palm)
                        {
                            ring = t;
                            RiggedFinger newRiggedFinger = ring.gameObject.AddComponent<RiggedFinger>();
                            newRiggedFinger.fingerType = Finger.FingerType.TYPE_RING;
                        }
                        if (pinkyStrings.Any(w => lowercaseName.Contains(w)) && t.parent == palm)
                        {
                            pinky = t;
                            RiggedFinger newRiggedFinger = pinky.gameObject.AddComponent<RiggedFinger>();
                            newRiggedFinger.fingerType = Finger.FingerType.TYPE_PINKY;
                        }
                    }
                }
            }
        }

        /**Triggers SetupRiggedFinger() in each RiggedFinger script for this RiggedHand */
        private void setupRiggedFingers()
        {
            RiggedFinger[] fingerModelList = GetComponentsInChildren<RiggedFinger>();
            for (int i = 0; i < 5; i++)
            {
                int fingersIndex = fingerModelList[i].fingerType.indexOf();
                fingers[fingersIndex] = fingerModelList[i];
                fingerModelList[i].SetupRiggedFinger(useMetaCarpals);
            }
        }

        /**Sets the modelPalmFacing vector in each RiggedFinger to match this RiggedHand */
        private void setFingerPalmFacing()
        {
            RiggedFinger[] fingerModelList = GetComponentsInChildren<RiggedFinger>();
            for (int i = 0; i < 5; i++)
            {
                int fingersIndex = fingerModelList[i].fingerType.indexOf();
                fingers[fingersIndex] = fingerModelList[i];
                fingerModelList[i].modelPalmFacing = modelPalmFacing;
            }
        }

        /**Calculates the palm facing direction by finding the vector perpendicular to the palm and two fingers  */
        private Vector3 calculateModelPalmFacing(Transform palm, Transform finger1,
          Transform finger2)
        {
            Vector3 a = palm.transform.InverseTransformPoint(palm.position);
            Vector3 b = palm.transform.InverseTransformPoint(finger1.position);
            Vector3 c = palm.transform.InverseTransformPoint(finger2.position);

            Vector3 side1 = b - a;
            Vector3 side2 = c - a;
            Vector3 perpendicular;

            if (Handedness == Chirality.Left)
            {
                perpendicular = Vector3.Cross(side1, side2);
            }
            else perpendicular = Vector3.Cross(side2, side1);
            //flip perpendicular if it is above palm
            Vector3 calculatedPalmFacing = CalculateZeroedVector(perpendicular);
            return calculatedPalmFacing;
        }

        /**Find finger direction by finding distance vector from palm to middle finger */
        private Vector3 calculateModelFingerPointing()
        {
            Vector3 distance = palm.transform.InverseTransformPoint(fingers[2].transform.GetChild(0).transform.position) - palm.transform.InverseTransformPoint(palm.position);
            Vector3 calculatedFingerPointing = CalculateZeroedVector(distance);
            return calculatedFingerPointing * -1f;
        }

        /**Finds nearest cardinal vector to a vector */
        public static Vector3 CalculateZeroedVector(Vector3 vectorToZero)
        {
            var zeroed = new Vector3();
            float max = Mathf.Max(Mathf.Abs(vectorToZero.x), Mathf.Abs(vectorToZero.y), Mathf.Abs(vectorToZero.z));
            if (Mathf.Abs(vectorToZero.x) == max)
            {
                zeroed = (vectorToZero.x < 0) ? new Vector3(1, 0, 0) : new Vector3(-1, 0, 0);
            }
            if (Mathf.Abs(vectorToZero.y) == max)
            {
                zeroed = (vectorToZero.y < 0) ? new Vector3(0, 1, 0) : new Vector3(0, -1, 0);
            }
            if (Mathf.Abs(vectorToZero.z) == max)
            {
                zeroed = (vectorToZero.z < 0) ? new Vector3(0, 0, 1) : new Vector3(0, 0, -1);
            }
            return zeroed;
        }

        // /**Stores a snapshot of original joint positions */
        // [ContextMenu("StoreJointsStartPose")]
        // public void StoreJointsStartPose() {
        //   foreach (Transform t in palm.parent.GetComponentsInChildren<Transform>()) {
        //     jointList.Add(t);
        //     localRotations.Add(t.localRotation);
        //     localPositions.Add(t.localPosition);
        //   }
        // }

        // /**Restores original joint positions, particularly after model has been placed in Leap's editor pose */
        // [ContextMenu("RestoreJointsStartPose")]
        // public void RestoreJointsStartPose() {
        //   //Debug.Log("RestoreJointsStartPose()");
        //   for (int i = 0; i < jointList.Count; i++) {
        //     Transform jointTrans = jointList[i];
        //     jointTrans.localRotation = localRotations[i];
        //     jointTrans.localPosition = localPositions[i];
        //   }
        // }

        private void updateDeformPositionsInFingers()
        {
            var riggedFingers = GetComponentsInChildren<RiggedFinger>();
            foreach (var finger in riggedFingers)
            {
                finger.deformPosition = deformPositionsInFingers;
            }
        }

        private void updateScaleLastFingerBoneInFingers()
        {
            var riggedFingers = GetComponentsInChildren<RiggedFinger>();
            foreach (var finger in riggedFingers)
            {
                finger.scaleLastFingerBone = scaleLastFingerBones;
            }
        }

        // These versions of GetPalmRotation & CalculateRotation return the opposite
        // vector compared to LeapUnityExtension.CalculateRotation.
        // This will be deprecated once LeapUnityExtension.CalculateRotation is
        // flipped in the next release of LeapMotion Core Assets.
        private Quaternion getRiggedPalmRotation()
        {
            if (hand_ != null)
            {
                LeapTransform trs = hand_.Basis;
                return calculateRotation(trs);
            }
            if (palm)
            {
                return palm.rotation;
            }
            return Quaternion.identity;
        }

        private Quaternion calculateRotation(LeapTransform trs)
        {
            Vector3 up = trs.yBasis.ToVector3();
            Vector3 forward = trs.zBasis.ToVector3();
            return Quaternion.LookRotation(forward, up);
        }



        // [Tooltip("When true, hands will be put into a Leap editor pose near the LeapServiceProvider's transform.  When False, the hands will be returned to their Start Pose if it has been saved.")]
        // [SerializeField]
        // private bool setEditorLeapPose = true;

        // public bool SetEditorLeapPose {
        //   get { return setEditorLeapPose; }
        //   set {
        //     if (value == false) {
        //       RestoreJointsStartPose();
        //     }
        //     setEditorLeapPose = value;
        //   }
        // }

        // [Tooltip("When True, hands will be put into a Leap editor pose near the LeapServiceProvider's transform.  When False, the hands will be returned to their Start Pose if it has been saved.")]
        // [SerializeField]
        // [HideInInspector]
        // private bool deformPositionsState = false;

    }

}
