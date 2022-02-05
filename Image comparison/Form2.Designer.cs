
namespace Image_comparison
{
    partial class Form2
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
            this.PB_label = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.step_label = new System.Windows.Forms.Label();
            this.count_label = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // PB_label
            // 
            this.PB_label.AutoSize = true;
            this.PB_label.Location = new System.Drawing.Point(11, 57);
            this.PB_label.Name = "PB_label";
            this.PB_label.Size = new System.Drawing.Size(27, 13);
            this.PB_label.TabIndex = 23;
            this.PB_label.Text = "00%";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(44, 53);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(414, 23);
            this.progressBar1.TabIndex = 22;
            // 
            // step_label
            // 
            this.step_label.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.step_label.Location = new System.Drawing.Point(-3, 14);
            this.step_label.Name = "step_label";
            this.step_label.Size = new System.Drawing.Size(479, 13);
            this.step_label.TabIndex = 24;
            this.step_label.Text = "Этап 1 из 3. Подготовка фото. Уменьшение размера";
            this.step_label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // count_label
            // 
            this.count_label.AutoSize = true;
            this.count_label.Location = new System.Drawing.Point(206, 37);
            this.count_label.Name = "count_label";
            this.count_label.Size = new System.Drawing.Size(74, 13);
            this.count_label.TabIndex = 25;
            this.count_label.Text = "Фото 1 из 50";
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(476, 87);
            this.ControlBox = false;
            this.Controls.Add(this.count_label);
            this.Controls.Add(this.step_label);
            this.Controls.Add(this.PB_label);
            this.Controls.Add(this.progressBar1);
            this.Name = "Form2";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Загрузка";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.ProgressBar progressBar1;
        public System.Windows.Forms.Label PB_label;
        public System.Windows.Forms.Label step_label;
        public System.Windows.Forms.Label count_label;
    }
}