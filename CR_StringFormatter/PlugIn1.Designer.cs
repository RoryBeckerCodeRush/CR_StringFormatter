namespace CR_StringFormatter
{
	partial class PlugIn1
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		public PlugIn1()
		{
			/// <summary>
			/// Required for Windows.Forms Class Composition Designer support
			/// </summary>
			InitializeComponent();
		}

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
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PlugIn1));
      this.spFormatItems = new DevExpress.CodeRush.Core.SearcherProvider();
      this.ctxInFormatItem = new DevExpress.CodeRush.Extensions.ContextProvider(this.components);
      this.cpFormatItem = new DevExpress.CodeRush.Core.CodeProvider(this.components);
      this.ipFormatItemIndexTooLarge = new DevExpress.CodeRush.Core.IssueProvider(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.spFormatItems)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ctxInFormatItem)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.cpFormatItem)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ipFormatItemIndexTooLarge)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
      // 
      // spFormatItems
      // 
      this.spFormatItems.ActiveFileOnly = false;
      this.spFormatItems.Description = "Navigates through matching format items.";
      this.spFormatItems.ProviderName = "Format Items";
      this.spFormatItems.Register = true;
      this.spFormatItems.UseForNavigation = true;
      this.spFormatItems.UseForRenaming = false;
      this.spFormatItems.CheckAvailability += new DevExpress.CodeRush.Core.CheckSearchAvailabilityEventHandler(this.spFormatItems_CheckAvailability);
      this.spFormatItems.SearchReferences += new DevExpress.CodeRush.Core.SearchReferencesEventHandler(this.spFormatItems_SearchReferences);
      // 
      // ctxInFormatItem
      // 
      this.ctxInFormatItem.Description = "Satisified if the caret is inside a format item (e.g., \"{0}\").";
      this.ctxInFormatItem.ProviderName = "Editor\\Code\\InFormatItem";
      this.ctxInFormatItem.Register = true;
      this.ctxInFormatItem.ContextSatisfied += new DevExpress.CodeRush.Core.ContextSatisfiedEventHandler(this.ctxInFormatItem_ContextSatisfied);
      // 
      // cpFormatItem
      // 
      this.cpFormatItem.ActionHintText = "Format Item";
      this.cpFormatItem.AutoActivate = true;
      this.cpFormatItem.AutoUndo = false;
      this.cpFormatItem.CodeIssueMessage = null;
      this.cpFormatItem.Description = "Brings up the String Formatter dialog...";
      this.cpFormatItem.DisplayName = "Format Item...";
      this.cpFormatItem.Image = ((System.Drawing.Bitmap)(resources.GetObject("cpFormatItem.Image")));
      this.cpFormatItem.NeedsSelection = false;
      this.cpFormatItem.ProviderName = "Format Item";
      this.cpFormatItem.Register = true;
      this.cpFormatItem.SupportsAsyncMode = false;
      this.cpFormatItem.Apply += new DevExpress.Refactor.Core.ApplyRefactoringEventHandler(this.cpFormatItem_Apply);
      this.cpFormatItem.CheckAvailability += new DevExpress.Refactor.Core.CheckAvailabilityEventHandler(this.cpFormatItem_CheckAvailability);
      // 
      // ipFormatItemIndexTooLarge
      // 
      this.ipFormatItemIndexTooLarge.DefaultIssueType = DevExpress.CodeRush.Core.CodeIssueType.None;
      this.ipFormatItemIndexTooLarge.Description = "The format item index must be less than the number of arguments passed in to a St" +
          "ring.Format call.";
      this.ipFormatItemIndexTooLarge.ProviderName = "Format item index too large";
      this.ipFormatItemIndexTooLarge.Register = true;
      this.ipFormatItemIndexTooLarge.CheckCodeIssues += new DevExpress.CodeRush.Core.CheckCodeIssuesEventHandler(this.ipFormatItemIndexTooLarge_CheckCodeIssues);
      ((System.ComponentModel.ISupportInitialize)(this.spFormatItems)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ctxInFormatItem)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.cpFormatItem)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ipFormatItemIndexTooLarge)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

		}

		#endregion

		private DevExpress.CodeRush.Core.SearcherProvider spFormatItems;
		private DevExpress.CodeRush.Extensions.ContextProvider ctxInFormatItem;
		private DevExpress.CodeRush.Core.CodeProvider cpFormatItem;
		private DevExpress.CodeRush.Core.IssueProvider ipFormatItemIndexTooLarge;
	}
}