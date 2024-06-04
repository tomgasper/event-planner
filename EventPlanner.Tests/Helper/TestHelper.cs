using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventPlanner.Tests.Helper
{
	public class TestHelper
	{
		public static DbSet<T> CreateMockDbSet<T>(IQueryable<T> data) where T : class
		{
			var mockSet = Substitute.For<DbSet<T>, IQueryable<T>>();

			((IQueryable<T>)mockSet).Provider.Returns(data.Provider);
			((IQueryable<T>)mockSet).Expression.Returns(data.Expression);
			((IQueryable<T>)mockSet).ElementType.Returns(data.ElementType);
			((IQueryable<T>)mockSet).GetEnumerator().Returns(data.GetEnumerator());

			return mockSet;
		}
	}
}
