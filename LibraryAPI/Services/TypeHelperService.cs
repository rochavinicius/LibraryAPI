using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace LibraryAPI.Services
{
    public class TypeHelperService : ITypeHelperService
    {
        public bool TypeHasProperties<TSource>(string fields)
        {
            if (fields == null)
            {
                return true;
            }

            var fieldsAfterSplit = fields.Split(',');

            foreach(var field in fieldsAfterSplit)
            {
                var propertyName = field.Trim();

                var propertyInfo = typeof(TSource).GetProperty(field, BindingFlags.IgnoreCase
                    | BindingFlags.Public
                    | BindingFlags.Instance);

                if (propertyInfo == null)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
