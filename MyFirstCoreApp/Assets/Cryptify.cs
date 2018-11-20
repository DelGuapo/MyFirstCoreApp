using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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

        public string AES_Encrypt(string fileName)
        {
            return crypt.AES_Encrypt();
        }

        public string AES_Decrypt(string fileName)
        {
            return crypt.AES_Decrypt();
        }

        /* Private facing _criptify does the cryptographic work with the injected 
         * DataProtectionProvider.
         */
        private class _criptify
        {
            IDataProtector _protector;
            string _hash;
            int _bufferSizeMb = 1;
            public _criptify(IDataProtectionProvider
                provider, string hash)
            {
                _hash = hash;
                _protector = provider.CreateProtector(hash);
                
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

            public string AES_Encrypt(string inputFilePath = "C:\\temp\\test.txt")
            {
                string inputFile = Path.GetFileName(inputFilePath);
                string directory = Path.GetDirectoryName(inputFilePath);
                //http://stackoverflow.com/questions/27645527/aes-encryption-on-large-files

                byte[] salt = System.Text.Encoding.UTF8.GetBytes(encode(_hash));

                //create output file name
                FileStream fsCrypt = new FileStream(inputFilePath + ".aes", FileMode.Create);

                //Set Rijndael symmetric encryption algorithm
                RijndaelManaged AES = new RijndaelManaged();
                AES.KeySize = 256;
                AES.BlockSize = 128;
                AES.Padding = PaddingMode.PKCS7;

                //write salt to the begining of the output file, so in this case can be random every time
                
                fsCrypt.Write(salt, 0, salt.Length);

                //create output stream (encrypted)
                CryptoStream cs = new CryptoStream(fsCrypt, AES.CreateEncryptor(), CryptoStreamMode.Write);

                FileStream fsIn = new FileStream(inputFilePath, FileMode.Open);

                //create a buffer (1mb) so only this amount will allocate in the memory and not the whole file
                byte[] buffer = new byte[1048576];
                int read;

                try
                {
                    while ((read = fsIn.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        //Application.DoEvents(); // -> for responsive GUI, using Task will be better!
                        cs.Write(buffer, 0, read);
                    }

                    //close up
                    fsIn.Close();

                }
                catch (Exception err)
                {
                    Console.WriteLine(err.Message);
                }
                finally
                {
                    cs.Close();
                    fsCrypt.Close();
                }

                return inputFile;
            }


            public string AES_Decrypt(string inputFilePath = "C:\\temp\\test.txt.aes")
            {
                //todo:
                // - create error message on wrong password
                // - on cancel: close and delete file
                // - on wrong password: close and delete file!
                // - create a better filen name
                // - could be check md5 hash on the files but it make this slow

                string inputFile = Path.GetFileName(inputFilePath);
                string directory = Path.GetDirectoryName(inputFilePath);

                byte[] saltBytes = System.Text.Encoding.UTF8.GetBytes(_hash);
                byte[] salt = new byte[32];

                FileStream fsCrypt = new FileStream(inputFilePath, FileMode.Open);
                fsCrypt.Read(salt, 0, salt.Length);

                RijndaelManaged AES = new RijndaelManaged();
                AES.KeySize = 256;
                AES.BlockSize = 128;
                var key = new Rfc2898DeriveBytes(saltBytes, salt, 50000);
                AES.Key = key.GetBytes(AES.KeySize / 8);
                AES.IV = key.GetBytes(AES.BlockSize / 8);
                AES.Padding = PaddingMode.PKCS7;
                AES.Mode = CipherMode.CFB;

                CryptoStream cs = new CryptoStream(fsCrypt, AES.CreateDecryptor(), CryptoStreamMode.Read);

                FileStream fsOut = new FileStream(inputFilePath + ".decrypted", FileMode.Create);

                int read;
                byte[] buffer = new byte[1048576];

                try
                {
                    while ((read = cs.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        //Application.DoEvents();
                        fsOut.Write(buffer, 0, read);
                    }
                }
                catch (System.Security.Cryptography.CryptographicException ex_CryptographicException)
                {

                    Console.WriteLine("CryptographicException error: " + ex_CryptographicException.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }

                try
                {
                    cs.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error by closing CryptoStream: " + ex.Message);
                }
                finally
                {
                    fsOut.Close();
                    fsCrypt.Close();
                }

                return "DONE";
            }
        }

    }
}
