namespace SchoolAppointmentApp.Entities;

public record ErrorType(
  string Title,
  string Message,
  int StatusCode,
  string TraceId
);