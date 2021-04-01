using System.Collections;
using Fonet.Fo.Flow;
using Fonet.Fo.Pagination;
using Fonet.Fo.Properties;

namespace Fonet.Fo
{
    internal class StandardElementMapping
    {
        public const string URI = "http://www.w3.org/1999/XSL/Format";

        private static Hashtable foObjs;

        static StandardElementMapping()
        {
            foObjs = new Hashtable();

            // Declarations and Pagination and Layout Formatting Objects
            foObjs.Add("root", Root.GetMaker());
            foObjs.Add("declarations", Declarations.GetMaker());
            foObjs.Add("color-profile", ColorProfile.GetMaker());
            foObjs.Add("page-sequence", PageSequence.GetMaker());
            foObjs.Add("layout-master-set", LayoutMasterSet.GetMaker());
            foObjs.Add("page-sequence-master", PageSequenceMaster.GetMaker());
            foObjs.Add("single-page-master-reference", SinglePageMasterReference.GetMaker());
            foObjs.Add("repeatable-page-master-reference", RepeatablePageMasterReference.GetMaker());
            foObjs.Add("repeatable-page-master-alternatives", RepeatablePageMasterAlternatives.GetMaker());
            foObjs.Add("conditional-page-master-reference", ConditionalPageMasterReference.GetMaker());
            foObjs.Add("simple-page-master", SimplePageMaster.GetMaker());
            foObjs.Add("region-body", RegionBody.GetMaker());
            foObjs.Add("region-before", RegionBefore.GetMaker());
            foObjs.Add("region-after", RegionAfter.GetMaker());
            foObjs.Add("region-start", RegionStart.GetMaker());
            foObjs.Add("region-end", RegionEnd.GetMaker());
            foObjs.Add("flow", Flow.Flow.GetMaker());
            foObjs.Add("static-content", StaticContent.GetMaker());
            foObjs.Add("title", Title.GetMaker());

            // Block-level Formatting Objects
            foObjs.Add("block", Block.GetMaker());
            foObjs.Add("block-container", BlockContainer.GetMaker());

            // Inline-level Formatting Objects
            foObjs.Add("bidi-override", BidiOverride.GetMaker());
            foObjs.Add("character", Character.GetMaker());
            foObjs.Add("initial-property-set", InitialPropertySet.GetMaker());
            foObjs.Add("external-graphic", ExternalGraphic.GetMaker());
            foObjs.Add("instream-foreign-object", InstreamForeignObject.GetMaker());
            foObjs.Add("inline", Inline.GetMaker());
            foObjs.Add("inline-container", InlineContainer.GetMaker());
            foObjs.Add("leader", Leader.GetMaker());
            foObjs.Add("page-number", PageNumber.GetMaker());
            foObjs.Add("page-number-citation", PageNumberCitation.GetMaker());

            // Formatting Objects for Tables
            foObjs.Add("table-and-caption", TableAndCaption.GetMaker());
            foObjs.Add("table", Table.GetMaker());
            foObjs.Add("table-column", TableColumn.GetMaker());
            foObjs.Add("table-caption", TableCaption.GetMaker());
            foObjs.Add("table-header", TableHeader.GetMaker());
            foObjs.Add("table-footer", TableFooter.GetMaker());
            foObjs.Add("table-body", TableBody.GetMaker());
            foObjs.Add("table-row", TableRow.GetMaker());
            foObjs.Add("table-cell", TableCell.GetMaker());

            // Formatting Objects for Lists
            foObjs.Add("list-block", ListBlock.GetMaker());
            foObjs.Add("list-item", ListItem.GetMaker());
            foObjs.Add("list-item-body", ListItemBody.GetMaker());
            foObjs.Add("list-item-label", ListItemLabel.GetMaker());

            // Dynamic Effects: Link and Multi Formatting Objects
            foObjs.Add("basic-link", BasicLink.GetMaker());
            foObjs.Add("multi-switch", MultiSwitch.GetMaker());
            foObjs.Add("multi-case", MultiCase.GetMaker());
            foObjs.Add("multi-toggle", MultiToggle.GetMaker());
            foObjs.Add("multi-properties", MultiProperties.GetMaker());
            foObjs.Add("multi-property-set", MultiPropertySet.GetMaker());

            // Out-of-Line Formatting Objects
            foObjs.Add("float", Float.GetMaker());
            foObjs.Add("footnote", Footnote.GetMaker());
            foObjs.Add("footnote-body", FootnoteBody.GetMaker());

            // Other Formatting Objects
            foObjs.Add("wrapper", Wrapper.GetMaker());
            foObjs.Add("marker", Marker.GetMaker());
            foObjs.Add("retrieve-marker", RetrieveMarker.GetMaker());
        }

        public void AddToBuilder(FOTreeBuilder builder)
        {
            builder.AddElementMapping(URI, foObjs);
            builder.AddPropertyMapping(URI, FOPropertyMapping.getGenericMappings());
        }
    }

}