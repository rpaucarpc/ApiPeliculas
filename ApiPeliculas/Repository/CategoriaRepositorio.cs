using ApiPeliculas.Data;
using ApiPeliculas.Models;
using ApiPeliculas.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Repository
{
    public class CategoriaRepositorio : ICategoriaRepository
    {
        private readonly ApplicationDbContext _bd;
        public CategoriaRepositorio(ApplicationDbContext bd)
        {
            _bd = bd;
        }
        public bool ActualizarCategoria(Categoria categoria)
        {
            _bd.Categorias.Update(categoria);
            return Guardar();
        }

        public bool BorrarCategoria(Categoria categoria)
        {
            _bd.Categorias.Remove(categoria);
            return Guardar();
        }

        public bool CrearCategoria(Categoria categoria)
        {
            _bd.Categorias.Add(categoria);
            return Guardar();
        }

        public bool ExisteCategoria(string nombreCategoria)
        {
            bool valor = _bd.Categorias.Any(c => c.Nombre.ToLower().Trim() == nombreCategoria.ToLower().Trim());
            return valor;
        }

        public bool ExisteCategoria(int categoriaId)
        {
            bool valor = _bd.Categorias.Any(c => c.Id == categoriaId);
            return valor;
        }

        public Categoria GetCategoria(int categoriaId)
        {
            return _bd.Categorias.FirstOrDefault(c=>c.Id == categoriaId);
        }

        public ICollection<Categoria> GetCategorias()
        {
            return _bd.Categorias.OrderBy(c=>c.Nombre).ToList();
        }

        public bool Guardar()
        {
            return _bd.SaveChanges() >= 0;
        }
    }
}
