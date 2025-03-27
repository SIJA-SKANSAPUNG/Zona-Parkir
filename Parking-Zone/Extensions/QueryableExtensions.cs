using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Parking_Zone.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> WhereIf<T>(
            this IQueryable<T> query,
            bool condition,
            Expression<Func<T, bool>> predicate)
        {
            return condition ? query.Where(predicate) : query;
        }

        public static IQueryable<T> OrderByProperty<T>(
            this IQueryable<T> query,
            string propertyName,
            bool ascending = true)
        {
            if (string.IsNullOrEmpty(propertyName))
                return query;

            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, propertyName);
            var lambda = Expression.Lambda(property, parameter);

            var methodName = ascending ? "OrderBy" : "OrderByDescending";
            var methodCallExpression = Expression.Call(
                typeof(Queryable),
                methodName,
                new[] { typeof(T), property.Type },
                query.Expression,
                Expression.Quote(lambda));

            return query.Provider.CreateQuery<T>(methodCallExpression);
        }

        public static async Task<PaginatedList<T>> ToPaginatedListAsync<T>(
            this IQueryable<T> query,
            int pageNumber,
            int pageSize)
        {
            var totalItems = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedList<T>(items, totalItems, pageNumber, pageSize);
        }

        public static IQueryable<T> IncludeMultiple<T>(
            this IQueryable<T> query,
            params Expression<Func<T, object>>[] includes)
            where T : class
        {
            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }
            return query;
        }

        public static IQueryable<T> Between<T, TKey>(
            this IQueryable<T> query,
            Expression<Func<T, TKey>> keySelector,
            TKey low,
            TKey high)
            where TKey : IComparable<TKey>
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var keyAccess = Expression.Invoke(keySelector, parameter);
            var lowConstant = Expression.Constant(low);
            var highConstant = Expression.Constant(high);

            var greaterThanOrEqual = Expression.GreaterThanOrEqual(keyAccess, lowConstant);
            var lessThanOrEqual = Expression.LessThanOrEqual(keyAccess, highConstant);
            var andExpression = Expression.AndAlso(greaterThanOrEqual, lessThanOrEqual);

            var lambda = Expression.Lambda<Func<T, bool>>(andExpression, parameter);
            return query.Where(lambda);
        }

        public static IQueryable<T> Search<T>(
            this IQueryable<T> query,
            string searchTerm,
            params Expression<Func<T, string>>[] propertySelectors)
        {
            if (string.IsNullOrEmpty(searchTerm) || propertySelectors == null || !propertySelectors.Any())
                return query;

            var parameter = Expression.Parameter(typeof(T), "x");
            Expression combinedExpression = null;

            foreach (var selector in propertySelectors)
            {
                var propertyAccess = Expression.Invoke(selector, parameter);
                var searchConstant = Expression.Constant(searchTerm, typeof(string));
                var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                var containsExpression = Expression.Call(propertyAccess, containsMethod, searchConstant);

                combinedExpression = combinedExpression == null
                    ? containsExpression
                    : Expression.OrElse(combinedExpression, containsExpression);
            }

            var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);
            return query.Where(lambda);
        }
    }

    public class PaginatedList<T>
    {
        public List<T> Items { get; }
        public int PageNumber { get; }
        public int TotalPages { get; }
        public int TotalItems { get; }
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;

        public PaginatedList(List<T> items, int totalItems, int pageNumber, int pageSize)
        {
            Items = items;
            PageNumber = pageNumber;
            TotalItems = totalItems;
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        }
    }
} 