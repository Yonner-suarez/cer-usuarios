using microUsuarios.API.Utils;
using MySql.Data.MySqlClient;
using System.Data;

namespace microUsuarios.API.Dao
{
    public class DAUtil
    {
        //encriptar 
        public static string encriptar(string texto)
        {
            using (MySqlConnection conn = new MySqlConnection(Variables.Conexion.cnx))
            {
                string respuesta = "";
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT HEX( AES_ENCRYPT ( CONVERT ( @pi_texto USING UTF8), @pi_llave ) ) ;", conn);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@pi_texto", texto);
                    cmd.Parameters.AddWithValue("@pi_llave", Variables.Token.Llave);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        respuesta = reader[0].ToString();
                    }
                    reader.Close();
                    reader.Dispose();
                }
                catch (Exception ex)
                {
                    return null;
                    //Helper.logSentryIO(ex);
                }
                return respuesta;
            }
        }

        public static string desencriptar(string texto)
        {
            using (MySqlConnection conn = new MySqlConnection(Variables.Conexion.cnx))
            {
                string respuesta = "";
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT CONVERT (AES_DECRYPT(UNHEX(@pi_texto), @pi_llave) USING UTF8);", conn);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@pi_texto", texto);
                    cmd.Parameters.AddWithValue("@pi_llave", Variables.Token.Llave);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        respuesta = reader[0].ToString();
                    }
                    reader.Close();
                    reader.Dispose();
                }
                catch (Exception ex)
                {
                    return null;
                    //Helper.logSentryIO(ex);
                }
                return respuesta;
            }
        }
    }
}
