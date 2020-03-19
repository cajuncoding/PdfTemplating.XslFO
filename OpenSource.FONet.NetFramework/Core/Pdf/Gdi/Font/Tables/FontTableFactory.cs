using System;

namespace Fonet.Pdf.Gdi.Font {
    /// <summary>
    ///     Instantiates a font table from a table tag.
    /// </summary>
    internal sealed class FontTableFactory {
        /// <summary>
        ///     Prevent instantiation since this is a factory class.
        /// </summary>
        private FontTableFactory() {}

        /// <summary>
        ///     Creates an instance of a class that implements the FontTable interface.
        /// </summary>
        /// <param name="tableName">
        ///     One of the pre-defined TrueType tables from the <see cref="TableNames"/> class.
        /// </param>
        /// <returns>
        ///     A subclass of <see cref="FontTable"/> that is capable of parsing 
        ///     a TrueType table.
        /// </returns>
        /// <exception cref="ArgumentException">
        ///     If a class capable of parsing <i>tableName</i> is not available.
        /// </exception>
        public static FontTable Make(string tableName, FontFileReader reader) {
            DirectoryEntry entry = reader.GetDictionaryEntry(tableName);
            switch (tableName) {
                case TableNames.Head:
                    return new HeaderTable(entry);
                case TableNames.Hhea:
                    return new HorizontalHeaderTable(entry);
                case TableNames.Hmtx:
                    return new HorizontalMetricsTable(entry);
                case TableNames.Maxp:
                    return new MaximumProfileTable(entry);
                case TableNames.Loca:
                    return new IndexToLocationTable(entry);
                case TableNames.Glyf:
                    return new GlyfDataTable(entry);
                case TableNames.Cvt:
                    return new ControlValueTable(entry);
                case TableNames.Prep:
                    return new ControlValueProgramTable(entry);
                case TableNames.Fpgm:
                    return new FontProgramTable(entry);
                case TableNames.Post:
                    return new PostTable(entry);
                case TableNames.Os2:
                    return new OS2Table(entry);
                case TableNames.Name:
                    return new NameTable(entry);
                case TableNames.Kern:
                    return new KerningTable(entry);
                default:
                    throw new ArgumentException("Unrecognised table name '" + tableName + "'", "tableName");
            }
        }
    }
}