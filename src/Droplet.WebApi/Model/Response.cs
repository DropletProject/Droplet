using System;
using System.Collections.Generic;
using System.Text;

namespace Droplet.WebApi.Model
{
    public class Response
    {
        public Response(object data)
        {
            Code = 0;
            Data = data;
            Message = "成功";
        }
        public Response(string message, int code = 500)
        {
            Code = code;
            Message = message;
        }

        /// <summary>
        /// 错误码
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 结果
        /// </summary>
        public object Data { get; set; }

        public bool Success { get { return Code == 0; } }
    }
}
