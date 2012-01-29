using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Efmap.Constraints
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    sealed class UniqueAttribute : Attribute
    {
        public UniqueAttribute()
        {

        }
    }
}
