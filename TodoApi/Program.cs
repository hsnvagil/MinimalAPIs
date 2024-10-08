using Microsoft.EntityFrameworkCore;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TodoDbContext>(opt => opt.UseInMemoryDatabase("TodoList"));

var app = builder.Build();

app.MapGet("/todoitems", async (TodoDbContext db) =>
    await db.TodoItems.ToListAsync());

app.MapGet("/todoitems/{id}", async (int id, TodoDbContext db) =>
    await db.TodoItems.FindAsync(id)
        is TodoItem todo
        ? Results.Ok(todo)
        : Results.NotFound());

app.MapPost("/todoitems", async (TodoItem todo, TodoDbContext db) =>
{
    db.TodoItems.Add(todo);
    await db.SaveChangesAsync();
    return Results.Created($"/todoitems/{todo.Id}", todo);
});

app.MapPut("/todoitems/{id}", async (int id, TodoItem inputTodo, TodoDbContext db) =>
{
    var todo = await db.TodoItems.FindAsync(id);

    if (todo is null) return Results.NotFound();

    todo.Name = inputTodo.Name;
    todo.IsCompleted = inputTodo.IsCompleted;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/todoitems/{id}", async (int id, TodoDbContext db) =>
{
    var todo = await db.TodoItems.FindAsync(id);

    if (todo is null) return Results.NotFound();

    db.TodoItems.Remove(todo);
    await db.SaveChangesAsync();
    return Results.Ok(todo);
});

app.UseHttpsRedirection();

app.Run();