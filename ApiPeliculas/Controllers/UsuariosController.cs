using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos;
using ApiPeliculas.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace ApiPeliculas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "ApiPeliculasUsuarios")]
    public class UsuariosController : Controller
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        public UsuariosController(IUsuarioRepository usuarioRepository, IMapper mapper, IConfiguration config)
        {
            _usuarioRepository = usuarioRepository;
            _mapper = mapper;
            _config = config;
        }

        [HttpGet]
        public IActionResult GetUsuarios()
        {
            var listaUsuarios = _usuarioRepository.GetUsuarios();
            var listaUsuariosDto = new List<UsuarioDto>();

            foreach (var Usuario in listaUsuarios)
            {
                listaUsuariosDto.Add(_mapper.Map<UsuarioDto>(Usuario));
            }

            return Ok(listaUsuariosDto);

        }

        [HttpGet("{UsuarioId:int}", Name ="GetUsuario")]
        public IActionResult GetUsuario(int UsuarioId)
        {
            var itemUsuario = _usuarioRepository.GetUsuario(UsuarioId);

            if (itemUsuario == null)
            {
                return NotFound();
            }

            var UsuarioDto = _mapper.Map<UsuarioDto>(itemUsuario);
            return Ok(UsuarioDto);
        }

        [HttpPost("Registro")]
        public IActionResult Registro([FromBody]UsuarioAuthDto UsuarioAuthDto)
        {
            UsuarioAuthDto.Usuario = UsuarioAuthDto.Usuario.ToLower();

            if ( _usuarioRepository.ExisteUsuario(UsuarioAuthDto.Usuario) )
            {
                return BadRequest("El usuario ya existe.");
            }

            var usuarioACrear = new Usuario
            {
                UsuarioA = UsuarioAuthDto.Usuario
            };

            var usuarioCreado = _usuarioRepository.Registro(usuarioACrear, UsuarioAuthDto.Password);
            return Ok(usuarioCreado);

        }
        [HttpPost("Login")]
        public IActionResult Login([FromBody] UsuarioAuthLoginDto usuarioAuthLoginDto)
        {
            var usuarioDesdeRepo = _usuarioRepository.Login(usuarioAuthLoginDto.Usuario, usuarioAuthLoginDto.Password);
            if (usuarioDesdeRepo == null)
            {
                return Unauthorized();
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuarioDesdeRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, usuarioDesdeRepo.UsuarioA.ToString())
            };

            // Generación de token

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
            var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor 
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credenciales
            };
            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok( new { token = tokenHandler.WriteToken(token) } );
        }
    }
}
