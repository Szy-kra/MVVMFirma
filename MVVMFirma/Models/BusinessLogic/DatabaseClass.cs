using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMFirma.Models.BusinessLogic
{
    public class DatabaseClass
    {
        #region Baza danych
        protected FakturyEntities fakturyEntities;
        #endregion
        #region Konstruktor
        public DatabaseClass(FakturyEntities fakturyEntities)
        {
            this.fakturyEntities = fakturyEntities;
        }
        #endregion
    }
}
