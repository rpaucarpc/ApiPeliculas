using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos;
using ApiPeliculas.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeliculaController : Controller
    {
        private readonly IPeliculaRepository _peliculaRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public PeliculaController(IPeliculaRepository peliculaRepository, IMapper mapper, IWebHostEnvironment hostingEnvironment)
        {
            _peliculaRepository = peliculaRepository;
            _mapper = mapper;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        public IActionResult GetPeliculas()
        {
            var listaPeliculas = _peliculaRepository.GetPeliculas();
            var listaPeliculasDto = new List<PeliculaDto>();

            foreach (var Pelicula in listaPeliculas)
            {
                listaPeliculasDto.Add(_mapper.Map<PeliculaDto>(Pelicula));
            }

            return Ok(listaPeliculasDto);

        }

        [HttpGet("{PeliculaId:int}", Name ="GetPelicula")]
        public IActionResult GetPelicula(int PeliculaId)
        {
            var itemPelicula = _peliculaRepository.GetPelicula(PeliculaId);

            if (itemPelicula == null)
            {
                return NotFound();
            }

            var PeliculaDto = _mapper.Map<PeliculaDto>(itemPelicula);
            return Ok(PeliculaDto);
        }

        [HttpGet("GetPeliculasEnCategoria/{categoriaId:int}")]
        public IActionResult GetPeliculasEnCategoria(int categoriaId)
        {
            var listaPelicula = _peliculaRepository.GetPeliculasEnCategoria(categoriaId);
            if (listaPelicula == null)
            {
                return NotFound();
            }

            var itemPelicula = new List<PeliculaDto>();
            foreach (var item in listaPelicula)
            {
                itemPelicula.Add(_mapper.Map<PeliculaDto>(item));
            }
            return Ok(itemPelicula);
        }


        [HttpGet("Buscar")]
        public IActionResult Buscar(string nombrePelicula)
        {
            try
            {
                var resultado = _peliculaRepository.BuscarPelicula(nombrePelicula);
                if ( resultado.Any() )
                {
                    return Ok(resultado);
                }

                return NotFound();
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Error recuperando datos de la aplicación.");
            }
        }

        [HttpPost]
        public IActionResult CrearPelicula([FromForm]PeliculaCreateDto PeliculaDto)
        {
            if (PeliculaDto == null)
            {
                return BadRequest(ModelState);
            }

            if (_peliculaRepository.ExistePelicula(PeliculaDto.Nombre))
            {
                ModelState.AddModelError("", "La categoría ya existe");
                return StatusCode(404, ModelState);
            }

            // Subida de archivos
            var archivo = PeliculaDto.Foto;
            string rutaPrincipal = _hostingEnvironment.WebRootPath;
            var archivos = HttpContext.Request.Form.Files;

            if (archivo.Length > 0)
            {
                // Nueva imagen
                string nombreFoto = Guid.NewGuid().ToString();
                var subidas = Path.Combine(rutaPrincipal, @"fotos");
                var extension = Path.GetExtension(archivos[0].FileName);

                using (var fileStreams = new FileStream(Path.Combine(subidas, nombreFoto + extension), FileMode.Create))
                {
                    archivos[0].CopyTo(fileStreams);
                }
                PeliculaDto.RutaImagen = @"\fotos\" + nombreFoto + extension;
            }

            var Pelicula = _mapper.Map<Pelicula>(PeliculaDto);
            if(!_peliculaRepository.CrearPelicula(Pelicula))
            {
                ModelState.AddModelError("", $"Algo salio mal guardando el registro {Pelicula.Descripcion}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetPelicula", new { PeliculaId = Pelicula.Id}, Pelicula);
        }

        [HttpPatch("{PeliculaId:int}", Name = "ActualizarPelicula")]
        public IActionResult ActualizarPelicula(int PeliculaId, [FromBody] PeliculaDto PeliculaDto)
        {
            if ( PeliculaDto == null || PeliculaId != PeliculaDto.Id)
            {
                return BadRequest(ModelState);
            }

            var Pelicula = _mapper.Map<Pelicula>(PeliculaDto);
            if ( ! _peliculaRepository.ActualizarPelicula(Pelicula))
            {
                ModelState.AddModelError("", $"Algo salio mal actualizando el registro {Pelicula.Descripcion}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        [HttpDelete("{PeliculaId:int}", Name = "EliminarPelicula")]
        public IActionResult EliminarPelicula(int PeliculaId)
        {
            if (!_peliculaRepository.ExistePelicula(PeliculaId))
            {
                return NotFound();
            }

            var Pelicula = _peliculaRepository.GetPelicula(PeliculaId);

            if (!_peliculaRepository.BorrarPelicula(Pelicula))
            {
                ModelState.AddModelError("", $"Algo salio mal eliminando el registro {Pelicula.Descripcion}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
    }
}
