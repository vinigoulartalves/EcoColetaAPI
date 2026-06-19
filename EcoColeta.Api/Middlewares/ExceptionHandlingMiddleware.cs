using System.Net;
using System.Text.Json;
using EcoColeta.Api.Exceptions;
using EcoColeta.Api.ViewModels.Responses;

namespace EcoColeta.Api.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await TratarExcecaoAsync(context, ex);
        }
    }

    private async Task TratarExcecaoAsync(HttpContext context, Exception ex)
    {
        var (statusCode, mensagem) = ex switch
        {
            NotFoundException => (HttpStatusCode.NotFound, ex.Message),
            BusinessException => (HttpStatusCode.BadRequest, ex.Message),
            UnauthorizedAppException => (HttpStatusCode.Unauthorized, ex.Message),
            _ => (HttpStatusCode.InternalServerError, "Ocorreu um erro interno no servidor.")
        };

        if (statusCode == HttpStatusCode.InternalServerError)
            _logger.LogError(ex, "Erro não tratado: {Mensagem}", ex.Message);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var resposta = new ErroResponse
        {
            StatusCode = (int)statusCode,
            Mensagem = mensagem,
            Detalhe = statusCode == HttpStatusCode.InternalServerError ? null : ex.Message
        };

        var json = JsonSerializer.Serialize(resposta, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
