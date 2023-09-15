using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.EfCore.Extensions
{
    internal static class NoTrackingExtension
    {
        public static IQueryable<T> TrackElements<T>(this DbSet<T> elements, bool track) where T : class
        {
            return track ? elements : elements.AsNoTracking();
        }
    }
}
