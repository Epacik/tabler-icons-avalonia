using Avalonia;
using Svg;
using System;
using System.Collections.Generic;
using System.Text;

namespace TablerIcons
{
    internal static class Helpers
    {
        /// <summary>
        /// Gets an attribute on an enum field value
        /// </summary>
        /// <typeparam name="T">The type of the attribute you want to retrieve</typeparam>
        /// <param name="enumVal">The enum value</param>
        /// <returns>The attribute of type T that exists on the enum value</returns>
        /// <example><![CDATA[string desc = myEnumVariable.GetAttributeOfType<DescriptionAttribute>().Description;]]></example>
        public static T GetAttributeOfType<T>(this Enum enumVal) where T : System.Attribute
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
            return (attributes.Length > 0) ? (T)attributes[0] : null;
        }

        public static System.Drawing.Color ToSystemDrawingColor(this global::Avalonia.Media.Color color)
        {
            return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static (SvgUnit X, SvgUnit Y) ToSvgXY(this RelativePoint point)
        {

            var unit = point.Unit == RelativeUnit.Absolute ? SvgUnitType.Point : SvgUnitType.Percentage;
            var x = new SvgUnit(unit, (float)point.Point.X);
            var y = new SvgUnit(unit, (float)point.Point.Y);

            return (x, y);
        }
    }
}
