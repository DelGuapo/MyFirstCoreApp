using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

namespace MyFirstCoreApp
{
    
    public class Cryptify
    {
        private _criptify crypt;
        public  Cryptify(string hash, string keyLocation = null)
        {
            /* Cryptify is a public facing class that interfaces with a private class (_cryptify).
             * We needed to do this to inject the DataProtection
             * This is equivilent to adding the service in the app Startup with addService()
             */
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddDataProtection();

            /* Create an instance of a _cryptography class (below) and inject the services added above
             */
            var services = serviceCollection.BuildServiceProvider();
            crypt = ActivatorUtilities.CreateInstance<_criptify>(services, hash);
        }

        /* [encode] andn [decode] are the two externally facing functions
         *  that interact with private _cryptify.
         */
        public string encode(string nakedValue)
        {
            return crypt.encode(nakedValue);
        }

        public string decode(string encryptedValue)
        {
            return crypt.decode(encryptedValue);
        }

        public fileEncryptionStatus AES_Encrypt(string filePath, string password, string output, Boolean destroyOld)
        {
            return crypt.AES_Encrypt(filePath, password,output, destroyOld);
        }

        public fileEncryptionStatus AES_Decrypt(string filePath, string password, string output, Boolean destroyOld)
        {
            return crypt.AES_Decrypt(filePath, password, output,destroyOld);
        }


        public class fileEncryptionStatus
        {
            public int code;
            public string message;
            public fileEncryptionStatus(int code, string message)
            {
                this.code = code;
                this.message = message;
            }
        }
        /* Private facing _criptify does the cryptographic work with the injected 
         * DataProtectionProvider.
         */
        private class _criptify
        {
            IDataProtector _protector;
            string _hash;
            string _hash2;
            int _bufferSizeMb = 1;
            public _criptify(IDataProtectionProvider
                provider, string hash)
            {
                _hash = hash;
                _protector = provider.CreateProtector(hash);
                _hash2 = _protector.Protect(hash);
                
            }

            public string encode(string nakedValue)
            {
                return _protector.Protect(nakedValue);
            }

            public string decode(string encryptedValue)
            {
                try
                {
                    return _protector.Unprotect(encryptedValue);
                }
                catch (Exception err)
                {
                    return new ASCIIify().skull;
                }
                
            }

            public fileEncryptionStatus AES_Encrypt(string inputFilePath, string password, string outputFilePath = null, Boolean destroyOld = true)
            {
                //http://stackoverflow.com/questions/27645527/aes-encryption-on-large-files

                if (!File.Exists(inputFilePath))
                {
                    return new fileEncryptionStatus(404, "Source file does not exist");
                }

                /* Each file requires unique password. */
                byte[] passwordBytes = Encoding.ASCII.GetBytes(password);
                string inputFile = Path.GetFileName(inputFilePath);
                string directory = Path.GetDirectoryName(inputFilePath);
                
                byte[] saltBytes = Encoding.ASCII.GetBytes(_hash);

                //create output file name
                if (outputFilePath == null)
                {
                    outputFilePath = inputFilePath + ".aes";
                }
                FileStream fsCrypt = new FileStream(outputFilePath, FileMode.Create);

                //Set Rijndael symmetric encryption algorithm
                RijndaelManaged AES = new RijndaelManaged();
                AES.KeySize = 256;
                AES.BlockSize = 128;

                var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                AES.Key = key.GetBytes(AES.KeySize / 8);
                AES.IV = key.GetBytes(AES.BlockSize / 8);
                AES.Padding = PaddingMode.Zeros;
                AES.Mode = CipherMode.CBC;

                //create output stream (encrypted)
                CryptoStream cs = new CryptoStream(fsCrypt, AES.CreateEncryptor(), CryptoStreamMode.Write);

                FileStream fsIn = new FileStream(inputFilePath, FileMode.Open);

                int data;
                
                while ((data = fsIn.ReadByte()) != -1)
                    cs.WriteByte((byte)data);
                 
                fsIn.Close();
                cs.Close();
                fsCrypt.Close();

                if (destroyOld)
                {
                    File.Delete(inputFilePath);
                }
                return new fileEncryptionStatus(201, Path.GetFileName(outputFilePath));
            }


            public fileEncryptionStatus AES_Decrypt(string inputFilePath, string password, string outputFilePath, Boolean destroyOld = true)
            {
                //todo:
                // - create error message on wrong password
                // - on cancel: close and delete file
                // - on wrong password: close and delete file!
                // - create a better filen name
                // - could be check md5 hash on the files but it make this slow

                if (!File.Exists(inputFilePath))
                {
                    return new fileEncryptionStatus(404, "Source file does not exist");
                }

                byte[] passwordBytes = Encoding.ASCII.GetBytes(password);
                byte[] saltBytes = Encoding.ASCII.GetBytes(_hash);

                string inputFile = Path.GetFileName(inputFilePath);
                string directory = Path.GetDirectoryName(inputFilePath);
                
                FileStream fsCrypt = new FileStream(inputFilePath, FileMode.Open);

                //Set Rijndael symmetric encryption algorithm
                RijndaelManaged AES = new RijndaelManaged();
                AES.KeySize = 256;
                AES.BlockSize = 128;

                var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                AES.Key = key.GetBytes(AES.KeySize / 8);
                AES.IV = key.GetBytes(AES.BlockSize / 8);
                AES.Padding = PaddingMode.Zeros;
                AES.Mode = CipherMode.CBC;


                /* Stream this crap and decrypt it like a boss */
                CryptoStream cs = new CryptoStream(fsCrypt, AES.CreateDecryptor(), CryptoStreamMode.Read);

                FileStream fsOut = new FileStream(outputFilePath, FileMode.Create);

                int data;
                while ((data = cs.ReadByte()) != -1)
                    fsOut.WriteByte((byte)data);

                fsOut.Close();
                cs.Close();
                fsCrypt.Close();

                if (destroyOld)
                {
                    File.Delete(inputFilePath);
                }

                return new fileEncryptionStatus(201, Path.GetFileName(outputFilePath));
            }
        }

    }

    
}
