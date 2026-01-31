namespace TodoList.API.Exceptions;

public class NotFoundException(string message) : Exception(message);