using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstCoreApp
{
    
    public class Cryptify
    {
        private _cryptophy crypt;
        public  Cryptify(string hash)
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
            crypt = ActivatorUtilities.CreateInstance<_cryptophy>(services,hash);
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


        /* Private facing _cryptophy does the cryptographic work with the injected 
         * DataProtectionProvider.
         */
        private class _cryptophy
        {
            IDataProtector _protector;
            public _cryptophy(IDataProtectionProvider provider, string hash)
            {
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
        }

    }
}
