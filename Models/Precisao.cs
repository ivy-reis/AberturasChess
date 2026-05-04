namespace LivroAberturasAPI.Models;

public class Precisao
{
    public int Id { get; set; }
    
    // Campos novos que o C# pediu:
    public decimal PrecisaoGeral { get; set; }
    public int LancesBrilhantes { get; set; }
    public int Capivara { get; set; }

    // Relacionamentos
    public int PartidaId { get; set; }
    public Partida? Partida { get; set; }
}