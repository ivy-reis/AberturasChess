namespace LivroAberturasAPI.Models;

public class Abertura
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    
    // Campo novo que o C# pediu:
    public string Cor { get; set; } = string.Empty; 

    // Relacionamentos
    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }
    public ICollection<Variante> Variantes { get; set; } = new List<Variante>();
}