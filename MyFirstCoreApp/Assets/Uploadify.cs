using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MyFirstCoreApp
{

    public class Uploadify
    {
        string uploadTarget;
        public List<uploadStatus> uploads;

        public Uploadify(string targetPath)
        {
            this.uploadTarget = targetPath;
        }

        public async Task<object> UploadFilesAsync(List<IFormFile> files)
        {
            uploads = new List<uploadStatus>();
            List<Task> tasks = new List<Task>();
            foreach (var formFile in files)
            {
                
                string fileName = ContentDispositionHeaderValue.Parse(formFile.ContentDisposition).FileName.Trim('"');
                uploadStatus file = new uploadStatus();
                file.url = fileName;
                string targetPath = uploadTarget + fileName;
                try
                {
                    using (var stream = new FileStream(targetPath, FileMode.Create))
                    {
                        string newFile = uploadTarget + fileName;
                        Task copyFile = formFile.CopyToAsync(stream);// <<==== why is this erroring out here.
                        tasks.Add(copyFile);
                        file.success = true;
                    }
                }
                catch(Exception err)
                {
                    file.success = false;
                    file.errorMessage = err.Message;
                }
                uploads.Add(file);
            }
            
            /* merge all uploads  */
            Task mergedTasks = Task.WhenAll(tasks);
            return mergedTasks;
        }


        public class uploadStatus
        {
            public Action<object> doUplaod;
            public string url { get; set; }
            public Boolean success { get; set; }
            public string errorMessage { get; set; }
        }
    }
}
