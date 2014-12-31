/*
 * Please see "ScriptInterpreter.cs" for information.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;

namespace ScriptEngine
{
    #region Specific Expressions
    internal delegate void ehExecuteBehaviorSpeech(string behaviorname, string texttosay);
    class BehaviorSpeechExpression : AbstractExpression
    {
        string BehaviorName;
        string TextToSay;

        public BehaviorSpeechExpression(IExpression behaviorexpr, IExpression speechexpr)
        {
            this.BehaviorName = ((BehaviorExpression)behaviorexpr).BehaviorName;
            this.TextToSay = ((SpeechExpression)speechexpr).TextToSay;
        }

        public static IExpression Interpret(string line) { return null; }

        public static event ehExecuteBehaviorSpeech evExecuteBehaviorSpeech;
        public override void Execute()
        {
            evExecuteBehaviorSpeech(BehaviorName, TextToSay);
        }

        public override string LogInfo()
        {
            string msg = "BehaviorSpeechExpression"
                + "\t" + this.BehaviorName
                + "\t" + this.TextToSay;
            return msg;
        }
    }

    class BehaviorPauseExpression : AbstractExpression
    {
        string BehaviorName;
        int PauseDuration;

        public BehaviorPauseExpression(IExpression behaviorexpr, IExpression exprnext)
        {
            this.BehaviorName = ((BehaviorExpression)behaviorexpr).BehaviorName;
            this.PauseDuration = ((PauseExpression)exprnext).PauseDuration;
        }

        public static IExpression Interpret(string line) { return null; }

        public static event ehExecuteBehavior evExecuteBehavior;
        public override void Execute()
        {
            evExecuteBehavior(BehaviorName);
            Thread.Sleep(PauseDuration);
        }
    }

    internal delegate void ehExecuteBehavior(string behaviorname);
    class BehaviorExpression : AbstractExpression
    {
        internal string BehaviorName;

        private BehaviorExpression(string behaviorname)
        {
            this.BehaviorName = behaviorname;
        }

        public static IExpression Interpret(string line)
        {
            if (Regex.IsMatch(line, @"^(\{behavior\|)(\w+)(\})$"))
            {
                Match m = Regex.Match(line, @"^(\{behavior\|)(\w+)(\})$");
                IExpression behaviorexpr = new BehaviorExpression(m.Groups[2].Value);
                return behaviorexpr;
            }
            else return null;
        }

        public static event ehExecuteBehavior evExecuteBehavior;
        public override void Execute()
        {
            evExecuteBehavior(BehaviorName);
        }

        public override string LogInfo()
        {
            string msg = "BehaviorExpression" + "\t" + this.BehaviorName;
            return msg;
        }
    }

    public delegate void ehExecuteSpeech(string text);
    class SpeechExpression : AbstractExpression
    {
        internal string TextToSay;

        private SpeechExpression(string content)
        {
            this.TextToSay = content;
        }

        public static IExpression Interpret(string line)
        {
            if (line != String.Empty)
            {
                string result = line;
                IExpression speechexpr = new SpeechExpression(result);
                return speechexpr;
            }
            else return null;
        }

        public static event ehExecuteSpeech evExecuteSpeech;
        public override void Execute()
        {
            evExecuteSpeech(TextToSay);
        }

        public override string LogInfo()
        {
            string msg = "SpeechExpression" + "\t" + this.TextToSay;
            return msg;
        }
    }

    class PauseExpression : AbstractExpression
    {
        internal int PauseDuration;

        private PauseExpression(int pausedur)
        {
            this.PauseDuration = pausedur;
        }

        public static IExpression Interpret(string line)
        {
            // To match a pure "\pau=?\" using $; see C# Regex
            if (Regex.IsMatch(line, @"^(\\pau=)(\w+)(\\)$"))
            {
                Match m = Regex.Match(line, @"^(\\pau=)(\w+)(\\)$");
                // Groups[0] is the whole match value; [1]: "\pau="; [2]: "\";
                IExpression pauseexpr = new PauseExpression(Convert.ToInt32(m.Groups[2].Value));
                return pauseexpr;
            }
            else return null;
        }

        public override void Execute()
        {
            Thread.Sleep(PauseDuration);
        }
    }

    internal delegate void ehSlideControl(string cmd);
    class SlideExpression : AbstractExpression
    {
        private SlideExpression() { }

        public static IExpression Interpret(string line)
        {
            if (line.StartsWith("{slide}"))
            {
                IExpression slideexpr = new SlideExpression();
                return slideexpr;
            }
            else return null;
        }

        public static event ehSlideControl evSlideControl;
        public override void Execute()
        {
            evSlideControl("NextSlide");
        }
    }

    public delegate void ehCameraPhoto();
    class CameraExpression : AbstractExpression
    {
        private CameraExpression() { }

        public static IExpression Interpret(string line)
        {
            if (line.StartsWith("{camera}"))
            {
                IExpression cameraexpr = new CameraExpression();
                return cameraexpr;
            }
            else return null;
        }

        public static event ehCameraPhoto evCameraPhoto;
        public override void Execute()
        {
            evCameraPhoto();
        }
    }

    public delegate void ehFetchQuizAnswers(out bool disagree, out int useranswer, out int useranswer2);
    class QuizExpression : AbstractExpression
    {
        List<string> QuizAnswer;

        private QuizExpression(string content)
        {
            QuizAnswer = new List<string>();
            string[] qa = content.Split('|');
            QuizAnswer.AddRange(qa.ToList<string>());
        }

        public static IExpression Interpret(string line)
        {
            if (Regex.IsMatch(line, @"^(\{quiz\|)(\w+)(\})$"))
            {
                Match m = Regex.Match(line, @"^(\{quiz\|)(\w+)(\})$");
                IExpression quizexpr = new QuizExpression(m.Groups[2].Value);
                return quizexpr;
            }
            else return null;
        }

        public static event ehFetchQuizAnswers evFetchQuizAnswers;
        public static event ehExecuteSpeech evExecuteSpeech;
        public override void Execute()
        {
            bool disagreement;
            int useranswer1, useranswer2;
            evFetchQuizAnswers(out disagreement, out useranswer1, out useranswer2);
            string speechtoanswers;

            if (useranswer1 == -1) // this means the answer data is not ready!
            {
                speechtoanswers = "It seems no one knows the answer!";
            }
            else
            {
                if (disagreement == true) { speechtoanswers = QuizAnswer.Last<string>(); }
                else { speechtoanswers = QuizAnswer[useranswer1]; }
            }

            evExecuteSpeech(speechtoanswers);
        }
    }

    public delegate void ehChangeConfig(string config);
    class ConfigExpression : AbstractExpression
    {
        string Config;

        private ConfigExpression(string content)
        {
            this.Config = content;
        }

        static List<string> ConfigCommands = null;
        private static void ReadConfigCommands()
        {
            XmlDocument xmldoc = new XmlDocument();
            string file = Environment.CurrentDirectory + "\\..\\..\\ScriptEngine\\ConfigCommands.xml";

            try
            {
                xmldoc.Load(file);
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            ConfigCommands = new List<string>();

            XmlNode configexpressions = xmldoc.SelectSingleNode("ScriptInterpreter/ConfigExpressions");
            if (configexpressions != null) // No such node "NaoIP"
            {
                foreach (XmlNode cmd in configexpressions.ChildNodes)
                {
                    ConfigCommands.Add(cmd.InnerText);
                }
            }
            else
                Debug.WriteLine("Config Commands do not exist!");
        }

        public static IExpression Interpret(string line)
        {
            if (ConfigCommands == null)
            {
                ReadConfigCommands();
            }

            IExpression configexpr = null;
            foreach (string cmd in ConfigCommands)
            {
                if (line.Contains(cmd))
                {
                    string result = line.Replace("{", "").Replace("}", "");
                    configexpr = new ConfigExpression(result);
                    break;
                }
            }

            return configexpr;
        }

        public static event ehChangeConfig evChangeConfig;
        public override void Execute()
        {
            evChangeConfig(Config);
        }
    }

    public delegate void ehChangeMood(string mood);
    class MoodExpression : AbstractExpression
    {
        string Mood;

        private MoodExpression(string mood)
        {
            this.Mood = mood;
        }

        public static IExpression Interpret(string line)
        {
            if (Regex.IsMatch(line, @"^(\{mood\|)(\w+)(\})$"))
            {
                Match m = Regex.Match(line, @"^(\{mood\|)(\w+)(\})$");
                IExpression moodexpr = new MoodExpression(m.Groups[2].Value);
                return moodexpr;
            }
            else return null;
        }

        public static event ehChangeMood evChangeMood;
        public override void Execute()
        {
            evChangeMood(Mood);
        }
    }

    public delegate void ehInterrupt();
    class InterruptExpression : AbstractExpression
    {
        private InterruptExpression() { }

        public static IExpression Interpret(string line)
        {
            if (line.StartsWith("{pause}"))
            {
                IExpression interruptexpr = new InterruptExpression();
                return interruptexpr;
            }
            else return null;
        }

        public static event ehInterrupt evInterrupt;
        public override void Execute()
        {
            evInterrupt();
        }
    }

    #endregion Specific Expressions


    interface IExpression
    {
        void Execute();
        string LogInfo();
    }

    /// <summary>
    /// End with a Speech Sentence
    /// Can contain only one Sentence and Behavior.
    /// Within the Unit there is no End Point Sync.
    /// </summary>
    abstract class AbstractExpression : IExpression
    {
        public int LineNumber;
        public abstract void Execute();
        public virtual string LogInfo() { return this.GetType().ToString(); }
    }
}
