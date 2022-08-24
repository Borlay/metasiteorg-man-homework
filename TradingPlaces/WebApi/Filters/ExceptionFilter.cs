using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace TradingPlaces.WebApi.Filters
{
    public class ExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext exceptionContext)
        {
            if (exceptionContext.Exception is ArgumentException ae)
            {
                exceptionContext.Result = new ObjectResult(exceptionContext.Exception.Message)
                {
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }
            else
            {
                exceptionContext.Result = new ObjectResult(exceptionContext.Exception.Message)
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }

            base.OnException(exceptionContext);
        }
    }
}
