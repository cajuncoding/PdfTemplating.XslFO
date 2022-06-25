/*
Copyright 2012 Brandon Bernard

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

     http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;
using PdfTemplating.SystemCustomExtensions;
using System.Diagnostics;

namespace PdfTemplating.SystemReflectionCustomExtensions
{
    public static class AssemblyCustomExtensions
    {
        public static List<String> GetManifestResourceNamesByRegex(this Assembly assembly, String resourceNameRegexPattern)
        {
            Regex rx = new Regex(resourceNameRegexPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            Debug.WriteLine("Get Executing Assembly Resource Names retrieved from RegEx Filter: {0}".FormatArgs(resourceNameRegexPattern));
            return assembly.GetManifestResourceNamesByRegex(rx);
        }

        public static List<String> GetManifestResourceNamesByRegex(this Assembly assembly, Regex rxResourceNamePatter)
        {
            var resourceNames = from name in assembly.GetManifestResourceNames()
                                where rxResourceNamePatter.IsMatch(name)
                                select name;

            Debug.WriteLine("Matched Resource Names:");
            foreach (var name in resourceNames)
            {
                Console.WriteLine(" - {0}".FormatArgs(name));
            }

            return resourceNames.ToList();
        }

        public static IEnumerable<Stream> GetManifestResourceStreamsByRegex(this Assembly assembly, String resourceNameRegexPattern)
        {
            var resourceStreams = from resourceName in assembly.GetManifestResourceNamesByRegex(resourceNameRegexPattern)
                                  select assembly.GetManifestResourceStream(resourceName);

            return resourceStreams;
        }

        public static IEnumerable<Type> GetTypesInheritingFrom<TInhertisFrom>(this Assembly assembly)
        {
            var types = from t in assembly.GetTypes()
                        where typeof(TInhertisFrom).IsAssignableFrom(t)
                        select t;
            return types;
        }

        public static IEnumerable<TypeWithAttributes<TAttributeFilter>> GetTypesByAttributeFilter<TAttributeFilter>(this Assembly assembly)
        {
            List<Type> types = null;
            try
            {
                types = Assembly.GetAssembly(typeof(TAttributeFilter)).GetTypes().ToList();
            }
            catch (ReflectionTypeLoadException exc)
            {
                types = (from type in exc.Types
                         where type != null
                         select type).ToList();
            }

            var typesWithAttributes = from type in types
                                      let attributesOfTypeSpecified = type.GetCustomAttributes(typeof(TAttributeFilter), true)
                                      where
                                          attributesOfTypeSpecified != null
                                          && attributesOfTypeSpecified.Length > 0
                                      select new TypeWithAttributes<TAttributeFilter>(type, attributesOfTypeSpecified.Cast<TAttributeFilter>().ToList());

            return typesWithAttributes;
        }

        public static Dictionary<String, String> GetAssemblyAttributeProperties(this Assembly assembly)
        {
            Object[] atts = assembly.GetCustomAttributes(false);
            Dictionary<String, String> kvAttrProperties = new Dictionary<string, string>();
            foreach (var attrItem in atts)
            {
                PropertyInfo[] propertyInfo = attrItem.GetType().GetProperties();

                foreach (PropertyInfo pi in propertyInfo)
                {
                    if (pi.Name != "TypeId")
                    {
                        var attValue = pi.GetValue(attrItem, null).ToString();
                        kvAttrProperties.Add(attrItem.GetType().Name, attValue);
                        //Break out of the properties as we are ready to move on to the next attribute
                        break;
                    }
                }
            }

            return kvAttrProperties;
        }

    }

    public class TypeWithAttributes<TAttribute>
    {
        public TypeWithAttributes(Type type, List<TAttribute> attributes)
        {
            this.Type = type;
            this.Attributes = attributes;
        }
        public Type Type { get; private set; }
        public List<TAttribute> Attributes { get; private set; }
    }

    public static class EventInfoCustomExtensions
    {

        public static IEnumerable<Delegate> GetSubscribedEventHandlers(this EventInfo eventInfo, object target)
        {
            //BBernard - 12/17/2012
            //NOTE:  To get the actual Event Handler (ie. Methods) subscribed to an Event on and object instance
            //       we must get the actual backing Delegate field (using the same name as the Event).  Then
            //       we can retreive the Invocation List of subscribed methods.
            var type = eventInfo.DeclaringType;
            var fieldInfo = type.GetField(eventInfo.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField);

            //Now use GetValue() to get the reference of the actual backing event delegate -- this is usually
            //automatically created in the background by the compiler so dynamically resolving it works best in all cases.
            var backgingDelegate = fieldInfo.GetValue(target) as Delegate;

            //Event handlers may always be null so we have to validate
            if (backgingDelegate != null)
            {
                foreach (var handlerDelegate in backgingDelegate.GetInvocationList())
                {
                    yield return handlerDelegate;
                }
            }
        }

    }

    public static class FieldInfoCustomExtensions
    {

    }

    public static class MethodInfoCustomExtensions
    {
        public static string GetMethodSignature(this MethodInfo methodInfo)
        {
            var parameters = (from param in methodInfo.GetParameters()
                              select String.Format("{0} {1}", param.ParameterType.Name, param.Name)).ToArray();

            string signature = String.Format("{0} {1}({2})", methodInfo.ReturnType.Name, methodInfo.Name, String.Join(",", parameters));
            return signature;
        }
    }

    public static class CustomAttributeCustomExtensions
    {
        public static List<OfType> GetAttributes<OfType>(this ICustomAttributeProvider attrProvider) where OfType : Attribute
        {
            var attributes = from attr in attrProvider.GetCustomAttributes(typeof(OfType), false)
                             where attr is OfType
                             select attr as OfType;

            return attributes.ToList();
        }  
    }

}
