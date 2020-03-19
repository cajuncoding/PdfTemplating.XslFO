using System;
using Fonet.DataTypes;
using Fonet.Fo.Expr;

namespace Fonet.Fo
{
    internal class PropertyMaker
    {
        private const string UNKNOWN = "UNKNOWN";

        private string propName;

        /// <summary>
        ///     Return the name of the property whose value is being set.
        /// </summary>
        protected string PropName
        {
            get { return propName; }
        }

        /// <summary>
        ///     Construct an instance of a PropertyMaker.
        /// </summary>
        /// <remarks>
        ///     The property name is set to "UNKNOWN".
        /// </remarks>
        protected PropertyMaker()
        {
            this.propName = UNKNOWN;
        }

        /// <summary>
        ///     Construct an instance of a PropertyMaker for the given property.
        /// </summary>
        /// <param name="propName">The name of the property to be made.</param>
        protected PropertyMaker(string propName)
        {
            this.propName = propName;
        }

        /// <summary>
        ///     Default implementation of isInherited.
        /// </summary>
        /// <returns>A boolean indicating whether this property is inherited.</returns>
        public virtual bool IsInherited()
        {
            return false;
        }

        /// <summary>
        ///     Return a boolean indicating whether this property inherits the
        ///     "specified" value rather than the "computed" value. The default is 
        ///     to inherit the "computed" value.
        /// </summary>
        /// <returns>If true, property inherits the value specified.</returns>
        public virtual bool InheritsSpecified()
        {
            return false;
        }

        /// <summary>
        ///     Return an object implementing the PercentBase interface.  This is 
        ///     used to handle properties specified as a percentage of some "base 
        ///     length", such as the content width of their containing box.  
        ///     Overridden by subclasses which allow percent specifications. See
        ///     the documentation on properties.xsl for details.
        /// </summary>
        /// <param name="fo"></param>
        /// <param name="pl"></param>
        /// <returns></returns>
        public virtual IPercentBase GetPercentBase(FObj fo, PropertyList pl)
        {
            return null;
        }

        /// <summary>
        ///     Return a Maker object which is used to set the values on components 
        ///     of compound property types, such as "space".  Overridden by property 
        ///     maker subclasses which handle compound properties.
        /// </summary>
        /// <param name="subprop">
        ///     The name of the component for which a Maker is to returned, for 
        ///     example "optimum", if the FO attribute is space.optimum='10pt'.
        /// </param>
        /// <returns></returns>
        protected virtual PropertyMaker GetSubpropMaker(string subprop)
        {
            return null;
        }

        /// <summary>
        ///     Return a property value for the given component of a compound 
        ///     property.
        /// </summary>
        /// <remarks>
        ///     NOTE: this is only to ease porting when calls are made to 
        ///     PropertyList.get() using a component name of a compound property,
        ///     such as get("space.optimum"). 
        ///     The recommended technique is: get("space").getOptimum().
        ///     Overridden by property maker subclasses which handle compound properties.
        /// </remarks>
        /// <param name="p">A property value for a compound property type such as SpaceProperty.</param>
        /// <param name="subprop">The name of the component whose value is to be returned.</param>
        /// <returns></returns>
        public virtual Property GetSubpropValue(Property p, string subprop)
        {
            return null;
        }

        /// <summary>
        ///     Return a property value for a compound property. If the property
        ///     value is already partially initialized, this method will modify it.
        /// </summary>
        /// <param name="baseProp">
        ///     The Property object representing the compound property, such as 
        ///     SpaceProperty.
        /// </param>
        /// <param name="partName">The name of the component whose value is specified.</param>
        /// <param name="propertyList">The propertyList being built.</param>
        /// <param name="value"></param>
        /// <param name="fo">The FO whose properties are being set.</param>
        /// <returns>A compound property object.</returns>
        public Property Make(Property baseProp, string partName,
                             PropertyList propertyList, string value,
                             FObj fo)
        {
            if (baseProp == null)
            {
                baseProp = MakeCompound(propertyList, fo);
            }
            PropertyMaker spMaker = GetSubpropMaker(partName);
            if (spMaker != null)
            {
                Property p = spMaker.Make(propertyList, value, fo);
                if (p != null)
                {
                    return SetSubprop(baseProp, partName, p);
                }
            }
            return baseProp;
        }

        /// <summary>
        ///     Set a component in a compound property and return the modified
        ///     compound property object.  This default implementation returns 
        ///     the original base property without modifying it.  It is overridden 
        ///     by property maker subclasses which handle compound properties.
        /// </summary>
        /// <param name="baseProp">
        ///     The Property object representing the compound property, such as SpaceProperty.
        /// </param>
        /// <param name="partName">The name of the component whose value is specified.</param>
        /// <param name="subProp">
        ///     A Property object holding the specified value of the component to be set.
        /// </param>
        /// <returns>The modified compound property object.</returns>
        protected virtual Property SetSubprop(Property baseProp, string partName, Property subProp)
        {
            return baseProp;
        }

        /// <summary>
        ///     Create a Property object from an attribute specification.
        /// </summary>
        /// <param name="propertyList">The PropertyList object being built for this FO.</param>
        /// <param name="value">The attribute value.</param>
        /// <param name="fo">The current FO whose properties are being set.</param>
        /// <returns>The initialized Property object.</returns>
        public virtual Property Make(PropertyList propertyList, string value, FObj fo)
        {
            try
            {
                Property pret = null;
                string pvalue = value;
                pret = CheckEnumValues(value);
                if (pret == null)
                {
                    pvalue = CheckValueKeywords(value);
                    Property p = PropertyParser.parse(pvalue,
                                                      new PropertyInfo(this,
                                                                       propertyList, fo));
                    pret = ConvertProperty(p, propertyList, fo);
                }
                else if (IsCompoundMaker())
                {
                    pret = ConvertProperty(pret, propertyList, fo);
                }
                if (pret == null)
                {
                    throw new PropertyException("No conversion defined");
                }
                else if (InheritsSpecified())
                {
                    pret.SpecifiedValue = pvalue;
                }
                return pret;
            }
            catch (PropertyException propEx)
            {
                throw new FonetException("Error in " + propName + " property value '" + value + "': " + propEx.Message);
            }
        }

        public Property ConvertShorthandProperty(
            PropertyList propertyList, Property prop, FObj fo)
        {
            Property pret = null;
            try
            {
                pret = ConvertProperty(prop, propertyList, fo);
                if (pret == null)
                {
                    string sval = prop.GetNCname();
                    if (sval != null)
                    {
                        pret = CheckEnumValues(sval);
                        if (pret == null)
                        {
                            String pvalue = CheckValueKeywords(sval);
                            if (!pvalue.Equals(sval))
                            {
                                Property p =
                                    PropertyParser.parse(pvalue,
                                                         new PropertyInfo(this,
                                                                          propertyList,
                                                                          fo));
                                pret = ConvertProperty(p, propertyList, fo);
                            }
                        }
                    }
                }
            }
            catch (FonetException)
            {
            }
            catch (PropertyException)
            {
            }
            if (pret != null)
            {
            }
            return pret;
        }

        protected virtual bool IsCompoundMaker()
        {
            return false;
        }

        public virtual Property CheckEnumValues(string value)
        {
            return null;
        }

        /// <summary>
        ///     Return a String to be parsed if the passed value corresponds to
        ///     a keyword which can be parsed and used to initialize the property.
        ///     For example, the border-width family of properties can have the
        ///     initializers "thin", "medium", or "thick". The foproperties.xml
        ///     file specifies a length value equivalent for these keywords,
        ///     such as "0.5pt" for "thin". These values are considered parseable,
        ///     since the Length object is no longer responsible for parsing
        ///     unit expresssions.
        /// </summary>
        /// <param name="value">The string value of property attribute.</param>
        /// <returns>
        ///     A string containging a parseable equivalent or null if the passed 
        ///     value isn't a keyword initializer for this Property.
        /// </returns>
        protected virtual string CheckValueKeywords(string value)
        {
            return value;
        }

        /// <summary>
        ///     Return a Property object based on the passed Property object.
        ///     This method is called if the Property object built by the parser
        ///     isn't the right type for this property.
        ///     It is overridden by subclasses when the property specification in
        ///     foproperties.xml specifies conversion rules.
        /// </summary>
        /// <param name="p">The Property object return by the expression parser</param>
        /// <param name="propertyList">The PropertyList object being built for this FO.</param>
        /// <param name="fo">The current FO whose properties are being set.</param>
        /// <returns>
        ///     A Property of the correct type or null if the parsed value
        ///     can't be converted to the correct type.
        /// </returns>
        public virtual Property ConvertProperty(Property p,
                                                PropertyList propertyList,
                                                FObj fo)
        {
            return null;
        }

        protected virtual Property ConvertPropertyDatatype(Property p,
                                                           PropertyList propertyList,
                                                           FObj fo)
        {
            return null;
        }

        /// <summary>
        ///     Return a Property object representing the initial value.
        /// </summary>
        /// <param name="propertyList">The PropertyList object being built for this FO.</param>
        /// <returns></returns>
        public virtual Property Make(PropertyList propertyList)
        {
            return null;
        }

        /// <summary>
        ///     Return a Property object representing the initial value.
        /// </summary>
        /// <param name="propertyList">The PropertyList object being built for this FO.</param>
        /// <param name="parentFO">The parent FO for the FO whose property is being made.</param>
        /// <returns>
        ///     A Property subclass object holding a "compound" property object
        ///     initialized to the default values for each component.
        /// </returns>
        protected virtual Property MakeCompound(PropertyList propertyList,
                                                FObj parentFO)
        {
            return null;
        }

        /// <summary>
        ///     Return a Property object representing the value of this property,
        ///     based on other property values for this FO.
        ///     A special case is properties which inherit the specified value,
        ///     rather than the computed value.
        /// </summary>
        /// <param name="propertyList">The PropertyList for the FO.</param>
        /// <returns>
        ///     Property A computed Property value or null if no rules are 
        ///     specified (in foproperties.xml) to compute the value.
        /// </returns>
        public virtual Property Compute(PropertyList propertyList)
        {
            if (InheritsSpecified())
            {
                // recalculate based on last specified value
                // Climb up propertylist and find last spec'd value
                // NEED PROPNAME!!! get from Maker
                Property specProp =
                    propertyList.GetNearestSpecifiedProperty(propName);
                if (specProp != null)
                {
                    // Only need to do this if the value is relative.
                    String specVal = specProp.SpecifiedValue;
                    if (specVal != null)
                    {
                        try
                        {
                            return Make(propertyList, specVal,
                                        propertyList.getParentFObj());
                        }
                        catch (FonetException)
                        {
                            return null;
                        }
                    }
                }
            }
            return null;
        }

        public virtual bool IsCorrespondingForced(PropertyList propertyList)
        {
            return false;
        }

        public virtual Property GetShorthand(PropertyList propertyList)
        {
            return null;
        }

        public static PropertyMaker Maker(string propName)
        {
            throw new Exception("This method should not be called.");
        }
    }
}