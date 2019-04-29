using LG.Main.Host.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace LG.Main.Host.Web.Filters
{
    public class ResultFilter : IResultFilter
    {

        public ResultFilter()
        {
        }

        public virtual void OnResultExecuting(ResultExecutingContext context)
        {
            StringBuilder errMessage = new StringBuilder();
            foreach (var state in context.ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    errMessage.AppendLine(error.ErrorMessage);
                }
            }
            if (errMessage.Length > 0)
            {
                context.Result = new ObjectResult(new Response(errMessage.ToString(), (int)HttpStatusCode.BadRequest));
                return;
            }
            if (context.Result is ObjectResult)
            {
                var result = context.Result as ObjectResult;
                context.Result = new ObjectResult(new Response(result.Value));
            }
            else if (context.Result is EmptyResult)
            {
                context.Result = new ObjectResult(new Response(default(object)));
            }
        }

        public virtual void OnResultExecuted(ResultExecutedContext context)
        {
            //no action
        }
    }
}
