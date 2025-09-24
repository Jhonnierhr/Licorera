namespace Licorera.Models
{
    public class CarritoItem
    {
        public int ProductoId { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string? Categoria { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
        public decimal Subtotal => Precio * Cantidad;
        public int StockDisponible { get; set; }
        public DateTime FechaAgregado { get; set; } = DateTime.Now;
    }

    public class Carrito
    {
        public List<CarritoItem> Items { get; set; } = new List<CarritoItem>();
        public decimal Total => Items.Sum(i => i.Subtotal);
        public int TotalItems => Items.Sum(i => i.Cantidad);
        public int TotalProductos => Items.Count;

        public void AgregarItem(CarritoItem item)
        {
            var itemExistente = Items.FirstOrDefault(i => i.ProductoId == item.ProductoId);
            if (itemExistente != null)
            {
                itemExistente.Cantidad += item.Cantidad;
            }
            else
            {
                Items.Add(item);
            }
        }

        public void EliminarItem(int productoId)
        {
            Items.RemoveAll(i => i.ProductoId == productoId);
        }

        public void ActualizarCantidad(int productoId, int nuevaCantidad)
        {
            var item = Items.FirstOrDefault(i => i.ProductoId == productoId);
            if (item != null)
            {
                if (nuevaCantidad <= 0)
                {
                    EliminarItem(productoId);
                }
                else
                {
                    item.Cantidad = nuevaCantidad;
                }
            }
        }

        public void VaciarCarrito()
        {
            Items.Clear();
        }
    }
}