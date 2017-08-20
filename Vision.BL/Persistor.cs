using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vision.BL.Model;
using Vision.Data;

namespace Vision.BL
{
    public class Persistor
    {
        Storage _storage;

        public Persistor()
        {
            _storage = new Storage();
        }

        public void Save(Context context, string filename)
        {
            _storage.Save(context, filename);
        }

        public Context Load(string filename)
        {
            var result = _storage.Load<Context>(filename);
            return result;
        }
    }
}
