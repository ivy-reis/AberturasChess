using LivroAberturasAPI.Data;
using LivroAberturasAPI.DTOs;
using LivroAberturasAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LivroAberturasAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize] // 🔒 Protege o controller inteiro
public class VariantesController : ControllerBaseCustomizado
{
    private readonly AppDbContext _context;

    public VariantesController(AppDbContext context)
    {
        _context = context;
    }

    // ==========================================
    // LER (GET): Busca TODAS as variantes do usuário logado
    // ==========================================
    [HttpGet]
    public async Task<IActionResult> GetMinhasVariantes()
    {
        var usuarioId = ObterUsuarioIdLogado();

        var variantes = await _context.Variantes
            .Include(v => v.Abertura)
            .Where(v => v.Abertura!.UsuarioId == usuarioId)
            .ToListAsync();
            
        return Ok(variantes);
    }

    // ==========================================
    // LER (GET): Busca uma variante específica pelo ID
    // ==========================================
    [HttpGet("{id}")]
    public async Task<IActionResult> GetVariantePorId(int id)
    {
        var usuarioId = ObterUsuarioIdLogado();

        var variante = await _context.Variantes
            .Include(v => v.Abertura)
            .FirstOrDefaultAsync(v => v.Id == id && v.Abertura!.UsuarioId == usuarioId);

        if (variante == null) 
            return NotFound(new { erro = "Variante não encontrada ou acesso negado." });

        return Ok(variante);
    }

    // ==========================================
    // LER (GET): Busca as variantes de uma abertura específica
    // ==========================================
    [HttpGet("abertura/{aberturaId}")]
    public async Task<IActionResult> GetVariantesDaAbertura(int aberturaId)
    {
        var usuarioId = ObterUsuarioIdLogado();

        var aberturaExiste = await _context.Aberturas
            .AnyAsync(a => a.Id == aberturaId && a.UsuarioId == usuarioId);

        if (!aberturaExiste)
            return NotFound(new { erro = "Abertura não encontrada ou acesso negado." });

        var variantes = await _context.Variantes
            .Where(v => v.AberturaId == aberturaId)
            .ToListAsync();

        return Ok(variantes);
    }

    // ==========================================
    // CRIAR (POST)
    // ==========================================
    [HttpPost]
    public async Task<IActionResult> CriarVariante(VarianteDTO dto)
    {
        var usuarioId = ObterUsuarioIdLogado();

        var abertura = await _context.Aberturas
            .FirstOrDefaultAsync(a => a.Id == dto.AberturaId && a.UsuarioId == usuarioId);

        if (abertura == null)
            return BadRequest(new { erro = "Você só pode adicionar variantes às suas próprias aberturas." });

        var novaVariante = new Variante
        {
            AberturaId = dto.AberturaId,
            Nome = dto.Nome,
            Lances = dto.Lances 
        };

        _context.Variantes.Add(novaVariante);
        await _context.SaveChangesAsync();

        return Created($"/api/Variantes/{novaVariante.Id}", novaVariante);
    }

    // ==========================================
    // ATUALIZAR (PUT)
    // ==========================================
    [HttpPut("{id}")]
    public async Task<IActionResult> EditarVariante(int id, VarianteDTO dto)
    {
        var usuarioId = ObterUsuarioIdLogado();

        var variante = await _context.Variantes
            .Include(v => v.Abertura)
            .FirstOrDefaultAsync(v => v.Id == id && v.Abertura!.UsuarioId == usuarioId);

        if (variante == null) 
            return NotFound(new { erro = "Variante não encontrada ou sem permissão." });

        variante.Nome = dto.Nome;
        variante.Lances = dto.Lances;

        await _context.SaveChangesAsync();
        return Ok(variante);
    }

    // ==========================================
    // DELETAR (DELETE)
    // ==========================================
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletarVariante(int id)
    {
        var usuarioId = ObterUsuarioIdLogado();

        var variante = await _context.Variantes
            .Include(v => v.Abertura)
            .FirstOrDefaultAsync(v => v.Id == id && v.Abertura!.UsuarioId == usuarioId);

        if (variante == null)
            return NotFound(new { erro = "Variante não encontrada ou não pertence a você." });

        // Removida a trava de partidas. Deleção em cascata ativada!
        _context.Variantes.Remove(variante);
        await _context.SaveChangesAsync();

        return NoContent(); // HTTP 204
    }
}