using LivroAberturasAPI.Data;
using LivroAberturasAPI.DTOs;
using LivroAberturasAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LivroAberturasAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PartidasController : ControllerBaseCustomizado 
{
    private readonly AppDbContext _context;

    public PartidasController(AppDbContext context)
    {
        _context = context;
    }

    // ==========================================
    // LER (GET): Busca as partidas de uma variante específica
    // ==========================================
    [HttpGet("variante/{varianteId}")]
    public async Task<IActionResult> GetPartidas(int varianteId)
    {
        var usuarioId = ObterUsuarioIdLogado();

        var temPermissao = await _context.Variantes
            .Include(v => v.Abertura)
            .AnyAsync(v => v.Id == varianteId && v.Abertura!.UsuarioId == usuarioId);

        if (!temPermissao) return Forbid(); 

        var partidas = await _context.Partidas
            .Where(p => p.VarianteId == varianteId)
            .Include(p => p.Precisao)
            .ToListAsync();

        return Ok(partidas);
    }

    // ==========================================
    // LER (GET): Busca uma única partida pelo ID
    // ==========================================
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPartidaPorId(int id)
    {
        var usuarioId = ObterUsuarioIdLogado();

        var partida = await _context.Partidas
            .Include(p => p.Variante)
                .ThenInclude(v => v!.Abertura)
            .Include(p => p.Precisao)
            // Checagem de segurança: Garante que a partida pertence a uma variante de uma abertura do usuário logado
            .FirstOrDefaultAsync(p => p.Id == id && p.Variante!.Abertura!.UsuarioId == usuarioId);

        if (partida == null) return NotFound(new { erro = "Partida não encontrada ou sem permissão." });

        return Ok(partida);
    }

    // ==========================================
    // CRIAR (POST): Registra Partida e Precisão juntas
    // ==========================================
    [HttpPost]
    public async Task<IActionResult> RegistrarPartida(PartidaDTO dto)
    {
        var usuarioId = ObterUsuarioIdLogado();

        var temPermissao = await _context.Variantes
            .Include(v => v.Abertura)
            .AnyAsync(v => v.Id == dto.VarianteId && v.Abertura!.UsuarioId == usuarioId);

        if (!temPermissao) return BadRequest(new { erro = "Operação negada. Variante inválida." });

        var novaPartida = new Partida
        {
            VarianteId = dto.VarianteId,
            LinkPartida = dto.LinkPartida,
            Resultado = dto.Resultado,
            Precisao = new Precisao
            {
                PrecisaoGeral = dto.PrecisaoGeral,
                LancesBrilhantes = dto.LancesBrilhantes,
                Capivara = dto.Capivara
            }
        };

        _context.Partidas.Add(novaPartida);
        await _context.SaveChangesAsync(); 

        return Created($"/api/Partidas/{novaPartida.Id}", novaPartida);
    }

    // ==========================================
    // ATUALIZAR (PUT): Edita dados da Partida e da Precisão
    // ==========================================
    [HttpPut("{id}")]
    public async Task<IActionResult> AtualizarPartida(int id, PartidaDTO dto)
    {
        var usuarioId = ObterUsuarioIdLogado();

        var partida = await _context.Partidas
            .Include(p => p.Variante)
                .ThenInclude(v => v!.Abertura)
            .Include(p => p.Precisao)
            .FirstOrDefaultAsync(p => p.Id == id && p.Variante!.Abertura!.UsuarioId == usuarioId);

        if (partida == null) return NotFound(new { erro = "Partida não encontrada ou sem permissão." });

        // Atualiza a entidade principal
        partida.LinkPartida = dto.LinkPartida;
        partida.Resultado = dto.Resultado;

        // Atualiza a entidade dependente (1:1)
        if (partida.Precisao != null)
        {
            partida.Precisao.PrecisaoGeral = dto.PrecisaoGeral;
            partida.Precisao.LancesBrilhantes = dto.LancesBrilhantes;
            partida.Precisao.Capivara = dto.Capivara;
        }

        await _context.SaveChangesAsync();
        return Ok(partida);
    }

    // ==========================================
    // DELETAR (DELETE): Remove a Partida e a Precisão em cascata
    // ==========================================
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletarPartida(int id)
    {
        var usuarioId = ObterUsuarioIdLogado();

        var partida = await _context.Partidas
            .Include(p => p.Variante)
                .ThenInclude(v => v!.Abertura)
            .FirstOrDefaultAsync(p => p.Id == id && p.Variante!.Abertura!.UsuarioId == usuarioId);

        if (partida == null) return NotFound(new { erro = "Partida não encontrada ou sem permissão." });

        // O EF Core cuidará de deletar a 'Precisao' automaticamente em cascata
        _context.Partidas.Remove(partida);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}