namespace TaskManager.Data.Repositories;

public class TarefasRepository : ITarefasRepository
{
    private readonly IMongoCollection<Tarefa> _tarefas;

    public TarefasRepository(IDatabaseConfig databaseConfig)
    {
        var client = new MongoClient(databaseConfig.ConnectionString);
        var database = client.GetDatabase(databaseConfig.DatabaseName);

        _tarefas = database.GetCollection<Tarefa>("tarefas");
    }

    public async Task Adicionar(Tarefa tarefa)
    {
        await _tarefas.InsertOneAsync(tarefa);
    }

    public async Task Atualizar(string id, Tarefa tarefaAtualizada)
    {
        await _tarefas.ReplaceOneAsync(t => t.Id == id, tarefaAtualizada);
    }

    public async Task<IEnumerable<Tarefa>> Buscar()
    {
        return await _tarefas.Find(_ => true).ToListAsync();
    }

    public async Task<Tarefa> Buscar(string id)
    {
        return await _tarefas.Find(t => t.Id == id).FirstOrDefaultAsync();
    }

    public async Task Remover(string id)
    {
        await _tarefas.DeleteOneAsync(t => t.Id == id);
    }
}