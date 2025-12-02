using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Data;
using MyApi.Models;

namespace MyApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PedidoController : ControllerBase
{
    private readonly AppDbContext _db;

    public PedidoController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Pedido>>> GetAll()
    {
        var pedidos = await _db.Pedidos.Include(p => p.Items).ThenInclude(i => i.Produto).AsNoTracking().ToListAsync();
        return Ok(pedidos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Pedido>> Get(int id)
    {
        var pedido = await _db.Pedidos.Include(p => p.Items).ThenInclude(i => i.Produto).FirstOrDefaultAsync(p => p.Id == id);
        if (pedido == null) return NotFound();
        return Ok(pedido);
    }

    [HttpPost]
    public async Task<ActionResult<Pedido>> CreatePedido([FromBody] Pedido pedidoInput)
    {
        if (pedidoInput == null || pedidoInput.Items == null || !pedidoInput.Items.Any())
            return BadRequest(new { error = "Pedido precisa ter ao menos um item." });

        using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var produtoIds = pedidoInput.Items.Select(i => i.ProdutoId).Distinct().ToList();
            var produtos = await _db.Produtos.Where(p => produtoIds.Contains(p.Id)).ToDictionaryAsync(p => p.Id);

            foreach (var item in pedidoInput.Items)
            {
                if (!produtos.ContainsKey(item.ProdutoId))
                    return BadRequest(new { error = $"Produto {item.ProdutoId} não encontrado." });
                if (item.Quantidade <= 0)
                    return BadRequest(new { error = "Quantidade deve ser maior que zero." });
                var produto = produtos[item.ProdutoId];
                if (item.Quantidade > produto.Estoque)
                    return BadRequest(new { error = $"Estoque insuficiente para produto {produto.Nome}. Disponível: {produto.Estoque}." });
            }

            var pedido = new Pedido
            {
                Cliente = pedidoInput.Cliente
            };
            _db.Pedidos.Add(pedido);
            await _db.SaveChangesAsync();

            decimal total = 0m;

            foreach (var item in pedidoInput.Items)
            {
                var produto = produtos[item.ProdutoId];
                var pedidoItem = new PedidoItem
                {
                    PedidoId = pedido.Id,
                    ProdutoId = produto.Id,
                    Quantidade = item.Quantidade,
                    PrecoUnitario = produto.Preco
                };
                _db.PedidoItems.Add(pedidoItem);

                produto.Estoque -= item.Quantidade;
                total += pedidoItem.PrecoUnitario * item.Quantidade;

                Console.WriteLine($"Agora resta {produto.Estoque} {produto.Nome} no estoque");
            }

            pedido.Total = total;
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            var created = await _db.Pedidos.Include(p => p.Items).ThenInclude(i => i.Produto).FirstOrDefaultAsync(p => p.Id == pedido.Id);
            return CreatedAtAction(nameof(Get), new { id = pedido.Id }, created);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
