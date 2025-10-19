using microUsuarios.API.Model.Request;
using microUsuarios.API.Model;
using microUsuarios.API.Utils;
using MySql.Data.MySqlClient;
using microUsuarios.API.Model.Response;

namespace microUsuarios.API.Dao
{
    public class DACliente
    {
        public static GeneralResponse ObtenerCliente(int idCliente, IniciarSesionRequest req = null)
        {
            var response = new GeneralResponse();
            var cliente = new ClienteResponse();

            using (MySqlConnection conn = new MySqlConnection(Variables.Conexion.cnx))
            {
                try
                {
                    conn.Open();

                    string sqlSelect;

                    if (idCliente > 0)
                    {
                        // Caso: obtener cliente por Id
                        sqlSelect = @"
                                        SELECT 
                                            cer_int_id_usuario,
                                            cer_varchar_nombre,
                                            cer_varchar_correo,
                                            cer_varchar_nro_documento,
                                            cer_enum_tipo_persona,
                                            cer_varchar_codigo_postal,
                                            cer_varchar_direccion,
                                            cer_datetime_created_at,
                                            cer_int_created_by,
                                            cer_enum_rol,
                                            cer_varchar_nro_telefono,
                                            cer_varchar_contraseña
                                        FROM tbl_cer_usuario
                                        WHERE cer_int_id_usuario = @IdCliente
                                          AND cer_enum_rol = 'Cliente'
                                          AND cer_tinyint_estado = 1;";
                    }
                    else if (idCliente == -1 && req != null)
                    {
                        // Caso: validar por correo y contraseña
                        sqlSelect = @"
                                        SELECT 
                                            cer_int_id_usuario,
                                            cer_varchar_nombre,
                                            cer_varchar_correo,
                                            cer_varchar_nro_documento,
                                            cer_enum_tipo_persona,
                                            cer_varchar_codigo_postal,
                                            cer_varchar_direccion,
                                            cer_datetime_created_at,
                                            cer_int_created_by,
                                            cer_enum_rol,
                                            cer_varchar_nro_telefono,
                                            cer_varchar_contraseña
                                        FROM tbl_cer_usuario
                                        WHERE cer_varchar_correo = @Correo
                                          AND cer_varchar_contraseña = @Contrasena
                                          AND cer_enum_rol = 'Cliente'
                                          AND cer_tinyint_estado = 1;";
                    }
                    else
                    {
                        response.status = Variables.Response.BadRequest;
                        response.message = "Parámetros inválidos para la búsqueda de cliente.";
                        response.data = null;
                        return response;
                    }

                    var cmd = new MySqlCommand(sqlSelect, conn);

                    if (idCliente > 0)
                    {
                        cmd.Parameters.AddWithValue("@IdCliente", idCliente);
                    }
                    else if (req != null)
                    {
                        cmd.Parameters.AddWithValue("@Correo", req.Correo);
                        cmd.Parameters.AddWithValue("@Contrasena", new Encrypt().Encript(req.Contrasenia));
                    }

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            cliente.Id = Convert.ToInt32(reader["cer_int_id_usuario"]);
                            cliente.Nombre = reader["cer_varchar_nombre"].ToString();
                            cliente.Correo = reader["cer_varchar_correo"].ToString();
                            cliente.Documento = reader["cer_varchar_nro_documento"].ToString();
                            cliente.TipoPersona = reader["cer_enum_tipo_persona"].ToString();
                            cliente.CodigoPostal = reader["cer_varchar_codigo_postal"].ToString();
                            cliente.Direccion = reader["cer_varchar_direccion"].ToString();
                            cliente.Telefono = reader["cer_varchar_nro_telefono"].ToString();
                            cliente.Contrasenia = new Encrypt().Decrypt(reader["cer_varchar_contraseña"].ToString());
                            cliente.FechaCreacion = reader["cer_datetime_created_at"] != DBNull.Value
                                ? Convert.ToDateTime(reader["cer_datetime_created_at"])
                                : DateTime.MinValue;
                            cliente.CreadoPor = reader["cer_int_created_by"] != DBNull.Value
                                ? Convert.ToInt32(reader["cer_int_created_by"])
                                : 0;
                            cliente.Rol = reader["cer_enum_rol"].ToString();

                            response.status = Variables.Response.OK;
                            response.message = "Cliente obtenido correctamente.";
                            response.data = cliente;
                        }
                        else
                        {
                            response.status = Variables.Response.NotFound;
                            response.message = "No se encontró un cliente activo con los parámetros proporcionados.";
                            response.data = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    response.status = Variables.Response.ERROR;
                    response.message = "Error al obtener cliente: " + ex.Message;
                    response.data = null;
                }
                finally
                {
                    conn.Close();
                }
            }

            return response;
        }


        public static GeneralResponse CrearCliente(AgregarUsuarioRequest request)
        {
            var response = new GeneralResponse();

            using (MySqlConnection conn = new MySqlConnection(Variables.Conexion.cnx))
            {
                try
                {
                    conn.Open();

                    string sqlInsert = @"
                            INSERT INTO tbl_cer_usuario 
                            (
                                cer_varchar_nombre,
                                cer_varchar_correo,
                                cer_varchar_nro_documento,
                                cer_varchar_contraseña,
                                cer_enum_rol,
                                cer_enum_tipo_persona,
                                cer_varchar_codigo_postal,
                                cer_varchar_direccion,
                                cer_int_created_by
                            )
                            VALUES
                            (
                                @Nombre,
                                @Correo,
                                @Documento,
                                @Contrasena,
                                @Rol,
                                @TipoPersona,
                                @CodigoPostal,
                                @Direccion,
                                NULL
                            );";

                    var cmdInsert = new MySqlCommand(sqlInsert, conn);
                    cmdInsert.Parameters.AddWithValue("@Nombre", request.Nombre);
                    cmdInsert.Parameters.AddWithValue("@Correo", request.Correo);
                    cmdInsert.Parameters.AddWithValue("@Documento", request.NroDocumento);
                    cmdInsert.Parameters.AddWithValue("@Contrasena", new Encrypt().Encript(request.Contrasenia));
                    cmdInsert.Parameters.AddWithValue("@Rol", "Cliente");
                    cmdInsert.Parameters.AddWithValue("@TipoPersona", request.TipoPersona);
                    cmdInsert.Parameters.AddWithValue("@CodigoPostal", request.CodigoPostal);
                    cmdInsert.Parameters.AddWithValue("@Direccion", request.Direccion);

                    cmdInsert.ExecuteNonQuery();

                    long nuevoId = cmdInsert.LastInsertedId;

                    string sqlUpdate = @"
                                        UPDATE tbl_cer_usuario 
                                        SET cer_int_created_by = @Id 
                                        WHERE cer_int_id_usuario = @Id;";

                    var cmdUpdate = new MySqlCommand(sqlUpdate, conn);
                    cmdUpdate.Parameters.AddWithValue("@Id", nuevoId);
                    cmdUpdate.ExecuteNonQuery();

                    response.status = Variables.Response.OK;
                    response.message = "Cliente creado correctamente.";
                    int idUsuario = Convert.ToInt32(nuevoId);
                    string token = JWTHelper.GenerarToken(idUsuario, request.Correo, "Cliente");
                    response.data = token;
                }
                catch (Exception ex)
                {
                    response.status = Variables.Response.OK;
                    response.message = "Error al crear cliente: " + ex.Message;
                }
                finally
                {
                    conn.Close();
                }
            }

            return response;
        }
        public static GeneralResponse ActualizarCliente(int idUsuario, AgregarUsuarioRequest request)
        {
            var response = new GeneralResponse();

            using (MySqlConnection conn = new MySqlConnection(Variables.Conexion.cnx))
            {
                try
                {
                    conn.Open();

                    string sqlUpdate = @"
                                        UPDATE tbl_cer_usuario 
                                        SET 
                                            cer_varchar_nombre        = @Nombre,
                                            cer_varchar_correo        = @Correo,
                                            cer_varchar_nro_documento = @Documento,
                                            cer_varchar_contraseña    = @Contrasena,
                                            cer_enum_rol              = @Rol,
                                            cer_enum_tipo_persona     = @TipoPersona,
                                            cer_varchar_codigo_postal = @CodigoPostal,
                                            cer_varchar_direccion     = @Direccion,
                                            cer_varchar_nro_telefono  = @Telefono,
                                            cer_int_updated_by        = @IdUsuario
                                        WHERE cer_int_id_usuario = @IdUsuario;";

                    var cmdUpdate = new MySqlCommand(sqlUpdate, conn);
                    cmdUpdate.Parameters.AddWithValue("@IdUsuario", idUsuario);
                    cmdUpdate.Parameters.AddWithValue("@Nombre", request.Nombre);
                    cmdUpdate.Parameters.AddWithValue("@Correo", request.Correo);
                    cmdUpdate.Parameters.AddWithValue("@Documento", request.NroDocumento);
                    cmdUpdate.Parameters.AddWithValue("@Contrasena", new Encrypt().Encript(request.Contrasenia));
                    cmdUpdate.Parameters.AddWithValue("@Rol", request.Cargo ?? "Cliente");
                    cmdUpdate.Parameters.AddWithValue("@TipoPersona", request.TipoPersona);
                    cmdUpdate.Parameters.AddWithValue("@CodigoPostal", request.CodigoPostal);
                    cmdUpdate.Parameters.AddWithValue("@Direccion", request.Direccion);
                    cmdUpdate.Parameters.AddWithValue("@Telefono", request.Telefono);

                    int rows = cmdUpdate.ExecuteNonQuery();

                    if (rows > 0)
                    {
                        response.status = Variables.Response.OK;
                        response.message = "Cliente actualizado correctamente.";
                        response.data = true;
                    }
                    else
                    {
                        response.status = Variables.Response.ERROR;
                        response.message = "No se encontró el cliente para actualizar.";
                        response.data = false;
                    }
                }
                catch (Exception ex)
                {
                    response.status = Variables.Response.ERROR;
                    response.message = "Error al actualizar cliente: " + ex.Message;
                }
                finally
                {
                    conn.Close();
                }
            }

            return response;
        }


        public static GeneralResponse ValidarCliente(AgregarUsuarioRequest request)
        {
            var response = new GeneralResponse();

            using (MySqlConnection conn = new MySqlConnection(Variables.Conexion.cnx))
            {
                try
                {
                    conn.Open();

                    string sql = @"
                                SELECT COUNT(1) 
                                FROM tbl_cer_usuario 
                                WHERE (cer_varchar_correo = @Correo OR cer_varchar_nro_documento = @Documento)
                                  AND cer_enum_rol = 'Cliente'
                                  AND cer_tinyint_estado = 1;";

                    var cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@Correo", request.Correo);
                    cmd.Parameters.AddWithValue("@Documento", request.NroDocumento);

                    int count = Convert.ToInt32(cmd.ExecuteScalar());

                    if (count > 0)
                    {
                        response.status = Variables.Response.ERROR;
                        response.message = "Ya existe un cliente con el mismo correo o número de documento.";
                        response.data = true;
                    }
                    else
                    {
                        response.status = Variables.Response.OK;
                        response.message = "Validación exitosa, no existen duplicados.";
                        response.data = false;
                    }
                }
                catch (Exception ex)
                {
                    response.status = Variables.Response.ERROR;
                    response.message = "Error al validar cliente: " + ex.Message;
                    response.data = null;
                }
                finally
                {
                    conn.Close();
                }
            }

            return response;
        }

        public static GeneralResponse EliminarCuentaCliente(int idCliente)
        {
            var response = new GeneralResponse();

            using (MySqlConnection conn = new MySqlConnection(Variables.Conexion.cnx))
            {
                try
                {
                    conn.Open();

                    string sqlUpdate = @"
                                        UPDATE tbl_cer_usuario
                                        SET 
                                            cer_tinyint_estado = 0,
                                            cer_datetime_deleted_at = NOW()
                                        WHERE cer_int_id_usuario = @IdCliente
                                          AND cer_enum_rol = 'Cliente'
                                          AND cer_tinyint_estado = 1;";

                    var cmd = new MySqlCommand(sqlUpdate, conn);
                    cmd.Parameters.AddWithValue("@IdCliente", idCliente);

                    int filasAfectadas = cmd.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                    {
                        response.status = Variables.Response.OK;
                        response.message = "Cliente eliminado lógicamente correctamente.";
                        response.data = true;
                    }
                    else
                    {
                        response.status = Variables.Response.BadRequest;
                        response.message = $"No se encontró un cliente activo con Id {idCliente}.";
                        response.data = false;
                    }
                }
                catch (Exception ex)
                {
                    response.status = Variables.Response.ERROR;
                    response.message = "Error al eliminar cliente: " + ex.Message;
                    response.data = null;
                }
                finally
                {
                    conn.Close();
                }
            }

            return response;
        }

    }
}
