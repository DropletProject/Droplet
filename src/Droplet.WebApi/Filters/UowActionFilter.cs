using Droplet.Data.Uow;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace LG.Main.Host.Web.Filters
{
    public class UowActionFilter : IAsyncActionFilter
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        public UowActionFilter(IUnitOfWork unitOfWork, ILogger<UowActionFilter> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var controllerActionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;

            if (controllerActionDescriptor == null)
            {
                await next();
                return;
            }

            var unitOfWorkAttr = controllerActionDescriptor.MethodInfo.GetCustomAttribute<UnitOfWorkAttribute>();

            if (unitOfWorkAttr == null)
            {
                await next();
                return;
            }

            if (unitOfWorkAttr.IsTransactional)
            {
                _unitOfWork.Begin();
            }

            var result = await next();
            if (result.Exception == null || result.ExceptionHandled)
            {
                await _unitOfWork.CompleteAsync();
            }
        }
    }
}
