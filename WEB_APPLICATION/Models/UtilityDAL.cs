using System ; 
using System.Data.SqlClient ; 
using System.Configuration ; 
namespace  WEB_APPLICATION.Models {
public class UtilityDAL 
{
    public static String  returnString(SqlDataReader reader , string column_name ) 
    {
        // first we get the index of the column 
        int index_in_row =  reader.GetOrdinal(column_name ) ;  
        // then checking the value using the index value 
        string string_value = reader.IsDBNull(index_in_row) ? null : reader.GetString(index_in_row)  ;
        // the statement above basically says chekc if iti is null if so return null if not  return a string 

        return string_value  ;
    }
    public static bool returnBit (SqlDataReader reader , string columnName)
        {
            // extracting the index 
            int index = reader.GetOrdinal(columnName ) ; 
            bool bit = reader.IsDBNull(index) ? false  : reader.GetBoolean(index) ;  
            return bit ; 
        }
    public static int  returnInt  ( SqlDataReader reader , string columnName ) 
    {
        // getting the index of the column 
        int index_row = reader.GetOrdinal(columnName) ; 
        int value = reader.IsDBNull(index_row ) ? 0 : reader.GetInt32(index_row) ;
        return value ;
        // 0 is returned for null values 
    }
    public static DateTime returnDateTime (SqlDataReader reader , string columnName ) 
    {
        // index of the column 
        int index_r = reader.GetOrdinal(columnName) ; // gets the index of that specific column in the row 
        // assigning value  - before checking if the value in this row at that specific row is empty or not 
        DateTime  value = reader.IsDBNull(index_r) ? DateTime.MinValue : reader.GetDateTime(index_r) ;
        return value ; 
    }

    public static TimeSpan returnTimeSpan (SqlDataReader reader  , string columnName ) 
    {
        // getting the column index to be passed to the GetTimeSpan
        int index = reader.GetOrdinal(columnName );
         // quick if statements structure | statment that is evaluted true / false  ? value if true : value if false 
        TimeSpan time = reader.IsDBNull(index) ? TimeSpan.MinValue : reader.GetTimeSpan(index) ; 
        return time ;  
    }
        public static float returnFloat(SqlDataReader reader, string columnName)
        {
            int index = reader.GetOrdinal(columnName);
            float value = reader.IsDBNull(index) ? 0f : Convert.ToSingle(reader.GetValue(index));
            return value;
        }

        public static SqlConnection createConnection () 
    {
        string connectionString = ConfigurationManager.ConnectionStrings["LearningPlatformDataBase"].ConnectionString ; 
        SqlConnection conn = new SqlConnection(connectionString )  ;
        return conn ;  
    }
    // these 2 consecutive methods parse to and from Role.enum
     public static User.Role parseStringToRole(string role)
    {
        if (role == "student") return User.Role.Student;
        if (role == "instructor") return User.Role.Instructor;
        if (role == "admin") return User.Role.Admin;
        return User.Role.Student; // default fallback
    }
        public static  string roleToString(User.Role role ) 
    {
        if (role == User.Role.Admin ) {return "admin" ;}
        else if (role == User.Role.Instructor ) {return "instructor" ;}
        else if (role == User.Role.Student ) {return "student" ;}
        return "" ; // in hte case it is unknown 
    }


}
}