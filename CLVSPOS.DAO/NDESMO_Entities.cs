namespace CLVSPOS.DAO
{
    using CLVSSUPER.MODELS;
    using MODELS;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Linq;

    public class SuperV2_Entities : DbContext
    {
        // Your context has been configured to use a 'SuperV2_Entities' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'NDESMO.DAO.SuperV2_Entities' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'SuperV2_Entities' 
        // connection string in the application configuration file.
        public SuperV2_Entities()
            : base("name=SuperV2_Entities")
        {
        }

        public virtual DbSet<Companys> Companys { get; set; }
        public virtual DbSet<MailData> MailData { get; set; }
        public virtual DbSet<ParamsViewCompany> ParamsViewCompany { get; set; }
        public virtual DbSet<ViewParams> ViewParams { get; set; }
        public virtual DbSet<Permissions> Permissions { get; set; }
        public virtual DbSet<PermsByUser> PermsByUser { get; set; }
        public virtual DbSet<UserAssign> UserAssign { get; set; }
        public virtual DbSet<SAPConnection> SAPConnection { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<CompanyByUsers> CompanyByUsers { get; set; }
        public virtual DbSet<Store> Store { get; set; }
        public virtual DbSet<Series> Series { get; set; }
        public virtual DbSet<SeriesByUser> SeriesByUser { get; set; }
        public virtual DbSet<Logs> Logs { get; set; }
        public virtual DbSet<ViewLineAgrupation> ViewLineAgrupation { get; set; }
        public virtual DbSet<CompanyUdfs> CompanyUdfs { get; set; }
        public virtual DbSet<Settings> Settings { get; set; }
        public virtual DbSet<DBObjectName> DBObjectName { get; set; }
        public virtual DbSet<PaydeskBalance> PaydeskBalance { get; set; }

        public virtual DbSet<PPTransaction> PPTransaction { get; set; }
        public virtual DbSet<PPTerminal> PPTerminal { get; set; }
        public virtual DbSet<PPTransactionLogger> PPTransactionLogger { get; set; }
        public virtual DbSet<PPBalance> PPBalance { get; set; }
        public virtual DbSet<PPTerminalByUser> PPTerminalByUser { get; set; }
    }

    public class Companys : CompanysModel
    {
        [ForeignKey("SAPConnectionId")]
        public virtual SAPConnection SAPConnection { get; set; }
        public virtual ICollection<UserAssign> UserMapp { get; set; }
        public virtual ICollection<ParamsViewCompany> v_ParamsViewByCompany { get; set; }
        [ForeignKey("MailDataId")]
        public virtual MailData V_MailData { get; set; }
    }

    public class MailData : MailDataModel
    {
        public virtual ICollection<Companys> V_Companys { get; set; }
    }

    public class ParamsViewCompany
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public virtual Companys V_Companys { get; set; }
        public int ViewsId { get; set; }
        public int ParamsId { get; set; }
        [ForeignKey("ParamsId")]
        public virtual ViewParams V_ViewParams { get; set; }
        [Required]
        public string Descrip { get; set; }
        public int Order { get; set; }
        public bool Visibility { get; set; }
        public bool Display { get; set; }
        public string Text { get; set; }
    }

    public class ViewParams
    {
        public int Id { get; set; }
        [Required]
        [Index(IsUnique = true)]
        [MaxLength(200)]
        public string Name { get; set; }
        public int Type { get; set; }
    }

    public class Permissions
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
    }

    public class PermsByUser
    {
        public int Id { get; set; }
        public int UserMappId { get; set; }
        public int PermId { get; set; }
        public bool BoolValue { get; set; }
        public string TextValue { get; set; }
        public int IntValue { get; set; }
        public double DoubleValue { get; set; }
        public decimal DecimalValue { get; set; }
        [ForeignKey("UserMappId")]
        public virtual UserAssign V_UserAssgn { get; set; }
        [ForeignKey("PermId")]
        public virtual Permissions V_Permissions { get; set; }
    }

    public class SyncPermsByUser
    {
        public int Id { get; set; }
        public int UserMappId { get; set; }
        public int PermId { get; set; }
        public bool BoolValue { get; set; }
        public string TextValue { get; set; }
        public int IntValue { get; set; }
        public double DoubleValue { get; set; }
        public decimal DecimalValue { get; set; }        
    }

    public class UserAssign
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [DefaultValue(false)]
        public bool SuperUser { get; set; }
        [Required]
        public string SAPUser { get; set; }
        [Required]
        public string SAPPass { get; set; }
        [Required]
        public int SlpCode { get; set; }
        //[Required]
        [DefaultValue(null)]
        public int? StoreId { get; set; }
        //descuento minimo que se otorga por usuario
        public decimal? minDiscount { get; set; }
        //[MaxLength(10)]
        public string CenterCost { get; set; }
        public bool Active { get; set; }
        //Lista de precios default para vista de consulta de precios
        public int PriceListDef { get; set; }
        [Required]
        public int CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public virtual Companys Companys { get; set; }
        public virtual ICollection<PermsByUser> V_PermsByUser { get; set; }
        [ForeignKey("StoreId")]
        public virtual Store V_Store { get; set; }
        public virtual ICollection<SeriesByUser> V_SeriesByUser { get; set; }
    }

    public class SAPConnection: SAPConnectionModel
    {

    }

    public class PermsUserEdit
    {
        public string UserId { get; set; }
        public List<Permissions> UserPerms { get; set; }
    }

    public class Users
    {
        // Identificador del usuario
        [Key]
        public string Id { get; set; }
        // Nombre completo del usuario
        public string FullName { get; set; }
        // Email del usuario
        public string Email { get; set; }
        // Confirmacion del Email del usuario
        public bool EmailConfirmed { get; set; }
        // Nombre del usuario
        public string UserName { get; set; }
        // Contrasenna del usuario
        public string PasswordHash { get; set; }
        // Estado de actividad del usuario
        public bool Active { get; set; }
        // Fecha de creacion del usuario
        public DateTime CreateDate { get; set; }
    }

    public class SyncCompanyByUsers
    {
        // Identificador de la asignacion de compannias por usuarios
        [Key]
        public int Id { get; set; }
        // Id del usuario
        public string UserId { get; set; }
        // Identificador de la compannia a la que esta registrado el usuario
        public int CompanyId { get; set; }
        // Estado de actividad de la asignacion
        public bool Status { get; set; }
        // Fecha de creacion de la asignacion
        public DateTime CreationDate { get; set; }       
    }

    //modelo de las compañias por ususario
    public class CompanyByUsers
    {
        // Identificador de la asignacion de compannias por usuarios
        [Key]
        public int Id { get; set; }
        // Id del usuario
        public string UserId { get; set; }
        // Identificador de la compannia a la que esta registrado el usuario
        public int CompanyId { get; set; }
        // Estado de actividad de la asignacion
        public bool Status { get; set; }
        // Fecha de creacion de la asignacion
        public DateTime CreationDate { get; set; }
        // Lista de usuarios, por usuario por compannia
        [ForeignKey("UserId")]
        public virtual Users V_Users { get; set; }
        // Lista de permisos, por usuario por compannia
        public virtual ICollection<Permissions> V_Permissions { get; set; }
    }

    // modelos para los almasenes que puede tener la compañia.
    public class Store
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string WhCode { get; set; }
        public string WhName { get; set; }
        public int CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public virtual Companys V_Companys { get; set; }
        public bool Active { get; set; }
        public virtual ICollection<UserAssign> V_UserMapp { get; set; }
    }

    public class SyncStore
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string WhCode { get; set; }
        public string WhName { get; set; }
        public int CompanyId { get; set; }
        [ForeignKey("CompanyId")]        
        public bool Active { get; set; }        
    }

    // modelo para las series de numeracion
    public class Series
    {
        [Key] //Id de la serie
        public int Id { get; set; } 
        // nombre de la serie
        public string Name { get; set; }
        //typo de la serie
        public int DocType { get; set; }
        //numeracion de la serie
        public int Numbering { get; set; }
        // el numero de la serie
        public int Serie { get; set; }
        // comapñia de la serie
        public int CompanyId { get; set; }
        //estado de la serie
        public bool Active { get; set; }
        // online/offline
        public int Type { get; set; }
    }
    // modelos para las series de numeracion para el usuario
    public class SeriesByUser {
        public int Id { get; set; }
        public int SerieId { get; set; }
        public int UsrMappId { get; set; }
        [ForeignKey("SerieId")]
        public virtual Series V_Series { get; set; }
        [ForeignKey("UsrMappId")]
        public virtual UserAssign V_UserId { get; set; }
    }

    public class SyncUserAssign
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [DefaultValue(false)]
        public bool SuperUser { get; set; }
        [Required]
        public string SAPUser { get; set; }
        [Required]
        public string SAPPass { get; set; }
        [Required]
        public int SlpCode { get; set; }
        [DefaultValue(null)]
        public int? StoreId { get; set; }
        public decimal? minDiscount { get; set; }
        public string CenterCost { get; set; }
        public bool Active { get; set; }
        public int PriceListDef { get; set; }
        public int CompanyId { get; set; }
                
    }
    public class Logs : LogModel
    {

    }
    public class ViewLineAgrupation : GroupLine
    {

    }
}