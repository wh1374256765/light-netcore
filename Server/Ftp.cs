using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;

namespace App.Server
{
    public class Ftp
    {
        static Dictionary<string, Ftp> ftps = new Dictionary<string, Ftp>();

        static Ftp defaultFtp;

        static public void Init(IConfiguration config)
        {
            var configs = config.GetSection("Ftp").Get<List<FtpConfig>>();

            configs.ForEach(config =>
            {
                Ftp ftp = new Ftp();
                ftp.Option = config;

                Logger.Info($"ftp [{config.Name}] connected.");

                ftps[config.Name] = ftp;

                if (config.IsDefault)
                {
                    defaultFtp = ftp;
                }
            });
        }

        static public Ftp Get(string name)
        {
            return ftps[name];
        }

        static public Ftp GetDefault()
        {
            return defaultFtp;
        }

        public FtpConfig Option { get; set; }

        public Stream GetStream(string filepath)
        {
            var folder = filepath.Substring(0, filepath.LastIndexOf('/') + 1);
            var folders = folder.Split('/').ToList();
            folders = folders.Where(x => x != "").ToList();

            var valid_folders = new List<string>();
            folders.ForEach(x =>
            {
                var folder_path = string.Join("/", valid_folders.ToArray());
                if (!this.ExistFolder(folder_path, x))
                {
                    this.CreateFolder(folder_path + '/' + x);
                }
                valid_folders.Add(x);
            });

            var request = (FtpWebRequest)FtpWebRequest.Create(this.Option.Server + "/" + filepath);
            request.Credentials = new NetworkCredential(this.Option.UserName, this.Option.Password);
            request.KeepAlive = true;            
            request.Method = WebRequestMethods.Ftp.AppendFile;
            request.UseBinary = true;

            var stream = request.GetRequestStream();
            
            return stream;
        }

        public void Upload(string filepath, Stream stream)
        {

            var folder = filepath.Substring(0, filepath.LastIndexOf('/') + 1);
            var folders = folder.Split('/').ToList();
            folders = folders.Where(x => x != "").ToList();

            var valid_folders = new List<string>();
            folders.ForEach(x =>
            {
                var folder_path = string.Join("/", valid_folders.ToArray());
                if (!this.ExistFolder(folder_path, x))
                {
                    this.CreateFolder(folder_path + '/' + x);
                }
                valid_folders.Add(x);
            });

            var request = (FtpWebRequest)FtpWebRequest.Create(this.Option.Server + "/" + filepath);
            request.Credentials = new NetworkCredential(this.Option.UserName, this.Option.Password);
            request.KeepAlive = false;
            request.Method = WebRequestMethods.Ftp.AppendFile;
            request.UseBinary = true;

            using (var request_stream = request.GetRequestStream())
            {
                stream.CopyTo(request_stream);
                stream.Close();
            }
        }

        public bool ExistFolder(string path, string folder)
        {
            var request = (FtpWebRequest)FtpWebRequest.Create(this.Option.Server + "/" + path);

            request.Credentials = new NetworkCredential(this.Option.UserName, this.Option.Password);
            request.Method = WebRequestMethods.Ftp.ListDirectory;
            WebResponse response = request.GetResponse();
            using (var stream = new StreamReader(response.GetResponseStream()))
            {
                var result = stream.ReadToEnd();
                return result.Contains(folder);
            }
        }

        public void CreateFolder(string folder_path)
        {
            var request = (FtpWebRequest)FtpWebRequest.Create(this.Option.Server + "/" + folder_path);

            request.Credentials = new NetworkCredential(this.Option.UserName, this.Option.Password);
            request.Method = WebRequestMethods.Ftp.MakeDirectory;

            request.GetResponse();
        }

        public Stream GetFile(string file_path)
        {
            var request = (FtpWebRequest)FtpWebRequest.Create(this.Option.Server + "/" + file_path);

            request.Credentials = new NetworkCredential(this.Option.UserName, this.Option.Password);
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            var memory_stream = new MemoryStream();
            var stream = request.GetResponse().GetResponseStream();
            stream.CopyTo(memory_stream);
            stream.Dispose();

            return memory_stream;
        }
    }

    public class FtpConfig
    {
        public string Name { get; set; }

        public string Server { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public Boolean IsDefault { get; set; }
    }
}
