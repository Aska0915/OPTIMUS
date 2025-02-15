﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ApiNewProject.Entities
{
    public partial class Lote
    {
        public int LoteId { get; set; }
        public int? DetalleCompraId { get; set; }
        public int? ProductoId { get; set; }
        public string? NumeroLote { get; set; }
        public decimal? PrecioCompra { get; set; }
        public decimal? PrecioPorUnidad { get; set; }
        public decimal? PrecioPorPresentacion { get; set; }
        public decimal? PrecioPorUnidadProducto { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public int? Cantidad { get; set; }
        public ulong? EstadoLote { get; set; }
        [JsonIgnore]
        public virtual Detallecompra? DetalleCompra { get; set; }
        [JsonIgnore]
        public virtual Producto? Producto { get; set; }
    }
}
