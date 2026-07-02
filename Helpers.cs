using Microsoft.Data.SqlClient;
using System.Data;

namespace Suvarna.DGMS.Api.Helpers
{
    public static class Helpers
    {
        // =====================================================
        // GET STRING SAFE
        // =====================================================

        public static string GetStringSafe(this SqlDataReader reader, string columnName)
        {
            object value = reader[columnName];

            return value == DBNull.Value ? string.Empty : Convert.ToString(value);
        }

        // =====================================================
        // GET INT SAFE
        // =====================================================

        public static int? GetIntSafe(this SqlDataReader reader, string columnName)
        {
            object value = reader[columnName];

            return value == DBNull.Value ? null : Convert.ToInt32(value);
        }

        // =====================================================
        // GET LONG SAFE
        // =====================================================

        public static long? GetLongSafe(this SqlDataReader reader, string columnName)
        {
            object value = reader[columnName];

            return value == DBNull.Value ? null : Convert.ToInt64(value);
        }

        // =====================================================
        // GET DECIMAL SAFE
        // =====================================================

        public static decimal? GetDecimalSafe(this SqlDataReader reader, string columnName)
        {
            object value = reader[columnName];

            return value == DBNull.Value ? null : Convert.ToDecimal(value);
        }

        // =====================================================
        // GET DATETIME SAFE
        // =====================================================

        public static DateTime? GetDateTimeSafe(this SqlDataReader reader, string columnName)
        {
            object value = reader[columnName];

            return value == DBNull.Value ? null : Convert.ToDateTime(value);
        }

        // =====================================================
        // GET BOOL SAFE
        // =====================================================

        public static bool? GetBoolSafe(this SqlDataReader reader, string columnName)
        {
            object value = reader[columnName];

            return value == DBNull.Value ? null : Convert.ToBoolean(value);
        }
    }
}