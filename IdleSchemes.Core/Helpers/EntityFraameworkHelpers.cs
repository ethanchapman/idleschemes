using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore;

namespace IdleSchemes.Core.Helpers {
    public static class EntityFraameworkHelpers {

        public static async Task<ImmutableList<T>> ToImmutableListAsync<T>(this IQueryable<T> queryable) {
            var list = await queryable.ToListAsync();
            return list.ToImmutableList();
        }

    }
}
