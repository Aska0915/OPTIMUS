﻿namespace VistaNewProject.Models
{
    public class Pedido
    {
        public int PedidoId { get; set; }
        public int? ClienteId { get; set; }
        public string? TipoServicio { get; set; }
        public DateTime? FechaPedido { get; set; }
        public decimal? ValorTotalPedido { get; set; }
        public string? EstadoPedido { get; set; }


        public virtual Cliente? Cliente { get; set; }
        public virtual ICollection<Domicilio> Domicilios { get; set; }

    }
}
