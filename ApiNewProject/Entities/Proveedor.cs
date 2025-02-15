﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ApiNewProject.Entities
{
    public partial class Proveedor
    {
        public Proveedor()
        {
            Compras = new HashSet<Compra>();
        }

        public int ProveedorId { get; set; }
        public string? NombreEmpresa { get; set; }
        public string? NombreContacto { get; set; }
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
        public string? Correo { get; set; }
        public ulong? EstadoProveedor { get; set; }
        [JsonIgnore]
        public virtual ICollection<Compra> Compras { get; set; }
    }
}
