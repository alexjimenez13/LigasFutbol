using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using LigasFutbol.Models;

namespace LigasFutbol.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly string _connectionString;

        public UsuarioController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("CONEXION_DB");
        }

        [HttpGet]
        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Registro(Usuario model)
        {
            if (!ModelState.IsValid)
                return View(model);

            bool existe;
            using (var con = new SqlConnection(_connectionString))
            {
                con.Open();
                using (var cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM FUT_USUARIOS WHERE CORREO = @CORREO", con))
                {
                    cmd.Parameters.AddWithValue("@CORREO", model.CORREO);
                    existe = (int)cmd.ExecuteScalar() > 0;
                }
            }

            if (existe)
            {
                ModelState.AddModelError(string.Empty, "El usuario o el correo ya existen.");
                return View(model);
            }

            var hash = ConvertirSha256(model.CONTRASENA);
            using (var con = new SqlConnection(_connectionString))
            {
                con.Open();
                using (var cmd = new SqlCommand(
                    @"INSERT INTO FUT_USUARIOS 
                      (NOMBRE, CORREO, NOMBRE_USUARIO, PRIMER_APELLIDO, SEGUNDO_APELLIDO, CONTRASENA) 
                      VALUES (@NOMBRE, @CORREO, @USUARIO, @P_APELLIDO, @S_APELLIDO, @PASS)", con))
                {
                    cmd.Parameters.AddWithValue("@NOMBRE", model.NOMBRE);
                    cmd.Parameters.AddWithValue("@CORREO", model.CORREO);
                    cmd.Parameters.AddWithValue("@USUARIO", model.NOMBRE_USUARIO);
                    cmd.Parameters.AddWithValue("@P_APELLIDO", model.PRIMER_APELLIDO);
                    cmd.Parameters.AddWithValue("@S_APELLIDO", model.SEGUNDO_APELLIDO);
                    cmd.Parameters.AddWithValue("@PASS", hash);
                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string NOMBRE_USUARIO, string CONTRASENA)
        {
            if (string.IsNullOrEmpty(NOMBRE_USUARIO) || string.IsNullOrEmpty(CONTRASENA))
            {
                ModelState.AddModelError(string.Empty, "Debes ingresar usuario y contraseña.");
                return View();
            }

            var hash = ConvertirSha256(CONTRASENA);
            Usuario usuario = null;

            using (var con = new SqlConnection(_connectionString))
            {
                con.Open();
                using (var cmd = new SqlCommand(
                    @"SELECT TOP 1 Id, NOMBRE, CORREO, NOMBRE_USUARIO, TOKEN_RECUPERACION 
                      FROM FUT_USUARIOS 
                      WHERE NOMBRE_USUARIO = @USUARIO AND CONTRASENA = @PASS", con))
                {
                    cmd.Parameters.AddWithValue("@USUARIO", NOMBRE_USUARIO);
                    cmd.Parameters.AddWithValue("@PASS", hash);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = new Usuario
                            {
                                Id = reader.GetInt32(0),
                                NOMBRE = reader.GetString(1),
                                CORREO = reader.GetString(2),
                                NOMBRE_USUARIO = reader.GetString(3),
                                TOKEN_RECUPERACION = reader.IsDBNull(4)
                                                      ? null
                                                      : reader.GetString(4)
                            };
                        }
                    }
                }
            }

            if (usuario == null)
            {
                ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos.");
                return View();
            }

            HttpContext.Session.SetInt32("UsuarioId", usuario.Id);
            HttpContext.Session.SetString("NOMBRE_USUARIO", usuario.NOMBRE_USUARIO);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult OlvidoContraseña()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult OlvidoContraseña(string CORREO)
        {
            if (string.IsNullOrEmpty(CORREO))
            {
                ModelState.AddModelError(string.Empty, "Ingresa tu correo electrónico.");
                return View();
            }

            int userId;
            using (var con = new SqlConnection(_connectionString))
            {
                con.Open();
                using (var cmd = new SqlCommand(
                    "SELECT Id FROM FUT_USUARIOS WHERE CORREO = @CORREO", con))
                {
                    cmd.Parameters.AddWithValue("@CORREO", CORREO);
                    userId = cmd.ExecuteScalar() as int? ?? 0;
                }
            }

            if (userId == 0)
            {
                ModelState.AddModelError(string.Empty, "No existe un usuario con ese correo.");
                return View();
            }

            var token = Guid.NewGuid().ToString();
            using (var con = new SqlConnection(_connectionString))
            {
                con.Open();
                using (var cmd = new SqlCommand(
                    "UPDATE FUT_USUARIOS SET TOKEN_RECUPERACION = @TOKEN WHERE Id = @Id", con))
                {
                    cmd.Parameters.AddWithValue("@TOKEN", token);
                    cmd.Parameters.AddWithValue("@Id", userId);
                    cmd.ExecuteNonQuery();
                }
            }

            ViewBag.Mensaje = "Se ha generado un token de recuperación. (Simulación)";
            return View();
        }

        [HttpGet]
        public IActionResult CambioContraseña()
        {
            if (HttpContext.Session.GetInt32("UsuarioId") == null)
                return RedirectToAction("Login");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CambioContraseña(string contraseñaActual, string nuevaContraseña)
        {
            var id = HttpContext.Session.GetInt32("UsuarioId");
            if (id == null)
                return RedirectToAction("Login");

            bool ok;
            using (var con = new SqlConnection(_connectionString))
            {
                con.Open();
                using (var cmd = new SqlCommand(
                    @"SELECT COUNT(*) FROM FUT_USUARIOS 
                      WHERE Id = @Id AND CONTRASENA = @PASS", con))
                {
                    cmd.Parameters.AddWithValue("@Id", id.Value);
                    cmd.Parameters.AddWithValue("@PASS", ConvertirSha256(contraseñaActual));
                    ok = (int)cmd.ExecuteScalar() > 0;
                }
            }

            if (!ok)
            {
                ModelState.AddModelError(string.Empty, "La contraseña actual es incorrecta.");
                return View();
            }

            using (var con = new SqlConnection(_connectionString))
            {
                con.Open();
                using (var cmd = new SqlCommand(
                    "UPDATE FUT_USUARIOS SET CONTRASENA = @PASS WHERE Id = @Id", con))
                {
                    cmd.Parameters.AddWithValue("@PASS", ConvertirSha256(nuevaContraseña));
                    cmd.Parameters.AddWithValue("@Id", id.Value);
                    cmd.ExecuteNonQuery();
                }
            }

            ViewBag.Mensaje = "La contraseña se cambió exitosamente.";
            return View();
        }

        [HttpGet]
        public IActionResult Restablecer(string token)
        {
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login");

            ViewBag.Token = token;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Restablecer(string token, string nuevaContraseña)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(nuevaContraseña))
            {
                ModelState.AddModelError(string.Empty, "Datos inválidos.");
                return View();
            }

            int userId;
            using (var con = new SqlConnection(_connectionString))
            {
                con.Open();
                using (var cmd = new SqlCommand(
                    "SELECT Id FROM FUT_USUARIOS WHERE TOKEN_RECUPERACION = @TOKEN", con))
                {
                    cmd.Parameters.AddWithValue("@TOKEN", token);
                    userId = cmd.ExecuteScalar() as int? ?? 0;
                }
            }

            if (userId == 0)
            {
                ModelState.AddModelError(string.Empty, "Token inválido.");
                return View();
            }

            using (var con = new SqlConnection(_connectionString))
            {
                con.Open();
                using (var cmd = new SqlCommand(
                    @"UPDATE FUT_USUARIOS 
                      SET CONTRASENA = @PASS, TOKEN_RECUPERACION = NULL 
                      WHERE Id = @Id", con))
                {
                    cmd.Parameters.AddWithValue("@PASS", ConvertirSha256(nuevaContraseña));
                    cmd.Parameters.AddWithValue("@Id", userId);
                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        private string ConvertirSha256(string texto)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(texto));
            var sb = new StringBuilder();
            foreach (var b in bytes)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }
}
