using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Leayal.Forms
{
    [ComVisible(true)]
    [TypeConverter(typeof(AvailableIntRangeConverter))]
    public struct AvailableIntRange
    {
        static AvailableIntRange() { }
        private int _minimizeValue, _maximizeValue;
        public int MinimizeValue
        {
            get { return this._minimizeValue; }
            set { this._minimizeValue = value; }
        }
        public int MaximizeValue
        {
            get { return this._maximizeValue; }
            set { this._maximizeValue = value; }
        }
        public AvailableIntRange(int min, int max) : this()
        {
            if (max < min) throw new System.InvalidOperationException("Min must be smaller than Max or equal to Max.");
            this._minimizeValue = min;
            this._maximizeValue = max;
        }

        public override bool Equals(object obj)
        {
            if (obj != null)
                if (obj is AvailableIntRange)
                    return (this == (AvailableIntRange)obj);
            return false;
        }

        public override int GetHashCode()
        {
            return this._maximizeValue ^ this._minimizeValue;
        }

        public override string ToString()
        {
            return string.Format("{{Min={0}, Max={1}}}", this.MinimizeValue, this.MaximizeValue);
        }

        public static bool operator !=(AvailableIntRange air1, AvailableIntRange air2)
        {
            return !(air1 == air2);
        }

        public static bool operator ==(AvailableIntRange air1, AvailableIntRange air2)
        {
            if (air1.MinimizeValue == air2.MinimizeValue)
                if (air1.MaximizeValue == air2.MaximizeValue)
                    return true;
            return false;
        }
    }

    public class AvailableIntRangeConverter : TypeConverter
    {
        /// <summary>Initializes a new <see cref="T:Leayal.Forms.AvailableIntRangeConverter" /> object.</summary>
        public AvailableIntRangeConverter() { }

        /// <summary>Determines whether this converter can convert an object in the specified source type to the native type of the converter.</summary>
        /// <returns>This method returns true if this object can perform the conversion.</returns>
        /// <param name="context">A <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that can be used to get additional information about the environment this converter is being called from. This may be null, so you should always check. Also, properties on the context object may also return null. </param>
        /// <param name="sourceType">The type you want to convert from. </param>
        /// <filterpriority>1</filterpriority>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>Gets a value indicating whether this converter can convert an object to the given destination type using the context.</summary>
        /// <returns>This method returns true if this converter can perform the conversion; otherwise, false.</returns>
        /// <param name="context">A <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that can be used to get additional information about the environment this converter is being called from. This can be null, so always check. Also, properties on the context object can return null.</param>
        /// <param name="destinationType">A <see cref="T:System.Type" /> that represents the type you want to convert to. </param>
        /// <filterpriority>1</filterpriority>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor))
            {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }

        /// <summary>Converts the specified object to the converter's native type.</summary>
        /// <returns>The converted object. </returns>
        /// <param name="context">A <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that can be used to get additional information about the environment this converter is being called from. This may be null, so you should always check. Also, properties on the context object may also return null. </param>
        /// <param name="culture">An <see cref="T:System.Globalization.CultureInfo" /> object that contains culture specific information, such as the language, calendar, and cultural conventions associated with a specific culture. It is based on the RFC 1766 standard. </param>
        /// <param name="value">The object to convert. </param>
        /// <exception cref="T:System.NotSupportedException">The conversion cannot be completed.</exception>
        /// <filterpriority>1</filterpriority>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string str = value as string;
            if (str == null)
            {
                return base.ConvertFrom(context, culture, value);
            }
            string str1 = str.Trim();
            if (str1.Length == 0)
            {
                return null;
            }
            if (culture == null)
            {
                culture = CultureInfo.CurrentCulture;
            }
            char listSeparator = culture.TextInfo.ListSeparator[0];
            string[] strArrays = str1.Split(new char[] { listSeparator });
            int[] numArray = new int[(int)strArrays.Length];
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(int));
            for (int i = 0; i < (int)numArray.Length; i++)
            {
                numArray[i] = (int)converter.ConvertFromString(context, culture, strArrays[i]);
            }
            if ((int)numArray.Length != 2)
            {
                throw new ArgumentException();
            }
            return new AvailableIntRange(numArray[0], numArray[1]);
        }

        /// <summary>Converts the specified object to the specified type.</summary>
        /// <returns>The converted object.</returns>
        /// <param name="context">A <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that can be used to get additional information about the environment this converter is being called from. This may be null, so you should always check. Also, properties on the context object may also return null. </param>
        /// <param name="culture">An <see cref="T:System.Globalization.CultureInfo" /> object that contains culture specific information, such as the language, calendar, and cultural conventions associated with a specific culture. It is based on the RFC 1766 standard. </param>
        /// <param name="value">The object to convert. </param>
        /// <param name="destinationType">The type to convert the object to. </param>
        /// <exception cref="T:System.NotSupportedException">The conversion cannot be completed.</exception>
        /// <filterpriority>1</filterpriority>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException("destinationType");
            }
            if (value is AvailableIntRange)
            {
                if (destinationType == typeof(string))
                {
                    AvailableIntRange size = (AvailableIntRange)value;
                    if (culture == null)
                    {
                        culture = CultureInfo.CurrentCulture;
                    }
                    string str = string.Concat(culture.TextInfo.ListSeparator, " ");
                    TypeConverter converter = TypeDescriptor.GetConverter(typeof(int));
                    string[] strArrays = new string[2];
                    int num = 0;
                    int num1 = num + 1;
                    strArrays[num] = converter.ConvertToString(context, culture, size.MinimizeValue);
                    int num2 = num1;
                    num1 = num2 + 1;
                    strArrays[num2] = converter.ConvertToString(context, culture, size.MaximizeValue);
                    return string.Join(str, strArrays);
                }
                if (destinationType == typeof(InstanceDescriptor))
                {
                    AvailableIntRange size1 = (AvailableIntRange)value;
                    ConstructorInfo constructor = typeof(AvailableIntRange).GetConstructor(new Type[] { typeof(int), typeof(int) });
                    if (constructor != null)
                    {
                        return new InstanceDescriptor(constructor, new object[] { size1.MinimizeValue, size1.MaximizeValue });
                    }
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        /// <summary>Creates an object of this type by using a specified set of property values for the object. This is useful for creating non-changeable objects that have changeable properties.</summary>
        /// <returns>The newly created object, or null if the object could not be created. The default implementation returns null.</returns>
        /// <param name="context">A <see cref="T:System.ComponentModel.TypeDescriptor" /> through which additional context can be provided. </param>
        /// <param name="propertyValues">A dictionary of new property values. The dictionary contains a series of name-value pairs, one for each property returned from the <see cref="M:System.Drawing.SizeConverter.GetProperties(System.ComponentModel.ITypeDescriptorContext,System.Object,System.Attribute[])" /> method. </param>
        /// <filterpriority>1</filterpriority>
        public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
        {
            if (propertyValues == null)
            {
                throw new ArgumentNullException("propertyValues");
            }
            object item = propertyValues["MinimizeValue"];
            object obj = propertyValues["MaximizeValue"];
            if (item == null || obj == null || !(item is int) || !(obj is int))
            {
                throw new ArgumentException("");
            }
            return new AvailableIntRange((int)item, (int)obj);
        }

        /// <summary>Determines whether changing a value on this object should require a call to the <see cref="M:System.Drawing.SizeConverter.CreateInstance(System.ComponentModel.ITypeDescriptorContext,System.Collections.IDictionary)" /> method to create a new value.</summary>
        /// <returns>true if the <see cref="M:System.Drawing.SizeConverter.CreateInstance(System.ComponentModel.ITypeDescriptorContext,System.Collections.IDictionary)" /> object should be called when a change is made to one or more properties of this object.</returns>
        /// <param name="context">A <see cref="T:System.ComponentModel.TypeDescriptor" /> through which additional context can be provided. </param>
        /// <filterpriority>1</filterpriority>
        public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>Retrieves the set of properties for this type. By default, a type does not have any properties to return. </summary>
        /// <returns>The set of properties that should be exposed for this data type. If no properties should be exposed, this may return null. The default implementation always returns null.</returns>
        /// <param name="context">A <see cref="T:System.ComponentModel.TypeDescriptor" /> through which additional context can be provided. </param>
        /// <param name="value">The value of the object to get the properties for. </param>
        /// <param name="attributes">An array of <see cref="T:System.Attribute" /> objects that describe the properties. </param>
        /// <filterpriority>1</filterpriority>
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(AvailableIntRangeConverter), attributes);
            return properties.Sort(new string[] { "MinimizeValue", "MaximizeValue" });
        }
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}
