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

using System.Collections.Generic;
using PdfTemplating.SystemCustomExtensions;
using System.IO;
using PdfTemplating.SystemIOCustomExtensions;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml.XPath;
using System.Xml.Linq;
using System;
using System.Xml;

namespace PdfTemplating.SystemXmlLinqCustomExtensions
{

    public static class XmlLinqExtensions
    {
        public static bool IsXLink(this XElement xEl)
        {
            //bool bIsXLink = xEl.ContainsAttributeNamespaceName("xlink:href", StringComparison.CurrentCultureIgnoreCase);
            bool bIsXLink = xEl.Attributes().Any(xAttr => xAttr.Name.NamespaceName.ToLower().Contains("xlink"));
            return bIsXLink;
        }

        public static XDocument Clone(this XDocument doc)
        {
            return new XDocument(doc);
        }

        public static XElement ElementClone(this XElement element)
        {
            return element.DescendantsAndSelfClone();
        }

        public static XElement DescendantsAndSelfClone(this XElement element)
        {
            return new XElement(element);
            //return new XElement(element.Name,
            //                element.Attributes(),
            //                element.Nodes().Select(n =>
            //                {
            //                    XElement el = n as XElement;
            //                    return el != null ? el.DescendantsAndSelfClone() : n;
            //                })
            //            );
        }

        public static XElement Element(this XElement element, string name, string prefix)
        {
            //Provide Safe Return so that Chaining does not break.
            if (element == null) return null;

            XElement output = null;

            if (!String.IsNullOrEmpty(name) && !String.IsNullOrEmpty(prefix) && element.HasElements)
            {
                XNamespace nameSpace = element.GetNamespaceOfPrefix(prefix);
                output = Element(element, name, nameSpace);
            }

            return output;
        }

        public static XElement Element(this XElement element, string name, XNamespace nameSpace)
        {
            //Provide Safe Return so that Chaining does not break.
            if (element == null) return null;

            XElement output = null;

            if (!String.IsNullOrEmpty(name) && nameSpace != null && element.HasElements)
            {
                    output = element.Element(XName.Get(name, nameSpace.NamespaceName));
            }

            return output;
        }

        public static String ValueOrDefault(this XElement element)
        {
            return element.ValueOrDefault(String.Empty);
        }

        public static String ValueOrDefault(this XElement element, string defaultValue)
        {
            return element != null ? (element.Value ?? defaultValue) : defaultValue;
        }

        public static String ValueOrDefault(this XAttribute attr)
        {
            return attr.ValueOrDefault(String.Empty);
        }

        public static String ValueOrDefault(this XAttribute attr, string defaultValue)
        {
            return attr != null ? (attr.Value ?? defaultValue) : defaultValue;
        }

        public static String ValueOrDefault(this XText text)
        {
            return text.ValueOrDefault(String.Empty);
        }

        public static String ValueOrDefault(this XText text, string defaultValue)
        {
            return text != null ? (text.Value ?? defaultValue) : defaultValue;
        }

        public static bool ValueIsNullOrEmpty(this XElement element)
        {
            return element.ValueOrDefault().IsNullOrEmpty();
        }

        public static bool ValueIsNullOrEmpty(this XAttribute element)
        {
            return element.ValueOrDefault().IsNullOrEmpty();
        }

        public static bool ValueIsNullOrEmpty(this XText element)
        {
            return element.ValueOrDefault().IsNullOrEmpty();
        }

        //Overloaded Element Method to allow specifying a Default Element if not found!
        public static XElement Element(this XContainer element, XName name, XElement xDefaultElement)
        {
            XElement xElFound = element.Element(name);
            return xElFound != null ? xElFound : xDefaultElement;
        }

        public static XElement ElementIgnoreCase(this XContainer container, XName name)
        {
            //Provide Safe Return so that Chaining does not break.
            if (container == null) return null;

            foreach (XElement element in container.Elements())
            {
                if ((String.IsNullOrEmpty(name.NamespaceName) || element.Name.NamespaceName.Equals(name.NamespaceName, StringComparison.CurrentCultureIgnoreCase))
                    && element.Name.LocalName.Equals(name.LocalName, StringComparison.CurrentCultureIgnoreCase))
                {
                    return element;
                }
            }
            return null;
        }

        public static bool HasNamespaceDeclarations(this XElement element)
        {
            return (element.NamespaceDeclarationAttributes().Count() > 0);
        }

        public static IEnumerable<XAttribute> NamespaceDeclarationAttributes(this XElement element)
        {
            return element.NamespaceDeclarationAttributes(false);
        }

        public static IEnumerable<XAttribute> NamespaceDeclarationAttributes(this XElement element, bool includeAllDescendants)
        {
            if (element != null)
            {
                return (includeAllDescendants ? element.DescendantsAndSelf().Attributes() : element.Attributes()).Where(a => a.IsNamespaceDeclaration);
            }
            return new List<XAttribute>().AsEnumerable();
        }

        public static XNamespace GetNamespace(this XElement element)
        {
            return element.Name.Namespace;
        }

        public static XNamespace GetNamespace(this XAttribute attribute)
        {
            return attribute.Name.Namespace;
        }

        public static XElement SetNamespace(this XElement element, XNamespace xNamespace)
        {
            element.Name = xNamespace.GetName(element.Name.LocalName);
            return element;
        }

        public static String GetNamespacePrefix(this XElement element)
        {
            return element.GetPrefixOfNamespace(element.Name.Namespace) ?? String.Empty;
        }

        public static String GetPrefix(this XNamespace xNameSpace, XElement xElementScope)
        {
            return xElementScope.GetPrefixOfNamespace(xNameSpace) ?? String.Empty;
        }

        public static XAttribute AddNamespaceDeclarationAttribute(this XElement element, String namespacePrefix, XNamespace xNamespace)
        {
            return element.AddNamespaceDeclarationAttribute(namespacePrefix, xNamespace.NamespaceName, true);
        }

        public static XAttribute AddNamespaceDeclarationAttribute(this XElement element, String namespacePrefix, XNamespace xNamespace, bool updateNamespaceIfPrefixExists)
        {
            return element.AddNamespaceDeclarationAttribute(namespacePrefix, xNamespace.NamespaceName, updateNamespaceIfPrefixExists);
        }

        public static XAttribute AddNamespaceDeclarationAttribute(this XElement element, String namespacePrefix, String namespaceName)
        {
            return element.AddNamespaceDeclarationAttribute(namespaceName, namespaceName, true);
        }

        public static XAttribute AddNamespaceDeclarationAttribute(this XElement element, String namespacePrefix, String namespaceName, bool updateNamespaceIfPrefixExists)
        {
            if(updateNamespaceIfPrefixExists)
            {
                var existingAttr = element.NamespaceDeclarationAttributes(false).Where(attr => attr.IsNamespaceDeclaration && attr.Name.LocalName.EqualsIgnoreCase(namespacePrefix)).FirstOrDefault();
                if(existingAttr != null)
                {
                    existingAttr.Remove();
                }
            }

            XAttribute namespaceDeclarationAttr = new XAttribute(XNamespace.Xmlns.GetName(namespacePrefix), namespaceName);
            element.Add(new XAttribute(XNamespace.Xmlns.GetName(namespacePrefix), namespaceName));
            return namespaceDeclarationAttr;
        }

        public static XDocument RemoveNamespaceDeclarationAttributes(this XDocument document)
        {
            //Make method safe for Chaining by doing nothing when null 
            if (document != null)
            {
                document.Root.RemoveNamespaceDeclarationAttributes();
            }
            return document;
        }

        public static XElement RemoveNamespaceDeclarationAttributes(this XElement element)
        {
            //Make method safe for Chaining by doing nothing when null 
            if (element != null)
            {
                element.NamespaceDeclarationAttributes().Remove();
            }
            return element;
        }

        public static XDocument RemoveNamespaces(this XDocument document)
        {
            //Make method safe for Chaining by doing nothing when null 
            if (document != null)
            {
                document.Root.RemoveNamespaces(true);
            }
            return document;
        }

        public static XElement RemoveNamespaces(this XElement element)
        {
            return element.RemoveNamespaces(true);
        }

        /// <summary>
        /// Removes all Namespaces from the specified Element with option to include and traverse all Descendents (True by Default via Overloaded Method).
        /// </summary>
        /// <param name="element"></param>
        /// <param name="bIncludeAllDescendants"></param>
        /// <returns></returns>
        public static XElement RemoveNamespaces(this XElement element, bool bIncludeAllDescendants)
        {
            //Make method safe for Chaining by doing nothing when null 
            //(ie. since this isn't an iteration loop we must check the single item for null)
            if (element != null)
            {
                IEnumerable<XElement> elements = new List<XElement>();

                //BBernard - 11/15/2011
                //Note:  Optimized code to remove unneccessary steps.
                if (bIncludeAllDescendants)
                {
                    //Remove all Namespaces for all Descendants
                    elements = element.DescendantsAndSelf();
                }
                else
                {
                    ((List<XElement>)elements).Add(element);
                }

                foreach (XElement e in elements)
                {
                    if (e.Name.Namespace != XNamespace.None)
                    {
                        e.Name = XNamespace.None.GetName(e.Name.LocalName);
                    }

                    //Remove Namespace Attribute Definitions
                    e.RemoveNamespaceDeclarationAttributes();

                    //Remove Attribute Specifications from Attributes that are not Namespace definitions
                    e.ReplaceAttributes(
                        from a in e.Attributes()
                        select (a.Name.Namespace == XNamespace.None ? a : new XAttribute(XNamespace.None.GetName(a.Name.LocalName), a.Value))
                    );
                }
            }

            return element;
        }


        public static IEnumerable<XNamespace> GetNamespaces(this XElement element)
        {
            if (element != null)
            {
                var xAttributeQuery = (from xEl in element.DescendantsAndSelf()
                                      from xAttr in xEl.Attributes()
                                      where xAttr.IsNamespaceDeclaration
                                      select xEl.GetNamespaceOfPrefix(xAttr.Name.LocalName)).Distinct();

                return xAttributeQuery;
            }
            return new List<XNamespace>().AsEnumerable();
        }

        public static Dictionary<String, XNamespace> GetNamespacesPrefixDictionary(this XDocument document)
        {
            if(document != null)
            {
                return document.Root.GetNamespacesPrefixDictionary();
            }
            return new XElement("Default").GetNamespacesPrefixDictionary();
        }

        public static Dictionary<String, XNamespace> GetNamespacesPrefixDictionary(this XElement element)
        {
            var dictionary = new Dictionary<String, XNamespace>();
            if (element != null)
            {
                if (element != null)
                {
                    dictionary =   (from xEl in element.DescendantsAndSelf()
                                    from xAttr in xEl.Attributes()
                                    where xAttr.IsNamespaceDeclaration
                                    select new
                                    {
                                        Prefix = xAttr.Name.LocalName,
                                        Namespace = xEl.GetNamespaceOfPrefix(xAttr.Name.LocalName)
                                    })
                                    .ToDictionary(
                                        ns => ns.Prefix, 
                                        ns => ns.Namespace
                                    );
                }
            }
            return dictionary;
        }

        /// <summary>
        /// Changes existing Namespace Prefix strings to the mapped target string.  Note However that the actual Namespace values remain un-altered
        /// and therefore will not match if the elements are merged into other document contexts; yielding unexpected results.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="kvNamespacePrefixMappings"></param>
        /// <returns></returns>
        public static XElement AliasNamespacePrefixes(this XElement element, Dictionary<string, string> kvNamespacePrefixMappings)
        {
            //Make method safe for Chaining by doing nothing when null 
            if (element != null)
            {
                //Updating Only Elements that contain valid Namespace alias declarations will allow LinqToXml to automatically manage and bind
                //all namespace prefix updates to the document without the need to iterate each element -- for maximum performance.
                foreach (var xEl in element.DescendantsAndSelf().Where(el => el.HasNamespaceDeclarations()))
                {
                    xEl.ReplaceAttributes(
                        from a in xEl.Attributes()
                        let xmlnsPrefix = a.Name.LocalName
                        select (a.IsNamespaceDeclaration && kvNamespacePrefixMappings.ContainsKey(xmlnsPrefix)) ? new XAttribute(XNamespace.Xmlns.GetName(kvNamespacePrefixMappings[xmlnsPrefix]), a.Value) : a
                    );
                }
            }
            return element;
        }

        /// <summary>
        /// Alias all descendent elements of the element specified that match the source namespace into the target namespace specified.
        /// Source and Target namespaces must be defined as Xmlns Declaration XAttribute objects whereby the XAttribute.Name.LocalName property is the Prefix
        /// to search for and create, and the XAttribute.Value is the NamespaceName to be created.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="xSourceXmlnsDeclaration"></param>
        /// <param name="xTargetXmlnsDeclaration"></param>
        /// <returns></returns>
        public static XElement AliasNamespaceByPrefix(this XElement element, String sourceXmlnsPrefix, String targetXmlnsPrefix, String targetNamespaceName)
        {
            //Make method safe for Chaining by doing nothing when null 
            if (element != null && !sourceXmlnsPrefix.IsNullOrEmpty() && !targetXmlnsPrefix.IsNullOrEmpty() && !targetNamespaceName.IsNullOrEmpty())
            {
                //Initialize the map criteria for source and target namespace and prefix elements based on the defined LookupType
                //Note:  We also validate that the source item exists and raise an error to stop processing if the Source does not exist.
                var kvNamespaceDictionary = element.GetNamespacesPrefixDictionary();
                
                XNamespace sourceNamespace = null;
                kvNamespaceDictionary.TryGetValue(sourceXmlnsPrefix, out sourceNamespace);

                if (sourceNamespace == null)
                {
                    throw new XmlException("Namespace alias process failed; reference to undeclared namespace prefix: {0}".FormatArgs(sourceXmlnsPrefix));
                }
                else
                {
                    //Now to correctly Alias the Namespaces (i.e. without String manipulation and unnecessary serialization)
                    //we start by first adding the Namespace delcaration and then implementing the namespace on all appropriate
                    //elements, thereby allowing the Xml engine to automatically prefix the elements correctly during serialization.
                    XNamespace existingTargetNamespace = null;
                    kvNamespaceDictionary.TryGetValue(targetXmlnsPrefix, out existingTargetNamespace);

                    XNamespace targetNamespace = XNamespace.Get(targetNamespaceName);
                    element.AddNamespaceDeclarationAttribute(targetXmlnsPrefix, targetNamespace);

                    //Now map all namespaces on elements, that are set to the source namespace, to the new Namespace so that they will automatically 
                    //implement the prefix as defined when the namespace was created above.
                    XNamespace currentNamespace = null;
                    foreach (var xEl in element.DescendantsAndSelf())
                    {
                        //Process the current element
                        //Note:  If an element contains the existing namespace for the Target prefix we merge it into the new
                        //       target so no nodes loose their prefix after the alias process is complete.
                        currentNamespace = xEl.GetNamespace();
                        if(currentNamespace.Equals(sourceNamespace) || currentNamespace.Equals(existingTargetNamespace))
                        {
                            xEl.SetNamespace(targetNamespace);
                        }

                        //Process all attributes of the current element
                        //Note:  We must co-erce the Linq request ToList() to allow manipulation of the values.
                        List<XAttribute> attributesList = xEl.Attributes().ToList();
                        xEl.Attributes().Remove();
                        foreach(var xAttr in attributesList)
                        {
                            //Note:  If an attribute contains the existing namespace for the Target prefix we merge it into the new
                            //       target so no nodes loose their prefix after the alias process is complete.
                            currentNamespace = xAttr.GetNamespace();
                            if(currentNamespace.Equals(sourceNamespace) || currentNamespace.Equals(existingTargetNamespace))
                            {
                                xEl.Add(new XAttribute(targetNamespace.GetName(xAttr.Name.LocalName), xAttr.Value));
                            }
                            else
                            {
                                xEl.Add(xAttr);
                            }
                        }
                    }

                    //Now we neeed to remove the previously defined Source namespace declarations
                    element.NamespaceDeclarationAttributes(true).Where(attr => attr.Value == sourceNamespace.NamespaceName || (existingTargetNamespace != null && attr.Value == existingTargetNamespace.NamespaceName)).Remove();
                }
            }
            return element;
        }


        public static XNamespace GetNamespaceOfPrefix(this XContainer container, string prefix)
        {
            return container.Document.Root.GetNamespaceOfPrefix(prefix);
        }

         public static IEnumerable<XElement> Elements(this XContainer container, string name, string prefix)
        {
            if (name == null) throw new Exception("To perform a retrieval you must provide a valid name.");
            if (prefix == null) throw new Exception("To perform a retrieval you must provide a valid prefix.");

            XNamespace nameSpace = container.GetNamespaceOfPrefix(prefix);
            return container.Elements(name, nameSpace);
        }

        public static IEnumerable<XElement> Elements(this XContainer container, string name, XNamespace nameSpace)
        {
            if (nameSpace == null) throw new Exception("To perform a retrieval with the Namspace filter a valid XNamespace must be provided.");
            return container.Elements(XName.Get(name, nameSpace.NamespaceName));
        }

        //BBernard - 12/17/2010
        //NOTE: Created a pass through for simplicity and consistency in naming and not writing unnecessary code by
        //      developers not familiar with Linq.
        public static IEnumerable<XElement> ElementsIgnoreCase(this XContainer container)
        {
            return container.Elements();
        }

        public static IEnumerable<XElement> ElementsIgnoreCase(this XContainer container, XName name)
        {
            //Provide Safe Return so that Chaining does not break.
            if (container == null) yield return null;

            foreach (XElement element in container.Elements())
            {
                if ((String.IsNullOrEmpty(name.NamespaceName) || element.Name.NamespaceName.Equals(name.NamespaceName, StringComparison.CurrentCultureIgnoreCase))
                    && element.Name.LocalName.Equals(name.LocalName, StringComparison.CurrentCultureIgnoreCase))
                {
                    yield return element;
                }
            }
        }

        public static IEnumerable<XElement> ElementsRemoveNamespaces(this XContainer container)
        {
            return container.ElementsRemoveNamespaces(null);
        }

        public static IEnumerable<XElement> ElementsRemoveNamespaces(this XContainer container, XName name)
        {
            //Provide Safe Return so that Chaining does not break.
            if (container == null) yield return null;

            foreach (XElement element in container.Elements())
            {
                //Return element
                if (name == null || element.Name.LocalName.Equals(name.LocalName, StringComparison.CurrentCultureIgnoreCase))
                {
                    yield return element.RemoveNamespaces(false);
                }
            }
        }

        public static IEnumerable<XElement> Descendants(this XContainer container, string name, string prefix)
        {
            if (name == null) throw new Exception("To perform a retrieval you must provide a valid name.");
            if (prefix == null) throw new Exception("To perform a retrieval you must provide a valid prefix.");
            XNamespace nameSpace = container.GetNamespaceOfPrefix(prefix);
            return container.Descendants(name, nameSpace);
        }

        public static IEnumerable<XElement> Descendants(this XContainer container, string name, XNamespace nameSpace)
        {
            if (nameSpace == null) throw new Exception("To perform a retrieval with the Namspace filter a valid XNamespace must be provided.");
            return container.Descendants(XName.Get(name, nameSpace.NamespaceName));
        }

        //Note:  Only XElements have DescendantsAndSelf() methods defined (as opposed to XContainer used in most cases).
        public static IEnumerable<XElement> DescendantsAndSelf(this XElement container, string name, string prefix)
        {
            XNamespace nameSpace = container.GetNamespaceOfPrefix(prefix);
            return container.DescendantsAndSelf(name, nameSpace);
        }

        //Note:  Only XElements have DescendantsAndSelf() methods defined (as opposed to XContainer used in most cases).
        public static IEnumerable<XElement> DescendantsAndSelf(this XElement container, string name, XNamespace nameSpace)
        {
            if (nameSpace == null) throw new Exception("To perform a retrieval with the Namspace filter a valid XNamespace must be provided.");
            return container.DescendantsAndSelf(XName.Get(name, nameSpace.NamespaceName));
        }

        public static XElement DescendantIgnoreCase(this XContainer container, XName name)
        {
            //Provide Safe Return so that Chaining does not break.
            if (container == null) return null;

            //Note:  By using the Enumeration we delay the processing and get maximum speed
            //       because the yield will be executed only once.
            foreach (XElement element in container.DescendantsIgnoreCase(name))
            {
                return element;
            }
            return null;
        }

        //BBernard - 12/17/2010
        //NOTE: Created a pass through for simplicity and consistency in naming and not writing unnecessary code by
        //      developers not familiar with Linq.
        public static IEnumerable<XElement> DescendantsIgnoreCase(this XContainer container)
        {
            return container.Descendants();
        }

        public static IEnumerable<XElement> DescendantsIgnoreCase(this XContainer container, XName name)
        {
            //Provide Safe Return so that Chaining does not break.
            if (container == null) yield return null;

            foreach (XElement element in container.Descendants())
            {
                if ((String.IsNullOrEmpty(name.NamespaceName) || element.Name.NamespaceName.Equals(name.NamespaceName, StringComparison.CurrentCultureIgnoreCase))
                    && element.Name.LocalName.Equals(name.LocalName, StringComparison.CurrentCultureIgnoreCase))
                {
                    yield return element;
                }
            }
        }

        //BBernard - 12/17/2010
        //NOTE: Allows Chaining of fetches without Failures due to no items found; minimal performance hit is maintained
        //      while providing much cleaner code via chained fetches.
        public static IEnumerable<XElement> DescendantsIgnoreCase(this IEnumerable<XElement> list, XName name)
        {
            //Provide Safe Return so that Chaining does not break.
            if (list == null) yield return null;

            foreach (XElement element in list)
            {
                foreach (XElement descendantElement in element.DescendantsIgnoreCase(name))
                {
                    yield return descendantElement;
                }
            }
        }

        public static IEnumerable<XElement> DescendantsRemoveNamespaces(this XContainer container)
        {
            return container.ElementsRemoveNamespaces(null);
        }

        public static IEnumerable<XElement> DescendantsRemoveNamespaces(this XContainer container, XName name)
        {
            //Provide Safe Return so that Chaining does not break.
            if (container == null) yield return null;

            foreach (XElement element in container.Descendants())
            {
                //Return element
                if (name == null || element.Name.LocalName.Equals(name.LocalName, StringComparison.CurrentCultureIgnoreCase))
                {
                    yield return element.RemoveNamespaces(false);
                }
            }
        }

        public static XAttribute Attribute(this XElement element, string name, string prefix)
        {
            //Provide Safe Return so that Chaining does not break.
            if (element == null) return null;

            XAttribute output = null;

            if (!String.IsNullOrEmpty(name) && !String.IsNullOrEmpty(prefix) && element.HasAttributes)
            {
                XNamespace nameSpace = element.GetNamespaceOfPrefix(prefix);
                output = Attribute(element, name, nameSpace);
            }

            return output;
        }

        public static XAttribute Attribute(this XElement element, string name, XNamespace nameSpace)
        {
            //Provide Safe Return so that Chaining does not break.
            if (element == null) return null;

            XAttribute output = null;

            if (!String.IsNullOrEmpty(name) && nameSpace != null && element.HasAttributes)
            {
                output = element.Attribute(XName.Get(name, nameSpace.NamespaceName));
            }

            return output;
        }

        public static XAttribute AttributeIgnoreCase(this IEnumerable<XElement> list, XName name)
        {
            //Provide Safe Return so that Chaining does not break.
            if (list == null) return null;

            foreach (XElement element in list)
            {
                XAttribute attribute = element.AttributeIgnoreCase(name);
                if (attribute != null)
                {
                    return attribute;
                }
            }
            return null;
        }

        public static XAttribute AttributeIgnoreCase(this XElement element, XName name)
        {
            //Provide Safe Return so that Chaining does not break.
            if (element == null) return null;

            foreach (XAttribute attribute in element.Attributes())
            {
                if ((String.IsNullOrEmpty(name.NamespaceName) || attribute.Name.NamespaceName.Equals(name.NamespaceName, StringComparison.CurrentCultureIgnoreCase))
                    && attribute.Name.LocalName.Equals(name.LocalName, StringComparison.CurrentCultureIgnoreCase))
                {
                    return attribute;
                }
            }
            return null;
        }

        public static IEnumerable<XAttribute> AttributesIgnoreCase(this XElement element, XName name)
        {
            //Provide Safe Return so that Chaining does not break.
            if (element == null) yield return null;

            foreach (XAttribute attr in element.Attributes())
            {
                if ((String.IsNullOrEmpty(name.NamespaceName) || attr.Name.NamespaceName.Equals(name.NamespaceName, StringComparison.CurrentCultureIgnoreCase))
                    && attr.Name.LocalName.Equals(name.LocalName, StringComparison.CurrentCultureIgnoreCase))
                {
                    yield return attr;
                }
            }
        }

        //BBernard - 12/17/2010
        //NOTE: Allows Chaining of fetches after a multiple Descendents Fetch without Failures due to no items found; minimal performance hit is maintained
        //      while providing much cleaner code via chained fetches.
        //NOTE: Since DescendantsIgnoreCase() can be run without a name its not necessary to create an AttributesIgnoreCase extension
        //      for an XContainer, only for IEnumerables...
        public static IEnumerable<XAttribute> AttributesIgnoreCase(this IEnumerable<XElement> list, XName name)
        {
            //Provide Safe Return so that Chaining does not break.
            if (list == null) yield return null;

            foreach (XElement element in list)
            {
                foreach (XAttribute descendantAttribute in element.AttributesIgnoreCase(name))
                {
                    yield return descendantAttribute;
                }
            }
        }

        public static bool ContainsAttributeLocalName(this XElement xEl, string strAttributeLocalName, StringComparison enumComparison)
        {
            return xEl.Attributes().Any(xAttr => xAttr.Name.LocalName.Equals(strAttributeLocalName, enumComparison));
        }

        public static bool ContainsAttribute(this XElement xEl, string strAttributeLocalName, StringComparison enumComparison)
        {
            return xEl.Attributes().Any(xAttr => xAttr.Name.NamespaceName.Equals(strAttributeLocalName, enumComparison));
        }

        public static string GetDelimitedStructureName(this XElement xEl)
        {
            return xEl.GetDelimitedStructureName(".", false);
        }

        public static string GetDelimitedStructureName(this XElement xEl, Func<XElement, String> fnGetNameFromElement)
        {
            return xEl.GetDelimitedStructureName(".", false, fnGetNameFromElement);
        }

        public static string GetDelimitedStructureName(this XElement xEl, Func<XElement, String> fnGetNameFromElement, bool bIncludeRoot)
        {
            return xEl.GetDelimitedStructureName(".", bIncludeRoot, fnGetNameFromElement);
        }

        public static string GetDelimitedStructureName(this XElement xEl, string strDelimiter, bool bIncludeRoot)
        {
            return xEl.GetDelimitedStructureName(strDelimiter, bIncludeRoot, null);
        }

        public static string GetDelimitedStructureName(this XElement xEl, string strDelimiter, bool bIncludeRoot, Func<XElement, String> fnGetNameFromElement)
        {
            string strTreeName;
            string strElementNameToUse = String.Empty;

            //If we have Callback Delegate for processing the Key name, then use it
            if (fnGetNameFromElement != null)
            {
                strElementNameToUse = fnGetNameFromElement(xEl);
            }

            //Ensure we always have a fallback for the Element name
            //Note:  we will use the Element name by default if no delegate function was specified
            //       in addition to when that function returns null/empty result.
            if (String.IsNullOrEmpty(strElementNameToUse))
            {
                strElementNameToUse = xEl.Name.LocalName;
            }

            //Recursively Climb the Structure to build the Delimited Name!
            if (xEl.Parent != null)
            {
                //If Recursion returns a Value then Add the Delimiter, otherwise do not
                strTreeName = GetDelimitedStructureName(xEl.Parent, strDelimiter, bIncludeRoot, fnGetNameFromElement);
                return (String.IsNullOrEmpty(strTreeName) ? "" : strTreeName + strDelimiter) + strElementNameToUse;
            }

            //Only return the Root if specified, otherwise return Empty
            XElement xRootEl = xEl.Document != null ? xEl.Document.Root : xEl;
            return (bIncludeRoot && xRootEl.Name == xEl.Name) ? strElementNameToUse : String.Empty;
        }

        public static string GetInnerXml(this XElement xEl)
        {
            XmlReader xmlReader = xEl.CreateReader();
            xmlReader.MoveToContent();
            return xmlReader.ReadInnerXml();
        }

        public static string GetInnerText(this XElement xEl)
        {
            return xEl.Value;
        }

        public static IEnumerable<XElement> RemoveWhere(this IEnumerable<XElement> list, Func<XElement, bool> predicate)
        {
            list.Where(predicate).Remove();
            return list;
        }

        public static XContainer RemoveElementsWhere(this XContainer container, Func<XElement, bool> predicate)
        {
            container.ElementsIgnoreCase().Where(predicate).Remove();
            return container;
        }

        public static XContainer RemoveElementsWhere(this XContainer container, XName name, Func<XElement, bool> predicate)
        {
            container.ElementsIgnoreCase(name).Where(predicate).Remove();
            return container;
        }

        public static XContainer RemoveDescendantsWhere(this XContainer container, Func<XElement, bool> predicate)
        {
            container.DescendantsIgnoreCase().Where(predicate).Remove();
            return container;
        }

        public static XContainer RemoveDescendantsWhere(this XContainer container, XName name, Func<XElement, bool> predicate)
        {
            container.DescendantsIgnoreCase(name).Where(predicate).Remove();
            return container;
        }
    }

    /// <summary>
    /// Provides extension methods for simple conversion between Dictionary and System.Xml.Linq classes.
    /// </summary>
    public static class XmlLinqDictionaryConversionExtenstions
    {
        private class XElementToDictionary
        {
            public string DelimitedStructureName { get; set; }
            public int Count { get; set; }
            public int CurrentIncrementValue { get; set; }
        }

        public static Dictionary<string, string> ToDictionary(this XElement xElement)
        {
            return xElement.ToDictionary("", "[{0}]", "[{0}]");
        }

        public static Dictionary<string, string> ToDictionary(this XElement xElement, string strNamespacePrefix)
        {
            return xElement.ToDictionary(strNamespacePrefix, "[{0}]", "[{0}]");
        }

        public static Dictionary<string, string> ToDictionary(this XElement xElement, string strNamespacePrefix, string strDuplicateCounterFormatString, string strAttributeNameFormatString)
        {
            return xElement.ToDictionary(strNamespacePrefix, strDuplicateCounterFormatString, strAttributeNameFormatString, null);
        }

        public static Dictionary<string, string> ToDictionary(this XElement xElement, string strNamespacePrefix, string strDuplicateCounterFormatString, string strAttributeNameFormatString, Func<XElement, String> fnGetKeyNameFromElement)
        {
            return xElement.ToDictionary(strNamespacePrefix, strDuplicateCounterFormatString, strAttributeNameFormatString, fnGetKeyNameFromElement, false);
        }

        public static Dictionary<string, string> ToDictionary(this XElement xElement, string strNamespacePrefix, string strDuplicateCounterFormatString, string strAttributeNameFormatString, Func<XElement, String> fnGetKeyNameFromElement, bool bIncludeRootNode)
        {
            Dictionary<string, string> kvPairs = new Dictionary<string, string>();
            Dictionary<string, XElementToDictionary> kvConvertDataByName = new Dictionary<string, XElementToDictionary>();
            XElementToDictionary objConvertData;
            string strName = String.Empty;
            string strFinalName = String.Empty;
            string strTemp = String.Empty;

            //Get a Flattened list of all Elements that contain only a value excluding elements that are simply containers for other elements.
            //Note:  We only need to worry about elements that are bottom level nodes with values and Not sub-elements.
            var lstElements = from xEl in xElement.DescendantsAndSelf()
                              where xEl.HasAttributes == true || xEl.HasElements == false
                              select xEl;

            //Determine all Key Names and the number of times it appears
            foreach (XElement xEl in lstElements)
            {
                strName = xEl.GetDelimitedStructureName(fnGetKeyNameFromElement, bIncludeRootNode);
                xEl.Add(new XAttribute("DelimitedStructureBaseNameForDictionaryEntry", strName));

                if (kvConvertDataByName.ContainsKey(strName))
                {
                    kvConvertDataByName[strName].Count++;
                }
                else
                {
                    kvConvertDataByName.Add(strName, new XElementToDictionary
                    {
                        DelimitedStructureName = strName,
                        Count = 0,
                        CurrentIncrementValue = 0
                    });
                }
            }

            //Now Process All elements correctly
            //Note:  All elements that appear more than once will have an incrementer appended to ensure uniqueness!
            foreach (XElement xEl in lstElements)
            {
                //Retrieve the Name
                //Note:  We cached it as an attribute of the Element so it doesn't have to be re-constructed
                strName = xEl.Attribute("DelimitedStructureBaseNameForDictionaryEntry").Value; //xEl.GetDelimitedStructureName();
                objConvertData = kvConvertDataByName[strName];

                //Adjust the Name for Uniqueness
                if (objConvertData.Count > 0)
                {
                    strName = String.Format("{1}" + strDuplicateCounterFormatString, objConvertData.CurrentIncrementValue, objConvertData.DelimitedStructureName);
                    objConvertData.CurrentIncrementValue++;
                }
                else
                {
                    strName = objConvertData.DelimitedStructureName;
                }

                //If a specific namespace prefix was specified then implement it here before adding to the Final Collection
                strNamespacePrefix = strNamespacePrefix.Trim(".".ToCharArray());
                strFinalName = String.IsNullOrEmpty(strNamespacePrefix) ? strName : String.Format("{0}.{1}", strNamespacePrefix, strName);

                //Now ensure that our FinalName is clean and has no extraneous delimiters (ie. if Name was blank we would have a trailing ".")
                strFinalName = strFinalName.Trim(".".ToCharArray());

                //Add the Element Value to the Dictionary only if it is a Leaf Element!
                //Note:  This is because the Value will be for ALL concatenated text of all child leaf elements, but since
                //       they will be serialized independently, we don't want the combined value added.
                if(!xEl.HasElements) kvPairs.Add(strFinalName, xEl.Value);

                //FOR Any Element (ie Leaf Elements or Branch Nodes) we always process the Attributes.
                //Get All valid Attributes of this Element and Add as denoted Sub-Items of the Dictionary (ie. Array notation).
                foreach (XAttribute xAttr in xEl.Attributes().Where(a => a.Name != "DelimitedStructureBaseNameForDictionaryEntry"))
                {
                    strTemp = String.Format("{1}" + strAttributeNameFormatString, xAttr.Name.LocalName, strFinalName);
                    kvPairs.Add(strTemp, xAttr.Value);
                }
            }

            return kvPairs;
        }


    }

    /// <summary>
    /// Provides extension methods for simple conversion between Streams and System.Xml.Linq classes.
    /// </summary>
    public static class XmlLinqStreamConversionExtensions
    {
        /// <summary>
        /// Converts an XDocument to a MemoryStream.
        /// </summary>
        /// <param name="xdoc">The XDocument to convert.</param>
        /// <returns>The equivalent XmlDocument.</returns>
        public static MemoryStream ToStream(this XDocument xdoc)
        {
            if (xdoc == null) throw new ArgumentNullException("XDocument object is null; Extension method XDocument.ToStream() cannot convert a null document to a valid stream.");
            return xdoc.Root.ToStream();
        }

        /// <summary>
        /// Converts an XElement to an MemoryStream.
        /// </summary>
        /// <param name="xmlelement">The XmlElement to convert.</param>
        /// <returns>The equivalent XElement.</returns>
        public static MemoryStream ToStream(this XElement xelement)
        {
            //Initialize Writer Settings to write to Memory
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
            {
                OmitXmlDeclaration = true,
                ConformanceLevel = ConformanceLevel.Fragment,
                CloseOutput = false,
                Encoding = ASCIIEncoding.UTF8
            };

            //Create the XmlWriter object
            MemoryStream memoryStream = new MemoryStream();
            using (XmlWriter xmlWriter = XmlTextWriter.Create(memoryStream, xmlWriterSettings))
            {
                //Write our Content to our Stream using the XmlWriter mediator
                xelement.WriteTo(xmlWriter);
                xmlWriter.Flush();
                xmlWriter.Close();
            }

            //Debugging code to see final output of data written to the Stream...
            //memoryStream.Position = 0;
            //StreamReader streamReader = new StreamReader(memoryStream);
            //String strOutput = streamReader.ReadToEnd();

            //Finalize and return the Stream prepared for Reading by re-setting back to Position 0.
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }

        public static XmlReader ToXmlReader(this Stream stream)
        {
            //Pass default XmlReader Settings
            //NOTE:  We do NOT actually want to close the stream by default so as to prevent unexpected behaviour, 
            //       but we must specify not to because the XmlReader will by default.
            return stream.ToXmlReader(new XmlReaderSettings()
            {
                CloseInput = false
            });
        }

        public static XmlReader ToXmlReader(this Stream stream, XmlReaderSettings xmlReaderSettings)
        {
            //Always reset the Stream to ensure we read from the Beginning because XmlTextReaders are Forward only!
            stream.Reset();
            return XmlReader.Create(stream, xmlReaderSettings);
        }

        public static XElement ToXElement(this Stream stream)
        {
            XElement xEl = null;
            //Ensure that our Stream is ready to read
            stream.Reset();

            //Read the stream into our XElement
            using (XmlReader reader = stream.ToXmlReader())
            {
                xEl = XElement.Load(reader);
            }
            return xEl;
        }

        public static XElement ToXElement(this Stream stream, XmlReaderSettings xmlReaderSettings)
        {
            XElement xEl = null;
            //Ensure that our Stream is ready to read
            stream.Reset();

            //Read the stream into our XElement
            using (XmlReader reader = stream.ToXmlReader(xmlReaderSettings))
            {
                xEl = XElement.Load(reader);
            }
            return xEl;
        }

        public static XDocument ToXDocument(this Stream stream)
        {
            XDocument xDoc = null;

            //Note:  The handling of the Stream position is handled by ToXmlReader() method.
            //Ensure that our Stream is ready to read
            //stream.Reset();

            //Read the stream into our XDocument
            using (XmlReader reader = stream.ToXmlReader())
            {
                xDoc = XDocument.Load(reader);
            }
            return xDoc;
        }

        public static XDocument ToXDocument(this Stream stream, XmlReaderSettings xmlReaderSettings)
        {
            XDocument xDoc = null;
            //Ensure that our Stream is ready to read
            stream.Reset();

            //Read the stream into our XDocument
            using (XmlReader reader = stream.ToXmlReader(xmlReaderSettings))
            {
                xDoc = XDocument.Load(reader);
            }
            return xDoc;
        }
    }

    /// <summary>
    /// Provides extension methods for simple conversion between System.Xml and System.Xml.Linq classes.
    /// </summary>
    public static class XmlLinqLegacyXmlConversionExtensions
    {
            /// <summary>
        /// Converts an XmlReader to an XDocument in a way that is chainable with Linq.
        /// </summary>
        /// <param name="xmlReader"></param>
        /// <returns></returns>
        public static XDocument ToXDocument(this XmlReader xmlReader)
        {
            return XDocument.Load(xmlReader);
        }

        /// <summary>
        /// Converts an XmlReader to an XElement in a way that is chainable with Linq.
        /// </summary>
        /// <param name="xmlReader"></param>
        /// <returns></returns>
        public static XElement ToXElement(this XmlReader xmlReader)
        {
            return XElement.Load(xmlReader);
        }

        /// <summary>
        /// Converts an XPathNavigator to an XDocument with Optimized in memory conversion (No Re-Parsing or Re-Serialization).
        /// </summary>
        /// <param name="xpathNavigator">The XPathNavigator to convert.</param>
        /// <returns>The equivalent XDocument.</returns>
        public static XDocument ToXDocument(this XPathNavigator xpathNavigator)
        {
            return xpathNavigator.ToXDocument(true);
        }

        /// <summary>
        /// Converts an XPathNavigator to an XElement with Optimized in memory conversion (No Re-Parsing or Re-Serialization).
        /// </summary>
        /// <param name="xpathNavigator">The XPathNavigator to convert.</param>
        /// <returns>The equivalent XElement.</returns>
        public static XElement ToXElement(this XPathNavigator xpathNavigator)
        {
            return xpathNavigator.ToXElement(true);
        }

        /// <summary>
        /// Converts an XPathNavigator to an XDocument with Optimized in memory conversion (No Re-Parsing or Re-Serialization).
        /// Supports parameter to force a Move to the Root Element (Defaults to True with overloaded methods 
        /// that do not require this param for full legacy/backwards compatibility Support).
        /// </summary>
        /// <param name="xpathNavigator">The XPathNavigator to convert.</param>
        /// <returns>The equivalent XDocument.</returns>
        public static XDocument ToXDocument(this XPathNavigator xpathNavigator, bool bMoveToRoot)
        {
            if(bMoveToRoot) xpathNavigator.MoveToRoot();
            return XDocument.Load(xpathNavigator.ReadSubtree());
        }

        /// <summary>
        /// Converts an XPathNavigator to an XElement with Optimized in memory conversion (No Re-Parsing or Re-Serialization).
        /// Supports parameter to force a Move to the Root Element (Defaults to True with overloaded methods
        /// that do not require this param for full legacy/backwards compatibility Support).
        /// </summary>
        /// <param name="xpathNavigator">The XPathNavigator to convert.</param>
        /// <returns>The equivalent XElement.</returns>
        public static XElement ToXElement(this XPathNavigator xpathNavigator, bool bMoveToRoot)
        {
            if (bMoveToRoot) xpathNavigator.MoveToRoot();
            return XElement.Load(xpathNavigator.ReadSubtree());
        }
        /// <summary>
        /// Converts an XmlDocument to an XDocument with Optimized in memory conversion (No Re-Parsing or Re-Serialization)..
        /// </summary>
        /// <param name="xmldoc">The XmlDocument to convert.</param>
        /// <returns>The equivalent XDocument.</returns>
        public static XDocument ToXDocument(this XmlDocument xmldoc)
        {
            return XDocument.Load(xmldoc.CreateNavigator().ReadSubtree());
        }

        /// <summary>
        /// Converts an XElement to an XDocument with Optimized in memory conversion (No Re-Parsing or Re-Serialization)..
        /// </summary>
        /// <param name="xelement">The XElement to convert.</param>
        /// <returns>The equivalent XDocument.</returns>
        public static XDocument ToXDocument(this XElement xelement)
        {
            return XDocument.Load(xelement.CreateReader());
        }

        /// <summary>
        /// Converts an XDocument to an XmlDocument with Optimized in memory conversion (No Re-Parsing or Re-Serialization)..
        /// </summary>
        /// <param name="xdoc">The XDocument to convert.</param>
        /// <returns>The equivalent XmlDocument.</returns>
        public static XmlDocument ToXmlDocument(this XDocument xdoc)
        {
            var xmldoc = new XmlDocument();
            xmldoc.Load(xdoc.CreateReader());
            return xmldoc;
        }

        /// <summary>
        /// Converts an XElement to an XmlElement with Optimized in memory conversion (No Re-Parsing or Re-Serialization).
        /// </summary>
        /// <param name="xelement">The XElement to convert.</param>
        /// <returns>The equivalent XmlElement.</returns>
        public static XmlElement ToXmlElement(this XElement xelement)
        {
            return new XmlDocument().ReadNode(xelement.CreateReader()) as XmlElement;
        }

        /// <summary>
        /// Converts an XmlElement to an XElement with Optimized in memory conversion (No Re-Parsing or Re-Serialization).
        /// </summary>
        /// <param name="xmlelement">The XmlElement to convert.</param>
        /// <returns>The equivalent XElement.</returns>
        public static XElement ToXElement(this XmlElement xmlelement)
        {
            return xmlelement.CreateNavigator().ToXElement();
        }

    }

    /// <summary>
    /// Provides Serialization/De-Serialization of Objects to/from Xml Strings for optimized processing with LINQ2Xml.  
    /// Also supports some handling for Legacy Xml Document object via overloads.
    /// </summary>
    public static class XmlLinqSerializationCustomExtensions
    {
        #region De-Serialization Extensions

        /// <summary>
        /// Deserialize Xml into the specificied object type by the Generics Type T.
        /// Provides error handling, stream manipulation and cleanup, and Xml Parsing optimizations when possible.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static T Deserialize<T>(this XmlDocument xml)
        {
            if (xml == null)
            {
                return default(T);
            }
            return Deserialize<T>(xml.OuterXml);
        }

        /// <summary>
        /// Deserialize Xml into the specificied object type by the Generics Type T.
        /// Provides error handling, stream manipulation and cleanup, and Xml Parsing optimizations when possible.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static T Deserialize<T>(this XDocument xml)
        {
            if (xml == null || xml.Root == null)
            {
                return default(T);
            }
            return Deserialize<T>(xml.Root.ToString());
        }

        /// <summary>
        /// Deserialize Xml into the specificied object type by the Generics Type T.
        /// Provides error handling, stream manipulation and cleanup, and Xml Parsing optimizations when possible.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static T Deserialize<T>(this XElement xml)
        {
            return Deserialize<T>(xml.ToString());
        }

        /// <summary>
        /// Deserialize Xml into the specificied object type by the Generics Type T.
        /// Provides error handling, stream manipulation and cleanup, and Xml Parsing optimizations when possible.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static T Deserialize<T>(this string xml)
        {
            if (!xml.IsNullOrEmpty())
            {
                Type type = typeof(T);
                XmlSerializer serializer = new XmlSerializer(type);
                byte[] buffer = xml.GetBytes(Encoding.UTF8);

                using (MemoryStream stream = new MemoryStream(buffer))
                using (XmlReader reader = stream.ToXmlReader())
                {
                    return (T)serializer.Deserialize(reader);
                }
            }

            return default(T);
        }

        #endregion

        #region Serialization Extensions

        /// <summary>
        /// Serializes an object into an Xml Document ready for processing with LINQ2Xml
        /// Provides error handling, stream manipulation and cleanup, and Xml Parsing optimizations when possible.
        /// </summary>
        /// <param name="o">The object to serialize</param>
        /// <returns>An Xml Document consisting of said object's data</returns>
        public static XDocument Serialize(this object o)
        {
            return o.Serialize(LoadOptions.None);
        }

        /// <summary>
        /// Serializes an object into an Xml Document ready for processing with LINQ2Xml
        /// Provides error handling, stream manipulation and cleanup, and Xml Parsing optimizations when possible.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="loadOptions"></param>
        /// <returns></returns>
        public static XDocument Serialize(this object obj, LoadOptions loadOptions)
        {
            return obj.Serialize(loadOptions, null, null, null);
        }

        /// <summary>
        /// Serializes an object into an Xml Document ready for processing with LINQ2Xml
        /// Provides error handling, stream manipulation and cleanup, and Xml Parsing optimizations when possible.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="loadOptions"></param>
        /// <param name="xmlRootSettings"></param>
        /// <returns></returns>
        public static XDocument Serialize(this object obj, LoadOptions loadOptions, XmlRootAttribute xmlRootSettings)
        {
            return obj.Serialize(loadOptions, xmlRootSettings, null, null);
        }

        /// <summary>
        /// Serializes an object into an Xml Document ready for processing with LINQ2Xml
        /// Provides error handling, stream manipulation and cleanup, and Xml Parsing optimizations when possible.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="loadOptions"></param>
        /// <param name="xmlRootSettings">Settings for the Root; can be null.</param>
        /// <param name="xmlOverrideSettings">Settings to override serialization behavior; can be null.</param>
        /// <param name="xmlNamespaces">Settigns to override namespaces; can be null to result in no namespaces.</param>
        /// <returns></returns>
        public static XDocument Serialize(this object obj, LoadOptions loadOptions, XmlRootAttribute xmlRootSettings, XmlAttributeOverrides xmlOverrideSettings, XmlSerializerNamespaces xmlNamespaces)
        {
            if (obj != null)
            {
                XmlSerializer serializer = new XmlSerializer(obj.GetType(), xmlOverrideSettings, null, xmlRootSettings, null);

                using (MemoryStream stream = new MemoryStream())
                using (XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8))
                {
                    writer.Formatting = Formatting.Indented;
                    writer.IndentChar = '\t'; //' ';
                    writer.Indentation = 1; //5;

                    //Init default No Namespaces collection
                    var defaultNoNamespaces = new XmlSerializerNamespaces().With(ns => 
                    {
                        ns.Add(String.Empty, String.Empty);
                    });

                    serializer.Serialize(writer, obj, xmlNamespaces ?? defaultNoNamespaces);
                    string xmlString = stream.ReadString(Encoding.UTF8);
                    XDocument xDoc = XDocument.Parse(xmlString, loadOptions);

                    return xDoc;
                }
            }
            return null;
        }

        #endregion
    }

}
