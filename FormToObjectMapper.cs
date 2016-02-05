using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;

namespace App.Lib
{
    public class FormToObjectMapper
    {
        public class FormMapping
        {
            public Type MapType { get; set; }
            public Func<Object, object> MappingFunc { get; set; }
        }

        public static List<FormMapping> _SupportedMappingTypes = new List<FormMapping>()
        {
            new FormMapping(){ 
                MapType = typeof(String), 
                MappingFunc = new Func<Object, object>((val) => { 
                    return val.ToString(); 
                }) 
            },
            new FormMapping(){ 
                MapType = typeof(Int16), 
                MappingFunc = new Func<Object, object>((val) => { 
                    return Convert.ToInt16(val); 
                }) 
            },
            new FormMapping(){ 
                MapType = typeof(Int32), 
                MappingFunc = new Func<Object, object>((val) => { 
                    return Convert.ToInt32(val); 
                }) 
            },
            new FormMapping(){ 
                MapType = typeof(Int64), 
                MappingFunc = new Func<Object, object>((val) => { 
                    return Convert.ToInt64(val); 
                }) 
            },
            new FormMapping(){ 
                MapType = typeof(Single), 
                MappingFunc = new Func<Object, object>((val) => { 
                    return Convert.ToSingle(val); 
                }) 
            },
            new FormMapping(){ 
                MapType = typeof(Decimal), 
                MappingFunc = new Func<Object, object>((val) => { 
                    return Convert.ToDecimal(val); 
                }) 
            },
            new FormMapping(){ 
                MapType = typeof(DateTime), 
                MappingFunc = new Func<Object, object>((val) => { 
                    return Convert.ToDateTime(val); 
                }) 
            },
            new FormMapping(){ MapType = typeof(DateTime?), 
                MappingFunc = new Func<Object, object>((val) => {
                    DateTime? date = null;
                    DateTime parsedDate;
                    if (DateTime.TryParse(val.ToString(), out parsedDate))
                        date = parsedDate;
                    return date; 
                }) 
            },
            new FormMapping(){ 
                MapType = typeof(IEnumerable<string>), 
                MappingFunc = new Func<Object, object>((val) => {
                    var result = (typeof(IEnumerable).IsAssignableFrom(val.GetType())) ? Convert.ToString(val).Split(',') : null;
                    return result; 
                }) 
            },
            new FormMapping(){ 
                MapType = typeof(Boolean), 
                MappingFunc = new Func<Object, object>((val) => {
                    return (val.ToString().IndexOf("true") > -1) ? true : false;
                }) 
            },
        };

        public static T FormDataToModel<T>(T model, NameValueCollection formData)
        {
            var requestInputs = formData.Cast<string>().Select(s => new { Key = s, Value = formData[s] });
            requestInputs = requestInputs.Where(x => !String.IsNullOrEmpty(x.Key) && !String.IsNullOrEmpty(x.Value));

            foreach (var data in requestInputs)
            {
                PropertyInfo property = ObjectUtils.GetObjectPropertyInfo(model, data.Key);

                if (property != null)
                {
                    var propertyName = data.Key.Split('.').Last();
                    object value = data.Value;
                    Type propType = ObjectUtils.GetObjectPropertyInfo(model, data.Key).PropertyType;

                    if (value != null)
                    {
                        var supportedType = _SupportedMappingTypes.Where(t => t.MapType == propType).FirstOrDefault();
                        ObjectUtils.SetObjectProperty(data.Key, model, supportedType.MappingFunc(value));
                    }
                }
            }
            return model;
        }
    }

    public static class ObjectUtils
    {
        /// <summary>
        /// Returns the Property information for an objects property. Works for Nested properties as well.
        /// </summary>
        /// <param name="obj">Object/Entity containing the property to fetch the value from.</param>
        /// <param name="property">The object property you need the value of. This property should follow the same naming convention used in web forms.</param>
        /// <returns></returns>
        public static PropertyInfo GetObjectPropertyInfo(object obj, string property)
        {
            //Split the property string on '.' to create a breadcrumb style trail to the possible nested property.
            var propertyBreadcrumbs = property.Split('.');
            PropertyInfo info = null;

            //Iterate through the propertyBreadcrumbs to get the PropertyInfo of the possible nested object property
            foreach (var p in propertyBreadcrumbs)
            {
                Type type = obj.GetType();
                info = type.GetProperty(p);

                if (info != null) obj = info.GetValue(obj, null);
            }

            return info;
        }

        /// <summary>
        /// Gets the value of an objects property. Works for Nested properties as well.
        /// </summary>
        /// <param name="obj">Object/Entity containing the property to fetch the value from.</param>
        /// <param name="property">The object property you need the value of. This property should follow the same naming convention used in web forms.</param>
        /// <returns>value as object</returns>
        public static object GetObjectPropertyValue(object obj, string property)
        {
            //Split the property string on '.' to create a breadcrumb style trail to the possible nested property.
            var propertyBreadcrumbs = property.Split('.');

            //Iterate through the propertyBreadcrumbs to get the value of the possible nested object property
            foreach (var p in propertyBreadcrumbs)
            {
                Type type = obj.GetType();
                PropertyInfo info = type.GetProperty(p);

                if (info != null) obj = info.GetValue(obj, null);
            }

            return obj;
        }

        /// <summary>
        /// Sets the value of an objects property. Works for nested properties as well.
        /// </summary>
        /// <param name="property">Property to Set (For Nested: PropertyName.PropertyName).</param>
        /// <param name="obj">Object to set property value on.</param>
        /// <param name="value">Value to set the property to.</param>
        public static void SetObjectProperty(string property, object obj, object value)
        {
            //Split the property string on '.' to create a breadcrumb style trail to the possible nested property.
            string[] propertyBreadcrumbs = property.Split('.');
            PropertyInfo propertyToSet = obj.GetType().GetProperty(propertyBreadcrumbs.Last());
            PropertyInfo nestedObj = null;

            //Iterate through the propertyBreadcrumbs to get to the object containing the property being set.
            for (int i = 0; i < propertyBreadcrumbs.Length - 1; i++)
            {
                nestedObj = obj.GetType().GetProperty(propertyBreadcrumbs[i]);
                obj = nestedObj.GetValue(obj, null);
            }

            propertyToSet.SetValue(obj, value, null);
        }
    }
}
