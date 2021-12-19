using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookWorld.Data
{
    public interface IDbInitializer
    {
        void Initialize();
    }
}
