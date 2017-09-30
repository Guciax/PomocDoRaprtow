using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PomocDoRaprtow
{


    class OptionProvider
    {
        private readonly Form1 form;

        public OptionProvider(Form1 form)
        {
            this.form = form;
        }

        public DateTime OdpadBegin => form.OdpadBegin.Value;
        public DateTime OdpadEnd =>  form.OdpadEnd.Value;
        
    }
}
