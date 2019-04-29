using LG.Main.Host.Web.Models;
using LG.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Droplet.WebApi.Model;

namespace LG.Main.Host.Web.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        private readonly ILogger _logger;
        private readonly IErrorInfoBuilder _errorInfoBuilder;
        public ExceptionFilter(ILogger<ExceptionFilter> logger, IErrorInfoBuilder errorInfoBuilder)
        {
            _logger = logger;
            _errorInfoBuilder = errorInfoBuilder;
        }

        public void OnException(ExceptionContext context)
        {
            HandleAndWrapException(context);
        }

        private void HandleAndWrapException(ExceptionContext context)
        {
            var result = _errorInfoBuilder.BuildForException(context.Exception);
            _logger.LogError(context.Exception, $"服务器发生错误,Code:{result.Code},Message:{result.Message}");
            context.Result = new ObjectResult(
                new Response(result.Message, result.Code)
            );

            context.Exception = null; //Handled!
        }

    }
}
