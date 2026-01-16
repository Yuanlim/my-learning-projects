using System;

namespace MyNewApp.Classes;

using MyNewApp.Dtos;

public class RequestFunction
{
    public static Todo? FindTodoById(List<Todo> todos, int id)
    {
        foreach (var todo in todos)
        {
            if (todo.Id == id)
            {
                return todo;
            }
        }
        return null;
    }
}
