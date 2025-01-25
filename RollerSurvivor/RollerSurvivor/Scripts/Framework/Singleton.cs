using System;
using System.Reflection;

namespace RollerSurvivor.Scripts.Framework
{
    public class Singleton<NullableT>
    {
        protected static NullableT _Instance;

        public static NullableT Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = (NullableT)Activator.CreateInstance(typeof(NullableT))!;
                    if (_Instance == null)
                        throw new InvalidOperationException($"Failed to create an instance of {typeof(NullableT)}.");
                }
                return _Instance;
            }
        }
    }
}