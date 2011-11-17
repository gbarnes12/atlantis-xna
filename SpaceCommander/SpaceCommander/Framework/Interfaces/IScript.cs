using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameApplicationTools.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IScript
    {
        IEnumerator<float> OnCreateEvent();
        IEnumerator<float> OnLoadEvent();
        IEnumerator<float> OnUpdateEvent();
    }
}
