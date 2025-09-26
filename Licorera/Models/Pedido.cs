using System;
using System.Collections.Generic;

namespace Licorera.Models;

public partial class Pedido
{
    public int PedidoId { get; set; }

    public int ClienteId { get; set; }

    public DateTime? Fecha { get; set; }

    public decimal Total { get; set; }

    public string? Estado { get; set; }

    public bool EliminadoPorAdmin { get; set; } = false;

    public virtual Cliente Cliente { get; set; } = null!;

    public virtual ICollection<DetallePedido> DetallePedidos { get; set; } = new List<DetallePedido>();
}
