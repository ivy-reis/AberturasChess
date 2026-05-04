using LivroAberturasAPI.Data;
using LivroAberturasAPI.DTOs;
using LivroAberturasAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LivroAberturasAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize] // 🔒 TRANCA O CONTROLLER INTEIRO: Só entra quem tem Token válido
public class AberturasController : ControllerBase
{
    private readonly AppDbContext _context;

    public AberturasController(AppDbContext context)
    {
        _context = context;
    }

    // Método Auxiliar: Pega o ID de dentro do Token
    private int ObterUsuarioIdLogado()
    {
        var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.Parse(idString!);
    }

    // GET: api/Aberturas (Lista só as do usuário logado)
    [HttpGet]
    public async Task<IActionResult> GetMinhasAberturas()
    {
        var usuarioId = ObterUsuarioIdLogado();

        var aberturas = await _context.Aberturas
            .Where(a => a.UsuarioId == usuarioId)
            // .Include(a => a.Variantes) // Descomente se quiser trazer as variantes juntas
            .ToListAsync();

        return Ok(aberturas); // HTTP 200
    }

    // POST: api/Aberturas (Criar nova)
    [HttpPost]
    public async Task<IActionResult> CriarAbertura(AberturaDTO dto)
    {
        var usuarioId = ObterUsuarioIdLogado();

        var novaAbertura = new Abertura
        {
            Nome = dto.Nome,
            Cor = dto.Cor,
            UsuarioId = usuarioId // Amarra a abertura ao dono do token!
        };

        _context.Aberturas.Add(novaAbertura);
        await _context.SaveChangesAsync();

        return Created($"/api/Aberturas/{novaAbertura.Id}", novaAbertura); // HTTP 201 (Exigência da Rubrica)
    }

    // DELETE: api/Aberturas/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletarAbertura(int id)
    {
        var usuarioId = ObterUsuarioIdLogado();

        var abertura = await _context.Aberturas
            .FirstOrDefaultAsync(a => a.Id == id && a.UsuarioId == usuarioId); // Garante que a abertura é dele

        if (abertura == null)
            return NotFound(new { erro = "Abertura não encontrada ou não pertence a você." }); // HTTP 404

        _context.Aberturas.Remove(abertura);
        await _context.SaveChangesAsync();

        return NoContent(); // HTTP 204 (Exigência da Rubrica: Sucesso sem devolver dados)
    }
}