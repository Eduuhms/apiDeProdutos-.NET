using System.ComponentModel.DataAnnotations.Schema;

namespace MyApi.Models;

public class Pedido
{
    public int Id { get; set; }
    public DateTime Data { get; set; } = DateTime.UtcNow;
    public string? Cliente { get; set; }

    [Column(TypeName = "TEXT")]
    public decimal Total { get; set; }

    public List<PedidoItem> Items { get; set; } = new();
}
