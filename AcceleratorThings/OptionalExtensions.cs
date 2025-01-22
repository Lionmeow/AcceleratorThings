using Il2CppMonomiPark.SlimeRancher.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcceleratorThings
{
    public static class OptionalExtensions
    {
        public static Optional<T> WithValue<T>(this Optional<T> @this, T value)
        {
            @this.Value = value;
            return @this;
        }
    }
}
