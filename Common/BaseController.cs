using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using App.Server;
using App.Server.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Common
{
    public class BaseController : ControllerBase
    {
        /// <summary>
        /// 返回成功
        /// </summary>
        /// <param name="msg">消息</param>
        /// <returns></returns>
        public MessageModel<T> Success<T>(string msg)
        {
            return Message<T>(true, msg, default);
        }

        /// <summary>
        /// 返回成功
        /// </summary>
        /// <param name="msg">消息</param>
        /// <param name="response">数据</param>
        /// <returns></returns>
        public MessageModel<T> Success<T>(string msg, T response)
        {
            return Message(true, msg, response);
        }

        /// <summary>
        /// 返回失败
        /// </summary>
        /// <param name="msg">消息</param>
        /// <returns></returns>
        public MessageModel<T> Fail<T>(string msg)
        {
            return Message<T>(false, msg, default);
        }
        /// <summary>
        /// 返回失败
        /// </summary>
        /// <param name="msg">消息</param>
        /// <param name="response">数据</param>
        /// <returns></returns>
        public MessageModel<T> Fail<T>(string msg, T response)
        {
            return Message(false, msg, response);
        }

        /// <summary>
        /// 返回消息
        /// </summary>
        /// <param name="success">失败/成功</param>
        /// <param name="msg">消息</param>
        /// <param name="response">数据</param>
        /// <returns></returns>
        public MessageModel<T> Message<T>(bool success, string msg, T response)
        {
            var data = new MessageModel<T>() { msg = msg, response = response, success = success };

            Logger.Info($"response data: {JsonConvert.SerializeObject(data)}");

            return data;
        }
    }
}
