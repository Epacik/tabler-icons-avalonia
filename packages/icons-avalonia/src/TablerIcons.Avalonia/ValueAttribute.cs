using System;
using System.Collections.Generic;
using System.Text;

namespace TablerIcons.Avalonia
{
    [AttributeUsage(AttributeTargets.All)]
    internal class ValueAttribute : Attribute
    {
        public ValueAttribute(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }
}
