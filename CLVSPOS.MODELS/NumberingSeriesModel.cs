using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLVSPOS.MODELS
{

    /// <summary>
    /// /Modelo para las series de numeracion
    /// </summary>
    public class NumberingSeriesModel
    {
         //Id de la serie
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
        // para ver el tipo que es.
        public string typeName { get; set; }
        // para ver el nombre de la compañia.
        public string CompanyName { get; set; }
        // para ver el Type
        public int Type { get; set; }
    }

    public class SyncSeriesModel
    {
        //Id de la serie
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
       
    }


    public class SeriesByUserModel
    {
        public int Id { get; set; }
        public int SerieId { get; set; }
        public int UsrMappId { get; set; }
        public string Name { get; set; }
        public int type { get; set; }
    }

    public class SyncSeriesByUser
    {
        public int Id { get; set; }
        public int SerieId { get; set; }
        public int UsrMappId { get; set; }        
    }

    public class DiapiSerie {
        public int NumType { get; set; }
        public int Serie { get; set; }

    }
}