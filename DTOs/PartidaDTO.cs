namespace LivroAberturasAPI.DTOs;

public class PartidaDTO
{
    public int VarianteId { get; set; }
    public string LinkPartida { get; set; } = string.Empty;
    public string Resultado { get; set; } = string.Empty; // "Vitoria", "Derrota", "Empate"
    
    // Dados para a relação 1:1 com Precisao
    public decimal PrecisaoGeral { get; set; }
    public int LancesBrilhantes { get; set; }
    public int Capivara { get; set; } // Blunders
}