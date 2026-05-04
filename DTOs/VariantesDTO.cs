namespace LivroAberturasAPI.DTOs;

public class VarianteDTO
{
    public int AberturaId { get; set; }
    public string Nome { get; set; } = string.Empty; // Ex: "Variante Najdorf"
    public string Lances { get; set; } = string.Empty; // Ex: "1.e4 c5 2.Nf3 d6..."
}