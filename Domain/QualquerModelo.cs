using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominandoEFCore.Domain
{
   [NotMapped]
    public class QualquerModelo
    {

        public string Descricao { get; set; }
    }
}
