﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Siege.Provisions.UnitOfWork;

namespace Siege.Provisions.Finders
{
	public class QuerySpecification<T>
	{
		private IQueryable<T> linqQuery;
		private readonly List<Action> conditions = new List<Action>();
		
		public QuerySpecification()
		{
		}

		public QuerySpecification(IQueryable<T> query)
		{
			linqQuery = query;
		}

		internal void WithUnitOfWork(IUnitOfWork unitOfWork)
		{
			this.linqQuery = unitOfWork.Query<T>();
		}

		public void Where(Expression<Func<T, bool>> expression)
		{
			this.conditions.Add(() => linqQuery = linqQuery.Where(expression));
		}
		
		internal IQueryable<T> ToIQueryable()
		{
			foreach(var condition in conditions)
			{
				condition();
			}

			return this.linqQuery.Select(x => x);
		}
	}
}