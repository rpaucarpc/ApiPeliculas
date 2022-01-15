using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos;
using ApiPeliculas.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace ApiPeliculas.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName ="ApiPeliculasCategorias")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class CategoriasController : Controller
    {
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IMapper _mapper;
        public CategoriasController(ICategoriaRepository categoriaRepository, IMapper mapper)
        {
            _categoriaRepository = categoriaRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtener todas las categorias
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(200, Type =typeof(List<CategoriaDto>))]
        [ProducesResponseType(400)]
        public IActionResult GetCategorias()
        {
            var listaCategorias = _categoriaRepository.GetCategorias();
            var listaCategoriasDto = new List<CategoriaDto>();

            foreach (var categoria in listaCategorias)
            {
                listaCategoriasDto.Add(_mapper.Map<CategoriaDto>(categoria));
            }

            return Ok(listaCategoriasDto);

        }
        /// <summary>
        /// Obtener una categoria individual
        /// </summary>
        /// <param name="categoriaId">Este es el id de la categoria</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("{categoriaId:int}", Name ="GetCategoria")]
        [ProducesResponseType(200, Type = typeof(CategoriaDto))]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public IActionResult GetCategoria(int categoriaId)
        {
            var itemCategoria = _categoriaRepository.GetCategoria(categoriaId);

            if (itemCategoria == null)
            {
                return NotFound();
            }

            var categoriaDto = _mapper.Map<CategoriaDto>(itemCategoria);
            return Ok(categoriaDto);
        }
        /// <summary>
        /// Crear una nueva categoria
        /// </summary>
        /// <param name="categoriaDto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(CategoriaDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public IActionResult CrearCategoria([FromBody]CategoriaDto categoriaDto)
        {
            if (categoriaDto == null)
            {
                return BadRequest(ModelState);
            }
            if (_categoriaRepository.ExisteCategoria(categoriaDto.Nombre))
            {
                ModelState.AddModelError("", "La categoría ya existe");
                return StatusCode(404, ModelState);
            }

            var categoria = _mapper.Map<Categoria>(categoriaDto);
            if(!_categoriaRepository.CrearCategoria(categoria))
            {
                ModelState.AddModelError("", $"Algo salio mal guardando el registro {categoria.Nombre}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetCategoria", new { categoriaId = categoria.Id}, categoria);
        }
        /// <summary>
        /// Actualizar una categoria existente
        /// </summary>
        /// <param name="categoriaId">Categoria Id</param>
        /// <param name="categoriaDto"></param>
        /// <returns></returns>
        [HttpPatch("{categoriaId:int}", Name = "ActualizarCategoria")]
        [ProducesResponseType(204)]
          [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ActualizarCategoria(int categoriaId, [FromBody] CategoriaDto categoriaDto)
        {
            if ( categoriaDto == null || categoriaId != categoriaDto.Id)
            {
                return BadRequest(ModelState);
            }

            var categoria = _mapper.Map<Categoria>(categoriaDto);
            if ( ! _categoriaRepository.ActualizarCategoria(categoria))
            {
                ModelState.AddModelError("", $"Algo salio mal actualizando el registro {categoria.Nombre}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        /// <summary>
        /// Borrar una categoria por Id
        /// </summary>
        /// <param name="categoriaId">Id de la categoria a eliminar</param>
        /// <returns></returns>
        [HttpDelete("{categoriaId:int}", Name = "EliminarCategoria")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public IActionResult EliminarCategoria(int categoriaId)
        {
            if (!_categoriaRepository.ExisteCategoria(categoriaId))
            {
                return NotFound();
            }

            var categoria = _categoriaRepository.GetCategoria(categoriaId);

            if (!_categoriaRepository.BorrarCategoria(categoria))
            {
                ModelState.AddModelError("", $"Algo salio mal eliminando el registro {categoria.Nombre}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
