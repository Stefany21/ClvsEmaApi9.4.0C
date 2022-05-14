using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLVSPOS.MODELS
{
    public class UserModel
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public bool Active { get; set; }
        public DateTime CreateDate { get; set; }
        public bool Owner { get; set; }
    }

    /// <summary>
    /// Modelo que se va a usar para la configuracion de usuarios, va a ser enviada como una lista
    /// </summary>
    public class UserAsingModel {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public bool  SuperUser { get; set; }
        public string SAPUser { get; set; }
        public string SAPPass { get; set; }
        public int SlpCode { get; set; }
        public int? StoreId { get; set; }
        public decimal? minDiscount { get; set; }
        public string CenterCost { get; set; }
        public bool Active { get; set; }
        public int PriceListDef { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string StoreName { get; set; }
        public List<SeriesByUserModel> Series { get; set; }
    }

    /// <summary>
    /// Modelo del usuario para registrar
    /// </summary>
    public class User
    {
        public string Email { get; set; } // correo del usuario
        public string Password { get; set; } // contrasenna del usuario
        public string FullName { get; set; } // nombre completo del usuario
    }

    public class LoggedUser
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string WhName { get; set; }
        public string WhCode { get; set; }
        public string PrefixId { get; set; }
    }
}