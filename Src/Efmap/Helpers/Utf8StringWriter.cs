using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Efmap.Helpers
{
    internal class Utf8StringWriter : StringWriter
{
    public override Encoding Encoding
    {
         get { return Encoding.UTF8; }
    }
}
}
