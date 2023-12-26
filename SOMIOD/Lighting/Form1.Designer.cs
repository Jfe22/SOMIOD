namespace Lighting
{
    partial class Form1
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
            this.buttonConnectApp = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxContainerValues = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxLightValue = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // buttonConnectApp
            // 
            this.buttonConnectApp.Location = new System.Drawing.Point(182, 9);
            this.buttonConnectApp.Name = "buttonConnectApp";
            this.buttonConnectApp.Size = new System.Drawing.Size(109, 36);
            this.buttonConnectApp.TabIndex = 0;
            this.buttonConnectApp.Text = "Connect App";
            this.buttonConnectApp.UseVisualStyleBackColor = true;
            this.buttonConnectApp.Click += new System.EventHandler(this.buttonConnectApp_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(164, 31);
            this.label1.TabIndex = 3;
            this.label1.Text = "Lighting App";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(542, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(119, 16);
            this.label2.TabIndex = 4;
            this.label2.Text = "Select a Container:";
            // 
            // comboBoxContainerValues
            // 
            this.comboBoxContainerValues.FormattingEnabled = true;
            this.comboBoxContainerValues.Location = new System.Drawing.Point(667, 24);
            this.comboBoxContainerValues.Name = "comboBoxContainerValues";
            this.comboBoxContainerValues.Size = new System.Drawing.Size(121, 21);
            this.comboBoxContainerValues.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(143, 222);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(148, 24);
            this.label3.TabIndex = 6;
            this.label3.Text = "Light Bulb value:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textBoxLightValue
            // 
            this.textBoxLightValue.Location = new System.Drawing.Point(297, 227);
            this.textBoxLightValue.Name = "textBoxLightValue";
            this.textBoxLightValue.Size = new System.Drawing.Size(128, 20);
            this.textBoxLightValue.TabIndex = 8;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.textBoxLightValue);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboBoxContainerValues);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonConnectApp);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonConnectApp;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxContainerValues;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxLightValue;
    }
}

