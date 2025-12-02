using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Data;
using MyApi.Models;

namespace MyApi.Controllers;

[ApiController]
[Route("api/produtos")]
public class ProdutosController : ControllerBase
{
    private readonly AppDbContext _db;

    public ProdutosController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Produto>>> GetAll()
    {
        var produtos = await _db.Produtos.AsNoTracking().ToListAsync();
        return Ok(produtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Produto>> Get(int id)
    {
        var produto = await _db.Produtos.FindAsync(id);
        if (produto == null) return NotFound();
        return Ok(produto);
    }

    [HttpPost]
    public async Task<ActionResult<Produto>> Create(Produto produto)
    {
        _db.Produtos.Add(produto);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = produto.Id }, produto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Produto updated)
    {
        if (id != updated.Id) return BadRequest();
        var exists = await _db.Produtos.AnyAsync(p => p.Id == id);
        if (!exists) return NotFound();
        _db.Entry(updated).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var produto = await _db.Produtos.FindAsync(id);
        if (produto == null) return NotFound();
        _db.Produtos.Remove(produto);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPatch("{id}/adicionar-estoque/{quantidade}")]
    public async Task<ActionResult<Produto>> AdicionarEstoque(int id, int quantidade)
    {
        if (quantidade <= 0)
            return BadRequest(new { error = "Quantidade deve ser maior que zero." });

        var produto = await _db.Produtos.FindAsync(id);
        if (produto == null) return NotFound();

        produto.Estoque += quantidade;
        await _db.SaveChangesAsync();

        return Ok(produto);
    }

    [HttpPatch("{id}/remover-estoque/{quantidade}")]
    public async Task<ActionResult<Produto>> RemoverEstoque(int id, int quantidade)
    {
        if (quantidade <= 0)
            return BadRequest(new { error = "Quantidade deve ser maior que zero." });

        var produto = await _db.Produtos.FindAsync(id);
        if (produto == null) return NotFound();

        if (quantidade > produto.Estoque)
        {
            // Sempre escrever a mensagem no console com o estoque atual
            Console.WriteLine($"Agora resta {produto.Estoque} {produto.Nome} no estoque");
            return BadRequest(new { error = "Quantidade solicitada é maior que o estoque disponível." });
        }

        produto.Estoque -= quantidade;
        await _db.SaveChangesAsync();

        // Mensagem solicitada no console
        Console.WriteLine($"Agora resta {produto.Estoque} {produto.Nome} no estoque");

        return Ok(produto);
    }
}
