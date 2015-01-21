using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.CodeRush.Core;
using DevExpress.CodeRush.PlugInCore;
using DevExpress.CodeRush.StructuralParser;

namespace CR_StringFormatter
{
  public partial class PlugIn1 : StandardPlugIn
  {
    // DXCore-generated code...
    #region InitializePlugIn
    public override void InitializePlugIn()
    {
      base.InitializePlugIn();

      //
      // TODO: Add your initialization code here.
      //
    }
    #endregion
    #region FinalizePlugIn
    public override void FinalizePlugIn()
    {
      //
      // TODO: Add your finalization code here.
      //

      base.FinalizePlugIn();
    }
    #endregion

    #region IsStringFormatCall
    public static bool IsStringFormatCall(IElement methodCallExpression)
    {
      IElement validMethodCallExpression = GetValidMethodCallExpression(methodCallExpression);
      if (validMethodCallExpression == null)
        return false;

      IMethodReferenceExpression formatCall = (validMethodCallExpression as IWithSource).Source as IMethodReferenceExpression;
      if (formatCall == null)
        return false;
      IExpression qualifier = formatCall.Source as IExpression;
      if (qualifier == null)
        return false;

      if (!(qualifier is IReferenceExpression || qualifier is IThisReferenceExpression || qualifier is IBaseReferenceExpression))
        return false;

      string formatCallName = formatCall.Name;
      List<string> expectedTypeNames = new List<string>();
      if (formatCallName == "Format")
        expectedTypeNames.Add("System.String");
      else if (formatCallName == "AppendFormat")
        expectedTypeNames.Add("System.Text.StringBuilder");
      else if (formatCallName == "Write" || formatCall.Name == "WriteLine")
      {
        expectedTypeNames.Add("System.Console");
        expectedTypeNames.Add("System.IO.TextWriter");
      }

      if (expectedTypeNames.Count > 0)
        foreach (string expectedTypeName in expectedTypeNames)
        {
          ITypeElement qualifierDeclaration = qualifier.Resolve(ParserServices.SourceTreeResolver) as ITypeElement;
          if (qualifierDeclaration != null && qualifierDeclaration.Is(expectedTypeName))
            return true;
        }
      return false;
    }
    #endregion
    private static IElement GetValidMethodCallExpression(IElement methodCallExpression)
    {
      if (methodCallExpression == null)
        return null;

      if (methodCallExpression is IAttributeVariableInitializer)
        return methodCallExpression.Parent;

      if (!(methodCallExpression is IWithArguments))
        return null;
      if (!(methodCallExpression is IWithSource))
        return null;

      return methodCallExpression;
    }
    #region InFirstStringArgument
    private static bool InFirstStringArgument(LanguageElement element, int line, int offset)
    {
      PrimitiveExpression primitiveExpression = element as PrimitiveExpression;
      if (primitiveExpression == null)
        return false;

      if (primitiveExpression.PrimitiveType != PrimitiveType.String)
        return false;

      LanguageElement methodCallExpression = primitiveExpression.Parent;
      if (!IsStringFormatCall(methodCallExpression))
        return false;

      IExpression formatStringArgument = GetFormatStringArgument(methodCallExpression);
      if (formatStringArgument == null)
        return false;
      return formatStringArgument.FirstNameRange.Contains(line, offset);
    }
    #endregion
    private static IExpression GetFormatStringArgument(LanguageElement methodCallExpression)
    {
      if (methodCallExpression is IAttributeVariableInitializer)
        methodCallExpression = methodCallExpression.Parent;

      IHasArguments hasArguments = methodCallExpression as IHasArguments;
      if (hasArguments == null)
        return null;
      if (hasArguments.ArgumentsCount <= 0)
        return null;

      IExpression firstArgument = hasArguments.Arguments[0];
      if (IsPrimitiveExpressionArg(firstArgument))
        return GetPrimitiveExpressionArg(firstArgument);
      else if (hasArguments.ArgumentsCount > 1)
      {
        IExpression secondArgument = hasArguments.Arguments[1];
        if (IsPrimitiveExpressionArg(secondArgument))
          return GetPrimitiveExpressionArg(secondArgument);
      }
      return null;
    }

    public static bool IsPrimitiveExpressionArg(IExpression exp)
    {
      if (exp is IPrimitiveExpression)
        return true;

      IAttributeVariableInitializer attrInitializer = exp as IAttributeVariableInitializer;
      if (attrInitializer != null)
        return attrInitializer.RightSide is IPrimitiveExpression;

      return false;
    }

    private static IPrimitiveExpression GetPrimitiveExpressionArg(IExpression exp)
    {
      if (exp is IPrimitiveExpression)
        return exp as IPrimitiveExpression;

      IAttributeVariableInitializer attrInitializer = exp as IAttributeVariableInitializer;
      if (attrInitializer != null)
        return attrInitializer.RightSide as IPrimitiveExpression;

      return null;
    }

    private bool InFormatItem(LanguageElement element, int line, int offset)
    {
      if (!InFirstStringArgument(element, line, offset))
        return false;

      FormatItems formatItems = GetFormatItems(element as PrimitiveExpression);
      return formatItems.GetFormatItemAtPos(line, offset) != null;
    }
    private void spFormatItems_CheckAvailability(object sender, CheckSearchAvailabilityEventArgs ea)
    {
      ea.Available = InFormatItem(ea.Element, ea.Caret.Line, ea.Caret.Offset);
    }

    private void spFormatItems_SearchReferences(object sender, SearchEventArgs ea)
    {
      int caretLine = ea.Caret.Line;
      int caretOffset = ea.Caret.Offset;

      if (!InFirstStringArgument(ea.Element, caretLine, caretOffset))
        return;

      FormatItems formatItems = GetFormatItems(ea.Element as PrimitiveExpression);
      if (formatItems.Count == 0)
        return;
      ISourceFile sourceFile = formatItems.SourceFile;

      FormatItemPos formatItemPos = formatItems.GetFormatItemPosAtPos(caretLine, caretOffset);
      FormatItem formatItem = formatItemPos.Parent;
      if (formatItem == null)
        return;

      // Add each occurrence of this format item to the navigation range...
      foreach (FormatItemPos position in formatItem.Positions)
      {
        SourceRange sourceRange = position.GetSourceRange(caretLine);
        ea.AddRange(new FileSourceRange(sourceFile, sourceRange));
      }

      if (formatItem.Argument != null)
        ea.AddRange(new FileSourceRange(sourceFile, formatItem.Argument.FirstNameRange));
    }

    #region GetFormatItems
    /// <summary>
    /// Parses the text in the specified PrimitiveExpression, collecting and returning a dictionary of FormatItems, indexed by the format item number.
    /// </summary>
    public static FormatItems GetFormatItems(IPrimitiveExpression primitiveExpression)
    {
      FormatItems formatItems = new FormatItems();
      formatItems.ParentMethodCall = GetValidMethodCallExpression(primitiveExpression.Parent) as IWithArguments;
      int argumentCount = formatItems.ParentMethodCall.Args.Count;
      formatItems.PrimitiveExpression = primitiveExpression;
      if (primitiveExpression == null)
        return formatItems;
      string text = primitiveExpression.Value as string;
      if (String.IsNullOrEmpty(text))
        return formatItems;
      bool lastCharWasOpenBrace = false;
      bool insideFormatItem = false;
      bool collectingFormatItemNumber = false;
      string numberStr = String.Empty;
      int lastOpenBraceOffset = 0;
      int length = 0;
      for (int i = 0; i < text.Length; i++)
      {
        char thisChar = text[i];
        if (thisChar == '{')
        {
          lastCharWasOpenBrace = !lastCharWasOpenBrace;
          lastOpenBraceOffset = i;
        }
        else if (thisChar == '}')
        {
          if (insideFormatItem)
          {
            insideFormatItem = false;
            if (numberStr != String.Empty)
            {
              int number = int.Parse(numberStr);
              const int INT_CountForBraceDelimeters = 2;
              int argumentIndex = number + 1;
              IExpression argument = null;
              if (argumentIndex < argumentCount)
                argument = formatItems.ParentMethodCall.Args[argumentIndex];

              if (!formatItems.HasFormatItem(number))
                formatItems.AddFormatItem(number, argument);
              formatItems[number].AddPosition(lastOpenBraceOffset, length + INT_CountForBraceDelimeters);
            }
          }
        }
        else if (lastCharWasOpenBrace)
        {
          length = 0;
          lastCharWasOpenBrace = false;
          insideFormatItem = true;
          collectingFormatItemNumber = true;
          numberStr = String.Empty;
          if (char.IsDigit(thisChar))
            numberStr = thisChar.ToString();		// First digit...
        }
        else if (collectingFormatItemNumber)
        {
          if (char.IsDigit(thisChar))
            numberStr += thisChar.ToString();		// Subsequent digit...
          else
            collectingFormatItemNumber = false;
        }
        length++;
      }
      return formatItems;
    }
    #endregion

    #region ctxInFormatItem_ContextSatisfied
    private void ctxInFormatItem_ContextSatisfied(ContextSatisfiedEventArgs ea)
    {
      ea.Satisfied = InFormatItem(CodeRush.Source.Active, CodeRush.Caret.Line, CodeRush.Caret.Offset);
    }
    #endregion

    #region cpFormatItem_CheckAvailability
    private void cpFormatItem_CheckAvailability(object sender, CheckContentAvailabilityEventArgs ea)
    {
      ea.Available = InFormatItem(ea.Element, ea.Caret.Line, ea.Caret.Offset);
    }
    #endregion

    private FormatItemPos GetActivePosition(ApplyContentEventArgs ea)
    {
      int caretLine = ea.Caret.Line;
      int caretOffset = ea.Caret.Offset;

      if (!InFirstStringArgument(ea.Element, caretLine, caretOffset))
        return null;

      FormatItems formatItems = GetFormatItems(ea.Element as PrimitiveExpression);
      return formatItems.GetFormatItemPosAtPos(caretLine, caretOffset);
    }
    #region GetFormatItemDetails
    private static void GetFormatItemDetails(TextDocument textDocument, SourceRange formatItemRange, out string numberStr, out string alignment, out string format)
    {
      format = textDocument.GetText(formatItemRange);
      alignment = String.Empty;
      numberStr = String.Empty;
      if (format.Length >= 3)
      {
        if (format[format.Length - 1] == '}')
          format = format.Remove(format.Length - 1, 1);
        if (format[0] == '{')
          format = format.Remove(0, 1);
        while (format.Length > 0 && char.IsDigit(format[0]))
        {
          numberStr += format[0];
          format = format.Remove(0, 1);
        }
        if (format.StartsWith(","))
        {
          format = format.Remove(0, 1);
          if (format.StartsWith("-"))
          {
            format = format.Remove(0, 1);
            alignment = "-";
          }
          // Remove alignment text...
          while (format.Length > 0 && char.IsDigit(format[0]))
          {
            alignment += format[0];
            format = format.Remove(0, 1);
          }
        }
        if (format.StartsWith(":"))
          format = format.Remove(0, 1);
      }
    }
    #endregion
    private void cpFormatItem_Apply(object sender, ApplyContentEventArgs ea)
    {
      FormatItemPos activePosition;
      activePosition = GetActivePosition(ea);
      if (activePosition == null)
        return;

      FormatItems formatItems = activePosition.Parent.Parent;

      int line = formatItems.PrimitiveExpression.FirstNameRange.Start.Line;
      SourceRange formatItemRange = activePosition.GetSourceRange(line);

      string number;
      string alignment;
      string format;
      GetFormatItemDetails(ea.TextDocument, formatItemRange, out number, out alignment, out format);

      using (FrmStringFormatter frmStringFormatter = new FrmStringFormatter())
      {
        if (number != String.Empty)
        {
          int argumentIndex = int.Parse(number) + 1;
          IExpression argument = null;
          IPrimitiveExpression primitiveExpression = null;
          if (argumentIndex < formatItems.ParentMethodCall.Args.Count)
          {
            argument = formatItems.ParentMethodCall.Args[argumentIndex];
            primitiveExpression = argument as IPrimitiveExpression;
          }
          if (primitiveExpression != null)
          {
            // Use primitiveExpression's PrimitiveType property
            PrimitiveType primitiveType = primitiveExpression.PrimitiveType;
            if (primitiveType == PrimitiveType.Single || primitiveType == PrimitiveType.Decimal || primitiveType == PrimitiveType.Double)
              frmStringFormatter.FormatItemExpressionType = FormatItemExpressionType.DateTime;
            else if (primitiveType == PrimitiveType.Char || primitiveType == PrimitiveType.String)
              frmStringFormatter.FormatItemExpressionType = FormatItemExpressionType.String;
            else if (primitiveType == PrimitiveType.SByte || primitiveType == PrimitiveType.Byte || primitiveType == PrimitiveType.Int16 || primitiveType == PrimitiveType.Int32 || primitiveType == PrimitiveType.Int64 || primitiveType == PrimitiveType.UInt16 || primitiveType == PrimitiveType.UInt32 || primitiveType == PrimitiveType.UInt64)
              frmStringFormatter.FormatItemExpressionType = FormatItemExpressionType.Integer;
            else
              frmStringFormatter.FormatItemExpressionType = FormatItemExpressionType.Custom;
          }
          else
          {
            if (argument != null)
            {
              IElement resolve = argument.Resolve(ParserServices.SourceTreeResolver);
              if (resolve.Name == "DateTime")
                frmStringFormatter.FormatItemExpressionType = FormatItemExpressionType.DateTime;
              else if (resolve.Name == "Double" || resolve.Name == "Single")
                frmStringFormatter.FormatItemExpressionType = FormatItemExpressionType.Real;
              else if (resolve.Name == "String")
                frmStringFormatter.FormatItemExpressionType = FormatItemExpressionType.String;
              else if (resolve.Name == "Int32" || resolve.Name == "Int64" || resolve.Name == "Int16")
                frmStringFormatter.FormatItemExpressionType = FormatItemExpressionType.Integer;
              else
              {
                // set default value...
              }
            }
            else
            {
              frmStringFormatter.FormatItemExpressionType = FormatItemExpressionType.Integer;
            }
          }
        }

        frmStringFormatter.FormatString = format;
        frmStringFormatter.AlignmentString = alignment;
        if (frmStringFormatter.ShowDialog() == DialogResult.OK)
        {
          string newFormatItemCode = String.Format("{{{0}{1}:{2}}}", number, frmStringFormatter.AlignmentString, frmStringFormatter.FormatString);
          ea.TextDocument.Replace(formatItemRange, newFormatItemCode, "Format Item");
        }
      }
    }

    private void ipFormatItemIndexTooLarge_CheckCodeIssues(object sender, CheckCodeIssuesEventArgs ea)
    {
      FormatItemTooLargeSearcher searcher = new FormatItemTooLargeSearcher(ipFormatItemIndexTooLarge.DisplayName);
      searcher.CheckCodeIssues(ea);
    }

    public static Expression formatStringArgument { get; set; }
  }
  public class FormatItemTooLargeSearcher : BaseCodeIssueSearcher
  {
    private string _Message;
    public FormatItemTooLargeSearcher(string message)
    {
      _Message = message;
    }

    public override void CheckCodeIssues(CheckCodeIssuesEventArgs ea)
    {
      IEnumerable<IElement> enumerable = ea.GetEnumerable(ea.Scope, new ElementTypeFilter(LanguageElementType.PrimitiveExpression));
      foreach (IElement element in enumerable)
      {
        IPrimitiveExpression iPrimitiveExpression = element as IPrimitiveExpression;
        if (iPrimitiveExpression != null)
        {
          if (iPrimitiveExpression.PrimitiveType == PrimitiveType.String)
          {
            IElement methodCallExpression = iPrimitiveExpression.Parent;
            if (PlugIn1.IsStringFormatCall(methodCallExpression))
            {
              FormatItems formatItems = PlugIn1.GetFormatItems(iPrimitiveExpression);
              if (formatItems.ParentMethodCall != null)
              {
                int argumentCount = GetValidArgumentsCount(formatItems);
                foreach (FormatItem formatItem in formatItems.Values)
                {
                  int argumentIndex = formatItem.Id + 1;
                  if (argumentIndex >= argumentCount)
                  {
                    int line = formatItems.PrimitiveExpression.FirstNameRange.Start.Line;
                    foreach (FormatItemPos position in formatItem.Positions)
                      ea.AddError(position.GetSourceRange(line), _Message);
                  }
                }
              }
            }
          }
        }
      }
    }

    private static int GetValidArgumentsCount(FormatItems formatItems)
    {
      if (formatItems == null)
        return -1;

      IWithArguments withArguments = formatItems.ParentMethodCall;
      if (withArguments == null)
        return -1;

      int argsCount = withArguments.Args.Count;
      if (argsCount > 0)
      {
        if (PlugIn1.IsPrimitiveExpressionArg(withArguments.Args[0]))
          return argsCount;
        else
          return argsCount - 1; // skip IFormatter argument...
      }
      return -1;
    }
  }
}