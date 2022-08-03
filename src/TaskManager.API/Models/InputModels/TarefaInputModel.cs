namespace TaskManager.Models.InputModels;

public record TarefaInputModel
{
    public string Nome { get; set; }

    public string Detalhes { get; set; }

    public bool? Concluido { get; set; }
}