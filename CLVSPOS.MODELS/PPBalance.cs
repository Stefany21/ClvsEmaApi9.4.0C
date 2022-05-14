using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace CLVSPOS.MODELS
{
	public class PPBalance
	{
        [Key]
        public int Id { get; set; }
        // Es todo el documento que retorna el bac
        public string XMLDocumentResponse { get; set; }
        // Codigo de respuesta proporcionado por el banco
        public string ResponseCode { get; set; }
        // Descripcion de la respuesta proporcionada por el banco
        public string ResponseCodeDescription { get; set; }
        // Numero de recibido
        public string AcqNumber { get; set; }
        // Nombre del local que solicita el cierre
        public string CardBrand { get; set; }
        // Hora, minutos, segundos del banco
        public string HotTime { get; set; }
        // Fecha del banco
        public string HostDate { get; set; }
        // Dinero reembolsado
        public string RefundsAmount { get; set; }
        // Cantidad de transacciones devueltas
        public string RefundsTransactions { get; set; }
        // Cantidad de dinero de las ventas
        public string SalesTransactions { get; set; }
        // Cantidad de transacciones aprobadas y cobradas
        public string SalesAmount { get; set; }
        // Impuestos en las ventas
        public string SalesTax { get; set; }

        public string SalesTip { get; set; }

        // Fecha de creacion del documento
        public DateTime CreationDate { get; set; }
        // Fecha de modificacion del documento
        public DateTime ModificationDate { get; set; }
        // Tipo del documento, si es pre-cierre o cierre
        public string TransactionType { get; set; }
        // Terminal al que se le obtuvo el balance
        public string TerminalCode { get; set; }
    }
}