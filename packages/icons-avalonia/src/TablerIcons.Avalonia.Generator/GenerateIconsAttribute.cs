using System;
using System.Collections.Generic;
using System.Text;

namespace TablerIcons.Avalonia.Generator
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    internal class GenerateIconsAttribute : Attribute
    {
        public GenerateIconsAttribute(string iconsPath)
        {
            IconsPath = iconsPath;
        }

        public string IconsPath { get; }
    }
}
