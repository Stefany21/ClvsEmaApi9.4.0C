using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CLVSSUPER.MODELS
{
    public class GroupLine
    {
        [Key]
        public int Id { get; set; }
        //Codigo de numeracion de la vista
        public int CodNum { get; set; }
        //Nombre de la vista
        public string NameView { get; set; }
        //Agrupaciones de la vista
        public bool isGroup { get; set; }
        //Orden de lineas desc o asce
        public bool LineMode { get; set; }
    }
}