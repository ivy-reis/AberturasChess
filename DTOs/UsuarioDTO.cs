namespace LivroAberturasAPI.DTOs;
// filtragem de dados entre usuário e api
public class UsuarioDTO
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}