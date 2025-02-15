﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ApiNewProject.Entities
{
    public partial class Presentacion
    {
        public Presentacion()
        {
            Productos = new HashSet<Producto>();
        }

        public int PresentacionId { get; set; }
        public string? NombrePresentacion { get; set; }
        public string? Contenido { get; set; }
        public int? CantidadPorPresentacion { get; set; }
        public string? DescripcionPresentacion { get; set; }
        public ulong? EstadoPresentacion { get; set; }
        [JsonIgnore]
        public virtual ICollection<Producto> Productos { get; set; }
    }
}
