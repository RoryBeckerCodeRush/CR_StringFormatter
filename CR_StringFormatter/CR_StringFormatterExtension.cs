using System.ComponentModel.Composition;
using DevExpress.CodeRush.Common;

namespace CR_StringFormatter
{
  [Export(typeof(IVsixPluginExtension))]
  public class CR_StringFormatterExtension : IVsixPluginExtension { }
}