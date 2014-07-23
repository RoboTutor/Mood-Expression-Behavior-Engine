﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConnectorNao;

namespace NaoManager
{
    public class BehaviorProfileHandOver : BehaviorProfileBase
    {
        public override string BehaviorName
        {
            get { return "HandOver"; }
        }

        public BehaviorProfileHandOver()
        {
            this.Parameters.Add("Width", new Parameter(0, "Pose"));
            this.Parameters.Add("Height", new Parameter(0, "Pose"));
            this.Parameters.Add("PalmDirection", new Parameter(0, "Pose"));
            this.Parameters.Add("Finger", new Parameter(0, "Pose"));
            this.Parameters.Add("HeadUpDown", new Parameter(0, "Pose"));
            this.Parameters.Add("HeadLeftRight", new Parameter(0, "Pose"));

            this.Parameters.Add("MotionSpeed", new Parameter(0, "Motion"));
            this.Parameters.Add("DecaySpeed", new Parameter(0, "Motion"));
            this.Parameters.Add("HoldTime", new Parameter(0, "Motion"));
            this.Parameters.Add("Repetition", new Parameter(0, "Motion"));
        }

        public override ConnectorNao.MotionTimeline LoadPreset(string setname, int setmode)
        {
            throw new NotImplementedException();
        }

        public override ConnectorNao.MotionTimeline LoadSingleParam(int paramind, int level)
        {
            throw new NotImplementedException();
        }

        protected override List<double> CoreParameterToJoint(bool right = true)
        {
            throw new NotImplementedException();
        }

        protected override List<PoseProfile> CoreParameterToPose(bool right = true)
        {
            double width   = Parameters["Width"].Value;
            double height  = Parameters["Height"].Value;
            double palmdir = Parameters["PalmDirection"].Value;
            double finger  = Parameters["Finger"].Value;
            double headud  = Parameters["HeadUpDown"].Value;
            double headlr  = Parameters["HeadLeftRight"].Value;

            // TODO move this initial pose to base class
            double rshoulderroll_init = 0;
            double rshoulderpitch_init = 45;
            double relbowroll_init = 88.5;
            double relbowyaw_init = 70;
            double rwristyaw_init = 60;
            double rhand_init = 0.6;

            double rshoulderroll  = -30 * width;
            double rshoulderpitch = -50 * height + 60;
            double relbowroll     = -43 * height + 45;
            double relbowyaw      = 90;
            double rwristyaw      = 90 * palmdir;
            double rhand = 0.4 * finger + 0.6;

            // degree
            double headPitch = base.NormalizeHeadPitch(headud);
            // degree
            double headYaw;
            if (headlr > 0.5)
            {
                headYaw = -40 * headlr + 20;
            }
            else
            {
                headYaw = double.NaN;
            }

            RArm rarm_init = new RArm(false, rshoulderpitch_init, rshoulderroll_init, relbowyaw_init, relbowroll_init, rwristyaw_init, rhand_init);
            Head head = new Head(false, headPitch, headYaw);
            RArm rarm_end = new RArm(false, rshoulderpitch, rshoulderroll, relbowyaw, relbowroll, rwristyaw, rhand);

            // Joints
            PoseProfile pose_rarm_init = new PoseProfile("InitPose", rarm_init);
            PoseProfile pose_head_init = new PoseProfile("InitPose", head);
            PoseProfile pose_rarm_end = new PoseProfile("EndPose", rarm_end);

            List<PoseProfile> lpp = new List<PoseProfile>();
            lpp.Add(pose_rarm_init);
            lpp.Add(pose_head_init);
            lpp.Add(pose_rarm_end);

            return lpp;
        }

        protected override void CoreAffectToParameter(double valence, double arousal = 0)
        {
            // pose
            double width = 0.05 * valence + 0.5;
            double height = 0.05 * valence + 0.5;
            double palmdir = 0.05 * valence + 0.5;
            double finger = 0.05 * valence + 0.5;
            // motion
            double motspd = 0.035 * valence + 0.65;
            double decspd = 0.035 * valence + 0.65;
            double holdtime = 0.0075 * valence + 0.125;
            double rep = 0; // could be more, depending on the meaning of this gesture
            // head
            double headud = 0.1 * valence;
            double headlr = 0.05 * valence + 0.5;

            Parameters["Width"].Value = width;
            Parameters["Height"].Value = height;
            Parameters["PalmDirection"].Value = palmdir;
            Parameters["Finger"].Value = finger;
            Parameters["HeadUpDown"].Value = headud;
            Parameters["HeadLeftRight"].Value = headlr;
            Parameters["MotionSpeed"].Value = motspd;
            Parameters["DecaySpeed"].Value = decspd;
            Parameters["HoldTime"].Value = holdtime;
            Parameters["Repetition"].Value = rep;
        }
    }
}
