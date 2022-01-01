using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos;
using AutoMapper;

namespace ApiPeliculas.PeliculasMapper
{
    public class PeliculasMappers : Profile
    {
        public PeliculasMappers()
        {
            CreateMap<Categoria, CategoriaDto>().ReverseMap();
        }
    }
}
