using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos;
using ApiPeliculas.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : Controller
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IMapper _mapper;
        public UsuariosController(IUsuarioRepository usuarioRepository, IMapper mapper)
        {
            _usuarioRepository = usuarioRepository;
            _mapper = mapper;
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

    }
}
