using System;
using System.Diagnostics;
using PdfTemplating.WebApp.Common;

namespace PdfTemplating.SystemEventHandlerCustomExtensions
{
    public static class SystemEventHandlerDelegateCustomExtensions
    {

        //Note:  We do not implement overloads that take in only an Object param as teh sender because this lends to easy bugs since
        //       mistaking the EventHandler<T> as the sender object is easy and yields Runtime errors.
        public static bool Raise(this EventHandler _this, object sender)
        {
            return _this.Raise(sender, null);
        }

        //Note:  We do not implement overloads that take in only an Object param as teh sender because this lends to easy bugs since
        //       mistaking the EventHandler<T> as the sender object is easy and yields Runtime errors.
        public static bool Raise(this EventHandler _this, object sender, EventArgs eventArgs)
        {
            if (_this != null)
            {
                _this(sender, eventArgs ?? EventArgs.Empty);
                return true;
            }
            return false;
        }

        //Note:  We do not implement overloads that take in only an Object param as teh sender because this lends to easy bugs since
        //       mistaking the EventHandler<T> as the sender object is easy and yields Runtime errors.
        public static bool Raise(this EventHandler<EventArgs> _this, object sender)
        {
            return _this.Raise(sender, EventArgs.Empty);
        }

        //Note:  We do not implement overloads that take in only an Object param as teh sender because this lends to easy bugs since
        //       mistaking the EventHandler<T> as the sender object is easy and yields Runtime errors.
        public static bool Raise<TEventArgs>(this EventHandler<TEventArgs> _this, object sender) where TEventArgs : EventArgs
        {
            return _this.Raise<TEventArgs>(sender, null);
        }

        //Note:  We do not implement overloads that take in only an Object param as teh sender because this lends to easy bugs since
        //       mistaking the EventHandler<T> as the sender object is easy and yields Runtime errors.
        public static bool Raise<TEventArgs>(this EventHandler<TEventArgs> _this, object sender, TEventArgs eventArgs) where TEventArgs : EventArgs
        {
            try
            {
                if (_this != null)
                {
                    _this(sender, eventArgs);
                    return true;
                }
            }
            catch (Exception exc)
            {
                Debug.Write(exc.GetMessagesRecursively());
            }
            return false;
        }

        //public static EventHandler<TEventArgs> AddHandler<TEventArgs>(this EventHandler<TEventArgs> _this, EventHandler<TEventArgs> fnNewHandler) where TEventArgs : EventArgs
        //{
        //    if (fnNewHandler != null)
        //    {
        //        _this += fnNewHandler;
        //    }
        //    return _this;
        //}

        //public static EventHandler<TEventArgs> RemoveHandler<TEventArgs>(this EventHandler<TEventArgs> _this, EventHandler<TEventArgs> fnNewHandler) where TEventArgs : EventArgs
        //{
        //    if (fnNewHandler != null)
        //    {
        //        _this -= fnNewHandler;
        //    }
        //    return _this;
        //}
    }

}
