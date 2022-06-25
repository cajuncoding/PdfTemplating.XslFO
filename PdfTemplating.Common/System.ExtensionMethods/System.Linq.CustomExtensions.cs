/*
Copyright 2012 Brandon Bernard

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

     http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
using System.CustomExtensions;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;

namespace System.Linq.CustomExtensions
{
	public static class LinqToObjectGeneralExtensions
	{
		/// <summary>
		/// Chainable Loop Extension for initialization or execution within Linq Chains
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <param name="act"></param>
		/// <returns>IEnumerable</returns>
		public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> act)
		{
			if (source == null) throw new ArgumentNullException("source");
			foreach (T item in source)
			{
				item.With(act);
				yield return item;
			}
		}

		/// <summary>
		/// Efficient implementation of Distinct processing logic by implementing lightweight HashSet internally for performance.
		/// Provides simplified functionality not inherently provided by the normal Distinct LINQ extension as a separate comparer interface is not required.
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="source"></param>
		/// <param name="keySelector"></param>
		/// <returns></returns>
		public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			HashSet<TKey> uniqueKeys = new HashSet<TKey>();
			foreach (TSource item in source)
			{
				if (uniqueKeys.Add(keySelector(item)))
				{
					yield return item;
				}
			}
		}

        /// <summary>
        /// Efficient implementation to convert an IEnumerable into a Dictionary safely without errors, resulting in a distinct set of Keys without exceptions
        /// as the first item encountered will be used.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> ToDictionarySafely<TSource, TKey, TValue>(
            this IEnumerable<TSource> source, 
            Func<TSource, TKey> keySelector, 
            Func<TSource, TValue> valueSelector
        )
        {
            var resultsDictionary = new Dictionary<TKey, TValue>();
            foreach (var item in source)
            {
                var key = keySelector(item);
				if(!resultsDictionary.ContainsKey(key))
					resultsDictionary.Add(key, valueSelector(item));

            }
            return resultsDictionary;
        }

        public static IEnumerable<TSource> WhereNotNull<TSource>(this IEnumerable<TSource> source)
		{
			return source.Where(obj => obj != null);
		}

		public static IEnumerable<TSource> WhereNotNullOrDefault<TSource>(this IEnumerable<TSource> source)
		{
			return source.Where(obj => !Object.Equals(obj, default(TSource)));
		}

		public static IEnumerable<TSource> WhereNotNullOrDefault<TSource, TOut>(this IEnumerable<TSource> source, Converter<TSource, TOut> convert)
		{
			return source.Where(e => !Object.Equals(convert(e), default(TOut)));
		}

		public static IEnumerable<TSource> WhereIsIn<TSource>(this IEnumerable<TSource> source, params TSource[] list)
		{
			if (source == null) throw new ArgumentNullException("source");
			foreach (TSource item in source)
			{
				if (list.Contains(item))
				{
					yield return item;
				}
			}
		}

		/// <summary>
		/// Single Item reverse contains lookup to find if an item is within a list.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="item"></param>
		/// <param name="list"></param>
		/// <returns></returns>
		public static bool IsIn<T>(this T item, params T[] list)
		{
			if (item == null) throw new ArgumentNullException("source");
			return list.Contains(item);
		}

		/// <summary>
		/// Single String Item reverse contains lookup to find if an String item is within a list ignoring case sensitivity (case insensitive).  ONLY Supports String Comparisons and will use ToString() methods
		/// to render the object into it's String form for comparison.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="item"></param>
		/// <param name="list"></param>
		/// <returns></returns>
		public static bool IsInIgnoreCase(this String item, params String[] list)
		{
			if (item == null) throw new ArgumentNullException("source");
			return item.IsIn(StringComparer.CurrentCultureIgnoreCase, list);
		}

		/// <summary>
		/// Single Item reverse contains lookup, with support for Linq chaining. Also supports custom Comparisons that can be easily specified (i.e. Lambda Expression: (x,y) => x.CompareTo(y)).
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="item"></param>
		/// <param name="comparer"></param>
		/// <param name="list"></param>
		/// <returns></returns>
		public static bool IsIn<T>(this T item, IEqualityComparer<T> comparer, params T[] list)
		{
			if (item == null) throw new ArgumentNullException("source");
			return list.Contains(item, comparer);
		}
	}

	public static class LinqToObjectPagingExtensions
	{
		/// <summary>
		/// Provides easy methods for Paging Linq Results; performs all page math necessary to return the specified page (This is used by LinqToSql).
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <param name="source"></param>
		/// <param name="page"></param>
		/// <param name="pageSize"></param>
		/// <returns></returns>
		public static IQueryable<TSource> Page<TSource>(this IQueryable<TSource> source, int page, int pageSize)
		{
			return source.Skip((page - 1) * pageSize).Take(pageSize);
		}

		/// <summary>
		/// Provides easy methods for Paging Linq Results; performs all page math necessary to return the specified page (This is used by LinqToObjects).
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <param name="source"></param>
		/// <param name="page"></param>
		/// <param name="pageSize"></param>
		/// <returns></returns>
		public static IEnumerable<TSource> Page<TSource>(this IEnumerable<TSource> source, int page, int pageSize)
		{
			return source.Skip((page - 1) * pageSize).Take(pageSize);
		}

	}
}