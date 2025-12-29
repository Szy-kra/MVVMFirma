using System;
using System.Collections.Generic;

namespace MVVMFirma.Helper
{
    public static class Messenger
    {
        private static readonly Dictionary<Type, List<Action<object>>> Actions = new Dictionary<Type, List<Action<object>>>();

        public static void Register<T>(Action<T> action)
        {
            if (!Actions.ContainsKey(typeof(T)))
                Actions[typeof(T)] = new List<Action<object>>();

            Actions[typeof(T)].Add(x => action((T)x));
        }

        public static void Send<T>(T message)
        {
            if (Actions.ContainsKey(typeof(T)))
            {
                foreach (var action in Actions[typeof(T)])
                    action(message);
            }
        }
    }
}