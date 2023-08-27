using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotApi.Game.Extensions
{
  public static class ListExtenisons
  {
    public static void RemoveRange<T>(this List<T> list, IEnumerable<T> values)
    {
      foreach (var item in values)
      {
        list.Remove(item);
      }
    }
  }
}
