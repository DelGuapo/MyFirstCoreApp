﻿using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MyFirstCoreApp
{
    
    public class Uploadify
    {
        string uploadTarget;
        public Uploadify(string targetPath)
        {
            this.uploadTarget = targetPath;
        }

        public async Task<List<uploadStatus>> UploadFiles(List<IFormFile> files)
        {
            List<uploadStatus> uploads = new List<uploadStatus>();
            // full path to file in temp location
            
            foreach (var formFile in files)
            {
                var filePath = Path.GetTempFileName();
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    
                    string fileName = ContentDispositionHeaderValue.Parse(formFile.ContentDisposition).FileName.Trim('"');
                    uploadStatus file = new uploadStatus();
                    file.url = fileName;
                    file.status = formFile.CopyToAsync(stream).Wait;
                    uploads.Add(file);
                    
                }
            }

            return await uploads;
        }
        public class uploadStatus
        {
            public string url { get; set; }
            public Boolean status { get; set; }
            Boolean message { get; set; }
        }
    }
}