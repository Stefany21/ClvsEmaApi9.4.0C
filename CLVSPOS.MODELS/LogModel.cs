using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CLVSSUPER.MODELS
{
    public class LogModel
    {
        [Key]
        public int Id { get; set; }
        //tipo de documento
        public int TypeDocument { get; set; }
        //Objeto de documento
        public string Document { get; set; }
        //Fecha inicio creacion documento general
        public DateTime? StartTimeDocument { get; set; }
        //Fecha final creacion documento general
        public DateTime? EndTimeDocument { get; set; }
        //Tiempo duracion creacion documento
        public string ElapsedTimeCreateDocument { get; set; }
        //Fecha inicio creacion Compañia
        public DateTime? StartTimeCompany { get; set; }
        //Fecha final creacion compañia
        public DateTime? EndTimeCompany { get; set; }
        //tiempo duracion creacion compañia
        public string ElapsedTimeCompany { get; set; }
        // Fecha inicio creacion documento
        public DateTime? StartTimeSapDocument { get; set; }
        //Fecha final creacion documento
        public DateTime? EndTimeSapDocument { get; set; }       
        //Tiempo duracion creacion documento
        public string ElapsedTimeSapDocument { get; set; }
        // Guarda el error del proceso
        public string ErrorDetail { get; set; }
    }
}