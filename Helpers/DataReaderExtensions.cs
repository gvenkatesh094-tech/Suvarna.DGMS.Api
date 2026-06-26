using System;
using System.Data;

namespace Suvarna.DGMS.Api.Helpers   // <-- Change namespace
{
    public static class DataReaderExtensions   // MUST be top-level static class
    {
        public static string SafeGetString(this IDataRecord reader, string column)
        {
            int idx = reader.GetOrdinal(column);
            return reader.IsDBNull(idx) ? "" : reader.GetString(idx);
        }

        public static long SafeGetLong(this IDataRecord reader, string column)
        {
            int idx = reader.GetOrdinal(column);
            return reader.IsDBNull(idx) ? 0 : reader.GetInt64(idx);
        }

        public static int SafeGetInt(this IDataRecord reader, string column)
        {
            int idx = reader.GetOrdinal(column);
            return reader.IsDBNull(idx) ? 0 : reader.GetInt32(idx);
        }

        public static char SafeGetChar(this IDataRecord reader, string column)
        {
            int idx = reader.GetOrdinal(column);
            return reader.IsDBNull(idx) ? '\0' : reader.GetChar(idx);
        }

        public static bool SafeGetBool(this IDataRecord reader, string column)
        {
            int idx = reader.GetOrdinal(column);
            return reader.IsDBNull(idx) ? false : reader.GetBoolean(idx);
        }

        public static DateTime? SafeGetDateTime(this IDataRecord reader, string column)
        {
            int idx = reader.GetOrdinal(column);
            return reader.IsDBNull(idx) ? (DateTime?)null : reader.GetDateTime(idx);
        }

       
    }
}
