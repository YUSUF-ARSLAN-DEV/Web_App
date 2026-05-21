using System;
using System.Data.SqlClient;

namespace WEB_APPLICATION.Models
{
    public class EmailVerificationDAL
    {
        private SqlConnection conn = UtilityDAL.createConnection();

        /// <summary>
        /// Stores a new verification code for the given email.
        /// Deletes any old unused codes for the same email first.
        /// </summary>
        public void SaveCode(string email, string code, DateTime expiresAt)
        {
            try
            {
                conn.Open();

                // Remove any existing codes for this email to avoid confusion
                using (SqlCommand del = new SqlCommand(
                    "DELETE FROM EmailVerificationCode WHERE email = @email", conn))
                {
                    del.Parameters.AddWithValue("@email", email);
                    del.ExecuteNonQuery();
                }

                using (SqlCommand ins = new SqlCommand(
                    "INSERT INTO EmailVerificationCode (email, code, expiresAt, used) " +
                    "VALUES (@email, @code, @expiresAt, 0)", conn))
                {
                    ins.Parameters.AddWithValue("@email", email);
                    ins.Parameters.AddWithValue("@code", code);
                    ins.Parameters.AddWithValue("@expiresAt", expiresAt);
                    ins.ExecuteNonQuery();
                }
            }
            catch (SqlException) { /* swallow — caller handles */ }
            finally { conn.Close(); }
        }

        /// <summary>
        /// Returns true if the code is valid (matches, not expired, not used),
        /// and marks it as used on success.
        /// </summary>
        public bool VerifyCode(string email, string code)
        {
            try
            {
                int id = -1;
                conn.Open();

                using (SqlCommand sel = new SqlCommand(
                    "SELECT id FROM EmailVerificationCode " +
                    "WHERE email = @email AND code = @code AND used = 0 AND expiresAt > @now", conn))
                {
                    sel.Parameters.AddWithValue("@email", email);
                    sel.Parameters.AddWithValue("@code", code);
                    sel.Parameters.AddWithValue("@now", DateTime.Now);
                    object result = sel.ExecuteScalar();
                    if (result == null || result == DBNull.Value) return false;
                    id = Convert.ToInt32(result);
                }

                // Mark as used
                using (SqlCommand upd = new SqlCommand(
                    "UPDATE EmailVerificationCode SET used = 1 WHERE id = @id", conn))
                {
                    upd.Parameters.AddWithValue("@id", id);
                    upd.ExecuteNonQuery();
                }

                return true;
            }
            catch (SqlException) { return false; }
            finally { conn.Close(); }
        }

        /// <summary>
        /// Housekeeping — deletes expired codes. Call occasionally.
        /// </summary>
        public void CleanExpiredCodes()
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand(
                    "DELETE FROM EmailVerificationCode WHERE expiresAt < @now", conn))
                {
                    cmd.Parameters.AddWithValue("@now", DateTime.Now);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException) { }
            finally { conn.Close(); }
        }
    }
}
