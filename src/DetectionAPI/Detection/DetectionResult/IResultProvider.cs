using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DetectionAPI.Detection.DetectionResult
{
    public interface IResultProvider<T>
    {
        T[] DetectionResult();
    }
}
