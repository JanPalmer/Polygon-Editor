
namespace PRO1
{
    partial class CGP1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panelControl = new System.Windows.Forms.TableLayoutPanel();
            this.buttonNewPolygon = new System.Windows.Forms.Button();
            this.buttonFixedRelation = new System.Windows.Forms.Button();
            this.groupBoxAlgorithm = new System.Windows.Forms.GroupBox();
            this.radioButtonBuildIn = new System.Windows.Forms.RadioButton();
            this.radioButtonBresenham = new System.Windows.Forms.RadioButton();
            this.buttonClearSpace = new System.Windows.Forms.Button();
            this.buttonPerpendicularRelation = new System.Windows.Forms.Button();
            this.buttonHelp = new System.Windows.Forms.Button();
            this.canvas = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.panelControl.SuspendLayout();
            this.groupBoxAlgorithm.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.canvas)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 154F));
            this.tableLayoutPanel1.Controls.Add(this.panelControl, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.canvas, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(784, 511);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // panelControl
            // 
            this.panelControl.ColumnCount = 1;
            this.panelControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.panelControl.Controls.Add(this.buttonNewPolygon, 0, 0);
            this.panelControl.Controls.Add(this.buttonFixedRelation, 0, 1);
            this.panelControl.Controls.Add(this.groupBoxAlgorithm, 0, 6);
            this.panelControl.Controls.Add(this.buttonClearSpace, 0, 5);
            this.panelControl.Controls.Add(this.buttonPerpendicularRelation, 0, 2);
            this.panelControl.Controls.Add(this.buttonHelp, 0, 4);
            this.panelControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl.Location = new System.Drawing.Point(633, 3);
            this.panelControl.Name = "panelControl";
            this.panelControl.RowCount = 7;
            this.panelControl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 76F));
            this.panelControl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.panelControl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44F));
            this.panelControl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.panelControl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 47F));
            this.panelControl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 65F));
            this.panelControl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 108F));
            this.panelControl.Size = new System.Drawing.Size(148, 505);
            this.panelControl.TabIndex = 0;
            // 
            // buttonNewPolygon
            // 
            this.buttonNewPolygon.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonNewPolygon.Location = new System.Drawing.Point(3, 3);
            this.buttonNewPolygon.Name = "buttonNewPolygon";
            this.buttonNewPolygon.Size = new System.Drawing.Size(142, 70);
            this.buttonNewPolygon.TabIndex = 1;
            this.buttonNewPolygon.Text = "New Polygon";
            this.buttonNewPolygon.UseVisualStyleBackColor = true;
            this.buttonNewPolygon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.buttonNewPolygon_MouseClick);
            // 
            // buttonFixedRelation
            // 
            this.buttonFixedRelation.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonFixedRelation.Location = new System.Drawing.Point(3, 79);
            this.buttonFixedRelation.Name = "buttonFixedRelation";
            this.buttonFixedRelation.Size = new System.Drawing.Size(142, 39);
            this.buttonFixedRelation.TabIndex = 7;
            this.buttonFixedRelation.Text = "Add Fixed Length Relation";
            this.buttonFixedRelation.UseVisualStyleBackColor = true;
            this.buttonFixedRelation.MouseDown += new System.Windows.Forms.MouseEventHandler(this.buttonFixedRelation_MouseDown);
            // 
            // groupBoxAlgorithm
            // 
            this.groupBoxAlgorithm.Controls.Add(this.radioButtonBuildIn);
            this.groupBoxAlgorithm.Controls.Add(this.radioButtonBresenham);
            this.groupBoxAlgorithm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxAlgorithm.Location = new System.Drawing.Point(3, 400);
            this.groupBoxAlgorithm.Name = "groupBoxAlgorithm";
            this.groupBoxAlgorithm.Size = new System.Drawing.Size(142, 102);
            this.groupBoxAlgorithm.TabIndex = 0;
            this.groupBoxAlgorithm.TabStop = false;
            this.groupBoxAlgorithm.Text = "Current Edge Drawing Algorithm";
            // 
            // radioButtonBuildIn
            // 
            this.radioButtonBuildIn.AutoSize = true;
            this.radioButtonBuildIn.Checked = true;
            this.radioButtonBuildIn.Location = new System.Drawing.Point(6, 75);
            this.radioButtonBuildIn.Name = "radioButtonBuildIn";
            this.radioButtonBuildIn.Size = new System.Drawing.Size(67, 19);
            this.radioButtonBuildIn.TabIndex = 1;
            this.radioButtonBuildIn.TabStop = true;
            this.radioButtonBuildIn.Text = "Build-in";
            this.radioButtonBuildIn.UseVisualStyleBackColor = true;
            // 
            // radioButtonBresenham
            // 
            this.radioButtonBresenham.AutoSize = true;
            this.radioButtonBresenham.Location = new System.Drawing.Point(6, 39);
            this.radioButtonBresenham.Name = "radioButtonBresenham";
            this.radioButtonBresenham.Size = new System.Drawing.Size(84, 19);
            this.radioButtonBresenham.TabIndex = 0;
            this.radioButtonBresenham.Text = "Bresenham";
            this.radioButtonBresenham.UseVisualStyleBackColor = true;
            this.radioButtonBresenham.CheckedChanged += new System.EventHandler(this.radioButtonBresenham_CheckedChanged);
            // 
            // buttonClearSpace
            // 
            this.buttonClearSpace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonClearSpace.Location = new System.Drawing.Point(3, 335);
            this.buttonClearSpace.Name = "buttonClearSpace";
            this.buttonClearSpace.Size = new System.Drawing.Size(142, 59);
            this.buttonClearSpace.TabIndex = 2;
            this.buttonClearSpace.Text = "Clear Drawing Space";
            this.buttonClearSpace.UseVisualStyleBackColor = true;
            this.buttonClearSpace.MouseClick += new System.Windows.Forms.MouseEventHandler(this.buttonClearSpace_MouseClick);
            // 
            // buttonPerpendicularRelation
            // 
            this.buttonPerpendicularRelation.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonPerpendicularRelation.Location = new System.Drawing.Point(3, 124);
            this.buttonPerpendicularRelation.Name = "buttonPerpendicularRelation";
            this.buttonPerpendicularRelation.Size = new System.Drawing.Size(142, 38);
            this.buttonPerpendicularRelation.TabIndex = 6;
            this.buttonPerpendicularRelation.Text = "Add Perpendicularity Relation";
            this.buttonPerpendicularRelation.UseVisualStyleBackColor = true;
            this.buttonPerpendicularRelation.MouseDown += new System.Windows.Forms.MouseEventHandler(this.buttonPerpendicularRelation_MouseDown);
            // 
            // buttonHelp
            // 
            this.buttonHelp.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonHelp.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.buttonHelp.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.buttonHelp.Location = new System.Drawing.Point(108, 288);
            this.buttonHelp.Name = "buttonHelp";
            this.buttonHelp.Size = new System.Drawing.Size(37, 41);
            this.buttonHelp.TabIndex = 8;
            this.buttonHelp.Text = "🛈";
            this.buttonHelp.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.buttonHelp.UseVisualStyleBackColor = true;
            // 
            // canvas
            // 
            this.canvas.BackColor = System.Drawing.Color.White;
            this.canvas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.canvas.Location = new System.Drawing.Point(3, 3);
            this.canvas.Name = "canvas";
            this.canvas.Size = new System.Drawing.Size(624, 505);
            this.canvas.TabIndex = 1;
            this.canvas.TabStop = false;
            this.canvas.Paint += new System.Windows.Forms.PaintEventHandler(this.canvas_Paint);
            this.canvas.MouseDown += new System.Windows.Forms.MouseEventHandler(this.canvas_MouseDown);
            this.canvas.MouseMove += new System.Windows.Forms.MouseEventHandler(this.canvas_MouseMove);
            this.canvas.MouseUp += new System.Windows.Forms.MouseEventHandler(this.canvas_MouseUp);
            // 
            // CGP1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 511);
            this.Controls.Add(this.tableLayoutPanel1);
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(650, 500);
            this.Name = "CGP1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Computer Graphics Project 1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CGP1_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.CGP1_KeyUp);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panelControl.ResumeLayout(false);
            this.groupBoxAlgorithm.ResumeLayout(false);
            this.groupBoxAlgorithm.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.canvas)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.PictureBox canvas;
        private System.Windows.Forms.TableLayoutPanel panelControl;
        private System.Windows.Forms.Button buttonNewPolygon;
        private System.Windows.Forms.Button buttonFixedRelation;
        private System.Windows.Forms.GroupBox groupBoxAlgorithm;
        private System.Windows.Forms.RadioButton radioButtonBuildIn;
        private System.Windows.Forms.RadioButton radioButtonBresenham;
        private System.Windows.Forms.Button buttonClearSpace;
        private System.Windows.Forms.Button buttonHelp;
        private System.Windows.Forms.Button buttonPerpendicularRelation;
    }
}

