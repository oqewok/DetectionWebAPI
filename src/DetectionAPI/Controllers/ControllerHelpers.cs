using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DetectionAPI.Controllers
{
    public static class ControllerHelpers
    {
        //public static bool TryGetFormFieldValue(this IEnumerable<HttpContent> contents, string dispositionName, out string formFieldValue)
        //{
        //    if (contents == null)
        //    {
        //        throw new ArgumentNullException("contents");
        //    }

        //    HttpContent content = contents.FirstDispositionNameOrDefault(dispositionName);
        //    if (content != null)
        //    {
        //        formFieldValue = content.ReadAsStringAsync().Result;
        //        return true;
        //    }

        //    formFieldValue = null;
        //    return false;
        //}
    }
}
