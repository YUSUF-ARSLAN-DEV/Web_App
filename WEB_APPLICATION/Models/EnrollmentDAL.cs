using System;
using System.Collections.Generic;
using System.Data.SqlClient;

using System.Linq;
using System.Web;
using System.Web.Management;

namespace WEB_APPLICATION.Models
{
    public class EnrollmentDAL
    {
        private SqlConnection conn = UtilityDAL.createConnection() ; 

        // the method below takes a userId and a courseID and sets his enrollment status to false - soft delete - record still exists 

        public bool Enroll(int userId , int courseId ) // takes the user ID and the course they want to enroll two and creates a record 
        {
            bool success = false;
            EnrollmentRecord enrollment  = new EnrollmentRecord(courseId , userId ) ; 
            try
            {
                conn.Open();

                using ( SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Enrollment (userId , courseId , completionRate, enrollmentDate,activeStatus ) VALUES ( @userId , @courseId , @completionRate, @enrollmentDate, @activeStatus )", conn)) {
                    cmd.Parameters.AddWithValue("@userId", enrollment.userId);
                    cmd.Parameters.AddWithValue("@courseId", enrollment.courseId);
                    cmd.Parameters.AddWithValue("@completionRate", enrollment.completionRate);
                    cmd.Parameters.AddWithValue("@enrollmentDate", enrollment.enrollmentDate);
                    cmd.Parameters.AddWithValue("@activeStatus", enrollment.activeStatus);
                if (cmd.ExecuteNonQuery() > 0)
                    success = true;
                }
            }
            catch (SqlException e) { Console.WriteLine(e.Message); }
            finally { conn.Close(); }
            return success;

        }
        public bool UnEnroll(int userId , int courseId  ) // takes a user ID and removes them from the course they are enrolled too 
        {
            bool success = false ; 
            try { 
            conn.Open() ; 
            using (SqlCommand cmd = new SqlCommand("UPDATE  Enrollment SET activeStatus = 0  WHERE userId = @userId AND courseId = @courseId",conn))
            {
                cmd.Parameters.AddWithValue("@userId", userId ) ; 
                cmd.Parameters.AddWithValue("@courseId", courseId  ) ; 
                if ( cmd.ExecuteNonQuery() > 0  )
                {
                    success = true ; 

                }  
                else 
                { 
                
                    success = false ;    
                }
               
            }
            } 
            catch (SqlException e ) 
            {
                
            }
            finally
            {
                conn.Close() ;     
            }
             return success ;  
        }
         
         // this method takes an enrollmentID and updates the compeltetion rate 
        public bool UpdateCompletionRate  (int enrollmentId, int rate )
        {
            bool success = false ; 
            try 
            { 
                conn.Open() ; 
                using (SqlCommand cmd = new SqlCommand("UPDATE  Enrollment SET completionRate = @rate  WHERE enrollmentId = @enrollmentId",conn))
                {
                    cmd.Parameters.AddWithValue("@enrollmentId", enrollmentId ) ; 
                    cmd.Parameters.AddWithValue("@rate", rate  ) ; 
                    if ( cmd.ExecuteNonQuery() > 0 )
                        {
                            success = true ; 
                        }

                }
            }
            catch (SqlException e ) 
            {
                        
            }
            finally
            {
                conn.Close() ;     
            }
            return success ; 
        }

        public List<EnrollmentRecord> GetEnrollmentByCourse (int takenCourseId )
        {
            List<EnrollmentRecord> listOfRecords = new List<EnrollmentRecord>() ; 
            int enrollmentId ;  
            int userId  ;
            int courseId ;
            int completionRate ; 
            DateTime enrollmentDate ;   
            bool activeStatus ;    
            try
            {
                conn.Open();
                using ( SqlCommand cmd = new SqlCommand("SELECT * FROM Enrollment WHERE courseId = @courseId AND  activeStatus = 1", conn)) 
                {
                    cmd.Parameters.AddWithValue("@courseId", takenCourseId);
                    using (SqlDataReader reader = cmd.ExecuteReader() )
                    {
                        // keep iterating while there is rows to iterate through 
                         while (reader.Read())
                        {
                            enrollmentId = UtilityDAL.returnInt(reader,"enrollmentId") ; 
                            userId = UtilityDAL.returnInt(reader,"userId") ; 
                            courseId = UtilityDAL.returnInt(reader,"courseId") ;  
                            completionRate =  UtilityDAL.returnInt(reader,"completionRate") ;  
                            enrollmentDate =  UtilityDAL.returnDateTime(reader,"enrollmentDate") ; 
                            activeStatus = (bool)UtilityDAL.returnBit(reader,"activeStatus") ; 
                            EnrollmentRecord record = new  EnrollmentRecord(enrollmentId , courseId,  userId  , completionRate , enrollmentDate ,activeStatus) ;
                            listOfRecords.Add(record)  ; 
                        }   
                    }
                


                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                conn.Close();
            }
            return listOfRecords ; 
            
        
        }
    
        // the below method returns all enrollment records of a specific user - it only retrives the active enrollments only 
        public List<EnrollmentRecord> GetEnrollmentByUser(int desiredUserId)
        {
            List<EnrollmentRecord> studentEnrollments = new List<EnrollmentRecord> () ; 
            int enrollmentId ;  
            int userId  ;
            int courseId ;
            int completionRate ; 
            DateTime enrollmentDate ;   
            bool activeStatus ;  
            try
            {
                conn.Open();
                using ( SqlCommand cmd = new SqlCommand("SELECT * FROM Enrollment WHERE userId = @userId AND activeStatus = 1", conn ) ) 
                {
                    cmd.Parameters.AddWithValue("@userId", desiredUserId);
                    using (SqlDataReader reader = cmd.ExecuteReader() )
                    {
                        while(reader.Read()) // while there is a next row and it was moved towards 
                        {
                            enrollmentId = UtilityDAL.returnInt(reader,"enrollmentId") ; 
                            userId = UtilityDAL.returnInt(reader,"userId") ; 
                            courseId = UtilityDAL.returnInt(reader,"courseId") ;  
                            completionRate =  UtilityDAL.returnInt(reader,"completionRate") ;  
                            enrollmentDate =  UtilityDAL.returnDateTime(reader,"enrollmentDate") ; 
                            activeStatus = (bool)UtilityDAL.returnBit(reader,"activeStatus") ; 
                            EnrollmentRecord record = new  EnrollmentRecord(enrollmentId ,courseId , userId  , completionRate , enrollmentDate ,activeStatus) ;
                            studentEnrollments.Add(record) ; 
                        }
                    }
            
                }
            }
            catch (SqlException e)
            {
            Console.WriteLine(e.Message);
            }
            finally
            {
            conn.Close();
            }
            return studentEnrollments ; 
        }
        public bool IsEnrolled(int userId, int courseId) // returns true if the student is already enrolled  false if not 
        {
            int count = 0;
            try
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM Enrollment WHERE userId = @userId AND courseId = @courseId AND activeStatus = 1", conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@courseId", courseId);
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                        count = Convert.ToInt32(result);
                }
            }
            catch (SqlException e) { Console.WriteLine(e.Message); }
            finally { conn.Close(); }
            return count > 0;
        }
        public EnrollmentRecord GetEnrollment(int userId, int courseId) // returns a student specific enrollment record for a course 
        {
            EnrollmentRecord record = null;
            try
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT * FROM Enrollment WHERE userId = @userId AND courseId = @courseId AND activeStatus = 1", conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@courseId", courseId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            record = new EnrollmentRecord(
                                UtilityDAL.returnInt(reader, "enrollmentId"),
                                UtilityDAL.returnInt(reader, "courseId"),
                                UtilityDAL.returnInt(reader, "userId"),
                                UtilityDAL.returnInt(reader, "completionRate"),
                                UtilityDAL.returnDateTime(reader, "enrollmentDate"),
                                UtilityDAL.returnBit(reader, "activeStatus")
                            );
                        }
                    }
                }
            }
            catch (SqlException e) { Console.WriteLine(e.Message); }
            finally { conn.Close(); }
            return record;
        }
    
    }
}