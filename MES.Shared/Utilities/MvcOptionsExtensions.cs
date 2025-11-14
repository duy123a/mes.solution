using MES.Shared.ModelBinder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace MES.Shared.Utilities
{
    public static class MvcOptionsExtensions
    {
        public static void AddStringTrimModelBinderProvider(this MvcOptions options)
        {
            var binderToFind = options.ModelBinderProviders.FirstOrDefault(x => x.GetType() == typeof(SimpleTypeModelBinderProvider));
            if (binderToFind == null)
            {
                return;
            }

            int index = options.ModelBinderProviders.IndexOf(binderToFind);
            options.ModelBinderProviders.Insert(index, new TrimStringModelBinderProvider());
        }
    }
}
