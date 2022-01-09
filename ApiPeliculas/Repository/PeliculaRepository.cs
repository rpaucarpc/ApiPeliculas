using ApiPeliculas.Data;
using ApiPeliculas.Models;
using ApiPeliculas.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Repository
{
    public class PeliculaRepositorio : IPeliculaRepository
    {
        private readonly ApplicationDbContext _bd;
        public PeliculaRepositorio(ApplicationDbContext bd)
        {
            _bd = bd;
        }

        public bool ActualizarPelicula(Pelicula pelicula)
        {
            _bd.Peliculas.Update(pelicula);
            return Guardar();
        }

        public bool BorrarPelicula(Pelicula pelicula)
        {
            _bd.Peliculas.Remove(pelicula);
            return Guardar();
        }

        public IEnumerable<Pelicula> BuscarPelicula(string nombre)
        {
            IQueryable<Pelicula> query = _bd.Peliculas;
            if (!string.IsNullOrEmpty(nombre))
            {
                query = query.Where( e => e.Nombre.Contains(nombre) || e.Descripcion.Contains(nombre));
            }
            return query.ToList();
        }

        public bool CrearPelicula(Pelicula pelicula)
        {
            _bd.Peliculas.Add(pelicula);
            return Guardar();
        }

        public bool ExistePelicula(string nombre)
        {
            bool valor = _bd.Peliculas.Any(c => c.Nombre.ToLower().Trim() == nombre);
            return valor;
        }

        public bool ExistePelicula(int peliculaId)
        {
            bool valor = _bd.Peliculas.Any(c => c.Id == peliculaId);
            return valor;
        }

        public Pelicula GetPelicula(int peliculaId)
        {
            return _bd.Peliculas
                .FirstOrDefault(x=>x.Id == peliculaId);
        }

        public ICollection<Pelicula> GetPeliculas()
        {
            return _bd.Peliculas.OrderBy(c => c.Nombre).ToList();
        }

        public ICollection<Pelicula> GetPeliculasEnCategoria(int categoriaId)
        {
            return _bd.Peliculas
                .Include(ca => ca.Categoria)
                .Where(ca => ca.CategoriaId == categoriaId).ToList();
        }

        public bool Guardar()
        {
            return _bd.SaveChanges() >= 0;
        }
    }
}
