using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;

namespace Fonet.Fo
{
    internal sealed class PropertyListBuilder
    {
        private const string FONTSIZEATTR = "font-size";

        private Hashtable propertyListTable = new Hashtable();

        internal PropertyListBuilder() { }

        internal void AddList(Hashtable list)
        {
            foreach (object o in list.Keys)
            {
                propertyListTable.Add(o, list[o]);
            }
        }

        internal PropertyList MakeList(
            string ns,
            string elementName,
            Attributes attributes,
            FObj parentFO)
        {
            Debug.Assert(ns != null, "Namespace should never be null.");

            string space = "http://www.w3.org/TR/1999/XSL/Format";
            if (ns != null)
            {
                space = ns;
            }

            PropertyList parentPropertyList = parentFO != null ? parentFO.properties : null;
            PropertyList par = null;
            if (parentPropertyList != null && space.Equals(parentPropertyList.GetNameSpace()))
            {
                par = parentPropertyList;
            }

            PropertyList p = new PropertyList(par, space, elementName);
            p.SetBuilder(this);

            StringCollection propsDone = new StringCollection();

            string fontsizeval = attributes.getValue(FONTSIZEATTR);
            if (fontsizeval != null)
            {
                PropertyMaker propertyMaker = FindMaker(FONTSIZEATTR);
                if (propertyMaker != null)
                {
                    try
                    {
                        p.Add(FONTSIZEATTR,
                              propertyMaker.Make(p, fontsizeval, parentFO));
                    }
                    catch (FonetException) { }
                }
                propsDone.Add(FONTSIZEATTR);
            }

            for (int i = 0; i < attributes.getLength(); i++)
            {
                string attributeName = attributes.getQName(i);
                int sepchar = attributeName.IndexOf('.');
                string propName = attributeName;
                string subpropName = null;
                Property propVal = null;
                if (sepchar > -1)
                {
                    propName = attributeName.Substring(0, sepchar);
                    subpropName = attributeName.Substring(sepchar + 1);
                }
                else if (propsDone.Contains(propName))
                {
                    continue;
                }

                PropertyMaker propertyMaker = FindMaker(propName);

                if (propertyMaker != null)
                {
                    try
                    {
                        if (subpropName != null)
                        {
                            Property baseProp = p.GetExplicitBaseProperty(propName);
                            if (baseProp == null)
                            {
                                string baseValue = attributes.getValue(propName);
                                if (baseValue != null)
                                {
                                    baseProp = propertyMaker.Make(p, baseValue, parentFO);
                                    propsDone.Add(propName);
                                }
                            }
                            propVal = propertyMaker.Make(baseProp, subpropName,
                                                         p,
                                                         attributes.getValue(i),
                                                         parentFO);
                        }
                        else
                        {
                            propVal = propertyMaker.Make(p,
                                                         attributes.getValue(i),
                                                         parentFO);
                        }
                        if (propVal != null)
                        {
                            p[propName] = propVal;
                        }
                    }
                    catch (FonetException e)
                    {
                        FonetDriver.ActiveDriver.FireFonetError(e.Message);
                    }
                }
                else
                {
                    if (!attributeName.StartsWith("xmlns"))
                    {
                        FonetDriver.ActiveDriver.FireFonetWarning(
                            "property " + attributeName + " ignored");
                    }
                }
            }

            return p;
        }

        internal Property GetSubpropValue(
            string propertyName, Property p, string subpropName)
        {
            PropertyMaker maker = FindMaker(propertyName);
            if (maker != null)
            {
                return maker.GetSubpropValue(p, subpropName);
            }
            else
            {
                return null;
            }
        }

        internal Property GetShorthand(PropertyList propertyList, string propertyName)
        {
            PropertyMaker propertyMaker = FindMaker(propertyName);
            if (propertyMaker != null)
            {
                return propertyMaker.GetShorthand(propertyList);
            }
            else
            {
                FonetDriver.ActiveDriver.FireFonetError("No maker for " + propertyName);
                return null;
            }
        }

        internal Property MakeProperty(PropertyList propertyList, string propertyName)
        {
            Property p = null;
            PropertyMaker propertyMaker = FindMaker(propertyName);
            if (propertyMaker != null)
            {
                p = propertyMaker.Make(propertyList);
            }
            else
            {
                FonetDriver.ActiveDriver.FireFonetWarning("property " + propertyName + " ignored");
            }
            return p;
        }

        internal PropertyMaker FindMaker(string propertyName)
        {
            return (PropertyMaker)propertyListTable[propertyName];
        }
    }
}