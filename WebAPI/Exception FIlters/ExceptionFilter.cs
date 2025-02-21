using System.ComponentModel.DataAnnotations;
using Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebAPI;

internal class ExceptionFilter(ILogger<ExceptionFilter> logger) : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        logger.LogError(context.Exception, "API Error");
        
        if (context.Exception is AccountNotFoundException ex)
            context.Result = new NotFoundObjectResult(ex.Message);
        else if (context.Exception is ValidationException vex)
            context.Result = new BadRequestObjectResult(vex.Message);
        else
            context.Result = new ObjectResult(new
            {
                error = $"Internal server error: {context.Exception.Message}",
                StatusCode = 500
            });
        
        context.ExceptionHandled = true;
    }
}