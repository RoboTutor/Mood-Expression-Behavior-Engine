﻿/*
 * This code and information is provided "as is" without warranty of any kind, 
 * either expressed or implied, including but not limited to the implied warranties 
 * of merchantability and/or fitness for a particular purpose.
 * 
 * License: 
 * 
 * Email: junchaoxu86@gmail.com; k.v.hindriks@tudelft.nl
 * 
 * Copyright © Junchao Xu, Interactive Intelligence, TUDelft, 2014.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PPTController;
using System.Diagnostics;
using ConnectorNao;
using ScriptEngine;
using NaoManager;
using Messenger;

namespace DashBoard
{
    /// <summary>
    /// This is the dock. All components that are used 
    /// to send messages need to be instantiated here,
    /// and put into Docking().
    /// </summary>
    public partial class ComponentManager : Form
    {
        // Dockees
        NaoRobotManager NaoRobotMgr;
        ScriptInterpreter ScriptManager;
        SlidesController PPTSlidesController;
        // Docker
        Dictionary<string, IMessenger> Dockees;

        /// <summary>
        /// Constructor
        /// </summary>
        public ComponentManager()
        {
            InitializeComponent();

            NaoRobotMgr = new NaoRobotManager();
            ScriptManager = new ScriptInterpreter();
            PPTSlidesController = new SlidesController();

            Dockees = new Dictionary<string, IMessenger>();
            Docking(NaoRobotMgr);
            Docking(ScriptManager);
            Docking(PPTSlidesController);

            EventMap();

            // Main Tab
            this.rdbtnPositiveMoodCondition.Checked = true;
            // Tabs
            this.tbpRobotMgr.Controls.Add(NaoRobotMgr);
            this.tbpScriptEngine.Controls.Add(ScriptManager.UI);
            this.tbpPPT.Controls.Add(PPTSlidesController);
        }

        /// <summary>
        /// Put messengers into dock.
        /// </summary>
        /// <param name="imsgr"></param>
        private void Docking(IMessenger imsgr)
        {
            Dockees.Add(imsgr.ID, imsgr);
            imsgr.evSendMessage += this.MessageCenter;
        }

        private void EventMap()
        {

        }

        #region Messenger
        public string ID
        {
            get { return "DashBoard"; }
        }

        /// <summary>
        /// true: on the robot; false: on the PC.
        /// </summary>
        bool IsExternalScriptEngine = false;

        /// <summary>
        /// Central message distributor.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="sendto"></param>
        /// <param name="sendfrom"></param>
        private MessageEventArgs MessageCenter(string sendto, string sendfrom, MessageEventArgs message)
        {
            if (sendto == "ScriptEngine")
            {
                if (IsExternalScriptEngine == true) sendto = "ExternalScriptEngine";
            }

            if (Dockees.ContainsKey(sendto))
            {
                MessageEventArgs backmsg = Dockees[sendto].MessageHandler(sendfrom, message);
                return backmsg;
            }
            else
                return null;
        }

        private void SendMessage(string sendto, MessageEventArgs message)
        {
            if (Dockees.ContainsKey(sendto))
            {
                Dockees[sendto].MessageHandler(this.ID, message);
            }
        }
        #endregion Messenger

        #region Main Tab
        private void rdbtnPositiveMoodCondition_CheckedChanged(object sender, EventArgs e)
        {
            double mood = 10.0;
            SendMessage("NaoManager", new MessageEventArgs("ChangeRobotMood", new string[] { mood.ToString() }));
        }

        private void rdbtnNegativeMoodCondition_CheckedChanged(object sender, EventArgs e)
        {
            double mood = -10.0;
            SendMessage("NaoManager", new MessageEventArgs("ChangeRobotMood", new string[] { mood.ToString() }));
        }

        private void btnSecondPart_Click(object sender, EventArgs e)
        {
            string text = "Hi everyone, it is the time, for the second part of the lecture. Could you please return to your seats?";
            SendMessage("NaoManager", new MessageEventArgs("Say", new string[]{text}));
        }
        #endregion Main Tab
    }
}
