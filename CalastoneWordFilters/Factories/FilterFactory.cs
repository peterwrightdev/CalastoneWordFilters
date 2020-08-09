using CalastoneWordFilterer.Filters;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CalastoneWordFilterer.Factories
{
    // class for managing the creation of Filters
    public class FilterFactory : IFilterFactory
    {
        public List<Type> GetFilterTypes()
        {
            return Assembly.GetExecutingAssembly().GetTypes().AsEnumerable().Where(t => t.IsClass && t.Namespace == "CalastoneWordFilterer.Filters").ToList();
        }

        public ParameterInfo[] GetParametersForFilter(Type filter)
        {
            return filter.GetConstructors().FirstOrDefault().GetParameters();
        }

        public IFilter CreateFilter(Type filter, object[] parameters = null)
        {
            return (IFilter)filter.GetConstructors().FirstOrDefault().Invoke(parameters);
        }

    }
}
