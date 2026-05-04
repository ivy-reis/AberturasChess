namespace LivroAberturasAPI.Models;

public class Partida
{
    public int Id { get; set; }
    
    // Campos novos que o C# pediu:
    public string LinkPartida { get; set; } = string.Empty;
    public string Resultado { get; set; } = string.Empty;

    // Relacionamentos
    public int VarianteId { get; set; }
    public Variante? Variante { get; set; }
    
    // O 1:1 com a precisão
    public Precisao? Precisao { get; set; }
}