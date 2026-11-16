using RetroGames.Model.Model;

namespace RetroGames.Services;

public class JogoDetalheServiceModel
{
    public Jogo Jogo { get; set; } = new Jogo();
    public string NomePlataforma { get; set; } = string.Empty;
    public string DescricaoCondicao { get; set; } = string.Empty;
    public List<Genero> Generos { get; set; } = new List<Genero>();
}