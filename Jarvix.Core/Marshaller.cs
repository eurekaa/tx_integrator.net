using System;

namespace Ultrapulito.Jarvix.Core {

    static class Marshaller {

        /// <summary>
        /// Permette di effettuare cambi sul tipo di dati estendendo la funzione Convert.ChangeType anche per i tipi Nullable
        /// Ex: Marshaller.ChangeType<int?>(value); dove "value" può anche essere 'null'.
        /// </summary>
        /// <param type="dynamic" name="value">Il valore che si vuole convetire.</param>        
        /// <returns>T</returns>
        public static T ChangeType<T>(dynamic value) {
            Type conversionType = typeof(T);
            if (conversionType.IsGenericType &&
                conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>))) {
                if (value == null) { return default(T); }
                conversionType = Nullable.GetUnderlyingType(conversionType);
            }
            return (T)Convert.ChangeType(value, conversionType);
        }

    }
}
