using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace PSO2ProxyLauncherNew.Classes.PSO2.PSO2UserConfiguration
{
    [Serializable]
    [TypeConverter(typeof(ScreenResolutionConverter))]
    public struct ScreenResolution
    {
        static ScreenResolution() { }
        private int _width, _height;
        public int Width
        {
            get { return this._width; }
            set { this._width = value; }
        }
        public int Height
        {
            get { return this._height; }
            set { this._height = value; }
        }
        public ScreenResolution(int width, int height) : this()
        {
            this._width = width;
            this._height = height;
        }

        public override bool Equals(object obj)
        {
            if (obj != null)
                if (obj is ScreenResolution)
                    return (this == (ScreenResolution)obj);
            return false;
        }

        public override int GetHashCode()
        {
            return this._width ^ this._height;
        }

        public override string ToString()
        {
            return string.Format("{0}x{1}", this.Width, this.Height);
        }

        public static bool operator !=(ScreenResolution air1, ScreenResolution air2)
        {
            return !(air1 == air2);
        }

        public static bool operator ==(ScreenResolution air1, ScreenResolution air2)
        {
            if (air1.Width == air2.Width)
                if (air1.Height == air2.Height)
                    return true;
            return false;
        }
    }

    public class ScreenResolutionConverter : TypeConverter
    {
        /// <summary>Initializes a new <see cref="T:Leayal.Forms.ScreenResolutionConverter" /> object.</summary>
        public ScreenResolutionConverter() { }

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
            string[] strArrays = str1.Split(new char[] { 'x' });
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
            return new ScreenResolution(numArray[0], numArray[1]);
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
            if (value is ScreenResolution)
            {
                if (destinationType == typeof(string))
                {
                    ScreenResolution size = (ScreenResolution)value;
                    TypeConverter converter = TypeDescriptor.GetConverter(typeof(int));
                    string[] strArrays = new string[2];
                    int num = 0;
                    int num1 = num + 1;
                    strArrays[num] = converter.ConvertToString(context, culture, size.Width);
                    int num2 = num1;
                    num1 = num2 + 1;
                    strArrays[num2] = converter.ConvertToString(context, culture, size.Height);
                    return string.Join("x", strArrays);
                }
                if (destinationType == typeof(InstanceDescriptor))
                {
                    ScreenResolution size1 = (ScreenResolution)value;
                    ConstructorInfo constructor = typeof(ScreenResolution).GetConstructor(new Type[] { typeof(int), typeof(int) });
                    if (constructor != null)
                    {
                        return new InstanceDescriptor(constructor, new object[] { size1.Width, size1.Height });
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
            object item = propertyValues["Width"];
            object obj = propertyValues["Height"];
            if (item == null || obj == null || !(item is int) || !(obj is int))
            {
                throw new ArgumentException("");
            }
            return new ScreenResolution((int)item, (int)obj);
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
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(ScreenResolutionConverter), attributes);
            return properties.Sort(new string[] { "Width", "Height" });
        }
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}
