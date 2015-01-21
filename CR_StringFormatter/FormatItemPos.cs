using System;
using System.Collections.Generic;
using DevExpress.CodeRush.StructuralParser;

namespace CR_StringFormatter
{
	public class FormatItemPos
	{
		public int Offset;
		public int Length;
		public FormatItem Parent;
		private int StringOffset
		{
			get
			{
				return Parent.Parent.PrimitiveExpression.FirstNameRange.Start.Offset + 1 /* quote */;
			}
		}
		public SourceRange GetSourceRange(int line)
		{
			int stringOffset = StringOffset;
			return new SourceRange(line, stringOffset + Offset, line, stringOffset + Offset + Length);
		}
    public FormatItemPos(FormatItem parent, int offset, int length)
		{
			Parent = parent;
      Offset = offset;
			Length = length;
		}
	}
}
