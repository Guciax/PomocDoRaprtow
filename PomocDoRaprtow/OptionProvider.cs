using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PomocDoRaprtow
{
    internal class OptionProvider
    {
        private readonly Form1 form;

        public OptionProvider(Form1 form)
        {
            this.form = form;
        }

        public DateTime WasteSince => form.WasteSinceTimePicker.Value;
        public DateTime WasteTo => form.WasteToTimePicker.Value;
    }
}