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
public class AberturasController : ControllerBaseCustomizado
{
    private readonly AppDbContext _context;

    public AberturasController(AppDbContext context)
    {
        _context = context;
    }

    // ==========================================
    // GET: api/Aberturas (Lista só as do usuário logado)
    // ==========================================
    [HttpGet]
    public async Task<IActionResult> GetMinhasAberturas()
    {
        var usuarioId = ObterUsuarioIdLogado();

        var aberturas = await _context.Aberturas
            .Where(a => a.UsuarioId == usuarioId)
            .ToListAsync();

        return Ok(aberturas); // HTTP 200
    }

    // ==========================================
    // GET: api/Aberturas/{id} (Buscar apenas uma abertura)
    // ==========================================
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAberturaPorId(int id)
    {
        var usuarioId = ObterUsuarioIdLogado();

        var abertura = await _context.Aberturas
            .FirstOrDefaultAsync(a => a.Id == id && a.UsuarioId == usuarioId);

        if (abertura == null)
            return NotFound(new { erro = "Abertura não encontrada ou não pertence a você." }); // HTTP 404

        return Ok(abertura); // HTTP 200
    }

    // ==========================================
    // POST: api/Aberturas (Criar nova)
    // ==========================================
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

        return Created($"/api/Aberturas/{novaAbertura.Id}", novaAbertura); // HTTP 201
    }

    // ==========================================
    // PUT: api/Aberturas/{id} (Atualizar existente)
    // ==========================================
    [HttpPut("{id}")]
    public async Task<IActionResult> EditarAbertura(int id, AberturaDTO dto)
    {
        var usuarioId = ObterUsuarioIdLogado();
        
        var abertura = await _context.Aberturas
            .FirstOrDefaultAsync(a => a.Id == id && a.UsuarioId == usuarioId);

        if (abertura == null) 
            return NotFound(new { erro = "Abertura não encontrada." }); // HTTP 404

        abertura.Nome = dto.Nome;
        abertura.Cor = dto.Cor;

        await _context.SaveChangesAsync();
        return Ok(abertura); // HTTP 200
    }

    // ==========================================
    // DELETAR (DELETE)
    // ==========================================
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletarAbertura(int id)
    {
        var usuarioId = ObterUsuarioIdLogado();

        var abertura = await _context.Aberturas
            .FirstOrDefaultAsync(a => a.Id == id && a.UsuarioId == usuarioId);

        if (abertura == null)
            return NotFound(new { erro = "Abertura não encontrada ou não pertence a você." });

        // O EF Core cuidará de deletar as Variantes e as Partidas automaticamente em cascata
        _context.Aberturas.Remove(abertura);
        await _context.SaveChangesAsync();

        return NoContent(); // HTTP 204
    }
}