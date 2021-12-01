using Library.Core.Common;
using System;
using System.Collections.Generic;

namespace Models.Securites
{
    public class User: BaseModel
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Msisdn { get; set; }
        public string Email { get; set; }
        public string FacebookId { get; set; }
        public string GoogleId { get; set; }
        public string LinkedinId { get; set; }
        public string TwitterId { get; set; }
        public string Password { get; set; }
        public string ImageUrl { get; set; }
        public string ImageSource { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Gender { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public string City { get; set; }
        public string RegisterWith { get; set; }
        public string AppVersion { get; set; }
        public string OsName { get; set; }
        public string TelcoProvider { get; set; }
        public decimal Latitude { get; set; } = 0;
        public decimal Longitude { get; set; } = 0;
        public bool IsActive { get; set; }
        public string OsVersion { get; set; }
        //public DateTime CreateDate { get; set; }
        //public DateTime? UpdateDate { get; set; }
    }
}