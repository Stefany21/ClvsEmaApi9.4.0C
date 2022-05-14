using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLVSPOS.MODELS
{
	public class PPBalanceRequest
	{
        // Fecha inicial del los balances
        public DateTime From { get; set; }
        // Fecha final de los balances
        public DateTime To { get; set; }
        // Id del terminal que se le va a consultar
        public int TerminalId { get; set; }
        // Tipo de documento que se desea obtener, precierre, cierre
        public String DocumentType { get; set; }
    }
}