// <copyright file="GetStringFromEnum.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Facebook.Helpers
{
    using System.Reflection;
    using Facebook.Enums;

    /// <summary>
    /// GetStringFromEnum.
    /// </summary>
    public class GetStringFromEnum
    {
        /// <summary>
        /// Gets the enum string.
        /// </summary>
        /// <param name="enumIntValue">The enum int value.</param>
        /// <param name="enumType">Type of the enum.</param>
        /// <returns>Get Enum String.</returns>
        public static string GetEnumString(int enumIntValue, Type enumType)
        {
            Enum enumValue = (Enum)Enum.ToObject(enumType, enumIntValue);
            return GetStringValue(enumValue);
        }

        /// <summary>
        /// Gets the string value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>GetStringValue.</returns>
        public static string GetStringValue(Enum value)
        {
            Dictionary<Enum, StringValueAttribute> stringValues = new();
            string output = string.Empty;
            Type type = value.GetType();

            if (stringValues.ContainsKey(value))
            {
                output = (stringValues[value] as StringValueAttribute).Value;
            }
            else
            {
                // Look for our 'StringValueAttribute' in the field's custom attributes
                FieldInfo fi = type.GetField(value.ToString());
                StringValueAttribute[] attrs = fi.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];
                if (attrs.Length > 0)
                {
                    stringValues.Add(value, attrs[0]);
                    output = attrs[0].Value;
                }
            }

            return output;
        }
    }
}
