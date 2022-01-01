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
    public class CategoriasController : Controller
    {
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IMapper _mapper;
        public CategoriasController(ICategoriaRepository categoriaRepository, IMapper mapper)
        {
            _categoriaRepository = categoriaRepository;
            _mapper = mapper;
        }

        [HttpGet]
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

        [HttpGet("{categoriaId:int}", Name ="GetCategoria")]
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

        [HttpPost]
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
    }
}
