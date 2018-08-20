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
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddDataProtection();
            var services = serviceCollection.BuildServiceProvider();
            crypt = ActivatorUtilities.CreateInstance<_cryptophy>(services,hash);
        }

        public string encode(string nakedValue)
        {
            return crypt.encode(nakedValue);
        }

        public string decode(string encryptedValue)
        {
            return crypt.decode(encryptedValue);
        }

        private class _cryptophy
        {
            // the 'provider' parameter is provided by DI
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
                    return @"
                             _;~)                  (~;_
                            (   |                  |   )
                             ~', ',    ,''~'',   ,' ,'~
                                 ', ','       ',' ,'
                                   ',: {'} {'} :,'
                                     ;   /^\   ;
                                      ~\  ~  /~
                                    ,' ,~~~~~, ',
                                  ,' ,' ;~~~; ', ',
                                ,' ,'    '''    ', ',
                              (~  ;               ;  ~)
                               -;_)               (_;-
                            ";
                }
                
            }
        }

    }
}
