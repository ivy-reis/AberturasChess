using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LivroAberturasAPI.Controllers;

// A classe é abstrata porque ela não é um endpoint real, serve apenas como base.
// Ela herda do ControllerBase padrão do .NET.
public abstract class ControllerBaseCustomizado : ControllerBase
{
    /// <summary>
    /// Extrai o ID do usuário autenticado a partir do Token JWT.
    /// É marcado como 'protected' para que apenas os Controllers que herdarem desta classe possam acessá-lo.
    /// </summary>
    protected int ObterUsuarioIdLogado()
    {
        // Tenta buscar o claim de ID (padrão NameIdentifier ou "id")
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                   ?? User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
                   
        if (string.IsNullOrEmpty(idClaim))
        {
            // Lança uma exceção caso o token não tenha o ID (medida extra de segurança)
            throw new UnauthorizedAccessException("Usuário não autenticado ou token inválido.");
        }

        return int.Parse(idClaim);
    }
}