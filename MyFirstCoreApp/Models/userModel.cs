using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstCoreApp
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Hash { get; set; }
        public string Status {get;set;}
        public string Email {get;set;}
        public string Role {get;set;}
        public DateTime LoginDatetime {get;set;}
        public int LoginAttempts{get;set;}
        public int SecondFactorAuthentication { get; set; }
        public DateTime SecondFactorExpireDatetime { get; set; }
        //public string SessionToken { get; set; }
}
}
