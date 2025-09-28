using microUsuarios.API.Model;
using microUsuarios.API.Model.Request;
using microUsuarios.API.Model.Response;
using microUsuarios.API.Utils;
using MySql.Data.MySqlClient;
using System.Data;

namespace microUsuarios.API.Dao
{
    public static class DAEmpleado
    {
        
        public static GeneralResponse CrearEmpleado(AgregarUsuarioRequest request)
        {
            var response = new GeneralResponse();

            using (MySqlConnection conn = new MySqlConnection(Variables.Conexion.cnx))
            {
                try
                {
                    conn.Open();
                    string sql = @"
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
                                        @CreatedBy
                                    );";

                    var cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@Nombre", request.Nombre);
                    cmd.Parameters.AddWithValue("@Correo", request.Correo);
                    cmd.Parameters.AddWithValue("@Documento", request.NroDocumento);
                    cmd.Parameters.AddWithValue("@Contrasena", new Encrypt().Encript(request.Contrasenia));
                    cmd.Parameters.AddWithValue("@Rol", request.Cargo); // Admin / Logística
                    cmd.Parameters.AddWithValue("@TipoPersona", Variables.TipoPersona.Natural);
                    cmd.Parameters.AddWithValue("@CodigoPostal", "");
                    cmd.Parameters.AddWithValue("@Direccion", "");
                    
                    if(request.IdAdmin == 0)
                    {
                        response.status = Variables.Response.BadRequest;
                        response.message = "Nó se proporcionó el id de administrador que esta intentado crear el nuevo empleado";
                        return response;
                    }
                    cmd.Parameters.AddWithValue("@CreatedBy", request.IdAdmin);

                    cmd.ExecuteNonQuery();

                    response.status = response.status = Variables.Response.OK;
                    response.message = "Empleado creado correctamente.";
                    response.data = true;


                }
                catch (Exception ex)
                {
                    response.status = Variables.Response.ERROR;
                    response.message = "Error al crear empleado: " + ex.Message;
                    response.data = null;
                }
                finally
                {
                    conn.Close();
                }
            }

            return response;
        }
        public static GeneralResponse ObtenerEmpleado(int idEmpleado, IniciarSesionRequest req = null)
        {
            var response = new GeneralResponse();
            EmpleadoResponse empleado = null;

            using (MySqlConnection conn = new MySqlConnection(Variables.Conexion.cnx))
            {
                try
                {
                    conn.Open();

                    string sqlSelect;
                    if (idEmpleado > 0 && req == null)
                    {
                        // Buscar empleado por Id (solo Administrador o Logistica)
                        sqlSelect = @"
                                    SELECT 
                                        cer_int_id_usuario,
                                        cer_varchar_nombre,
                                        cer_varchar_correo,
                                        cer_varchar_nro_documento,
                                        cer_enum_rol,
                                        cer_enum_tipo_persona,
                                        cer_datetime_created_at,
                                        cer_int_created_by
                                    FROM tbl_cer_usuario
                                    WHERE cer_int_id_usuario = @IdEmpleado
                                      AND cer_enum_rol IN ('Administrador','Logistica')
                                      AND cer_tinyint_estado = 1;";
                    }
                    else if (idEmpleado == -1 && req != null)
                    {
                        // Buscar empleado por correo y contraseña (solo Administrador o Logistica)
                        sqlSelect = @"
                                    SELECT 
                                        cer_int_id_usuario,
                                        cer_varchar_nombre,
                                        cer_varchar_correo,
                                        cer_varchar_nro_documento,
                                        cer_enum_rol,
                                        cer_enum_tipo_persona,
                                        cer_datetime_created_at,
                                        cer_int_created_by
                                    FROM tbl_cer_usuario
                                    WHERE cer_varchar_correo = @Correo
                                      AND cer_varchar_contraseña = @Contrasena
                                      AND cer_enum_rol IN ('Administrador','Logistica')
                                      AND cer_tinyint_estado = 1;";
                    }

                    else
                    {
                        response.status = Variables.Response.BadRequest;
                        response.message = "Parámetros inválidos para la búsqueda de empleado.";
                        response.data = null;
                        return response;
                    }

                    var cmd = new MySqlCommand(sqlSelect, conn);

                    if (idEmpleado > 0 && req == null)
                    {
                        cmd.Parameters.AddWithValue("@IdEmpleado", idEmpleado);
                    }
                    else if (idEmpleado == -1 && req != null)
                    {
                        cmd.Parameters.AddWithValue("@Correo", req.Correo);
                        cmd.Parameters.AddWithValue("@Contrasena", new Encrypt().Encript(req.Contrasenia));
                    }

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            empleado = new EmpleadoResponse
                            {
                                Id = Convert.ToInt32(reader["cer_int_id_usuario"]),
                                Nombre = reader["cer_varchar_nombre"].ToString(),
                                Correo = reader["cer_varchar_correo"].ToString(),
                                Documento = reader["cer_varchar_nro_documento"].ToString(),
                                Rol = reader["cer_enum_rol"].ToString(),
                                TipoPersona = reader["cer_enum_tipo_persona"].ToString(),
                                FechaCreacion = reader["cer_datetime_created_at"] != DBNull.Value
                                    ? Convert.ToDateTime(reader["cer_datetime_created_at"])
                                    : DateTime.MinValue,
                                CreadoPor = reader["cer_int_created_by"] != DBNull.Value
                                    ? Convert.ToInt32(reader["cer_int_created_by"])
                                    : 0
                            };

                            response.data = empleado;
                            response.status = Variables.Response.OK;
                            response.message = "Empleado obtenido correctamente.";
                        }
                        else
                        {
                            response.status = Variables.Response.BadRequest;
                            response.message = "No se encontró un empleado con los parámetros proporcionados.";
                            response.data = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    response.status = Variables.Response.ERROR;
                    response.message = "Error al obtener empleado: " + ex.Message;
                    response.data = null;
                }
                finally
                {
                    conn.Close();
                }
            }

            return response;
        }

        public static GeneralResponse ObtenerEmpleados()
        {
            var response = new GeneralResponse();
            var empleados = new List<EmpleadoResponse>();

            using (MySqlConnection conn = new MySqlConnection(Variables.Conexion.cnx))
            {
                try
                {
                    conn.Open();

                    string sqlSelect = @"
                SELECT 
                    cer_int_id_usuario,
                    cer_varchar_nombre,
                    cer_varchar_correo,
                    cer_varchar_nro_documento,
                    cer_varchar_contraseña,
                    cer_enum_rol,
                    cer_enum_tipo_persona,
                    cer_datetime_created_at,
                    cer_int_created_by
                FROM tbl_cer_usuario
                WHERE cer_enum_rol IN ('Administrador','Logistica') and cer_tinyint_estado = 1 ;";

                    var cmd = new MySqlCommand(sqlSelect, conn);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var empleado = new EmpleadoResponse
                            {
                                Id = Convert.ToInt32(reader["cer_int_id_usuario"]),
                                Nombre = reader["cer_varchar_nombre"].ToString(),
                                Correo = reader["cer_varchar_correo"].ToString(),
                                Documento = reader["cer_varchar_nro_documento"].ToString(),
                                Contrasenia = reader["cer_varchar_contraseña"].ToString(),
                                Rol = reader["cer_enum_rol"].ToString(),
                                TipoPersona = reader["cer_enum_tipo_persona"].ToString(),
                                FechaCreacion = reader["cer_datetime_created_at"] != DBNull.Value
                                    ? Convert.ToDateTime(reader["cer_datetime_created_at"])
                                    : DateTime.MinValue,
                                CreadoPor = reader["cer_int_created_by"] != DBNull.Value
                                    ? Convert.ToInt32(reader["cer_int_created_by"])
                                    : 0
                            };

                            if (empleado != null) 
                            {
                                empleado.Contrasenia = new Encrypt().Decrypt(empleado.Contrasenia);
                            }

                            empleados.Add(empleado);
                        }
                    }

                    if (empleados.Count > 0)
                    {
                        response.status = Variables.Response.OK;
                        response.message = "Empleados obtenidos correctamente.";
                        response.data = empleados;
                    }
                    else
                    {
                        response.status = Variables.Response.BadRequest;
                        response.message = "No se encontraron empleados con rol Administrador o Logistica.";
                        response.data = null;
                    }
                }
                catch (Exception ex)
                {
                    response.status = Variables.Response.ERROR;
                    response.message = "Error al obtener empleados: " + ex.Message;
                    response.data = null;
                }
                finally
                {
                    conn.Close();
                }
            }

            return response;
        }
        public static GeneralResponse EliminarEmpleado(int idUsuario, int idAdmin)
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
                                            cer_datetime_deleted_at = NOW(),
                                            cer_int_updated_by = @IdAdmin
                                        WHERE cer_int_id_usuario = @IdUsuario
                                          AND cer_tinyint_estado = 1;";

                    var cmd = new MySqlCommand(sqlUpdate, conn);
                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                    cmd.Parameters.AddWithValue("@IdAdmin", idAdmin);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        response.status = Variables.Response.OK;
                        response.message = $"Usuario con Id {idUsuario} eliminado lógicamente.";
                        response.data = true;
                    }
                    else
                    {
                        response.status = Variables.Response.BadRequest;
                        response.message = $"No se encontró un usuario activo con Id {idUsuario}.";
                        response.data = false;
                    }
                }
                catch (Exception ex)
                {
                    response.status = Variables.Response.ERROR;
                    response.message = "Error al eliminar usuario: " + ex.Message;
                    response.data = null;
                }
                finally
                {
                    conn.Close();
                }
            }

            return response;
        }
        public static GeneralResponse ValidarEmpleado(AgregarUsuarioRequest request)
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
                                  AND cer_enum_rol IN ('Administrador','Logistica')
                                  AND cer_tinyint_estado = 1;";

                    var cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@Correo", request.Correo);
                    cmd.Parameters.AddWithValue("@Documento", request.NroDocumento);

                    int count = Convert.ToInt32(cmd.ExecuteScalar());

                    if (count > 0)
                    {
                        response.status = Variables.Response.ERROR;
                        response.message = "Ya existe un empleado con el mismo correo o número de documento.";
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
                    response.message = "Error al validar empleado: " + ex.Message;
                    response.data = null;
                }
                finally
                {
                    conn.Close();
                }
            }

            return response;
        }

        public static GeneralResponse ActualizarEmpleado(AgregarUsuarioRequest request, int idUsuario)
        {
            var response = new GeneralResponse();

            using (MySqlConnection conn = new MySqlConnection(Variables.Conexion.cnx))
            {
                try
                {
                    conn.Open();
                    string sql = @"
                                    UPDATE tbl_cer_usuario 
                                    SET 
                                        cer_varchar_nombre = @Nombre,
                                        cer_varchar_correo = @Correo,
                                        cer_varchar_nro_documento = @Documento,
                                        cer_varchar_contraseña = @Contrasena,
                                        cer_enum_rol = @Rol,
                                        cer_enum_tipo_persona = @TipoPersona,
                                        cer_varchar_codigo_postal = @CodigoPostal,
                                        cer_varchar_direccion = @Direccion,
                                        cer_int_updated_by = @UpdatedBy,
                                        cer_datetime_updated_at = NOW()
                                    WHERE cer_int_id_usuario = @IdUsuario;";

                    var cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@Nombre", request.Nombre);
                    cmd.Parameters.AddWithValue("@Correo", request.Correo);
                    cmd.Parameters.AddWithValue("@Documento", request.NroDocumento);
                    cmd.Parameters.AddWithValue("@Contrasena", new Encrypt().Encript(request.Contrasenia));
                    cmd.Parameters.AddWithValue("@Rol", request.Cargo);
                    cmd.Parameters.AddWithValue("@TipoPersona", Variables.TipoPersona.Natural);
                    cmd.Parameters.AddWithValue("@CodigoPostal", "");
                    cmd.Parameters.AddWithValue("@Direccion", "");

                    if (request.IdAdmin == 0)
                    {
                        response.status = Variables.Response.BadRequest;
                        response.message = "No se proporcionó el id de administrador que intenta actualizar el empleado.";
                        return response;
                    }
                    cmd.Parameters.AddWithValue("@UpdatedBy", request.IdAdmin);

                    if (idUsuario == 0)
                    {
                        response.status = Variables.Response.BadRequest;
                        response.message = "No se proporcionó el id del empleado a actualizar.";
                        return response;
                    }
                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        response.status = Variables.Response.OK;
                        response.message = "Empleado actualizado correctamente.";
                        response.data = true;
                    }
                    else
                    {
                        response.status = Variables.Response.BadRequest;
                        response.message = "No se encontró el empleado para actualizar.";
                        response.data = false;
                    }
                }
                catch (Exception ex)
                {
                    response.status = Variables.Response.ERROR;
                    response.message = "Error al actualizar empleado: " + ex.Message;
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
