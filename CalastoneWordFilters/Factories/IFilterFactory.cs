using CalastoneWordFilterer.Filters;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace CalastoneWordFilterer.Factories
{
    public interface IFilterFactory
    {
        List<Type> GetFilterTypes();

        ParameterInfo[] GetParametersForFilter(Type filter);

        public IFilter CreateFilter(Type filter, object[] parameters = null);
    }
}
