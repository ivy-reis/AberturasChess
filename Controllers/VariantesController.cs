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
[Authorize] // 🔒 Protege o controller inteiro
public class VariantesController : ControllerBase
{
    private readonly AppDbContext _context;

    public VariantesController(AppDbContext context)
    {
        _context = context;
    }

    // Método Auxiliar: Pega o ID de dentro do Token
    private int ObterUsuarioIdLogado()
    {
        var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.Parse(idString!);
    }

    // GET: api/Variantes/abertura/5 (Busca as variantes de uma abertura específica)
    [HttpGet("abertura/{aberturaId}")]
    public async Task<IActionResult> GetVariantesDaAbertura(int aberturaId)
    {
        var usuarioId = ObterUsuarioIdLogado();

        // SEGURANÇA: Verifica se a abertura pertence ao usuário logado ANTES de mostrar as variantes
        var aberturaExiste = await _context.Aberturas
            .AnyAsync(a => a.Id == aberturaId && a.UsuarioId == usuarioId);

        if (!aberturaExiste)
            return NotFound(new { erro = "Abertura não encontrada ou acesso negado." });

        var variantes = await _context.Variantes
            .Where(v => v.AberturaId == aberturaId)
            // .Include(v => v.Partidas) // Descomente se quiser trazer o histórico de partidas junto
            .ToListAsync();

        return Ok(variantes);
    }

    // POST: api/Variantes
    [HttpPost]
    public async Task<IActionResult> CriarVariante(VarianteDTO dto)
    {
        var usuarioId = ObterUsuarioIdLogado();

        // SEGURANÇA CRÍTICA: O usuário não pode injetar uma variante na abertura de outro jogador!
        var abertura = await _context.Aberturas
            .FirstOrDefaultAsync(a => a.Id == dto.AberturaId && a.UsuarioId == usuarioId);

        if (abertura == null)
            return BadRequest(new { erro = "Você só pode adicionar variantes às suas próprias aberturas." });

        var novaVariante = new Variante
        {
            AberturaId = dto.AberturaId,
            Nome = dto.Nome,
            Lances = dto.Lances // Adicionei essa propriedade imaginando a sua modelagem, ajuste se precisar!
        };

        _context.Variantes.Add(novaVariante);
        await _context.SaveChangesAsync();

        return Created($"/api/Variantes/{novaVariante.Id}", novaVariante);
    }

    // GET: api/Variantes
    [HttpGet]
    public async Task<IActionResult> GetTodasAsVariantes()
    {
        // Busca todas as variantes cadastradas no banco
        var variantes = await _context.Variantes.ToListAsync();
        
        return Ok(variantes); // HTTP 200
    }

    // DELETE: api/Variantes/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletarVariante(int id)
    {
        var usuarioId = ObterUsuarioIdLogado();

        // Busca a variante INCLUINDO a Abertura pai para checar quem é o dono
        var variante = await _context.Variantes
            .Include(v => v.Abertura)
            .FirstOrDefaultAsync(v => v.Id == id && v.Abertura.UsuarioId == usuarioId);

        if (variante == null)
            return NotFound(new { erro = "Variante não encontrada ou não pertence a você." });

        _context.Variantes.Remove(variante);
        await _context.SaveChangesAsync();

        return NoContent(); // Retorna 204 sem conteúdo, exigência da rubrica
    }
}