using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace ICMS.IntelligentAuxiliarySystem.Extensions
{
    public static class ObjectExtensions
    {
        public static object Clone(this object objSource)
        {
            var typeSource = objSource.GetType();
            var objTarget = Activator.CreateInstance(typeSource);
            var propertyInfo =
                typeSource.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var property in propertyInfo)
            {
                if (property.CanWrite)
                {
                    if (property.PropertyType.IsValueType || property.PropertyType.IsEnum ||
                        property.PropertyType == typeof(string))

                    {
                        property.SetValue(objTarget, property.GetValue(objSource, null), null);
                    }
                    else
                    {
                        var objPropertyValue = property.GetValue(objSource, null);
                        if (objPropertyValue == null)

                        {
                            property.SetValue(objTarget, null, null);
                        }
                        else
                        {
                            property.SetValue(objTarget, objPropertyValue.Clone(), null);
                        }
                    }
                }
            }

            return objTarget;
        }

        public static T Clone<T>(this T source)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source));
        }
    }
}
