using System;

namespace JumpStart
{
    public class IsNot
    {
        public IsNot Null<T>(T obj, string arg)
        {
            if (obj == null) throw new ArgumentException("Value cannot be null", arg);
            return this;
        }

        public IsNot NullOrEmpty(string str, string arg)
        {
            if (String.IsNullOrEmpty(str)) throw new ArgumentException("Value cannot be null or empty", arg);
            return this;
        }
    }
}
