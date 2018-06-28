using System;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace GEOBOX.OSC.IM.DataCheckerTool
{
    public static class PropertyChangedEventHandlerExtensions
    {
        public static void ChangeValue<T>(this PropertyChangedEventHandler propertyChangedEventHandler,
                                            Func<T> getter,
                                            Action<T> setter,
                                            T newValue,
                                            object sender,
                                            [CallerMemberName] string propertyName = null)
        {
            if (getter == null) throw new ArgumentNullException(nameof(getter));
            if (setter == null) throw new ArgumentNullException(nameof(setter));
            Contract.EndContractBlock();

            if (Equals(getter, newValue))
            {
                return;
            }
            setter(newValue);
            propertyChangedEventHandler?.Invoke(sender, new PropertyChangedEventArgs(propertyName));
        }
    }
}
