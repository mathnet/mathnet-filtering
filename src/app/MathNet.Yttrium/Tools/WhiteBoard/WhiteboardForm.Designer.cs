namespace MathNet.Symbolics.Whiteboard
{
    partial class WhiteboardForm
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
            if(disposing && (components != null))
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
            this.netron = new Netron.NetronLight.Win.DiagramControl();
            this.btnBuildSample = new System.Windows.Forms.Button();
            this.btnNewSignal = new System.Windows.Forms.Button();
            this.btnNewBus = new System.Windows.Forms.Button();
            this.btnNewPort = new System.Windows.Forms.Button();
            this.entitySelector = new MathNet.Symbolics.Whiteboard.EntitySelector();
            ((System.ComponentModel.ISupportInitialize)(this.netron)).BeginInit();
            this.SuspendLayout();
            // 
            // netron
            // 
            this.netron.AllowDrop = true;
            this.netron.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.netron.BackColor = System.Drawing.Color.Gold;
            this.netron.BackgroundType = Netron.NetronLight.CanvasBackgroundTypes.Gradient;
            this.netron.EnableAddConnection = true;
            this.netron.Location = new System.Drawing.Point(-2, 0);
            this.netron.Name = "netron";
            this.netron.Pan = new System.Drawing.Point(0, 0);
            this.netron.Scale = 1F;
            this.netron.Size = new System.Drawing.Size(730, 567);
            this.netron.TabIndex = 0;
            this.netron.Text = "Math.NET Whiteboard Diagram";
            // 
            // btnBuildSample
            // 
            this.btnBuildSample.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBuildSample.Location = new System.Drawing.Point(552, 573);
            this.btnBuildSample.Name = "btnBuildSample";
            this.btnBuildSample.Size = new System.Drawing.Size(163, 23);
            this.btnBuildSample.TabIndex = 1;
            this.btnBuildSample.Text = "Build Sample System";
            this.btnBuildSample.UseVisualStyleBackColor = true;
            this.btnBuildSample.Click += new System.EventHandler(this.btnBuildSample_Click);
            // 
            // btnNewSignal
            // 
            this.btnNewSignal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnNewSignal.Location = new System.Drawing.Point(12, 573);
            this.btnNewSignal.Name = "btnNewSignal";
            this.btnNewSignal.Size = new System.Drawing.Size(75, 23);
            this.btnNewSignal.TabIndex = 2;
            this.btnNewSignal.Text = "New Signal";
            this.btnNewSignal.UseVisualStyleBackColor = true;
            this.btnNewSignal.Click += new System.EventHandler(this.btnNewSignal_Click);
            // 
            // btnNewBus
            // 
            this.btnNewBus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnNewBus.Location = new System.Drawing.Point(94, 573);
            this.btnNewBus.Name = "btnNewBus";
            this.btnNewBus.Size = new System.Drawing.Size(75, 23);
            this.btnNewBus.TabIndex = 3;
            this.btnNewBus.Text = "New Bus";
            this.btnNewBus.UseVisualStyleBackColor = true;
            this.btnNewBus.Click += new System.EventHandler(this.btnNewBus_Click);
            // 
            // btnNewPort
            // 
            this.btnNewPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnNewPort.Location = new System.Drawing.Point(175, 573);
            this.btnNewPort.Name = "btnNewPort";
            this.btnNewPort.Size = new System.Drawing.Size(75, 23);
            this.btnNewPort.TabIndex = 4;
            this.btnNewPort.Text = "New Port";
            this.btnNewPort.UseVisualStyleBackColor = true;
            this.btnNewPort.Click += new System.EventHandler(this.btnNewPort_Click);
            // 
            // entitySelector
            // 
            this.entitySelector.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.entitySelector.BackColor = System.Drawing.SystemColors.Control;
            this.entitySelector.Entities = null;
            this.entitySelector.Location = new System.Drawing.Point(12, 572);
            this.entitySelector.Name = "entitySelector";
            this.entitySelector.Size = new System.Drawing.Size(398, 24);
            this.entitySelector.TabIndex = 5;
            this.entitySelector.Visible = false;
            this.entitySelector.CompiledEntitySelected += new System.EventHandler(this.entitySelector_CompiledEntitySelected);
            this.entitySelector.Canceled += new System.EventHandler(this.entitySelector_Canceled);
            // 
            // WhiteboardForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(727, 608);
            this.Controls.Add(this.entitySelector);
            this.Controls.Add(this.btnNewPort);
            this.Controls.Add(this.btnNewBus);
            this.Controls.Add(this.btnNewSignal);
            this.Controls.Add(this.btnBuildSample);
            this.Controls.Add(this.netron);
            this.Name = "WhiteboardForm";
            this.Text = "Math.NET Whiteboard";
            this.Load += new System.EventHandler(this.WhiteboardForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.netron)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Netron.NetronLight.Win.DiagramControl netron;
        private System.Windows.Forms.Button btnBuildSample;
        private System.Windows.Forms.Button btnNewSignal;
        private System.Windows.Forms.Button btnNewBus;
        private System.Windows.Forms.Button btnNewPort;
        private EntitySelector entitySelector;
    }
}