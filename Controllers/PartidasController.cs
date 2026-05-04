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
public class PartidasController : ControllerBase
{
    private readonly AppDbContext _context;

    public PartidasController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/Partidas/variante/5 (Busca as partidas de uma variante específica)
    [HttpGet("variante/{varianteId}")]
    public async Task<IActionResult> GetPartidas(int varianteId)
    {
        var partidas = await _context.Partidas
            .Where(p => p.VarianteId == varianteId)
            .Include(p => p.Precisao) // A MÁGICA DO 1:1: Traz a análise da engine junto com a partida
            .ToListAsync();

        return Ok(partidas);
    }

    // POST: api/Partidas (Cria a Partida E a Precisão juntas)
    [HttpPost]
    public async Task<IActionResult> RegistrarPartida(PartidaDTO dto)
    {
        // 1. Cria a entidade principal (Partida)
        var novaPartida = new Partida
        {
            VarianteId = dto.VarianteId,
            LinkPartida = dto.LinkPartida,
            Resultado = dto.Resultado
        };

        // 2. Cria a entidade dependente (Precisão) acoplada a ela (Relação 1:1)
        novaPartida.Precisao = new Precisao
        {
            PrecisaoGeral = dto.PrecisaoGeral,
            LancesBrilhantes = dto.LancesBrilhantes,
            Capivara = dto.Capivara
        };

        _context.Partidas.Add(novaPartida);
        await _context.SaveChangesAsync(); // Salva as duas tabelas de uma vez só!

        return Created($"/api/Partidas/{novaPartida.Id}", novaPartida);
    }
}