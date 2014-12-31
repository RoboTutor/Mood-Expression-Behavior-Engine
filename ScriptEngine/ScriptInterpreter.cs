﻿/*
 * This code and information is provided "as is" without warranty of any kind, 
 * either expressed or implied, including but not limited to the implied warranties 
 * of merchantability and/or fitness for a particular purpose.
 * 
 * License: 
 * Email: junchaoxu86@gmail.com; k.v.hindriks@tudelft.nl
 * Copyright © Junchao Xu, Interactive Intelligence, TUDelft, 2014.
 * 
 * Features:
 * 1. The ScriptInterpreter is built based on "Interpreter Pattern".
 *    To extend the expression, add new expression class in region "Specific Expressions" in the file "ScriptSyntax.cs".
 *    The class has to have a static method "public static IExpression Interpret(string)".
 *    This is usually the only thing you need to do for the extension!
 * 2. When speech is slower, the next behavior will not start.
 * 3. Slide command waits for behavior and speech.
 * 
 * TODO:
 * 1. When behavior is slower than speech: there should be a termination routine for behavior.
 * 
 * Known issues:
 * 1. The pause in NAO TTS:
 *      \pau=1000\
 *      Something to say
 *    does not work, but
 *      \pau=1000\ Something to say
 *    does work!
 *    (solved by add a Pause Expression)
 * 2. Sometimes, a behavior will not be performed.
 * 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Messenger;

namespace ScriptEngine
{
    /// <summary>
    /// The script interpreter
    /// </summary>
    public class ScriptInterpreter : IMessenger
    {
        ScriptEditor Editor;
        /// <summary>
        /// A GUI editor associated to the interpreter.
        /// </summary>
        public UserControl UI
        {
            get{return this.Editor;}
        }

        public ScriptInterpreter()
        {
            this.Editor = new ScriptEditor(this);

            EventMap();

            this.ExprTypeList = ListExpressions();
        }

        List<Type> ExprTypeList;
        private List<Type> ListExpressions()
        {
            var type = typeof(AbstractExpression);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p));

            List<Type> lt = types.ToList<Type>();

            // Optional since they always return null; slight better efficiency
            lt.Remove(typeof(AbstractExpression));
            lt.Remove(typeof(BehaviorSpeechExpression));
            lt.Remove(typeof(BehaviorPauseExpression));
            // Move speech expression to the end; because it is not mutual exclusive from other expressions
            lt.Remove(typeof(SpeechExpression));
            lt.Add(typeof(SpeechExpression));

            return lt;
        }

        /// <summary>
        /// Current procedure is to parse and execute piece-by-piece simultaneously
        /// Threads: The action of each sentence costs time, 
        ///          so the time can be used to parse next sentences.
        /// </summary>
        /// <param name="script">the whole script texts</param>
        internal void ExecuteScript(string[] script)
        {
            ScriptLineCount = script.Count();

            BackgroundWorker bgwProducer = new BackgroundWorker();
            bgwProducer.DoWork += new DoWorkEventHandler(ProcessScriptInQueue);
            EnabledParse = true;
            bgwProducer.RunWorkerAsync(script);

            BackgroundWorker bgwConsumer = new BackgroundWorker();
            bgwConsumer.DoWork += new DoWorkEventHandler(ExecuteScriptFromQueue);
            EnableExecute = true;
            bgwConsumer.RunWorkerAsync();
        }

        /// <summary>
        /// For thread-safe, can also use ConcurrentQueue
        /// </summary>
        Queue<IExpression> ExpressionQueue = new Queue<IExpression>();
        const int QueueSize = 10;
        Mutex MutQueue = new Mutex();

        int ScriptLineCount;

        bool ScriptParseComplete = false;
        bool EnabledParse = false;
        /// <summary>
        /// The thread that interprets script texts into IExpression objects.
        /// </summary>
        /// <param name="sender">see DoWorkEventHandler</param>
        /// <param name="e">see DoWorkEventHandler</param>
        private void ProcessScriptInQueue(object sender, DoWorkEventArgs e)
        {
            ExpressionQueue.Clear();

            ScriptParseComplete = false;

            string[] script = (string[])e.Argument;

            int i = 0;
            while (i < ScriptLineCount)
            {
                if (EnabledParse)
                {
	                if (ExpressionQueue.Count < QueueSize)
	                {
                        IExpression expr = null;
                        if (i < ScriptLineCount - 1)
                        {
                            expr = ParseScriptUnit(script[i], script[i + 1]);
                        }
                        else
                        {
                            expr = ParseScriptUnit(script[i], null);
                        }

                        if (expr != null)
                        {
	                        ((AbstractExpression)expr).LineNumber = i;
	                        // for combined expression (e.g., behavior + speech), i++ once more
	                        if (expr.GetType() == typeof(BehaviorSpeechExpression)
	                            || expr.GetType() == typeof(BehaviorPauseExpression))
	                        {
	                            i++;
	                        }
	
		                    MutQueue.WaitOne();
			                ExpressionQueue.Enqueue(expr);
		                    MutQueue.ReleaseMutex();
                        }
	
	                    i++;
	                }
	                else
	                {
	                    //Debug.WriteLine("{0}: ScriptUnit queue full - Queue Length {1}", "ScriptParser", ScriptQueue.Count);
	                }
                }

                Thread.Sleep(333);
            }

            ScriptParseComplete = true;
            Debug.WriteLine("{0}: All Script Parsed! Total lines: {1}", "ScriptParser", i);
        }

        bool EnableExecute = false;
        /// <summary>
        /// The thread that consumes the IExpression object into defined actions.
        /// </summary>
        /// <param name="sender">see DoWorkEventHandler</param>
        /// <param name="e">see DoWorkEventHandler</param>
        private void ExecuteScriptFromQueue(object sender, DoWorkEventArgs e)
        {
            int i = 0;
            while(i < ScriptLineCount)
            {
                if (EnableExecute)
                {
	                if (ExpressionQueue.Count > 0)
	                {
		                MutQueue.WaitOne();
		                IExpression expr = ExpressionQueue.Dequeue();
	                    MutQueue.ReleaseMutex();

                        this.Editor.ShowCurrentLine(((AbstractExpression)expr).LineNumber);
	
		                i++;

                        // Log Behavior and Speech
                        if (expr.GetType() == typeof(BehaviorExpression)
                            || expr.GetType() == typeof(BehaviorSpeechExpression)
                            || expr.GetType() == typeof(SpeechExpression))
                        {
	                        string curtime = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt");
                            string msg = curtime + "\t" + expr.LogInfo();
	                        string logfile = Directory.GetCurrentDirectory() + "\\..\\..\\log\\StorytellingLog.txt";
	                        StreamWriter logger = new StreamWriter(logfile, true);
	                        logger.WriteLine(msg);
	                        logger.Close();
                        }
	
	                    // Execute ScriptUnit; 
	                    // This is a blocking-call; It waits for finish of e.g., speech, behavior
                        expr.Execute();
	                }
	                else
	                {
	                    if (ScriptParseComplete == true)
	                    {
	                        break;
	                    }
	                    //Debug.WriteLine("{0}: ScriptUnit queue empty! Script Line {1}", "ScriptParser", i);
	                }
                }

                Thread.Sleep(100);
            }

            evScriptExeFinished();
        }

        // To synchronize Behavior and Speech
        AutoResetEvent areBehaviorDone = new AutoResetEvent(false);
        AutoResetEvent areSpeechDone = new AutoResetEvent(false);

        // To check if the coming behavior conflicts with random movements (leg, head)
        internal event ehLatestScriptUnit evLatestScriptUnit;
        internal delegate void ehLatestScriptUnit(string behaviorname);

        // Inform that all IExpression objects have been executed 
        internal event ehScriptExeFinished evScriptExeFinished;
        internal delegate void ehScriptExeFinished();

        /// <summary>
        /// Convert a single "script block" (1 or 2 lines of texts) to an IExpression object.
        /// A block may contain
        ///   1. behavior + speech (two lines)
        ///   2. behavior + pause (two lines)
        ///   3. only behavior or other syntax (one lines)
        /// When the first line is not a Behavior syntax, the nextline is ignored.
        /// </summary>
        /// <param name="line">the single line texts</param>
        /// <param name="nextline">the next single line texts</param>
        /// <returns>IExrepssion object</returns>
        internal IExpression ParseScriptUnit(string line, string nextline)
        {
            IExpression expression = ParseLine(line);

            if (expression != null && expression.GetType() == typeof(BehaviorExpression))
            {
                BehaviorExpression behaviorexpr = (BehaviorExpression)expression;

                // Send out for check if it is a leg movement
                evLatestScriptUnit(behaviorexpr.BehaviorName);

                if (nextline != null)
                {
                    IExpression combined_expr = null;
                    IExpression exprnext = ParseLine(nextline);
                    if (exprnext.GetType() == typeof(SpeechExpression))
                    {
                        combined_expr = new BehaviorSpeechExpression(behaviorexpr, exprnext);
                    }
                    else if (exprnext.GetType() == typeof(PauseExpression))
                    {
                        combined_expr = new BehaviorPauseExpression(behaviorexpr, exprnext);
                    }

                    return combined_expr;
                }
                else
                {
                    return behaviorexpr;
                }
            }

            return expression;
        }

        /// <summary>
        /// Convert a single line texts to an IExpression object.
        /// </summary>
        /// <param name="line">a single line texts</param>
        /// <returns>IExpression object</returns>
        private IExpression ParseLine(string line)
        {            
            if (line == String.Empty)
            {
                return null;
            }
            else if (line.StartsWith("#"))
            {
                return null; // comment
            }
            else
            {
                IExpression expression = null;
                for (int i = 0; i < this.ExprTypeList.Count; i++)
                {
                    Type t = this.ExprTypeList[i];
                    //expression = (IExpression)t.GetMethod("Interpret").Invoke(new Object(), new Object[] { line });
                    expression = (IExpression)t.InvokeMember("Interpret", 
                        BindingFlags.Static | BindingFlags.InvokeMethod | BindingFlags.Public, 
                        null, null, new object[] { line });
                    if (expression != null) break;
                }
                return expression;
            }
        }

        /// <summary>
        /// Pause the script interpretation and execution.
        /// The generated IExpression objects still remain in the Queue.
        /// </summary>
        internal void PauseScript()
        {
            EnabledParse = false;
            EnableExecute = false;

            SendMessage("NaoManager", new MessageEventArgs("ScriptPaused"));
        }

        /// <summary>
        /// Resume the script interpretation and execution.
        /// </summary>
        internal void ResumeScript()
        {
            EnableExecute = true;
            EnabledParse = true;
        }

        /// <summary>
        /// Stop the script interpretation and execution.
        /// All generated IExpression objects are lost/cleared.
        /// </summary>
        internal void StopScript()
        {
            //ScriptParseComplete = false; // done by ProcessScriptInQueue()
            //IsParserOnTheWay = false; // done by ExecuteScriptFromQueue()
            ScriptLineCount = 0; // This ends the Process and Execution threads: ends the loop 
            EnabledParse = false;
            EnableExecute = false;
            ExpressionQueue.Clear();
        }

        /// <summary>
        /// Check if there is a mis-spelled behavior name before running the script.
        /// </summary>
        /// <param name="script">the whole script texts</param>
        /// <returns>A error message containing the mis-spelled behavior list or "OK"</returns>
        internal string CheckBehaviors(string[] script)
        {

            int scriptlen = script.Count();
            int i = 0;
            string errorBehaviors = "";
            int behaviorCnt = 0;
            int errorBehaviorCnt = 0;
            while (i < scriptlen)
            {
                IExpression st = ParseScriptUnit(script[i], null);

                if (st != null && st.GetType() == typeof(BehaviorExpression))
                {
                    behaviorCnt++;

                    BehaviorExpression bst = (BehaviorExpression)st;

                    MessageEventArgs backmsg = SendMessage("NaoManager", new MessageEventArgs("CheckBehavior", new string[] { bst.BehaviorName }));
                    
                    if (backmsg != null)
                    {
                        bool isBehaviorExist = (bool)(backmsg.DataReturn);
                        if (isBehaviorExist == false)
                        {
                            errorBehaviorCnt++;
                            errorBehaviors += bst.BehaviorName + Environment.NewLine;
                        }
                    }
                }

                i++;
            }

            if (errorBehaviors.Length > 0)
            {
                errorBehaviors = "Among " + behaviorCnt
                    + " behaviors, " + errorBehaviorCnt
                    + " below do not exist:" + Environment.NewLine
                    + errorBehaviors;

                return errorBehaviors;
            }

            return "OK";
        }

        internal void BehaviorDone()
        {
            areBehaviorDone.Set();
        }

        internal void SpeechDone()
        {
            areSpeechDone.Set();
        }

        /// <summary>
        /// To connect the event handlers in IExpression objects to corresponding actions.
        /// To connect other event handlers of the Interpreter. 
        /// </summary>
        private void EventMap()
        {
            BehaviorSpeechExpression.evExecuteBehaviorSpeech += delegate(string behaviorname, string texttosay)
            {
                SendMessage("NaoManager", new MessageEventArgs("ExecuteBehavior", new string[] { behaviorname }));
                SendMessage("NaoManager", new MessageEventArgs("Say", new string[] { texttosay }));
                areBehaviorDone.WaitOne();
                areSpeechDone.WaitOne();
            };

            ehExecuteBehavior dlgExecuteBehavior = delegate(string behaviorname)
            {
                SendMessage("NaoManager", new MessageEventArgs("ExecuteBehavior", new string[] { behaviorname }));
                areBehaviorDone.WaitOne();
            };
            BehaviorExpression.evExecuteBehavior += dlgExecuteBehavior;
            BehaviorPauseExpression.evExecuteBehavior += dlgExecuteBehavior;

            ehExecuteSpeech dlgExecuteSpeech = delegate(string texttosay)
            {
                SendMessage("NaoManager", new MessageEventArgs("Say", new string[] { texttosay }));
                areSpeechDone.WaitOne();
            };
            SpeechExpression.evExecuteSpeech += dlgExecuteSpeech;

            CameraExpression.evCameraPhoto += delegate()
            {
                MessageEventArgs backmsg = SendMessage("NaoManager", new MessageEventArgs("CapturePhoto", null));
                string imgfile = backmsg.TextMsg;
                SendMessage("PPTController", new MessageEventArgs("ShowPhoto", new string[] { imgfile }));
            };

            SlideExpression.evSlideControl += delegate(string cmd)
            {
                SendMessage("PPTController", new MessageEventArgs(cmd, null));
            };

            QuizExpression.evFetchQuizAnswers += delegate(out bool disagree, out int useranswer, out int useranswer2)
            {
                MessageEventArgs backmsg = SendMessage("PPTController", new MessageEventArgs("FetchQuizAnswers", null));
                if (backmsg.MsgType == MessageEventArgs.MessageType.DATA)
                {
                    string[] data = backmsg.DataReturn as string[];
                    disagree = bool.Parse(data[0]);
                    useranswer = int.Parse(data[1]);
                    useranswer2 = int.Parse(data[2]);
                }
                else
                {
                    disagree = false;
                    useranswer = -1;
                    useranswer2 = -1;
                }
            };

            MoodExpression.evChangeMood += delegate(string mood)
            {
                SendMessage("NaoManager", new MessageEventArgs("ChangeRobotMood", new string[] { mood }));
            };

            ConfigExpression.evChangeConfig += delegate(string config)
            {
                SendMessage("NaoManager", new MessageEventArgs("Config", new string[] { config }));
            };

            InterruptExpression.evInterrupt += delegate()
            {
                this.Editor.EnableUI(ScriptEditorState.PAUSE);

                PauseScript();
                SendMessage("NaoManager", new MessageEventArgs("ScriptPaused"));
            };

            evScriptExeFinished += delegate()
            {
                SendMessage("NaoManager", new MessageEventArgs("ScriptEnded"));

                this.Editor.ScriptExecuted();

                string curtime = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt");
                string logfile = Directory.GetCurrentDirectory() + "\\..\\..\\log\\StorytellingLog.txt";
                StreamWriter logger = new StreamWriter(logfile, true);
                string strend = curtime + "\tStory Ended!";
                logger.WriteLine(strend);
                logger.WriteLine(" ----- ");
                logger.Close();
            };

            evLatestScriptUnit += delegate(string behaviorname)
            {
                SendMessage("NaoManager", new MessageEventArgs("CheckIdleMoveConflicts", new string[] { behaviorname }));
            };
        }

        #region Messenger
        public string ID
        {
            get { return "ScriptEngine"; }
        }

        public event ehSendMessage evSendMessage;
        public MessageEventArgs SendMessage(string sendto, MessageEventArgs msg)
        {
            if (evSendMessage != null)
            {
                return evSendMessage(sendto, this.ID, msg);
            }
            else
                return null;
        }

        public MessageEventArgs MessageHandler(string sendfrom, MessageEventArgs message)
        {
            if (message.MsgType == MessageEventArgs.MessageType.COMMAND)
            {
                if (message.Cmd == "NaoSpeechFinished")
                {
                    int len = int.Parse(message.CmdArgs[0]);
                    //Debug.WriteLine("{0}: NAO speech finished - {1}", this.ID, len );

                    SpeechDone();
                }
                else if (message.Cmd == "TTSNotConnected")
                {
                    Console.WriteLine("{0}: Speech is not said: {1}", this.ID, "TTSNotConnected!");
                    SpeechDone();
                }
                else if (message.Cmd == "NaoBehaviorFinished")
                {
                    string behaviorname = message.CmdArgs[0];
                    Debug.WriteLine("{0}: NAO behavior finished - {1}", this.ID, behaviorname);

                    BehaviorDone();
                }
                else if (message.Cmd == "NaoBehaviorNotExecuted")
                {
                    string behaviorname = message.CmdArgs[0];
                    Debug.WriteLine("{0}: NAO behavior NOT executed! - {1}", this.ID, behaviorname);
                    BehaviorDone();
                }
            }

            return null;
        }
        #endregion Messenger

    }


}
