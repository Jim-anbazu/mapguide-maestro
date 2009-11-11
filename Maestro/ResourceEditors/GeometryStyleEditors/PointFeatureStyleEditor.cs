#region Disclaimer / License
// Copyright (C) 2009, Kenneth Skovhede
// http://www.hexad.dk, opensource@hexad.dk
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
#endregion
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace OSGeo.MapGuide.Maestro.ResourceEditors.GeometryStyleEditors
{
	/// <summary>
	/// Summary description for PointFeatureStyleEditor.
	/// </summary>
	public class PointFeatureStyleEditor : System.Windows.Forms.UserControl
	{
		private static byte[] SharedComboDataSet = null;

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.PictureBox previewPicture;
		private System.Data.DataSet ComboBoxDataSet;
		private System.Data.DataTable SymbolMarkTable;
		private System.Data.DataColumn dataColumn1;
		private System.Data.DataColumn dataColumn2;
		private System.Data.DataTable SizeContextTable;
		private System.Data.DataColumn dataColumn3;
		private System.Data.DataColumn dataColumn4;
		private System.Data.DataTable UnitsTable;
		private System.Data.DataColumn dataColumn5;
		private System.Data.DataColumn dataColumn6;
		private System.Data.DataTable RotationTable;
		private System.Data.DataColumn dataColumn7;
		private System.Data.DataColumn dataColumn8;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.ComboBox HeigthText;
		private System.Windows.Forms.ComboBox WidthText;
		private System.Windows.Forms.ComboBox SizeUnits;
		private System.Windows.Forms.ComboBox SizeContext;
		private System.Windows.Forms.ComboBox Symbol;
		private ResourceEditors.GeometryStyleEditors.LineStyleEditor lineStyleEditor;

		private OSGeo.MapGuide.MaestroAPI.PointSymbolization2DType m_item;
        private OSGeo.MapGuide.MaestroAPI.MarkSymbolType m_lastMark = null;
        private OSGeo.MapGuide.MaestroAPI.FontSymbolType m_lastFont = null;

		private bool m_inUpdate = false;

		private OSGeo.MapGuide.MaestroAPI.FillType previousFill = null;
        private CheckBox DisplayPoints;
		private OSGeo.MapGuide.MaestroAPI.StrokeType previousEdge = null;
		private GroupBox groupBoxFont;
		private ComboBox fontCombo;
		private Label label10;
		private ComboBox comboBoxCharacter;
		private GroupBox groupBoxSymbolLocation;
		private Button button1;
		private TextBox ReferenceY;
		private Label label8;
		private TextBox ReferenceX;
		private Label label7;
		private Label label6;
		private CheckBox MaintainAspectRatio;
		private ComboBox RotationBox;
		private Label label9;
		private FillStyleEditor fillStyleEditor;
		public ColorComboBox colorFontForeground;
		private Label lblForeground;
        private Panel panel1;
        private ToolStrip toolStrip1;
        private ToolStripButton FontBoldButton;
        private ToolStripButton FontItalicButton;
        private ToolStripButton FontUnderlineButton;
        private Globalizator.Globalizator m_globalizor;

		public event EventHandler Changed;

		public PointFeatureStyleEditor()
		{
			if (SharedComboDataSet == null)
			{
				System.IO.Stream s = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(this.GetType(), "PointStyleComboDataset.xml");
				byte[] buf = new byte[s.Length];
				if (s.Read(buf, 0, (int)s.Length) != s.Length)
					throw new Exception("Failed while reading data from assembly");
				SharedComboDataSet = buf;
			}

			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			using(System.IO.MemoryStream ms = new System.IO.MemoryStream(SharedComboDataSet))
				ComboBoxDataSet.ReadXml(ms);

			// fix this by editing "PointStyleComboDataset.xml" to include fonts
			fontCombo.Items.Clear();

            foreach (FontFamily f in new System.Drawing.Text.InstalledFontCollection().Families)
                fontCombo.Items.Add(f.Name);

			colorFontForeground.AllowTransparent = false;
			colorFontForeground.ResetColors();
			colorFontForeground.SelectedIndexChanged += new EventHandler(colourFontForeground_SelectedIndexChanged);

			fillStyleEditor.displayFill.CheckedChanged += new EventHandler(displayFill_CheckedChanged);
			fillStyleEditor.fillCombo.SelectedIndexChanged += new EventHandler(fillCombo_SelectedIndexChanged);
			fillStyleEditor.foregroundColor.SelectedIndexChanged += new EventHandler(foregroundColor_SelectedIndexChanged);
			fillStyleEditor.backgroundColor.SelectedIndexChanged +=new EventHandler(backgroundColor_SelectedIndexChanged);

			lineStyleEditor.displayLine.CheckedChanged +=new EventHandler(displayLine_CheckedChanged);
			lineStyleEditor.thicknessUpDown.ValueChanged +=new EventHandler(thicknessCombo_SelectedIndexChanged);
			lineStyleEditor.colorCombo.SelectedIndexChanged +=new EventHandler(colorCombo_SelectedIndexChanged);
			lineStyleEditor.fillCombo.SelectedIndexChanged +=new EventHandler(fillCombo_Line_SelectedIndexChanged);

			m_item = new OSGeo.MapGuide.MaestroAPI.PointSymbolization2DType();
			m_item.Item = new OSGeo.MapGuide.MaestroAPI.MarkSymbolType();

            m_globalizor = new Globalizator.Globalizator(this);
		}

		private void setUIForMarkSymbol(bool enabled)
		{
            groupBoxSymbolLocation.Enabled = enabled;
            groupBox2.Enabled = enabled;
            groupBox3.Enabled = enabled;

            groupBoxFont.Enabled = !enabled;
		}

		private void UpdateDisplay()
		{
			if (m_inUpdate)
				return;

			try
			{
				m_inUpdate = true;

                if (m_item == null)
                {
                    DisplayPoints.Checked = false;
                    return;
                }

                DisplayPoints.Checked = true;

				if (m_item.Item == null)
					m_item.Item = new OSGeo.MapGuide.MaestroAPI.MarkSymbolType();

				// shared values
				WidthText.Text = m_item.Item.SizeX;
				HeigthText.Text = m_item.Item.SizeY;
				RotationBox.Text = m_item.Item.Rotation;

				SizeUnits.SelectedValue = m_item.Item.Unit;
				SizeContext.SelectedValue = m_item.Item.SizeContext.ToString();

				// specifics
				if (m_item.Item.GetType() == typeof(OSGeo.MapGuide.MaestroAPI.MarkSymbolType))
				{
					MaintainAspectRatio.Checked = m_item.Item.MaintainAspect;
					double d;
					if (double.TryParse(m_item.Item.InsertionPointX, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out d))
						ReferenceX.Text = d.ToString(m_globalizor.Culture);
					else
						ReferenceX.Text = m_item.Item.InsertionPointX;

					if (double.TryParse(m_item.Item.InsertionPointY, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out d))
						ReferenceY.Text = d.ToString(m_globalizor.Culture);
					else
						ReferenceY.Text = m_item.Item.InsertionPointY;

					OSGeo.MapGuide.MaestroAPI.MarkSymbolType t = (OSGeo.MapGuide.MaestroAPI.MarkSymbolType)m_item.Item;
					Symbol.SelectedValue = t.Shape.ToString();

					fillStyleEditor.displayFill.Checked = t.Fill != null;
					if (t.Fill != null)
					{
						fillStyleEditor.foregroundColor.CurrentColor = t.Fill.ForegroundColor;
						fillStyleEditor.backgroundColor.CurrentColor = t.Fill.BackgroundColor;
						fillStyleEditor.fillCombo.SelectedValue = t.Fill.FillPattern;
						if (fillStyleEditor.fillCombo.SelectedItem == null && fillStyleEditor.fillCombo.Items.Count > 0)
							fillStyleEditor.fillCombo.SelectedIndex = fillStyleEditor.fillCombo.FindString(t.Fill.FillPattern);
					}

					lineStyleEditor.displayLine.Checked = t.Edge != null;
					if (t.Edge != null)
					{
						lineStyleEditor.fillCombo.SelectedValue = t.Edge.LineStyle;
						if (lineStyleEditor.fillCombo.SelectedItem == null && lineStyleEditor.fillCombo.Items.Count > 0)
							lineStyleEditor.fillCombo.SelectedIndex = lineStyleEditor.fillCombo.FindString(t.Edge.LineStyle);

						lineStyleEditor.colorCombo.CurrentColor = t.Edge.Color;
						double o;
						if (double.TryParse(t.Edge.Thickness, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out o))
							lineStyleEditor.thicknessUpDown.Value = (decimal)o;
						else
							lineStyleEditor.thicknessUpDown.Value = 0;
					}

					setUIForMarkSymbol(true);
				}
				else if (m_item.Item.GetType() == typeof(OSGeo.MapGuide.MaestroAPI.FontSymbolType))
				{
					OSGeo.MapGuide.MaestroAPI.FontSymbolType f = (OSGeo.MapGuide.MaestroAPI.FontSymbolType)m_item.Item;

					// TODO: Dislike this hard coding, but with association from 'Shape' the 'Font...' string cannot be found or set from the Symbol combobox
					Symbol.SelectedIndex = 6;

                    fontCombo.SelectedIndex = fontCombo.FindString(f.FontName);
                    if (string.Compare(fontCombo.Text, f.FontName, true) == 0)
                        fontCombo.Text = f.FontName;

                    comboBoxCharacter.SelectedIndex = comboBoxCharacter.FindString(f.Character);
                    if (comboBoxCharacter.Text != f.Character)
                        comboBoxCharacter.Text = f.Character;

                    FontBoldButton.Checked = f.Bold && f.BoldSpecified;
                    FontItalicButton.Checked = f.Italic && f.ItalicSpecified;
                    FontUnderlineButton.Checked = f.Underlined && f.UnderlinedSpecified;

                    if (string.IsNullOrEmpty(f.ForegroundColorAsHTML))
                        colorFontForeground.CurrentColor = Color.Black;
                    else
                        colorFontForeground.CurrentColor = f.ForegroundColor;

					setUIForMarkSymbol(false);
				}
				else
					//TODO: Fix this
					MessageBox.Show(this, "Only symbols of type \"Mark\" and \"Font\" are currently supported", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);

				previewPicture.Refresh();
			} 
			finally
			{
				m_inUpdate = false;
			}
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}


		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PointFeatureStyleEditor));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.RotationBox = new System.Windows.Forms.ComboBox();
            this.RotationTable = new System.Data.DataTable();
            this.dataColumn7 = new System.Data.DataColumn();
            this.dataColumn8 = new System.Data.DataColumn();
            this.label9 = new System.Windows.Forms.Label();
            this.HeigthText = new System.Windows.Forms.ComboBox();
            this.WidthText = new System.Windows.Forms.ComboBox();
            this.SizeUnits = new System.Windows.Forms.ComboBox();
            this.UnitsTable = new System.Data.DataTable();
            this.dataColumn5 = new System.Data.DataColumn();
            this.dataColumn6 = new System.Data.DataColumn();
            this.SizeContext = new System.Windows.Forms.ComboBox();
            this.SizeContextTable = new System.Data.DataTable();
            this.dataColumn3 = new System.Data.DataColumn();
            this.dataColumn4 = new System.Data.DataColumn();
            this.Symbol = new System.Windows.Forms.ComboBox();
            this.SymbolMarkTable = new System.Data.DataTable();
            this.dataColumn1 = new System.Data.DataColumn();
            this.dataColumn2 = new System.Data.DataColumn();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.fillStyleEditor = new OSGeo.MapGuide.Maestro.ResourceEditors.GeometryStyleEditors.FillStyleEditor();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lineStyleEditor = new OSGeo.MapGuide.Maestro.ResourceEditors.GeometryStyleEditors.LineStyleEditor();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.previewPicture = new System.Windows.Forms.PictureBox();
            this.ComboBoxDataSet = new System.Data.DataSet();
            this.DisplayPoints = new System.Windows.Forms.CheckBox();
            this.groupBoxFont = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.FontBoldButton = new System.Windows.Forms.ToolStripButton();
            this.FontItalicButton = new System.Windows.Forms.ToolStripButton();
            this.FontUnderlineButton = new System.Windows.Forms.ToolStripButton();
            this.colorFontForeground = new OSGeo.MapGuide.Maestro.ResourceEditors.GeometryStyleEditors.ColorComboBox();
            this.lblForeground = new System.Windows.Forms.Label();
            this.comboBoxCharacter = new System.Windows.Forms.ComboBox();
            this.fontCombo = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBoxSymbolLocation = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.ReferenceY = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.ReferenceX = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.MaintainAspectRatio = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.RotationTable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UnitsTable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SizeContextTable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SymbolMarkTable)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.previewPicture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ComboBoxDataSet)).BeginInit();
            this.groupBoxFont.SuspendLayout();
            this.panel1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.groupBoxSymbolLocation.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.RotationBox);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.HeigthText);
            this.groupBox1.Controls.Add(this.WidthText);
            this.groupBox1.Controls.Add(this.SizeUnits);
            this.groupBox1.Controls.Add(this.SizeContext);
            this.groupBox1.Controls.Add(this.Symbol);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(0, 24);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(344, 208);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Symbol style";
            // 
            // RotationBox
            // 
            this.RotationBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.RotationBox.DataSource = this.RotationTable;
            this.RotationBox.DisplayMember = "Display";
            this.RotationBox.Location = new System.Drawing.Point(128, 176);
            this.RotationBox.Name = "RotationBox";
            this.RotationBox.Size = new System.Drawing.Size(208, 21);
            this.RotationBox.TabIndex = 29;
            this.RotationBox.ValueMember = "Value";
            this.RotationBox.SelectedIndexChanged += new System.EventHandler(this.RotationBox_SelectedIndexChanged);
            this.RotationBox.TextChanged += new System.EventHandler(this.Rotation_TextChanged);
            // 
            // RotationTable
            // 
            this.RotationTable.Columns.AddRange(new System.Data.DataColumn[] {
            this.dataColumn7,
            this.dataColumn8});
            this.RotationTable.TableName = "Rotation";
            // 
            // dataColumn7
            // 
            this.dataColumn7.Caption = "Display";
            this.dataColumn7.ColumnName = "Display";
            // 
            // dataColumn8
            // 
            this.dataColumn8.Caption = "Value";
            this.dataColumn8.ColumnName = "Value";
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(16, 184);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(88, 16);
            this.label9.TabIndex = 28;
            this.label9.Text = "Rotation";
            // 
            // HeigthText
            // 
            this.HeigthText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.HeigthText.Location = new System.Drawing.Point(128, 144);
            this.HeigthText.Name = "HeigthText";
            this.HeigthText.Size = new System.Drawing.Size(208, 21);
            this.HeigthText.TabIndex = 9;
            this.HeigthText.SelectedIndexChanged += new System.EventHandler(this.HeigthText_SelectedIndexChanged);
            this.HeigthText.TextChanged += new System.EventHandler(this.HeigthText_TextChanged);
            // 
            // WidthText
            // 
            this.WidthText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.WidthText.Location = new System.Drawing.Point(128, 112);
            this.WidthText.Name = "WidthText";
            this.WidthText.Size = new System.Drawing.Size(208, 21);
            this.WidthText.TabIndex = 8;
            this.WidthText.SelectedIndexChanged += new System.EventHandler(this.WidthText_SelectedIndexChanged);
            this.WidthText.TextChanged += new System.EventHandler(this.WidthText_TextChanged);
            // 
            // SizeUnits
            // 
            this.SizeUnits.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.SizeUnits.DataSource = this.UnitsTable;
            this.SizeUnits.DisplayMember = "Display";
            this.SizeUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SizeUnits.Location = new System.Drawing.Point(128, 80);
            this.SizeUnits.Name = "SizeUnits";
            this.SizeUnits.Size = new System.Drawing.Size(208, 21);
            this.SizeUnits.TabIndex = 7;
            this.SizeUnits.ValueMember = "Value";
            this.SizeUnits.SelectedIndexChanged += new System.EventHandler(this.SizeUnits_SelectedIndexChanged);
            // 
            // UnitsTable
            // 
            this.UnitsTable.Columns.AddRange(new System.Data.DataColumn[] {
            this.dataColumn5,
            this.dataColumn6});
            this.UnitsTable.TableName = "Units";
            // 
            // dataColumn5
            // 
            this.dataColumn5.Caption = "Display";
            this.dataColumn5.ColumnName = "Display";
            // 
            // dataColumn6
            // 
            this.dataColumn6.Caption = "Value";
            this.dataColumn6.ColumnName = "Value";
            // 
            // SizeContext
            // 
            this.SizeContext.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.SizeContext.DataSource = this.SizeContextTable;
            this.SizeContext.DisplayMember = "Display";
            this.SizeContext.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SizeContext.Location = new System.Drawing.Point(128, 48);
            this.SizeContext.Name = "SizeContext";
            this.SizeContext.Size = new System.Drawing.Size(208, 21);
            this.SizeContext.TabIndex = 6;
            this.SizeContext.ValueMember = "Value";
            this.SizeContext.SelectedIndexChanged += new System.EventHandler(this.SizeContext_SelectedIndexChanged);
            // 
            // SizeContextTable
            // 
            this.SizeContextTable.Columns.AddRange(new System.Data.DataColumn[] {
            this.dataColumn3,
            this.dataColumn4});
            this.SizeContextTable.TableName = "SizeContext";
            // 
            // dataColumn3
            // 
            this.dataColumn3.Caption = "Display";
            this.dataColumn3.ColumnName = "Display";
            // 
            // dataColumn4
            // 
            this.dataColumn4.Caption = "Value";
            this.dataColumn4.ColumnName = "Value";
            // 
            // Symbol
            // 
            this.Symbol.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Symbol.DataSource = this.SymbolMarkTable;
            this.Symbol.DisplayMember = "Display";
            this.Symbol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Symbol.Location = new System.Drawing.Point(128, 16);
            this.Symbol.Name = "Symbol";
            this.Symbol.Size = new System.Drawing.Size(208, 21);
            this.Symbol.TabIndex = 5;
            this.Symbol.ValueMember = "Value";
            this.Symbol.SelectedIndexChanged += new System.EventHandler(this.Symbol_SelectedIndexChanged);
            // 
            // SymbolMarkTable
            // 
            this.SymbolMarkTable.Columns.AddRange(new System.Data.DataColumn[] {
            this.dataColumn1,
            this.dataColumn2});
            this.SymbolMarkTable.TableName = "SymbolMark";
            // 
            // dataColumn1
            // 
            this.dataColumn1.Caption = "Display";
            this.dataColumn1.ColumnName = "Display";
            // 
            // dataColumn2
            // 
            this.dataColumn2.Caption = "Value";
            this.dataColumn2.ColumnName = "Value";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(16, 152);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(96, 16);
            this.label5.TabIndex = 4;
            this.label5.Text = "Height";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(16, 120);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(96, 16);
            this.label4.TabIndex = 3;
            this.label4.Text = "Width";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(16, 88);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 16);
            this.label3.TabIndex = 2;
            this.label3.Text = "Size units";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(16, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "Size context";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(16, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Symbol";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.fillStyleEditor);
            this.groupBox2.Location = new System.Drawing.Point(0, 416);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(344, 128);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Symbol fill";
            // 
            // fillStyleEditor
            // 
            this.fillStyleEditor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.fillStyleEditor.Location = new System.Drawing.Point(8, 16);
            this.fillStyleEditor.Name = "fillStyleEditor";
            this.fillStyleEditor.Size = new System.Drawing.Size(328, 104);
            this.fillStyleEditor.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.lineStyleEditor);
            this.groupBox3.Location = new System.Drawing.Point(0, 552);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(344, 128);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Symbol border";
            // 
            // lineStyleEditor
            // 
            this.lineStyleEditor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lineStyleEditor.Location = new System.Drawing.Point(8, 16);
            this.lineStyleEditor.Name = "lineStyleEditor";
            this.lineStyleEditor.Size = new System.Drawing.Size(328, 104);
            this.lineStyleEditor.TabIndex = 0;
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.previewPicture);
            this.groupBox4.Location = new System.Drawing.Point(0, 688);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(344, 48);
            this.groupBox4.TabIndex = 7;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Preview";
            // 
            // previewPicture
            // 
            this.previewPicture.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.previewPicture.BackColor = System.Drawing.Color.White;
            this.previewPicture.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.previewPicture.Location = new System.Drawing.Point(8, 16);
            this.previewPicture.Name = "previewPicture";
            this.previewPicture.Size = new System.Drawing.Size(328, 24);
            this.previewPicture.TabIndex = 0;
            this.previewPicture.TabStop = false;
            this.previewPicture.Paint += new System.Windows.Forms.PaintEventHandler(this.previewPicture_Paint);
            // 
            // ComboBoxDataSet
            // 
            this.ComboBoxDataSet.DataSetName = "ComboBoxDataSet";
            this.ComboBoxDataSet.Locale = new System.Globalization.CultureInfo("da-DK");
            this.ComboBoxDataSet.Tables.AddRange(new System.Data.DataTable[] {
            this.SymbolMarkTable,
            this.SizeContextTable,
            this.UnitsTable,
            this.RotationTable});
            // 
            // DisplayPoints
            // 
            this.DisplayPoints.AutoSize = true;
            this.DisplayPoints.Checked = true;
            this.DisplayPoints.CheckState = System.Windows.Forms.CheckState.Checked;
            this.DisplayPoints.Location = new System.Drawing.Point(0, 0);
            this.DisplayPoints.Name = "DisplayPoints";
            this.DisplayPoints.Size = new System.Drawing.Size(91, 17);
            this.DisplayPoints.TabIndex = 8;
            this.DisplayPoints.Text = "Display points";
            this.DisplayPoints.UseVisualStyleBackColor = true;
            this.DisplayPoints.CheckedChanged += new System.EventHandler(this.DisplayPoints_CheckedChanged);
            // 
            // groupBoxFont
            // 
            this.groupBoxFont.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxFont.Controls.Add(this.panel1);
            this.groupBoxFont.Controls.Add(this.colorFontForeground);
            this.groupBoxFont.Controls.Add(this.lblForeground);
            this.groupBoxFont.Controls.Add(this.comboBoxCharacter);
            this.groupBoxFont.Controls.Add(this.fontCombo);
            this.groupBoxFont.Controls.Add(this.label10);
            this.groupBoxFont.Location = new System.Drawing.Point(0, 240);
            this.groupBoxFont.Name = "groupBoxFont";
            this.groupBoxFont.Size = new System.Drawing.Size(344, 88);
            this.groupBoxFont.TabIndex = 9;
            this.groupBoxFont.TabStop = false;
            this.groupBoxFont.Text = "Font style";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.toolStrip1);
            this.panel1.Location = new System.Drawing.Point(248, 56);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(88, 24);
            this.panel1.TabIndex = 13;
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FontBoldButton,
            this.FontItalicButton,
            this.FontUnderlineButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(88, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // FontBoldButton
            // 
            this.FontBoldButton.CheckOnClick = true;
            this.FontBoldButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.FontBoldButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FontBoldButton.Image = ((System.Drawing.Image)(resources.GetObject("FontBoldButton.Image")));
            this.FontBoldButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.FontBoldButton.Name = "FontBoldButton";
            this.FontBoldButton.Size = new System.Drawing.Size(23, 22);
            this.FontBoldButton.Text = "B";
            this.FontBoldButton.ToolTipText = "Set bold font";
            this.FontBoldButton.Click += new System.EventHandler(this.FontBoldButton_Click);
            // 
            // FontItalicButton
            // 
            this.FontItalicButton.CheckOnClick = true;
            this.FontItalicButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.FontItalicButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FontItalicButton.Image = ((System.Drawing.Image)(resources.GetObject("FontItalicButton.Image")));
            this.FontItalicButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.FontItalicButton.Name = "FontItalicButton";
            this.FontItalicButton.Size = new System.Drawing.Size(23, 22);
            this.FontItalicButton.Text = "I";
            this.FontItalicButton.ToolTipText = "Set italic font";
            this.FontItalicButton.Click += new System.EventHandler(this.FontItalicButton_Click);
            // 
            // FontUnderlineButton
            // 
            this.FontUnderlineButton.CheckOnClick = true;
            this.FontUnderlineButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.FontUnderlineButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FontUnderlineButton.Image = ((System.Drawing.Image)(resources.GetObject("FontUnderlineButton.Image")));
            this.FontUnderlineButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.FontUnderlineButton.Name = "FontUnderlineButton";
            this.FontUnderlineButton.Size = new System.Drawing.Size(23, 22);
            this.FontUnderlineButton.Text = "U";
            this.FontUnderlineButton.ToolTipText = "Set underlined font";
            this.FontUnderlineButton.Click += new System.EventHandler(this.FontUnderlineButton_Click);
            // 
            // colorFontForeground
            // 
            this.colorFontForeground.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.colorFontForeground.Location = new System.Drawing.Point(56, 56);
            this.colorFontForeground.Name = "colorFontForeground";
            this.colorFontForeground.Size = new System.Drawing.Size(176, 21);
            this.colorFontForeground.TabIndex = 12;
            this.colorFontForeground.SelectedIndexChanged += new System.EventHandler(this.colourFontForeground_SelectedIndexChanged);
            // 
            // lblForeground
            // 
            this.lblForeground.Location = new System.Drawing.Point(16, 56);
            this.lblForeground.Name = "lblForeground";
            this.lblForeground.Size = new System.Drawing.Size(40, 16);
            this.lblForeground.TabIndex = 11;
            this.lblForeground.Text = "Color";
            // 
            // comboBoxCharacter
            // 
            this.comboBoxCharacter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxCharacter.DisplayMember = "Display";
            this.comboBoxCharacter.Location = new System.Drawing.Point(248, 24);
            this.comboBoxCharacter.MaxLength = 1;
            this.comboBoxCharacter.Name = "comboBoxCharacter";
            this.comboBoxCharacter.Size = new System.Drawing.Size(80, 21);
            this.comboBoxCharacter.TabIndex = 10;
            this.comboBoxCharacter.ValueMember = "Value";
            this.comboBoxCharacter.SelectedIndexChanged += new System.EventHandler(this.comboBoxCharacter_SelectedIndexChanged);
            this.comboBoxCharacter.TextChanged += new System.EventHandler(this.comboBoxCharacter_TextChanged);
            // 
            // fontCombo
            // 
            this.fontCombo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.fontCombo.DisplayMember = "Display";
            this.fontCombo.Location = new System.Drawing.Point(56, 24);
            this.fontCombo.Name = "fontCombo";
            this.fontCombo.Size = new System.Drawing.Size(176, 21);
            this.fontCombo.TabIndex = 9;
            this.fontCombo.ValueMember = "Value";
            this.fontCombo.SelectedIndexChanged += new System.EventHandler(this.fontCombo_SelectedIndexChanged);
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(16, 32);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(40, 16);
            this.label10.TabIndex = 8;
            this.label10.Text = "Font";
            // 
            // groupBoxSymbolLocation
            // 
            this.groupBoxSymbolLocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxSymbolLocation.Controls.Add(this.button1);
            this.groupBoxSymbolLocation.Controls.Add(this.ReferenceY);
            this.groupBoxSymbolLocation.Controls.Add(this.label8);
            this.groupBoxSymbolLocation.Controls.Add(this.ReferenceX);
            this.groupBoxSymbolLocation.Controls.Add(this.label7);
            this.groupBoxSymbolLocation.Controls.Add(this.label6);
            this.groupBoxSymbolLocation.Controls.Add(this.MaintainAspectRatio);
            this.groupBoxSymbolLocation.Location = new System.Drawing.Point(0, 336);
            this.groupBoxSymbolLocation.Name = "groupBoxSymbolLocation";
            this.groupBoxSymbolLocation.Size = new System.Drawing.Size(344, 72);
            this.groupBoxSymbolLocation.TabIndex = 10;
            this.groupBoxSymbolLocation.TabStop = false;
            this.groupBoxSymbolLocation.Text = "Symbol location";
            // 
            // button1
            // 
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.button1.Location = new System.Drawing.Point(284, 40);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(24, 24);
            this.button1.TabIndex = 25;
            this.button1.Text = "...";
            // 
            // ReferenceY
            // 
            this.ReferenceY.Location = new System.Drawing.Point(228, 40);
            this.ReferenceY.Name = "ReferenceY";
            this.ReferenceY.Size = new System.Drawing.Size(48, 20);
            this.ReferenceY.TabIndex = 24;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(212, 40);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(16, 16);
            this.label8.TabIndex = 23;
            this.label8.Text = "Y";
            // 
            // ReferenceX
            // 
            this.ReferenceX.Location = new System.Drawing.Point(140, 40);
            this.ReferenceX.Name = "ReferenceX";
            this.ReferenceX.Size = new System.Drawing.Size(48, 20);
            this.ReferenceX.TabIndex = 22;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(124, 40);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(16, 16);
            this.label7.TabIndex = 21;
            this.label7.Text = "X";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(12, 40);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(104, 16);
            this.label6.TabIndex = 20;
            this.label6.Text = "Reference point";
            // 
            // MaintainAspectRatio
            // 
            this.MaintainAspectRatio.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.MaintainAspectRatio.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.MaintainAspectRatio.Location = new System.Drawing.Point(124, 16);
            this.MaintainAspectRatio.Name = "MaintainAspectRatio";
            this.MaintainAspectRatio.Size = new System.Drawing.Size(208, 16);
            this.MaintainAspectRatio.TabIndex = 19;
            this.MaintainAspectRatio.Text = "Maintain aspect ratio";
            // 
            // PointFeatureStyleEditor
            // 
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(344, 744);
            this.Controls.Add(this.groupBoxSymbolLocation);
            this.Controls.Add(this.groupBoxFont);
            this.Controls.Add(this.DisplayPoints);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox3);
            this.Name = "PointFeatureStyleEditor";
            this.Size = new System.Drawing.Size(344, 744);
            this.Load += new System.EventHandler(this.PointFeatureStyleEditor_Load);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.RotationTable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UnitsTable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SizeContextTable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SymbolMarkTable)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.previewPicture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ComboBoxDataSet)).EndInit();
            this.groupBoxFont.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.groupBoxSymbolLocation.ResumeLayout(false);
            this.groupBoxSymbolLocation.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void PointFeatureStyleEditor_Load(object sender, System.EventArgs e)
		{
			UpdateDisplay();
		}

		private void previewPicture_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			if (m_item != null && m_item.Item as OSGeo.MapGuide.MaestroAPI.MarkSymbolType != null)
				FeaturePreviewRender.RenderPreviewPoint(e.Graphics, new Rectangle(1, 1, previewPicture.Width - 2, previewPicture.Height - 2), (OSGeo.MapGuide.MaestroAPI.MarkSymbolType)m_item.Item);
			else if (m_item != null && m_item.Item as OSGeo.MapGuide.MaestroAPI.FontSymbolType != null)
				FeaturePreviewRender.RenderPreviewFontSymbol(e.Graphics, new Rectangle(1, 1, previewPicture.Width - 2, previewPicture.Height - 2), (OSGeo.MapGuide.MaestroAPI.FontSymbolType)m_item.Item);
			else
                FeaturePreviewRender.RenderPreviewPoint(e.Graphics, new Rectangle(1, 1, previewPicture.Width - 2, previewPicture.Height - 2), null);
		}

		private void Symbol_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (m_inUpdate)
				return;

            bool isSymbol = false;
            MaestroAPI.ShapeType selectedShape = OSGeo.MapGuide.MaestroAPI.ShapeType.Circle;

			// see if need to change symbol type
            foreach (string s in Enum.GetNames(typeof(OSGeo.MapGuide.MaestroAPI.ShapeType)))
                if (string.Compare(s, (string)Symbol.SelectedValue, true) == 0)
                {
                    selectedShape = (OSGeo.MapGuide.MaestroAPI.ShapeType)Enum.Parse(typeof(OSGeo.MapGuide.MaestroAPI.ShapeType), s);
                    isSymbol = true;
                    break;
                }

            if (m_item.Item is OSGeo.MapGuide.MaestroAPI.MarkSymbolType)
                m_lastMark = (OSGeo.MapGuide.MaestroAPI.MarkSymbolType)m_item.Item;
            else if (m_item.Item is OSGeo.MapGuide.MaestroAPI.FontSymbolType)
                m_lastFont = (OSGeo.MapGuide.MaestroAPI.FontSymbolType)m_item.Item;

            if (isSymbol)
            {
                bool update = m_item.Item != m_lastMark;

                if (m_lastMark == null)
                    m_lastMark = new OSGeo.MapGuide.MaestroAPI.MarkSymbolType();

                m_lastMark.Shape = selectedShape;
                m_item.Item = m_lastMark;
                
                setUIForMarkSymbol(true);
                if (update)
                    UpdateDisplay();
            }
			else if (Symbol.SelectedIndex == 6)
			{
			    // user wants to change away FROM a valid 'Mark' symbol type
			    // if ("Font..." == Symbol.SelectedText)

                bool update = m_item.Item != m_lastFont;

                if (m_lastFont == null)
                {
                    m_lastFont = new OSGeo.MapGuide.MaestroAPI.FontSymbolType();
                    m_lastFont.SizeContext = OSGeo.MapGuide.MaestroAPI.SizeContextType.DeviceUnits;
                    m_lastFont.Rotation = "0";
                    m_lastFont.SizeX = "10";
                    m_lastFont.SizeY = "10";
                    m_lastFont.Unit = OSGeo.MapGuide.MaestroAPI.LengthUnitType.Points;
                }

                m_item.Item = m_lastFont;
                setUIForMarkSymbol(false);
                if (update)
                    UpdateDisplay();
            }
			else
			{
				MessageBox.Show(this, "Only symbols of type \"Mark\" and \"Font\" are currently supported", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			previewPicture.Refresh();
			if (Changed != null)
				Changed(this, new EventArgs());
		}

		private void SizeContext_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (m_inUpdate)
				return;

			if (m_item.Item.GetType() == typeof(OSGeo.MapGuide.MaestroAPI.MarkSymbolType))
				((OSGeo.MapGuide.MaestroAPI.MarkSymbolType)m_item.Item).SizeContext = (OSGeo.MapGuide.MaestroAPI.SizeContextType)Enum.Parse((typeof(OSGeo.MapGuide.MaestroAPI.SizeContextType)), (string)SizeContext.SelectedValue);
			else if (m_item.Item.GetType() == typeof(OSGeo.MapGuide.MaestroAPI.FontSymbolType))
				((OSGeo.MapGuide.MaestroAPI.FontSymbolType)m_item.Item).SizeContext = (OSGeo.MapGuide.MaestroAPI.SizeContextType)Enum.Parse((typeof(OSGeo.MapGuide.MaestroAPI.SizeContextType)), (string)SizeContext.SelectedValue);
			previewPicture.Refresh();
			if (Changed != null)
				Changed(this, new EventArgs());
		}

		private void SizeUnits_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (m_inUpdate)
				return;

			if (m_item.Item.GetType() == typeof(OSGeo.MapGuide.MaestroAPI.MarkSymbolType))
				((OSGeo.MapGuide.MaestroAPI.MarkSymbolType)m_item.Item).Unit = (OSGeo.MapGuide.MaestroAPI.LengthUnitType)Enum.Parse(typeof(OSGeo.MapGuide.MaestroAPI.LengthUnitType), (string)SizeUnits.SelectedValue);
			else if (m_item.Item.GetType() == typeof(OSGeo.MapGuide.MaestroAPI.FontSymbolType))
				((OSGeo.MapGuide.MaestroAPI.FontSymbolType)m_item.Item).Unit = (OSGeo.MapGuide.MaestroAPI.LengthUnitType)Enum.Parse(typeof(OSGeo.MapGuide.MaestroAPI.LengthUnitType), (string)SizeUnits.SelectedValue);
			previewPicture.Refresh();
			if (Changed != null)
				Changed(this, new EventArgs());
		}

		private void WidthText_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (m_inUpdate)
				return;

			//TODO: Validate
			if (m_item.Item.GetType() == typeof(OSGeo.MapGuide.MaestroAPI.MarkSymbolType))
				((OSGeo.MapGuide.MaestroAPI.MarkSymbolType)m_item.Item).SizeX = WidthText.Text;
			else if (m_item.Item.GetType() == typeof(OSGeo.MapGuide.MaestroAPI.FontSymbolType))
				((OSGeo.MapGuide.MaestroAPI.FontSymbolType)m_item.Item).SizeX = WidthText.Text;
			previewPicture.Refresh();
			if (Changed != null)
				Changed(this, new EventArgs());
		}

		private void HeigthText_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (m_inUpdate)
				return;

			//TODO: Validate
			if (m_item.Item.GetType() == typeof(OSGeo.MapGuide.MaestroAPI.MarkSymbolType))
				((OSGeo.MapGuide.MaestroAPI.MarkSymbolType)m_item.Item).SizeY = HeigthText.Text;
			else if (m_item.Item.GetType() == typeof(OSGeo.MapGuide.MaestroAPI.FontSymbolType))
				((OSGeo.MapGuide.MaestroAPI.FontSymbolType)m_item.Item).SizeY = HeigthText.Text;
			previewPicture.Refresh();
			if (Changed != null)
				Changed(this, new EventArgs());
		}

		private void ReferenceX_TextChanged(object sender, System.EventArgs e)
		{
			if (m_inUpdate)
				return;

            if (m_item.Item.GetType() == typeof(OSGeo.MapGuide.MaestroAPI.MarkSymbolType))
            {
                double d;
                if (ReferenceX.Text.Trim().Length == 0)
                    ((OSGeo.MapGuide.MaestroAPI.MarkSymbolType)m_item.Item).InsertionPointY = "0.5";
                else if (double.TryParse(ReferenceX.Text, System.Globalization.NumberStyles.Float, m_globalizor.Culture, out d) || double.TryParse(ReferenceX.Text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out d))
                    ((OSGeo.MapGuide.MaestroAPI.MarkSymbolType)m_item.Item).InsertionPointX = Math.Min(Math.Max(0.0, d), 1.0).ToString(System.Globalization.CultureInfo.InvariantCulture);
                else
                    ((OSGeo.MapGuide.MaestroAPI.MarkSymbolType)m_item.Item).InsertionPointX = ReferenceX.Text;
            }
			previewPicture.Refresh();		
			if (Changed != null)
				Changed(this, new EventArgs());
		}

		private void ReferenceY_TextChanged(object sender, System.EventArgs e)
		{
			if (m_inUpdate)
				return;

            if (m_item.Item.GetType() == typeof(OSGeo.MapGuide.MaestroAPI.MarkSymbolType))
            {
                double d;
                if (ReferenceY.Text.Trim().Length == 0)
                    ((OSGeo.MapGuide.MaestroAPI.MarkSymbolType)m_item.Item).InsertionPointY = "0.5";
                else if (double.TryParse(ReferenceY.Text, System.Globalization.NumberStyles.Float, m_globalizor.Culture, out d) || double.TryParse(ReferenceY.Text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out d))
                    ((OSGeo.MapGuide.MaestroAPI.MarkSymbolType)m_item.Item).InsertionPointY = Math.Min(Math.Max(0.0, d), 1.0).ToString(System.Globalization.CultureInfo.InvariantCulture);
                else
                    ((OSGeo.MapGuide.MaestroAPI.MarkSymbolType)m_item.Item).InsertionPointY = ReferenceY.Text;
            }
			previewPicture.Refresh();		
			if (Changed != null)
				Changed(this, new EventArgs());
		}

		private void RotationBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (m_inUpdate)
				return;

			//TODO: Validate
			if (m_item.Item.GetType() == typeof(OSGeo.MapGuide.MaestroAPI.MarkSymbolType))
				((OSGeo.MapGuide.MaestroAPI.MarkSymbolType)m_item.Item).Rotation = (string)RotationBox.Text;
			else if (m_item.Item.GetType() == typeof(OSGeo.MapGuide.MaestroAPI.FontSymbolType))
				((OSGeo.MapGuide.MaestroAPI.FontSymbolType)m_item.Item).Rotation = (string)RotationBox.Text;
			previewPicture.Refresh();		
			if (Changed != null)
				Changed(this, new EventArgs());
		}

		private void displayFill_CheckedChanged(object sender, EventArgs e)
		{
			if (m_inUpdate)
				return;

			if (m_item.Item.GetType() == typeof(OSGeo.MapGuide.MaestroAPI.MarkSymbolType))
				if (fillStyleEditor.displayFill.Checked)
					((OSGeo.MapGuide.MaestroAPI.MarkSymbolType) m_item.Item).Fill = previousFill == null ? new OSGeo.MapGuide.MaestroAPI.FillType() : previousFill;
				else
				{
					if (((OSGeo.MapGuide.MaestroAPI.MarkSymbolType) m_item.Item).Fill != null)
						previousFill = ((OSGeo.MapGuide.MaestroAPI.MarkSymbolType) m_item.Item).Fill;
					((OSGeo.MapGuide.MaestroAPI.MarkSymbolType) m_item.Item).Fill = null;
				}
			previewPicture.Refresh();
			if (Changed != null)
				Changed(this, new EventArgs());
		}

		private void displayLine_CheckedChanged(object sender, EventArgs e)
		{
			if (m_inUpdate)
				return;

			if (m_item.Item.GetType() == typeof(OSGeo.MapGuide.MaestroAPI.MarkSymbolType))
				if (lineStyleEditor.displayLine.Checked)
					((OSGeo.MapGuide.MaestroAPI.MarkSymbolType) m_item.Item).Edge = previousEdge == null ? new OSGeo.MapGuide.MaestroAPI.StrokeType() : previousEdge;
				else
				{
					if (((OSGeo.MapGuide.MaestroAPI.MarkSymbolType) m_item.Item).Edge != null)
						previousEdge = ((OSGeo.MapGuide.MaestroAPI.MarkSymbolType) m_item.Item).Edge;
					((OSGeo.MapGuide.MaestroAPI.MarkSymbolType) m_item.Item).Edge = null;
				}
			previewPicture.Refresh();
			if (Changed != null)
				Changed(this, new EventArgs());
		}

		private void fillCombo_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (m_inUpdate)
				return;

			if (m_item.Item.GetType() == typeof(OSGeo.MapGuide.MaestroAPI.MarkSymbolType))
				((OSGeo.MapGuide.MaestroAPI.MarkSymbolType) m_item.Item).Fill.FillPattern = fillStyleEditor.fillCombo.Text;
			previewPicture.Refresh();
			if (Changed != null)
				Changed(this, new EventArgs());
		}

		private void foregroundColor_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (m_inUpdate)
				return;

			if (m_item.Item.GetType() == typeof(OSGeo.MapGuide.MaestroAPI.MarkSymbolType))
				((OSGeo.MapGuide.MaestroAPI.MarkSymbolType) m_item.Item).Fill.ForegroundColor = fillStyleEditor.foregroundColor.CurrentColor;
			previewPicture.Refresh();
			if (Changed != null)
				Changed(this, new EventArgs());
		}

		private void backgroundColor_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (m_inUpdate)
				return;

			if (m_item.Item.GetType() == typeof(OSGeo.MapGuide.MaestroAPI.MarkSymbolType))
				((OSGeo.MapGuide.MaestroAPI.MarkSymbolType) m_item.Item).Fill.BackgroundColor = fillStyleEditor.backgroundColor.CurrentColor;
			previewPicture.Refresh();
			if (Changed != null)
				Changed(this, new EventArgs());
		}

		private void thicknessCombo_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (m_inUpdate)
				return;

			//TODO: Validate
			if (m_item.Item.GetType() == typeof(OSGeo.MapGuide.MaestroAPI.MarkSymbolType))
				((OSGeo.MapGuide.MaestroAPI.MarkSymbolType) m_item.Item).Edge.Thickness =  lineStyleEditor.thicknessUpDown.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
			previewPicture.Refresh();
			if (Changed != null)
				Changed(this, new EventArgs());
		}

		private void colorCombo_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (m_inUpdate)
				return;

			if (m_item.Item.GetType() == typeof(OSGeo.MapGuide.MaestroAPI.MarkSymbolType))
				((OSGeo.MapGuide.MaestroAPI.MarkSymbolType) m_item.Item).Edge.Color = lineStyleEditor.colorCombo.CurrentColor;
			previewPicture.Refresh();
			if (Changed != null)
				Changed(this, new EventArgs());
		}

		private void fillCombo_Line_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (m_inUpdate)
				return;

			if (m_item.Item.GetType() == typeof(OSGeo.MapGuide.MaestroAPI.MarkSymbolType))
				((OSGeo.MapGuide.MaestroAPI.MarkSymbolType) m_item.Item).Edge.LineStyle = lineStyleEditor.fillCombo.Text;
			previewPicture.Refresh();
			if (Changed != null)
				Changed(this, new EventArgs());
		}

		private void colourFontForeground_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (m_inUpdate)
				return;

			if (m_item.Item.GetType() == typeof(OSGeo.MapGuide.MaestroAPI.FontSymbolType))
				((OSGeo.MapGuide.MaestroAPI.FontSymbolType)m_item.Item).ForegroundColor = colorFontForeground.CurrentColor;

            previewPicture.Refresh();
			if (Changed != null)
				Changed(this, new EventArgs());
		}

		private void fontCombo_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (m_inUpdate)
				return;

			//TODO: Validate
			if (!(m_item.Item is OSGeo.MapGuide.MaestroAPI.FontSymbolType))
				return;
			((OSGeo.MapGuide.MaestroAPI.FontSymbolType)m_item.Item).FontName = fontCombo.Text;

			comboBoxCharacter.Items.Clear();
			try
			{
				comboBoxCharacter.Font = new Font(fontCombo.SelectedText, (float)8.25);
			}
			catch
			{
				MessageBox.Show(this, "Cannot Preview Font '" + fontCombo.SelectedText  + "'", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			// populate with a basic A-Z
			for (char c = 'A'; c < 'Z'; c++)
				comboBoxCharacter.Items.Add(c);

			previewPicture.Refresh();
			if (Changed != null)
				Changed(this, new EventArgs());
		}

		private void comboBoxCharacter_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (m_inUpdate)
				return;

			//TODO: Validate
			if (m_item.Item.GetType() != typeof(OSGeo.MapGuide.MaestroAPI.FontSymbolType))
				return;
			((OSGeo.MapGuide.MaestroAPI.FontSymbolType)m_item.Item).Character = comboBoxCharacter.Text;

			previewPicture.Refresh();
			if (Changed != null)
				Changed(this, new EventArgs());
		}

		private void comboBoxCharacter_TextChanged(object sender, System.EventArgs e)
		{
			comboBoxCharacter_SelectedIndexChanged(sender, e);
		}


		private void button1_Click(object sender, System.EventArgs e)
		{
			MessageBox.Show(this, "This method is not yet implemented", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		public OSGeo.MapGuide.MaestroAPI.PointSymbolization2DType Item
		{
			get { return m_item; }
			set 
			{
				m_item = value;
				UpdateDisplay();
			}
		}

        private void DisplayPoints_CheckedChanged(object sender, EventArgs e)
        {
            foreach (Control c in this.Controls)
                c.Enabled = c == DisplayPoints || DisplayPoints.Checked;

            if (m_inUpdate)
                return;

            if (DisplayPoints.Checked)
            {
                if (DisplayPoints.Tag as OSGeo.MapGuide.MaestroAPI.PointSymbolization2DType != null)
                    this.Item = DisplayPoints.Tag as OSGeo.MapGuide.MaestroAPI.PointSymbolization2DType;
                if (m_item == null)
                    this.Item = new OSGeo.MapGuide.MaestroAPI.PointSymbolization2DType();
            }
            else
            {
                DisplayPoints.Tag = m_item;
                this.Item = null;
            }
        }

        private void WidthText_TextChanged(object sender, EventArgs e)
        {
            WidthText_SelectedIndexChanged(sender, e);
        }

        private void HeigthText_TextChanged(object sender, EventArgs e)
        {
            HeigthText_SelectedIndexChanged(sender, e);
        }

		private void Rotation_TextChanged(object sender, EventArgs e)
		{
			RotationBox_SelectedIndexChanged(sender, e);
		}
		
		private void ReferenceY_Leave(object sender, EventArgs e)
        {
            double d;
            if (m_item.Item is OSGeo.MapGuide.MaestroAPI.MarkSymbolType)
                if (!double.TryParse(((MaestroAPI.MarkSymbolType)m_item.Item).InsertionPointY, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out d))
                    MessageBox.Show(this, m_globalizor.Translate("You have entered a non-numeric value in the Reference Y field. Due to a bug in MapGuide, this will likely give an error when saving."), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void FontBoldButton_Click(object sender, EventArgs e)
        {
            if (m_inUpdate)
                return;

            if (m_item.Item is MaestroAPI.FontSymbolType)
            {
                ((MaestroAPI.FontSymbolType)m_item.Item).Bold = FontBoldButton.Checked;
                ((MaestroAPI.FontSymbolType)m_item.Item).BoldSpecified = true;
            }

            previewPicture.Refresh();
            if (Changed != null)
                Changed(this, new EventArgs());
        }

        private void FontItalicButton_Click(object sender, EventArgs e)
        {
            if (m_inUpdate)
                return;

            if (m_item.Item is MaestroAPI.FontSymbolType)
            {
                ((MaestroAPI.FontSymbolType)m_item.Item).Italic = FontItalicButton.Checked;
                ((MaestroAPI.FontSymbolType)m_item.Item).ItalicSpecified = true;
            }

            previewPicture.Refresh();
            if (Changed != null)
                Changed(this, new EventArgs());
        }

        private void FontUnderlineButton_Click(object sender, EventArgs e)
        {
            if (m_inUpdate)
                return;

            if (m_item.Item is MaestroAPI.FontSymbolType)
            {
                ((MaestroAPI.FontSymbolType)m_item.Item).Underlined = FontUnderlineButton.Checked;
                ((MaestroAPI.FontSymbolType)m_item.Item).UnderlinedSpecified = true;
            }

            previewPicture.Refresh();
            if (Changed != null)
                Changed(this, new EventArgs());
        }
    }
}
