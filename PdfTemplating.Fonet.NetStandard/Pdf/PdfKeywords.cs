using System.Collections;

namespace Fonet.Pdf
{
    public enum Keyword
    {
        Obj,
        EndObj,
        R,
        DictionaryBegin,
        DictionaryEnd,
        ArrayBegin,
        ArrayEnd,
        Stream,
        EndStream,
        True,
        False,
        Null,
        XRef,
        Trailer,
        StartXRef,
        Eof,
        BT,
        ET,
        Tf,
        Td,
        Tr,
        Tj
    }

    public sealed class KeywordEntries
    {
        private static readonly IDictionary Entries;

        static KeywordEntries()
        {
            Entries = new Hashtable();
            Entries.Add(Keyword.Obj, new byte[] { (byte)'o', (byte)'b', (byte)'j' });
            Entries.Add(Keyword.EndObj, new byte[] { (byte)'e', (byte)'n', (byte)'d', (byte)'o', (byte)'b', (byte)'j' });
            Entries.Add(Keyword.R, new byte[] { (byte)'R' });
            Entries.Add(Keyword.DictionaryBegin, new byte[] { (byte)'<', (byte)'<' });
            Entries.Add(Keyword.DictionaryEnd, new byte[] { (byte)'>', (byte)'>' });
            Entries.Add(Keyword.ArrayBegin, new byte[] { (byte)'[' });
            Entries.Add(Keyword.ArrayEnd, new byte[] { (byte)']' });
            Entries.Add(Keyword.Stream, new byte[] { (byte)'s', (byte)'t', (byte)'r', (byte)'e', (byte)'a', (byte)'m' });
            Entries.Add(Keyword.EndStream, new byte[] { (byte)'e', (byte)'n', (byte)'d', (byte)'s', (byte)'t', (byte)'r', (byte)'e', (byte)'a', (byte)'m' });
            Entries.Add(Keyword.True, new byte[] { (byte)'t', (byte)'r', (byte)'u', (byte)'e' });
            Entries.Add(Keyword.False, new byte[] { (byte)'f', (byte)'a', (byte)'l', (byte)'s', (byte)'e' });
            Entries.Add(Keyword.Null, new byte[] { (byte)'n', (byte)'u', (byte)'l', (byte)'l' });
            Entries.Add(Keyword.XRef, new byte[] { (byte)'x', (byte)'r', (byte)'e', (byte)'f' });
            Entries.Add(Keyword.Trailer, new byte[] { (byte)'t', (byte)'r', (byte)'a', (byte)'i', (byte)'l', (byte)'e', (byte)'r' });
            Entries.Add(Keyword.StartXRef, new byte[] { (byte)'s', (byte)'t', (byte)'a', (byte)'r', (byte)'t', (byte)'x', (byte)'r', (byte)'e', (byte)'f' });
            Entries.Add(Keyword.Eof, new byte[] { (byte)'%', (byte)'%', (byte)'E', (byte)'O', (byte)'F' });
            Entries.Add(Keyword.BT, new byte[] { (byte)'B', (byte)'T' });
            Entries.Add(Keyword.ET, new byte[] { (byte)'E', (byte)'T' });
            Entries.Add(Keyword.Tf, new byte[] { (byte)'T', (byte)'f' });
            Entries.Add(Keyword.Td, new byte[] { (byte)'T', (byte)'d' });
            Entries.Add(Keyword.Tr, new byte[] { (byte)'T', (byte)'r' });
            Entries.Add(Keyword.Tj, new byte[] { (byte)'T', (byte)'j' });
        }

        private KeywordEntries() { }

        public static byte[] GetKeyword(Keyword keyword)
        {
            return (byte[])Entries[keyword];
        }
    }
}