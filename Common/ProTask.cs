using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using App.Server;
using App.Server.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace App.Common
{
    public class ProTask
    {
        public static Dictionary<string, ProTask> Tasks = new Dictionary<string, ProTask>();

        public string Id { get; set; }
        public Action<Dictionary<string, object>> Action { get; set; }
        public Dictionary<string, object> Context = new Dictionary<string, object>();

        public static ProTask Add(string id, Action<Dictionary<string, object>> action)
        {
            var pro_task = new ProTask();

            pro_task.Id = id;
            pro_task.Action = action;

            Tasks.Add(id, pro_task);

            return pro_task;
        }

        public static ProTask Start(string id)
        {
            if (!Tasks.ContainsKey(id))
            {
                return null;
            }

            var pro_task = Tasks[id];

            Task.Run(() =>
            {
                pro_task.Action(pro_task.Context);
            });

            return pro_task;
        }

        public static ProTask Get(string id)
        {
            if (!Tasks.ContainsKey(id))
            {
                return null;
            }

            return Tasks[id];
        }

        public static void Remove(string id)
        {
            if (Tasks.ContainsKey(id))
            {
                Tasks.Remove(id);
            }
        }
    }
}
