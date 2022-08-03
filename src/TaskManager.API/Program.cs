var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<DatabaseConfig>(builder.Configuration.GetSection(nameof(DatabaseConfig)));
builder.Services.AddSingleton<IDatabaseConfig>(sp => sp.GetRequiredService<IOptions<DatabaseConfig>>().Value);
builder.Services.AddSingleton<ITarefasRepository, TarefasRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TaskManager.API", Version = "v1" });
});

var app = builder.Build();

app.MapGet("/", (ITarefasRepository _tarefasRepository) =>
{
    var tarefas = _tarefasRepository.Buscar().Result;
    return Results.Ok(tarefas);
});

app.MapGet("/{id}", (string id, ITarefasRepository _tarefasRepository) =>
{
    var tarefa = _tarefasRepository.Buscar(id).Result;

    if (tarefa == null)
        return Results.NotFound();

    return Results.Ok(tarefa);
});

app.MapPost("/adicionar/{id}", ([FromBody] TarefaInputModel novaTarefa, ITarefasRepository _tarefasRepository) =>
{
    var tarefa = new Tarefa(novaTarefa.Nome, novaTarefa.Detalhes);
    _tarefasRepository.Adicionar(tarefa);

    return Results.Created($"/adicionar/{tarefa.Id}", tarefa);
});

app.MapPut("/atualizar", (string id, [FromBody] TarefaInputModel tarefaAtualizada, ITarefasRepository _tarefasRepository) =>
{
    var tarefa = _tarefasRepository.Buscar(id).Result;

    if (tarefa is null)
        return Results.NotFound();

    tarefa.AtualizarTarefa(tarefaAtualizada.Nome, tarefaAtualizada.Detalhes, tarefaAtualizada.Concluido);

    _tarefasRepository.Atualizar(id, tarefa);

    return Results.Ok();
});

app.MapDelete("/remover/{id}", (string id, ITarefasRepository _tarefasRepository) =>
{
    var tarefa = _tarefasRepository.Buscar(id).Result;

    if (tarefa is null)
        return Results.NotFound();

    _tarefasRepository.Remover(id); 

    return Results.NoContent();
});

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskManager.API v1"));
}

app.Run();