using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Renci.SshNet; /* reference needed: Renci.SshNet.dll */


namespace MyFirstCoreApp
{
    /* based from this post: https://gist.github.com/piccaso/d963331dcbf20611b094  */
    public class SSHify
    {
        private ConnectionInfo ConnInfo;
        public Boolean connected;
        public string errorMessage;
        public Boolean persistConnection = true;
        private SshClient sshClient;
        private SftpClient sftpClient;

        /* DESTRUCTOR: Disconnect Connection */
        ~SSHify()
        {
            if (sshClient != null && sshClient.IsConnected)
            {
                sshClient.Disconnect();
                sshClient.Dispose();
            }
            if(sftpClient != null && sftpClient.IsConnected)
            {
                sftpClient.Disconnect();
                sftpClient.Dispose();
            }
        }

        /* Constructor: Make connection */
        public SSHify(string hostName, int port, string user, string password = null, string passphrase = null, string keyFile = null)
        {
            if(password == null && (passphrase == null || keyFile == null))
            {
                connected = false;
                errorMessage = "Missing [Password] or [Passphrase + keyfile]";
                return;
            }

 
            // Setup Credentials and Server Information
            ConnInfo = new ConnectionInfo(hostName, port, user,
                new AuthenticationMethod[]{

                // Pasword based Authentication
                new PasswordAuthenticationMethod(user,password)

                // Key Based Authentication (using keys in OpenSSH Format)
                //new PrivateKeyAuthenticationMethod(user,new PrivateKeyFile[]{
                //    new PrivateKeyFile(keyFile,passphrase)
                //}),
            });

            if (persistConnection)
            {
                try
                {
                    sshClient = new SshClient(ConnInfo);
                }
                catch(Exception err)
                {
                    connected = false;
                    errorMessage = err.Message;
                }
            }
        }

        public string shellScript(string cmdstr)
        {
            cmdstr = "ls";
            string response = "";
            try
            {
                connectSsh();
                // Execute a (SHELL) Command - prepare upload directory
                using (var cmd = sshClient.CreateCommand(cmdstr))
                {
                    cmd.Execute();
                    response = string.Format("Exit Code: [{0}]; \n Response: [{1}]", cmd.ExitStatus, cmd.Result);
                }
                disconnectSsh();
            }
            catch(Exception err)
            {
                errorMessage = err.Message;
                return null;
            }

            return response;
        }

        public Boolean uploadFile(string filePath, string remoteDirectory, string fileName = null)
        {
            if (fileName == null)
            {
                fileName = Path.GetFileName(filePath);
            }
            // Upload A File
            try
            {
                connectSftp();
                sftpClient.ChangeDirectory(remoteDirectory);
                using (var uplfileStream = System.IO.File.OpenRead(filePath))
                {
                    sftpClient.UploadFile(uplfileStream, fileName, progressCallbackFunction);
                }
                disconnectSftp();
            }
            catch(Exception err)
            {
                errorMessage = err.Message;
                return false;
            }
            
            return true;
        }

        public Boolean downloadFile(string remoteFile, string localDirectory, string saveAs = null)
        {
            if (saveAs == null)
            {
                saveAs = Path.GetFileName(remoteFile);
            }
            // Download A File
            try
            {
                connectSftp();
                using (Stream fileStream = File.Create(localDirectory + saveAs))
                {
                    sftpClient.DownloadFile(remoteFile, fileStream,progressCallbackFunction);
                }
                disconnectSftp();
            }
            catch (Exception err)
            {
                errorMessage = err.Message;
                return false;
            }

            return true;
        }

        public List<string> lsDir(string dir = "")
        {
            List<string> rtn = new List<string>();
            connectSftp();
            foreach (var f in sftpClient.ListDirectory(dir))
            {
                rtn.Add(f.FullName);
            }
            disconnectSftp();
            return rtn;
        }

        /* ONE CONNECTION PER INSTANCE */
        private void disconnectSftp()
        {
            if (persistConnection)
            {
                return;
            }

            if (sftpClient != null)
            {
                sftpClient.Disconnect();
            }
        }

        private Boolean connectSftp()
        {
            try
            {
                if (sftpClient == null)
                {
                    sftpClient = new SftpClient(ConnInfo);
                    sftpClient.Connect();
                }
            }
            catch(Exception err)
            {
                errorMessage = err.Message;
                return false;
            }
            return true;
        }

        /* ONE CONNECTION PER INSTANCE */
        private void disconnectSsh()
        {
            if (persistConnection)
            {
                return;
            }

            if (sshClient != null)
            {
                sshClient.Disconnect();
            }
        }

        /* ONE CONNECTION PER INSTANCE */
        private Boolean connectSsh()
        {
            try
            {
                if (sshClient == null)
                {
                    sshClient = new SshClient(ConnInfo);
                    sshClient.Connect();
                }
            }
            catch (Exception err)
            {
                errorMessage = err.Message;
                return false;
            }
            return true;
        }

        private void progressCallbackFunction( ulong bytesUploaded)
        {
            // dunno what to do with this now...
            Console.WriteLine(bytesUploaded.ToString());
        }
    }
    
}
