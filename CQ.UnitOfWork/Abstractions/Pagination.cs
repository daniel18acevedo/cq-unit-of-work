using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.Abstractions
{
    public record class Pagination<TItem>(
        List<TItem> Items,
        long TotalItems,
        long TotalPages);
}
