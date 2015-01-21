using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CR_StringFormatter
{
	public partial class FrmStringFormatter : Form
	{
		private FormatItemExpressionType _FormatItemExpressionType;
		private string _AlignmentString;
		private string _LeadingZeros;
		private bool _ModifyingInternally;
		private object _SampleData;
		private string _DecimalPlaces;
    public FrmStringFormatter()
		{
			InitializeComponent();
			grpCustomDateTime.Enabled = false;
			grpDateTime.Enabled = false;
		}

		private void InsertPreBuiltDateTime(object sender, EventArgs e)
		{
			Button button = sender as Button;
			if (button == null)
				return;

			txtFormatString.Text = button.Text;
		}

		private void InsertText(object sender, EventArgs e)
		{
			Button button = sender as Button;
			if (button == null)
				return;

			string newText = button.Text;
			string existingText = txtFormatString.Text;
			if (existingText != null && existingText.Length > 0)
			{
				if (char.IsLetter(existingText[existingText.Length - 1]))
					newText = " " + newText;		// Need to separate this out.
			}
			txtFormatString.AppendText(newText);
		}

		private void InsertCustomNumberFormatter()
		{
			string thousandsSeparator = String.Empty;
			string leadingZeros = _LeadingZeros;
			if (chkThousandsSeparator.Checked)
			{
				if (string.IsNullOrEmpty(_LeadingZeros))
					thousandsSeparator = "0,0";
				else
				{
					thousandsSeparator = ",0";
					leadingZeros = leadingZeros.Remove(0, 1);		// Take one zero off to compensate for the one we're adding.
				}
			}
			txtFormatString.Text = leadingZeros + thousandsSeparator + "." + _DecimalPlaces;
		}
    private void nudLeadingZeros_ValueChanged(object sender, EventArgs e)
		{
			_LeadingZeros = new string('0', (int)Math.Round(nudLeadingZeros.Value));
			InsertCustomNumberFormatter();
		}

		private void CalculateDecimalPlacesAndTrailingZeros()
		{
			char trailingChar = '#';
			if (chkShowTrailingZeros.Checked)
				trailingChar = '0';
			_DecimalPlaces = new string(trailingChar, (int)Math.Round(nudDecimalPlaces.Value));
		}
		private void chkShowTrailingZeros_CheckedChanged(object sender, EventArgs e)
		{
			CalculateDecimalPlacesAndTrailingZeros();
			InsertCustomNumberFormatter();
		}

		private void nudDecimalPlaces_ValueChanged(object sender, EventArgs e)
		{
			CalculateDecimalPlacesAndTrailingZeros();
			InsertCustomNumberFormatter();
		}

		private void chkThousandsSeparator_CheckedChanged(object sender, EventArgs e)
		{
			InsertCustomNumberFormatter();
		}

		private int GetNumHexDigits()
		{
			int numHexDigits = (int)Math.Round(nudHexDigits.Value);
			if (numHexDigits == 0)
				numHexDigits = 1;
			return numHexDigits;
		}
		private void nudHexDigits_ValueChanged(object sender, EventArgs e)
		{
			string text = txtFormatString.Text;
			if (text.StartsWith("X") || text.StartsWith("x"))
			{
				string formatChar = text[0].ToString();
				txtFormatString.Text = formatChar + GetNumHexDigits().ToString();
			}
		}

		private void InsertPreBuiltNumber(object sender, EventArgs e)
		{
			Button button = sender as Button;
			if (button == null)
				return;
			txtFormatString.Text = button.Text;
			UpdateSampleOutput();
		}

		private void UpdateSampleInput()
		{
			_ModifyingInternally = true;
			try
			{
				txtSampleInput.Text = _SampleData.ToString();
				UpdateSampleOutput();
			}
			finally
			{
				_ModifyingInternally = false;
			}
		}
		private void rbnString_CheckedChanged(object sender, EventArgs e)
		{
			if (rbnString.Checked)
			{
				grpCustomDateTime.Enabled = false;
				grpDateTime.Enabled = false;
				grpCustomNumber.Enabled = false;
				grpNumber.Enabled = false;
				_SampleData = "CustomerName";
				UpdateSampleInput();
			}
		}

		private void rbnInteger_CheckedChanged(object sender, EventArgs e)
		{
			if (rbnInteger.Checked)
			{
				grpCustomDateTime.Enabled = false;
				grpDateTime.Enabled = false;
				grpCustomNumber.Enabled = true;
				grpNumber.Enabled = true;
				_SampleData = 123456;
				UpdateSampleInput();
			}
		}

		private void rbnReal_CheckedChanged(object sender, EventArgs e)
		{
			if (rbnReal.Checked)
			{
				grpCustomDateTime.Enabled = false;
				grpDateTime.Enabled = false;
				grpCustomNumber.Enabled = true;
				grpNumber.Enabled = true;
				_SampleData = 123456.78901234d;
				UpdateSampleInput();
			}
		}

		private void rbnDateTime_CheckedChanged(object sender, EventArgs e)
		{
			if (rbnDateTime.Checked)
			{
				grpCustomDateTime.Enabled = true;
				grpDateTime.Enabled = true;
				grpCustomNumber.Enabled = false;
				grpNumber.Enabled = false;
				_SampleData = DateTime.Now;
				UpdateSampleInput();
			}
		}

		private void rbnOtherType_CheckedChanged(object sender, EventArgs e)
		{
			if (rbnOtherType.Checked)
			{
				grpCustomDateTime.Enabled = false;
				grpDateTime.Enabled = false;
				grpCustomNumber.Enabled = false;
				grpNumber.Enabled = false;
				_SampleData = "Other";
				UpdateSampleInput();
			}
		}

		private void UpdateSampleOutput()
		{
			try
			{
				lblSampleOutput.Text = string.Format("{0" + _AlignmentString + ":" + txtFormatString.Text + "}", _SampleData);
				lblSampleOutput.ForeColor = Color.Black;
			}
			catch (Exception ex)
			{
				lblSampleOutput.Text = ex.Message;
				lblSampleOutput.ForeColor = Color.Red;
			}
		}
		private void txtFormatString_TextChanged(object sender, EventArgs e)
		{
			UpdateSampleOutput();
		}

		private void nudAlignmentCharacters_ValueChanged(object sender, EventArgs e)
		{
			CalculateAlignmentString();
			UpdateSampleOutput();
		}

		private void CalculateAlignmentString()
		{
			int alignmnentChars = (int)Math.Round(nudAlignmentCharacters.Value);
			if (rbnAlignNone.Checked || alignmnentChars == 0)
				_AlignmentString = "";
			else
			{
				string alignDirection = String.Empty;
				if (rbnAlignLeft.Checked)
					alignDirection = "-";
				_AlignmentString = "," + alignDirection + alignmnentChars.ToString();
			}
		}

		private void rbnAlignLeft_CheckedChanged(object sender, EventArgs e)
		{
			CalculateAlignmentString();
			UpdateSampleOutput();
		}

		private void rbnAlignNone_CheckedChanged(object sender, EventArgs e)
		{
			CalculateAlignmentString();
			UpdateSampleOutput();
		}

		private void rbnAlignRight_CheckedChanged(object sender, EventArgs e)
		{
			CalculateAlignmentString();
			UpdateSampleOutput();
		}

		private void txtSampleInput_TextChanged(object sender, EventArgs e)
		{
			if (_ModifyingInternally)
				return;
			try
			{
				if (rbnInteger.Checked)
					_SampleData = int.Parse(txtSampleInput.Text);
				else if (rbnReal.Checked)
					_SampleData = double.Parse(txtSampleInput.Text);
				else if (rbnDateTime.Checked)
					_SampleData = DateTime.Parse(txtSampleInput.Text);
				else
					_SampleData = txtSampleInput.Text;
				UpdateSampleOutput();
			}
			catch (Exception ex)
			{
				lblSampleOutput.Text = ex.Message;
				lblSampleOutput.ForeColor = Color.Red;
			}
		}

		private void InsertHex(object sender, EventArgs e)
		{
			Button button = sender as Button;
			if (button == null)
				return;
			txtFormatString.Text = button.Text + GetNumHexDigits().ToString();
		}

		public string AlignmentString
		{
			get
			{
				if (_AlignmentString == null)
					return String.Empty;
				return _AlignmentString;
			}
			set
			{
				if (_AlignmentString == value)
					return;
				_AlignmentString = value;
				if (_AlignmentString == null)
					return;
				string alignmentChars = String.Empty;
				bool isLeft;
				
				if (_AlignmentString.StartsWith("-"))
				{
					isLeft = true;
					alignmentChars = _AlignmentString.Substring(1);
				}
				else
				{
					isLeft = false;
					alignmentChars = _AlignmentString;
				}
				int numSpaces;
				if (int.TryParse(alignmentChars, out numSpaces))
				{
					if (numSpaces > 0)
					{
						nudAlignmentCharacters.Value = numSpaces;
						rbnAlignLeft.Checked = isLeft;
						rbnAlignRight.Checked = !isLeft;
					}
				}

				// TODO: Set alignment control values based on this new value.
			}
		}

		public FormatItemExpressionType FormatItemExpressionType
		{
			get
			{
				return _FormatItemExpressionType;
			}
			set
			{
				_FormatItemExpressionType = value;
				switch (_FormatItemExpressionType)
				{
					case FormatItemExpressionType.String:
						rbnString.Checked = true;
						break;
					case FormatItemExpressionType.Integer:
						rbnInteger.Checked = true;
						break;
					case FormatItemExpressionType.Real:
						rbnReal.Checked = true;
						break;
					case FormatItemExpressionType.DateTime:
						rbnDateTime.Checked = true;
						break;
					case FormatItemExpressionType.Custom:
						rbnOtherType.Checked = true;
						break;
				}
			}
		}
		public string FormatString
		{
			get
			{
				return txtFormatString.Text;
			}
			set
			{
				txtFormatString.Text = value;
				// TODO: Set control values based on this value.
			}
		}
	}
}
