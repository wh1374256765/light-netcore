using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using App.Server;
using App.Server.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace App.Common
{
    public class UploadFile
    {
        public static Dictionary<string, UploadFile> Files = new Dictionary<string, UploadFile>();

        public string Id { get; set; }
        public IFormFile File { get; set; }
        public int Chunk { get; set; }
        public int Chunks { get; set; }
        public string FileName { get; set; }
        public string DestPath { get; set; }
        public string OutPath { get; set; }

        public static UploadFile Add(string id, int chunk, int chunks, string filename, string dest_path, string outpath)
        {
            var upload_file = new UploadFile();

            upload_file.Id = id;
            upload_file.Chunk = chunk;
            upload_file.Chunks = chunks;
            upload_file.FileName = filename;
            upload_file.DestPath = dest_path;
            upload_file.OutPath = outpath;

            Files.Add(id, upload_file);

            return upload_file;
        }

        public static void Remove(string id) {
            if (!Files.ContainsKey(id))
            {
                return;
            }

            Files.Remove(id);
        }

        public static UploadFile Upload(string id, IFormFile file)
        {
            if (!Files.ContainsKey(id))
            {
                return null;
            }

            Files[id].File = file;
            Files[id].Chunk = Files[id].Chunk;

            var dest_path = Files[id].DestPath;

            using (var stream = file.OpenReadStream())
            {
                Ftp.GetDefault().Upload(dest_path, stream);
            }

            return Files[id];
        }
    }
}
