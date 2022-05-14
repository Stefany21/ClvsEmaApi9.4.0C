using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CLVSPOS.MODELS
{
	public class PPTerminal
	{
        [Key]
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string TerminalId { get; set; }
        public bool Status { get; set; }
        public string Currency { get; set; }
        public double QuickPayAmount { get; set; }
    }

    public class PPTerminalByUser
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public string TerminalId { get; set; }
    }

    public class PPTerminalsByUser
    {
        public string UserId { get; set; }
        public List<PPTerminalByUser> TerminalsByUser { get; set; }
    }
}