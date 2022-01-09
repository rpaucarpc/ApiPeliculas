using ApiPeliculas.Models;
using System.Collections.Generic;

namespace ApiPeliculas.Repository.IRepository
{
    public interface IPeliculaRepository
    {
        ICollection<Pelicula> GetPeliculas();
        ICollection<Pelicula> GetPeliculasEnCategoria(int CategoriaId);
        Pelicula GetPelicula(int peliculaId);
        bool ExistePelicula(string nombre);
        IEnumerable<Pelicula> BuscarPelicula(string nombre);
        bool ExistePelicula(int peliculaId);
        bool CrearPelicula(Pelicula pelicula);
        bool ActualizarPelicula(Pelicula pelicula);
        bool BorrarPelicula(Pelicula pelicula);
        bool Guardar();
    }
}
