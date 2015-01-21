using System;
using System.Collections.Generic;
using DevExpress.CodeRush.StructuralParser;

namespace CR_StringFormatter
{
	public class FormatItem
	{
		private readonly List<FormatItemPos> _Positions = new List<FormatItemPos>();
    private int _Id;
    public int Id
    {
      get
      {
        return _Id;
      }
      set
      {
        _Id = value;
      }
    }
    private FormatItems _Parent;
    public FormatItems Parent
    {
      get
      {
        return _Parent;
      }
      private set
      {
        _Parent = value;
      }
    }

		public void AddPosition(int offset, int length)
		{
			Positions.Add(new FormatItemPos(this, offset, length));
		}
    public FormatItem(FormatItems parent, int id, IExpression argument)
		{
			Argument = argument;
      Parent = parent;
      Id = id;
		}

		public List<FormatItemPos> Positions
		{
			get
			{
				return _Positions;
			}
		}
    private IExpression _Argument;
    public IExpression Argument
    {
      get
      {
        return _Argument;
      }
      private set
      {
        _Argument = value;
      }
    }
	}
}
