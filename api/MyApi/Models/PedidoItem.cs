using System.ComponentModel.DataAnnotations.Schema;

namespace MyApi.Models;

public class PedidoItem
{
    public int Id { get; set; }
    public int PedidoId { get; set; }
    public Pedido? Pedido { get; set; }

    public int ProdutoId { get; set; }
    public Produto? Produto { get; set; }

    public int Quantidade { get; set; }

    [Column(TypeName = "TEXT")]
    public decimal PrecoUnitario { get; set; }
}
