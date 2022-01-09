﻿using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using static ApiPeliculas.Models.Pelicula;

namespace ApiPeliculas.Models.Dtos
{
    public class PeliculaCreateDto
    {
        [Required(ErrorMessage ="El nombre es obligatorio")]
        public string Nombre { get; set; }

        public string RutaImagen { get; set; }

        public IFormFile Foto { get; set; }

        [Required(ErrorMessage = "La descripcion es obligatorio")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "La duración es obligatorio")]
        public string Duracion { get; set; }

        public TipoClasificacion Clasificacion { get; set; }

        public int CategoriaId { get; set; }

    }
}
