namespace DashBoard
{
    partial class ComponentManager
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tbpScriptEngine = new System.Windows.Forms.TabPage();
            this.tbpPPT = new System.Windows.Forms.TabPage();
            this.tbpRobotMgr = new System.Windows.Forms.TabPage();
            this.tbpMain = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rdbtnPositiveMoodCondition = new System.Windows.Forms.RadioButton();
            this.rdbtnNegativeMoodCondition = new System.Windows.Forms.RadioButton();
            this.btnSecondPart = new System.Windows.Forms.Button();
            this.tbcMainTab = new System.Windows.Forms.TabControl();
            this.tbpMain.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tbcMainTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbpScriptEngine
            // 
            this.tbpScriptEngine.BackColor = System.Drawing.SystemColors.Control;
            this.tbpScriptEngine.Location = new System.Drawing.Point(4, 24);
            this.tbpScriptEngine.Name = "tbpScriptEngine";
            this.tbpScriptEngine.Padding = new System.Windows.Forms.Padding(3);
            this.tbpScriptEngine.Size = new System.Drawing.Size(1076, 634);
            this.tbpScriptEngine.TabIndex = 1;
            this.tbpScriptEngine.Text = "ScriptEngine";
            // 
            // tbpPPT
            // 
            this.tbpPPT.BackColor = System.Drawing.SystemColors.Control;
            this.tbpPPT.Location = new System.Drawing.Point(4, 24);
            this.tbpPPT.Name = "tbpPPT";
            this.tbpPPT.Padding = new System.Windows.Forms.Padding(3);
            this.tbpPPT.Size = new System.Drawing.Size(1076, 634);
            this.tbpPPT.TabIndex = 3;
            this.tbpPPT.Text = "PPT";
            // 
            // tbpRobotMgr
            // 
            this.tbpRobotMgr.BackColor = System.Drawing.SystemColors.Control;
            this.tbpRobotMgr.Location = new System.Drawing.Point(4, 24);
            this.tbpRobotMgr.Name = "tbpRobotMgr";
            this.tbpRobotMgr.Size = new System.Drawing.Size(1076, 634);
            this.tbpRobotMgr.TabIndex = 4;
            this.tbpRobotMgr.Text = "Robot";
            // 
            // tbpMain
            // 
            this.tbpMain.BackColor = System.Drawing.SystemColors.Control;
            this.tbpMain.Controls.Add(this.label1);
            this.tbpMain.Controls.Add(this.groupBox2);
            this.tbpMain.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbpMain.Location = new System.Drawing.Point(4, 24);
            this.tbpMain.Name = "tbpMain";
            this.tbpMain.Padding = new System.Windows.Forms.Padding(3);
            this.tbpMain.Size = new System.Drawing.Size(1076, 658);
            this.tbpMain.TabIndex = 0;
            this.tbpMain.Text = "Main";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(6, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(402, 24);
            this.label1.TabIndex = 42;
            this.label1.Text = "Some experiment controls can be added here.";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.groupBox1);
            this.groupBox2.Controls.Add(this.btnSecondPart);
            this.groupBox2.Location = new System.Drawing.Point(8, 53);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(239, 161);
            this.groupBox2.TabIndex = 41;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Experiment";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rdbtnPositiveMoodCondition);
            this.groupBox1.Controls.Add(this.rdbtnNegativeMoodCondition);
            this.groupBox1.Location = new System.Drawing.Point(6, 20);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(154, 72);
            this.groupBox1.TabIndex = 43;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Mood Condition 1st Part";
            // 
            // rdbtnPositiveMoodCondition
            // 
            this.rdbtnPositiveMoodCondition.AutoSize = true;
            this.rdbtnPositiveMoodCondition.Location = new System.Drawing.Point(6, 20);
            this.rdbtnPositiveMoodCondition.Name = "rdbtnPositiveMoodCondition";
            this.rdbtnPositiveMoodCondition.Size = new System.Drawing.Size(67, 19);
            this.rdbtnPositiveMoodCondition.TabIndex = 41;
            this.rdbtnPositiveMoodCondition.Text = "Positive";
            this.rdbtnPositiveMoodCondition.UseVisualStyleBackColor = true;
            this.rdbtnPositiveMoodCondition.Click += new System.EventHandler(this.rdbtnPositiveMoodCondition_CheckedChanged);
            // 
            // rdbtnNegativeMoodCondition
            // 
            this.rdbtnNegativeMoodCondition.AutoSize = true;
            this.rdbtnNegativeMoodCondition.Location = new System.Drawing.Point(6, 45);
            this.rdbtnNegativeMoodCondition.Name = "rdbtnNegativeMoodCondition";
            this.rdbtnNegativeMoodCondition.Size = new System.Drawing.Size(73, 19);
            this.rdbtnNegativeMoodCondition.TabIndex = 42;
            this.rdbtnNegativeMoodCondition.Text = "Negative";
            this.rdbtnNegativeMoodCondition.UseVisualStyleBackColor = true;
            this.rdbtnNegativeMoodCondition.Click += new System.EventHandler(this.rdbtnNegativeMoodCondition_CheckedChanged);
            // 
            // btnSecondPart
            // 
            this.btnSecondPart.Location = new System.Drawing.Point(6, 98);
            this.btnSecondPart.Name = "btnSecondPart";
            this.btnSecondPart.Size = new System.Drawing.Size(154, 44);
            this.btnSecondPart.TabIndex = 44;
            this.btnSecondPart.Text = "Start 2nd Part";
            this.btnSecondPart.UseVisualStyleBackColor = true;
            this.btnSecondPart.Click += new System.EventHandler(this.btnSecondPart_Click);
            // 
            // tbcMainTab
            // 
            this.tbcMainTab.Controls.Add(this.tbpMain);
            this.tbcMainTab.Controls.Add(this.tbpRobotMgr);
            this.tbcMainTab.Controls.Add(this.tbpPPT);
            this.tbcMainTab.Controls.Add(this.tbpScriptEngine);
            this.tbcMainTab.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbcMainTab.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbcMainTab.Location = new System.Drawing.Point(0, 0);
            this.tbcMainTab.Name = "tbcMainTab";
            this.tbcMainTab.SelectedIndex = 0;
            this.tbcMainTab.Size = new System.Drawing.Size(1084, 686);
            this.tbcMainTab.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight;
            this.tbcMainTab.TabIndex = 0;
            // 
            // ComponentManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1084, 686);
            this.Controls.Add(this.tbcMainTab);
            this.Name = "ComponentManager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Component Manager";
            this.tbpMain.ResumeLayout(false);
            this.tbpMain.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tbcMainTab.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabPage tbpScriptEngine;
        private System.Windows.Forms.TabPage tbpPPT;
        private System.Windows.Forms.TabPage tbpRobotMgr;
        private System.Windows.Forms.TabPage tbpMain;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnSecondPart;
        private System.Windows.Forms.RadioButton rdbtnNegativeMoodCondition;
        private System.Windows.Forms.RadioButton rdbtnPositiveMoodCondition;
        private System.Windows.Forms.TabControl tbcMainTab;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;

    }
}

