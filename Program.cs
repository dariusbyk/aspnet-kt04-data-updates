using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TodoDb>(options =>
{
    options.UseInMemoryDatabase("TodoList");
});

var app = builder.Build();

app.MapGet("/todoitems", async (TodoDb db) =>
{
    return await db.Todos.ToListAsync();
});

app.MapGet("/todoitems/complete", async (TodoDb db) =>
{
    return await db.Todos.Where(todo => todo.IsComplete).ToListAsync();
});

app.MapGet("/todoitems/{id}", async Task<IResult> (int id, TodoDb db) =>
{
    Todo todo = await db.Todos.FindAsync(id);

    if (todo == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(todo);
});

app.MapPost("/todoitems", async (Todo inputTodo, TodoDb db) =>
{
    Todo todo = new Todo();
    todo.Name = inputTodo.Name;
    todo.IsComplete = inputTodo.IsComplete;

    db.Todos.Add(todo);
    await db.SaveChangesAsync();

    return Results.Created("/todoitems/" + todo.Id, todo);
});

app.MapPut("/todoitems/{id}", async Task<IResult> (int id, Todo inputTodo, TodoDb db) =>
{
    Todo todo = await db.Todos.FindAsync(id);

    if (todo == null)
    {
        return Results.NotFound();
    }

    todo.Name = inputTodo.Name;
    todo.IsComplete = inputTodo.IsComplete;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/todoitems/{id}", async Task<IResult> (int id, TodoDb db) =>
{
    Todo todo = await db.Todos.FindAsync(id);

    if (todo == null)
    {
        return Results.NotFound();
    }

    db.Todos.Remove(todo);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();
