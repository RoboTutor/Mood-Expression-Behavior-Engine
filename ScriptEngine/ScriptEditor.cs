/*
 * This code and information is provided "as is" without warranty of any kind, 
 * either expressed or implied, including but not limited to the implied warranties 
 * of merchantability and/or fitness for a particular purpose.
 * 
 * License: 
 * 
 * Email: junchaoxu86@gmail.com; k.v.hindriks@tudelft.nl
 * 
 * Copyright © Junchao Xu, Interactive Intelligence, TUDelft, 2014.
 * 
 * Credit to: Pavel Torgashov, Fast Colored TextBox, http://www.codeproject.com/Articles/161871/Fast-Colored-TextBox-for-syntax-highlighting
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FastColoredTextBoxNS;

namespace ScriptEngine
{
    /// <summary>
    /// The authoring tool for creating scripts
    /// </summary>
    public partial class ScriptEditor : UserControl
    {
        ScriptInterpreter Parser = null;

        public ScriptEditor(ScriptInterpreter parser = null)
        {
            InitializeComponent();

            this.Parser = parser;
        }

        /// <summary>
        /// Obsolete. Used for automatic text sentiment analysis.
        /// </summary>
        //bool TextSentiEnabled = false;

        /// <summary>
        /// Change the UI states.
        /// </summary>
        /// <param name="state">the state of the editor</param>
        internal void EnableUI(ScriptEditorState state)
        {
            Invoke((MethodInvoker)delegate()
            {
                if (state == ScriptEditorState.PLAYING)
                {
                    this.fctbScriptEditor.ReadOnly = true;
                    this.btnExecute.Enabled = false;
                    this.btnLoad.Enabled = false;
                    this.btnPause.Enabled = true;
                    this.btnStop.Enabled = true;
                }
                else if (state == ScriptEditorState.PAUSE)
                {
                    this.btnExecute.Enabled = true;
                    this.btnPause.Enabled = false;
                }
                else if (state == ScriptEditorState.READY_TO_PLAY) // "Stop"
                {
                    this.fctbScriptEditor.ReadOnly = false;
                    this.fctbScriptEditor.SelectionColor = Color.Blue;
                    this.btnExecute.Enabled = true;
                    this.btnLoad.Enabled = true;
                    this.btnPause.Enabled = false;
                    this.btnStop.Enabled = false;
                }
            });
        }

        bool IsParserOnTheWay = false;
        private void btnExecute_Click(object sender, EventArgs e)
        {
            string[] script = this.fctbScriptEditor.Lines.ToArray();
            string res = Parser.CheckBehaviors(script);
            if (res != "OK") return;

            EnableUI(ScriptEditorState.PLAYING);

            if (IsParserOnTheWay)
            {
                Parser.ResumeScript();
            } 
            else // start new execution
            {
	            IsParserOnTheWay = true;
	            Parser.ExecuteScript(script);

                string curtime = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt");
                string logfile = Directory.GetCurrentDirectory() + "\\..\\..\\log\\StorytellingLog.txt";
                StreamWriter logger = new StreamWriter(logfile, true);
                string strstart = "----- " + curtime + "\tStory Started! -----";
                logger.WriteLine(strstart);
                logger.Close();
            }
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            EnableUI(ScriptEditorState.PAUSE);
            Parser.PauseScript();
            
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            Parser.StopScript();
            // Parser will emit evScriptExeFinished at the end of the parsing loop.
            // The following will happen
            //  1. send message to NaoManager
            //  2. EnableUI();
        }

        string CurrentFile;
        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = Environment.CurrentDirectory + "\\..\\..\\Script\\";
            DialogResult dr = ofd.ShowDialog();
            if (dr == DialogResult.OK)
            {
                CurrentFile = ofd.FileName;
                this.fctbScriptEditor.Text = File.ReadAllText(CurrentFile);

                EnableUI(ScriptEditorState.READY_TO_PLAY);
            }
        }

        private void fctbScriptEditor_TextChanged(object sender, FastColoredTextBoxNS.TextChangedEventArgs e)
        {
            ScriptSyntaxHighlight(e);

            if (string.IsNullOrEmpty(this.fctbScriptEditor.Text))
                btnExecute.Enabled = false;
            else
                btnExecute.Enabled = true;
        }

        //styles
        TextStyle BlueStyle = new TextStyle(Brushes.Blue, null, FontStyle.Regular);
        TextStyle GrayStyle = new TextStyle(Brushes.Gray, null, FontStyle.Regular);
        TextStyle MagentaStyle = new TextStyle(Brushes.Magenta, null, FontStyle.Regular);
        TextStyle PurpleStyle = new TextStyle(Brushes.Purple, null, FontStyle.Regular);
        TextStyle GreenStyle = new TextStyle(Brushes.Green, null, FontStyle.Italic);
        TextStyle BrownStyle = new TextStyle(Brushes.Brown, null, FontStyle.Italic);
        TextStyle MaroonStyle = new TextStyle(Brushes.Maroon, null, FontStyle.Regular);
        TextStyle BoldStyle = new TextStyle(null, null, FontStyle.Bold | FontStyle.Underline);
        MarkerStyle SameWordsStyle = new MarkerStyle(new SolidBrush(Color.FromArgb(40, Color.Gray)));

        /// <summary>
        /// See Regular Expression (regex)
        /// http://msdn.microsoft.com/en-us/library/az24scfc(v=vs.110).aspx
        /// </summary>
        /// <param name="e"></param>
        private void ScriptSyntaxHighlight(TextChangedEventArgs e)
        {
            //clear style of changed range
            e.ChangedRange.ClearStyle(BlueStyle, BoldStyle, GrayStyle, MagentaStyle, GreenStyle, BrownStyle);

            //comment highlighting; only at the beginning of a line!
            e.ChangedRange.SetStyle(GreenStyle, @"^#.*$", RegexOptions.Multiline);

            // TODO: keywords list into XML files
            //keyword highlighting
            // Command
            e.ChangedRange.SetStyle(BlueStyle, @"\b(behavior|slide|quiz|camera|pause)\b");
            e.ChangedRange.SetStyle(PurpleStyle, @"\b(mood|text_senti_enabled|constant_head|forwardlooking_head|freelooking_head)\b");

            // NAO TTS
            e.ChangedRange.SetStyle(MagentaStyle, @"\b(pau|rst|rspd|vct)\b");
            // NAO Motion
            e.ChangedRange.SetStyle(BrownStyle, @"\b(idle_leg|idle_head|idle_eye)\b");

            //class name highlighting
            e.ChangedRange.SetStyle(BoldStyle, @"\b(behavior|quiz)\s+(?<range>\w+?)\b");

            //clear folding markers
            e.ChangedRange.ClearFoldingMarkers();

            //set folding markers
            e.ChangedRange.SetFoldingMarkers(@"##<", @"##>"); 
        }

        private void tsbtnJumpLine_Click(object sender, EventArgs e)
        {
            int line = (int)this.nudJumpNum.Value;
            this.fctbScriptEditor.Navigate(line);
        }

        private void tsbtnSave_Click(object sender, EventArgs e)
        {
            if (CurrentFile != null)
            {
	            string script = fctbScriptEditor.Text;
	            File.WriteAllText(CurrentFile, script);
            }
        }

        private void btnSaveAs_Click(object sender, EventArgs e)
        {

        }

        private void insertBehavior_Click(object sender, EventArgs e)
        {
            fctbScriptEditor.InsertText("{behavior|}");
        }

        private void tsbtnRemoveBehavior_Click(object sender, EventArgs e)
        {
            Form removebehaviorform = new Form();
            removebehaviorform.Text = "RemoveBehavior";

            Label lb = new Label();
            lb.Text = "Input the Name of the Behavior to be removed:";
            lb.AutoSize = true;
            removebehaviorform.Controls.Add(lb);

            TextBox behname = new TextBox();
            behname.Location = new Point(behname.Location.X, lb.Location.Y + 3 + lb.Height);
            removebehaviorform.Controls.Add(behname);

            Button btnok = new Button();
            btnok.Click += 
                new EventHandler(delegate
                {
                    removebehaviorform.DialogResult = DialogResult.OK;
                    removebehaviorform.Close();
                });
            btnok.Text = "OK";
            btnok.Location = new Point(btnok.Location.X, behname.Location.Y + 3 + behname.Height);
            removebehaviorform.Controls.Add(btnok);

            removebehaviorform.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            removebehaviorform.AutoSize = true;

            removebehaviorform.StartPosition = FormStartPosition.CenterParent;
            DialogResult dr = removebehaviorform.ShowDialog();

            if (dr == DialogResult.OK)
            {
                string behaviorname = behname.Text;
                List<string> scriptlines = fctbScriptEditor.Lines.ToList();
                List<int> line_indices = scriptlines.IndexesWhere(t => t.Contains(behaviorname)).ToList();
                string res = "Find " + line_indices.Count + " occurrences!"
                    + Environment.NewLine + "Confirm to remove ALL these behaviors?";
                DialogResult mr = MessageBox.Show(res, "SearchResults", MessageBoxButtons.YesNo);
                if (mr == DialogResult.Yes)
                {
                	fctbScriptEditor.RemoveLines(line_indices);
                }
            }
        }

        private void insertQuiz_Click(object sender, EventArgs e)
        {
            Form quizform = new Form();
            quizform.Text = "InputNumberChoices";

            Label lb = new Label();
            lb.Text = "Input the Number of Quiz Choices:";
            quizform.Controls.Add(lb);

            NumericUpDown nud_num_answers = new NumericUpDown();
            nud_num_answers.Location = new Point(nud_num_answers.Location.X, lb.Location.Y + 3 + lb.Height);
            quizform.Controls.Add(nud_num_answers);

            Button btnok = new Button();
            btnok.Click += 
                new EventHandler(delegate
                {
                    quizform.DialogResult = DialogResult.OK;
                    quizform.Close();
                });
            btnok.Text = "OK";
            btnok.Location = new Point(btnok.Location.X, nud_num_answers.Location.Y + 3 + nud_num_answers.Height);
            quizform.Controls.Add(btnok);

            quizform.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            quizform.AutoSize = true;

            // does not work
            //quizform.Parent = sender as Control;
            quizform.StartPosition = FormStartPosition.CenterParent;
            DialogResult dr = quizform.ShowDialog();

            if (dr == DialogResult.OK)
            {
                int num_answers = (int)nud_num_answers.Value;
                StringBuilder sb = new StringBuilder("{quiz|");
                for (int i = 0; i < num_answers; i++)
                {
                    sb.Append(" answer").Append(i+1).Append(" |");
                }
                sb.Append(" inconclusive answer }");
            	fctbScriptEditor.InsertText(sb.ToString());
            }
        }

        private void tsbtnCheck_Click(object sender, EventArgs e)
        {
            string[] script = this.fctbScriptEditor.Lines.ToArray();

            string result = Parser.CheckBehaviors(script);

            if (result == "OK") MessageBox.Show("All behaviors exist!", "Check Behaviors");
            else MessageBox.Show(result, "Check Behaviors");
        }

        internal void ScriptExecuted()
        {
            IsParserOnTheWay = false;

            EnableUI(ScriptEditorState.READY_TO_PLAY);
        }

        internal void ShowCurrentLine(int linenum)
        {
            Invoke((MethodInvoker)delegate()
            {
                fctbScriptEditor.Navigate(linenum);
                fctbScriptEditor.Selection = new Range(this.fctbScriptEditor, 0, linenum, 0, linenum + 1);
                fctbScriptEditor.SelectionColor = Color.OrangeRed;
                //fctbScriptEditor.CurrentLineColor = Color.GreenYellow;
            });
        }
    }

    internal enum ScriptEditorState
    {
        PLAYING,
        PAUSE,
        READY_TO_PLAY,
    }
}
