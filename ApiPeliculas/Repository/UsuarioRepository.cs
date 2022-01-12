using ApiPeliculas.Data;
using ApiPeliculas.Models;
using ApiPeliculas.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Repository
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly ApplicationDbContext _bd;
        public UsuarioRepository(ApplicationDbContext bd)
        {
            _bd = bd;
        }
        public bool ExisteUsuario(string nombreUsuario)
        {
            if ( _bd.Usuarios.Any(x=>x.UsuarioA == nombreUsuario) )
            {
                return true;
            }

            return false;
        }

        public Usuario GetUsuario(int usuarioId)
        {
            return _bd.Usuarios.FirstOrDefault(x => x.Id == usuarioId);
        }

        public ICollection<Usuario> GetUsuarios()
        {
            return _bd.Usuarios.OrderBy(x => x.UsuarioA).ToList();
        }

        public bool Guardar()
        {
            return _bd.SaveChanges() >= 0;
        }

        public Usuario Login(string usuario, string password)
        {
            var user = _bd.Usuarios.FirstOrDefault( x => x.UsuarioA == usuario);

            if ( user == null )
            {
                return null;
            }

            if ( !VerificaPasswordHash( password, user.PasswordHash, user.PasswordSalt ) )
            {
                return null;
            }

            return user;
        }


        public Usuario Registro(Usuario usuario, string password)
        {
            byte[] passwordHash, passwordSalt;
            CrearPasswordHash(password, out passwordHash, out passwordSalt);

            usuario.PasswordHash = passwordHash;
            usuario.PasswordSalt = passwordSalt;

            _bd.Usuarios.Add(usuario);
            Guardar();
            return usuario;
        }
        private void CrearPasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
        private bool VerificaPasswordHash(string password, byte[]passwordHash, byte[]passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var hashComputado = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < hashComputado.Length; i++)
                {
                    if (hashComputado[i] != passwordHash[i])
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
