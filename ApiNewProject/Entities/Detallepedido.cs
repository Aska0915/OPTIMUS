﻿using System;
using System.Collections.Generic;

namespace ApiNewProject.Entities
{
    public partial class Detallepedido
    {
        public int DetallePedidoId { get; set; }
        public int? PedidoId { get; set; }
        public int? ProductoId { get; set; }
        public int? UnidadId { get; set; }
        public int? Cantidad { get; set; }
        public decimal? PrecioUnitario { get; set; }

        public virtual Pedido? Pedido { get; set; }
        public virtual Producto? Producto { get; set; }
        public virtual Unidad? Unidad { get; set; }
    }
}
