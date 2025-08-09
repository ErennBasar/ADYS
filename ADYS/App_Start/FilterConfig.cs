using System.Web.Mvc;
using ADYS.Filters;


namespace ADYS
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new NoCacheAttribute()); //global olarak tanımlandı
        }
    }
}
