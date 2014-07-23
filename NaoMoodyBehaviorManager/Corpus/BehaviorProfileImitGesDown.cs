﻿/*
 * TODO: The two presets should be merged!
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using ConnectorNao; // MTL

namespace NaoManager
{
    public class BehaviorProfileImitGesDown : BehaviorProfileBase
    {
        public override string BehaviorName
        {
            get{return "ImitationGestureBot";}
        }

        public BehaviorProfileImitGesDown()
        {
            // TODO: Read from XML files later
            this.Parameters.Add("Amplitude", new Parameter(0, "Pose"));
            this.Parameters.Add("PalmDirection", new Parameter(0, "Pose"));
            this.Parameters.Add("Finger", new Parameter(0, "Pose"));
            this.Parameters.Add("HeadUpDown", new Parameter(0, "Pose"));
            this.Parameters.Add("HeadLeftRight", new Parameter(0, "Pose"));

            this.Parameters.Add("MotionSpeed", new Parameter(0, "Motion"));
            this.Parameters.Add("DecaySpeed", new Parameter(0, "Motion"));
            this.Parameters.Add("HoldTime", new Parameter(0, "Motion"));
            this.Parameters.Add("Repetition", new Parameter(0, "Motion"));

            // TODO: presets load from XML files
            #region Preset
            List<string> PresetName = new List<string>() { "neutral", "veryunhappy", "veryhappy" };
            // final
            List<double> veryunhappy = new List<double>() { 0.20, 0.35, 0.46, 1, 0.5, 0.05, 0.09, -5, 6 };
            List<double> veryhappy = new List<double>() { 0.95, 1.00, 0.94, 4, 0.0, 0.43, 0.47, 5, 0 };
            List<double> neutral = new List<double>() { 0.55, 0.65, 0.70, 2, 0.2, 0.23, 0.27, 0, 0 };
            #endregion

            // be consistent with PresetName
            // "neutral", "veryunhappy", "veryhappy"
            Presets.Add("neutral", neutral);
            Presets.Add("veryunhappy", veryunhappy);
            Presets.Add("veryhappy", veryhappy);
        }

        /// <summary>
        /// TODO: move common initial/preparation pose to base behavior profile
        /// </summary>
        /// <param name="valence"></param>
        public void PreparationPose(double valence, out List<string> jointnames, out List<float> jointvals)
        {
            float rshoulderPitch = (float)(70 * Math.PI / 180);
            float rshoulderRoll = (float)(-10 * Math.PI / 180);
            float relbowYaw = (float)(45 * Math.PI / 180);
            float relbowRoll = (float)(88 * Math.PI / 180);
            float rwristYaw = (float)(-20 * Math.PI / 180);
            float rhand = 0.6F;
            float lshoulderPitch = (float)(70 * Math.PI / 180);
            float lshoulderRoll = (float)(10 * Math.PI / 180);
            float lelbowYaw = (float)(-45 * Math.PI / 180);
            float lelbowRoll = (float)(-88 * Math.PI / 180);
            float lwristYaw = (float)(20 * Math.PI / 180);
            float lhand = 0.6F;
            float LHipYawPitch = (float)(0 * Math.PI / 180);
            float LHipRoll = (float)(0 * Math.PI / 180);
            float LHipPitch = (float)(-25 * Math.PI / 180);
            float LKneePitch = (float)(40 * Math.PI / 180);
            float LAnklePitch = (float)(-20 * Math.PI / 180);
            float LAnkleRoll = (float)(0 * Math.PI / 180);
            float RHipRoll = (float)(0 * Math.PI / 180);
            float RHipPitch = (float)(-25 * Math.PI / 180);
            float RKneePitch = (float)(40 * Math.PI / 180);
            float RAnklePitch = (float)(-20 * Math.PI / 180);
            float RAnkleRoll = (float)(0 * Math.PI / 180);
            float HeadPitch = (float)(0 * Math.PI / 180);
            float HeadYaw = (float)(0 * Math.PI / 180);

            /*
            double headupdown    = 0; 
            if (valence >= 0) headupdown = 0.7 * valence; // 0~7
            else headupdown = 0.5 * valence; // 5~0
            // -35.0 ~ 25.0 deg
            HeadPitch = (float)(-5.0 * headupdown * Math.PI / 180);
            */

            jointvals = new List<float> { rshoulderPitch, rshoulderRoll, relbowYaw, relbowRoll, rwristYaw, rhand, 
                    lshoulderPitch, lshoulderRoll, lelbowYaw, lelbowRoll, lwristYaw, lhand, 
                    LHipYawPitch ,  LHipRoll , LHipPitch , LKneePitch , LAnklePitch , LAnkleRoll ,
                    RHipRoll , RHipPitch , RKneePitch , RAnklePitch , RAnkleRoll, HeadPitch, HeadYaw };
            jointnames = new List<string> { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw", "RHand", 
                    "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw", "LHand",
                    "LHipYawPitch", "LHipRoll","LHipPitch","LKneePitch","LAnklePitch","LAnkleRoll",
                    "RHipRoll","RHipPitch","RKneePitch","RAnklePitch","RAnkleRoll", "HeadPitch", "HeadYaw"};

            //naomotionproxy.angleInterpolationWithSpeed(jn, jv, 0.3f);

        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="valence"></param>
        public void EndPose(double valence, out List<string> jointnames, out List<float> jointvals)
        {
            float rshoulderPitch = (float)(70 * Math.PI / 180);
            float rshoulderRoll = (float)(-10 * Math.PI / 180);
            float relbowYaw = (float)(45 * Math.PI / 180);
            float relbowRoll = (float)(45 * Math.PI / 180);
            float rwristYaw = (float)(-20 * Math.PI / 180);
            float rhand = 0.6F;
            float lshoulderPitch = (float)(70 * Math.PI / 180);
            float lshoulderRoll = (float)(10 * Math.PI / 180);
            float lelbowYaw = (float)(-45 * Math.PI / 180);
            float lelbowRoll = (float)(-45 * Math.PI / 180);
            float lwristYaw = (float)(20 * Math.PI / 180);
            float lhand = 0.6F;
            float LHipYawPitch = (float)(0 * Math.PI / 180);
            float LHipRoll = (float)(0 * Math.PI / 180);
            float LHipPitch = (float)(-25 * Math.PI / 180);
            float LKneePitch = (float)(40 * Math.PI / 180);
            float LAnklePitch = (float)(-20 * Math.PI / 180);
            float LAnkleRoll = (float)(0 * Math.PI / 180);
            float RHipRoll = (float)(0 * Math.PI / 180);
            float RHipPitch = (float)(-25 * Math.PI / 180);
            float RKneePitch = (float)(40 * Math.PI / 180);
            float RAnklePitch = (float)(-20 * Math.PI / 180);
            float RAnkleRoll = (float)(0 * Math.PI / 180);
            float HeadPitch = (float)(0 * Math.PI / 180);
            float HeadYaw = (float)(0 * Math.PI / 180);

            /*
            double headupdown    = 0; 
            if (valence >= 0) headupdown = 0.7 * valence; // 0~7
            else headupdown = 0.5 * valence; // 5~0
            // -35.0 ~ 25.0 deg
            HeadPitch = (float)(-5.0 * headupdown * Math.PI / 180);
            */

            jointvals = new List<float> { rshoulderPitch, rshoulderRoll, relbowYaw, relbowRoll, rwristYaw, rhand, 
                    lshoulderPitch, lshoulderRoll, lelbowYaw, lelbowRoll, lwristYaw, lhand, 
                    LHipYawPitch ,  LHipRoll , LHipPitch , LKneePitch , LAnklePitch , LAnkleRoll ,
                    RHipRoll , RHipPitch , RKneePitch , RAnklePitch , RAnkleRoll, HeadPitch, HeadYaw };
            jointnames = new List<string> { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw", "RHand", 
                    "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw", "LHand",
                    "LHipYawPitch", "LHipRoll","LHipPitch","LKneePitch","LAnklePitch","LAnkleRoll",
                    "RHipRoll","RHipPitch","RKneePitch","RAnklePitch","RAnkleRoll", "HeadPitch", "HeadYaw"};


        }

        public override MotionTimeline LoadBehavior(double valence, double arousal, string args)
        {
            bool lr = true;
            if (args.Contains("L")) lr = false;
            else if (args.Contains("R")) lr = true;
            CoreAffectToParameter(valence);
            MotionTimeline mtl = CoreParameterToMotion(lr);
            return mtl;
        }

        public override ConnectorNao.MotionTimeline LoadSingleParam(int paramind, int level)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="setname">moods</param>
        /// <param name="setmode">APS, IPS, UPS</param>
        /// <returns></returns>
        public override MotionTimeline LoadPreset(string setname, int setmode)
        {
            throw new NotImplementedException();
        }

        #region Core
        /// <summary>
        /// Get behavior parameters computed according to the valence value
        /// </summary>
        /// <param name="valence">the value of valence, [-10, 10]</param>
        /// <returns>Imitation-Game-Behavior parameters</returns>
        protected override void CoreAffectToParameter(double valence, double arousal = 0)
        {
            double amplitude     = 0;   
            double palmdir       = 0;     
            double finger        = 0;   
            double headupdown    = 0; 
            double headleftright = 0;
            //
            double motionspeed   = 0;
            double holdtime      = 0;
            double decayspeed    = 0;
            double rep           = 0;

            //////////////////////////////////////////////////////////////////////////
            // 
            amplitude = 0.5 * valence + 5; // 0~10
            finger = 0.05 * valence + 0.5; // 0~1
            palmdir = 0.5 * valence + 5;   // 0~10
            // Speed
            motionspeed = 0.036 * valence + 0.6;
            decayspeed = motionspeed;
            holdtime = 0.5 - 0.05 * valence;  // 0 ~ 1s
            rep = 0;
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // Accompanying
            if (valence >= 0) headupdown = 0.7 * valence; // 0~7
            else headupdown = 0.5 * valence; // 5~0
            if (valence >= 0) headleftright = valence; // 0~10
            else headleftright = 0;
            //////////////////////////////////////////////////////////////////////////

            Parameters["PalmDirection"].Value = palmdir;
            Parameters["Amplitude"].Value = amplitude;
            Parameters["Finger"].Value = finger;
            Parameters["HeadUpDown"].Value = headupdown;
            Parameters["HeadLeftRight"].Value = headleftright;
            //
            Parameters["MotionSpeed"].Value = motionspeed;
            Parameters["HoldTime"].Value = holdtime;
            Parameters["DecaySpeed"].Value = decayspeed;
            Parameters["Repetition"].Value = rep;
        }



        /// <summary>
        /// Behavior parameters -> MotionTimeLine
        ///   1. Initial pose
        ///   2. End pose
        /// </summary>
        /// <param name="behparams"></param>
        /// <param name="pose">TL,TR,BL,BR</param>
        /// <returns></returns>
        protected override MotionTimeline CoreParameterToMotion(bool right = true)
        {
            double motionspeed = Parameters["MotionSpeed"].Value;
            double holdtime = Parameters["HoldTime"].Value;

            List<double> jointvalues = CoreParameterToJoint(right);

            double shoulderPitch  = jointvalues[0];
            double shoulderRoll   = jointvalues[1];
            double elbowYaw       = jointvalues[2];
            double elbowRoll      = jointvalues[3];
            double wristYaw       = jointvalues[4];
            double hand           = jointvalues[5];
            double headpitch      = jointvalues[6];
            double headyaw        = jointvalues[7];

            //////////////////////////////////////////////////////////////////////////
            // Accompanying
            PoseProfile HeadPose = new PoseProfile();
            HeadPose.Joints = new Head(false, headpitch, headyaw);
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            // initial pose that prepares to point
            double initHoldTime = 0.1;
            //double initSpeed = 0.3;
            // pose
            double shoulderPitch_1 = 70;
            double shoulderRoll_1 = -10;
            double elbowYaw_1 = 45;
            double elbowRoll_1 = 88;
            double wristYaw_1 = -20;
            double hand_1 = 0.6 * 180 / (double)Math.PI;
            //double headpitch_1 = 0;
            //double headyaw_1 = 0;
            //////////////////////////////////////////////////////////////////////////

            // Initial Frame
            PoseProfile initArmPose = new PoseProfile();
            if (right == true) // right
            {
	            initArmPose.Joints = new RArm(false,
	                    shoulderPitch_1,
	                    shoulderRoll_1,
	                    elbowYaw_1,
	                    elbowRoll_1,
	                    wristYaw_1,
	                    hand_1);
            } 
            else // left
            {
                initArmPose.Joints = new LArm(false,
                        shoulderPitch_1,
                        -shoulderRoll_1,
                        -elbowYaw_1,
                        -elbowRoll_1,
                        -wristYaw_1,
                        hand_1);
            }
            PoseProfile initHeadPose = new PoseProfile();
            //initHeadPose.Joints = new Head(false, headpitch_1, headyaw_1);
            initHeadPose.Joints = new Head(false, headpitch, headyaw);
            MotionFrame initMF = new MotionFrame();
            initMF.PoseList.Add(initArmPose);
            initMF.PoseList.Add(initHeadPose);
            //initMF.SpeedFraction = initSpeed;
            initMF.SpeedFraction = motionspeed;
            initMF.UsingSpeedFraction = true;
            initMF.HoldTime = initHoldTime;
            initMF.IsAbsolute = true;
            initMF.JointsToList();
            // End Frame (pointing)
            PoseProfile endArmPose = new PoseProfile();
            if (right == true) // right
            {
	            endArmPose.Joints = new RArm(false,
	                    shoulderPitch,
	                    shoulderRoll,
	                    elbowYaw,
	                    elbowRoll,
	                    wristYaw,
	                    hand);
            } 
            else // left
            {
                endArmPose.Joints = new LArm(false,
                    shoulderPitch,
                    shoulderRoll,
                    elbowYaw,
                    elbowRoll,
                    wristYaw,
                    hand);
            }
            //PoseProfile endHeadPose = new PoseProfile();
            //endHeadPose.Joints = new Head(false, headpitch, headyaw);
            MotionFrame endMF = new MotionFrame();
            endMF.PoseList.Add(endArmPose);
            //endMF.PoseList.Add(endHeadPose);
            endMF.SpeedFraction = motionspeed;
            endMF.UsingSpeedFraction = true;
            endMF.HoldTime = holdtime;
            endMF.IsAbsolute = true;
            endMF.JointsToList();

            // Time Line
            MotionTimeline mtl = new MotionTimeline();
            mtl.BehaviorsDesp = "IGBehavior";
            mtl.OrderedMFSeq.Add(initMF);
            mtl.OrderedMFSeq.Add(endMF);

            // decay
            mtl.PoseRecover = true;
            mtl.RecovSpd = motionspeed - 0.03D;
            if (mtl.RecovSpd < 0.02) mtl.RecovSpd = 0.02;
            return mtl;
        }

        /// <summary>
        /// Motion parameters -> joint values
        /// </summary>
        /// <param name="behparams">list of behavior parameter values</param>
        /// <param name="leftright">Left or right arm; True is right</param>
        /// <param name="updown">Up or down pose; True is up</param>
        /// <returns></returns>
        protected override List<double> CoreParameterToJoint(bool right = true)
        {
            double amplitude = this.Parameters["Amplitude"].Value;
            double palmdir = this.Parameters["Amplitude"].Value;
            double finger = this.Parameters["Amplitude"].Value;
            double headupdown = this.Parameters["Amplitude"].Value;
            double headleftright = this.Parameters["Amplitude"].Value;

            double shoulderPitch   = 0;
            double shoulderRoll    = 0;
            double elbowYaw        = 0;
            double elbowRoll       = 0;
            double wristYaw        = 0;
            double hand            = 0;
            double headpitch       = 0;
            double headyaw         = 0;

            //////////////////////////////////////////////////////////////////////////
            
            hand = finger * 180D / Math.PI; // deg

            //////////////////////////////////////////////////////////////////////////
            // Accompanying behavior
            // -35.0 ~ 25.0 deg
            headpitch = -5.0 * headupdown;
            // -33 ~ 33 deg
            double headlrfactor = 3.3D;
            if (right == true) headyaw = -headlrfactor * headleftright; // right < 0
            else headyaw = headlrfactor * headleftright;
            //////////////////////////////////////////////////////////////////////////


            // IGBehavior Down
            shoulderPitch = -2.5D * amplitude + 115D; // deg
            shoulderRoll = -3D * amplitude - 10D; // deg
            // computed wristYaw, elbowYaw, elbowRoll to fit the palm direction
            elbowRoll = -8.6 * amplitude + 88D; // deg
            wristYaw = 90D; // deg

            // keep the hand toward front
            double arg1 = wristYaw * Math.PI / 180D;
            double arg2 = elbowRoll * Math.PI / 180D;
            double arg3 = shoulderPitch * Math.PI / 180D;
            double arg4 = shoulderRoll * Math.PI / 180D;
            /* let OptPalmDirCompEY wristyaw elbowroll shoulderpitch shoulderroll = */
            Microsoft.FSharp.Math.Vector<double> result
                //= NaoJointChain.Arm.OptPalmDirCompEYWY(arg1, arg2, arg3);
                = NaoJointChain.Arm.OptPalmDirCompEY(arg1, arg2, arg3, arg4);
            elbowYaw = result[0] * 180 / Math.PI;
            Debug.WriteLine("ElbowYaw: {0}", elbowYaw);

            // add bias to WristYaw according to PalmDir
            double bias_wy = -180 + 18 * palmdir;
            wristYaw += bias_wy;

            
            
            // TEST extension set after all computations
            // need to adapt when sad UP (the upper arm is along x axis)
            // Implement it after experiment. It does not influence the gesture!!! Only conceptual optimal!
            // elbowRoll = 2.0;

            // right -> left
            if (right == false)
            {
                // shoulderPitch keeps
                shoulderRoll = -shoulderRoll;
                elbowRoll = -elbowRoll;
                elbowYaw = -elbowYaw;
                wristYaw = -wristYaw;

            }

            List<double> jointvalues = new List<double>();
            jointvalues.Add(shoulderPitch);
            jointvalues.Add(shoulderRoll);
            jointvalues.Add(elbowYaw);
            jointvalues.Add(elbowRoll);
            jointvalues.Add(wristYaw);
            jointvalues.Add(hand);
            jointvalues.Add(headpitch);
            jointvalues.Add(headyaw);

            return jointvalues;
        }
        #endregion Core



        protected override List<PoseProfile> CoreParameterToPose(bool right = true)
        {
            throw new NotImplementedException();
        }
    } // class IGBehaviorBehavior
}
