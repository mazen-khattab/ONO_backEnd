using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONO.Core.Interfaces
{
    public interface ISoftDeleteble
    {
        bool IsDeleted { get; set; }
    }
}
