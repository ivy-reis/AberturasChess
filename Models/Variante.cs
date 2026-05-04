namespace LivroAberturasAPI.Models;

public class Variante
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    
    // Campo novo que o C# pediu:
    public string Lances { get; set; } = string.Empty;

    // Relacionamentos
    public int AberturaId { get; set; }
    public Abertura? Abertura { get; set; }
    public ICollection<Partida> Partidas { get; set; } = new List<Partida>();
}