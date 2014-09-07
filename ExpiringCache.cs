using System;
using System.Threading;

namespace ItzWarty
{
   /// <summary>
   /// Utility class for the caching of LARGE OBJECTS.  Every instance of this class takes a thread
   /// from the thread pool for timing expiration.  This class CAN be used to cache small objects,
   /// though doing so would be inefficient and a better solution (ie: a multicache with cached
   /// values sharing timeout intervals with a common denominator used as a timer's timeout).
   /// 
   /// The SingleCache class holds a reference to objects for a set period of time.  When the cache
   /// expires, the object is released.  Cache expiration is dependent on garbage collection; 
   /// 
   /// Optionally, the cache can refresh the timer for the held object when the cache is accessed.
   /// 
   /// The null value cannot be cached by this cache object.
   /// </summary>
   /// <typeparam name="T"></typeparam>
   public class ExpiringCache<T> where T : class
   {
      private readonly bool m_resetTimerOnAccess;
      private readonly int m_expirationMs;
      private readonly string m_name;
      private readonly bool m_garbageCollectOnExpiration;
      private readonly Timer m_timer;

      public volatile T m_cachedValue;
      private volatile WeakReference<T> m_expiringValue;

      // As the cached value might be saved when we access the cache, access must be serial
      public readonly object m_accessLock = new object();

      /// <summary>
      /// Initialize a new instance of an expiring cache
      /// </summary>
      /// <param name="resetTimerOnAccess">
      /// Whether or not the expiration timer is reset whenever the cache is accessed.
      /// </param>
      /// <param name="expirationMs">
      /// The approximate number of milliseconds after the object is accessed before the cache
      /// expires and the held reference becomes weak.
      /// </param>
      /// <param name="name">
      /// Name of our cached value.  If set, a console message will be written when the cache 
      /// expires.
      /// </param>
      /// <param name="garbageCollectOnExpiration">
      /// If true, a full garbage collection will be performed when the cached object expires.
      /// If any references to the cached object are held by other objects, then memory will not
      /// be freed.
      /// </param>
      public ExpiringCache(
         bool resetTimerOnAccess, 
         int expirationMs, 
         string name = null, 
         bool garbageCollectOnExpiration = false)
      {
         m_resetTimerOnAccess = resetTimerOnAccess;
         m_expirationMs = expirationMs;
         m_name = name;
         m_garbageCollectOnExpiration = garbageCollectOnExpiration;
         m_timer = new Timer(HandleTimerTick, null, Timeout.Infinite, Timeout.Infinite);
      }

      /// <summary>
      /// When the cache expiration event occurs, our cached value becomes subject to garbage 
      /// collection.  It is important to note that because of our use of a weak reference, the
      /// object may still be saved by a TryGetValue invocation before the Garbage Collector runs.
      /// </summary>
      /// <param name="state"></param>
      private void HandleTimerTick(object state)
      {
         lock (m_accessLock)
         {
            m_expiringValue = new WeakReference<T>(m_cachedValue);
            m_cachedValue = null;

            if (m_garbageCollectOnExpiration)
            {
               Console.WriteLine("Cache \"" + m_name + "\" has expired");
               GC.Collect(GC.MaxGeneration);
            }
         }
      }

      public T GetOrPutValue(Func<T> getter)
      {
         lock (m_accessLock)
         {
            // Get the cached value
            var value = m_cachedValue;
            if (value == null && m_expiringValue != null)
               m_expiringValue.TryGetTarget(out value);

            if (value == null) // Expired/Wasn't set
            {
               m_cachedValue = getter();
               m_expiringValue = null;
               m_timer.Change(m_expirationMs, Timeout.Infinite);
            }
            else // Cache still valid
            {
               // Restore the cached value, which could be pointing to null
               m_cachedValue = value;

               // Conditionally reset the expiration timer
               if (m_resetTimerOnAccess)
                  m_timer.Change(m_expirationMs, Timeout.Infinite);
            }
            return m_cachedValue;
         }
      }

      /// <summary>
      /// Try to get the cached value. If the value has been dereferenced but has not yet been 
      /// garbage collected, then the value will be saved.
      /// 
      /// This method will reset the cache expiration timer if such was specified in the 
      /// constructor.
      /// </summary>
      /// <param name="value"></param>
      /// <returns></returns>
      public bool TryGetValue(out T value)
      {
         lock (m_accessLock)
         {
            // Get the cached value
            value = m_cachedValue;
            if (value == null && m_expiringValue != null)
               m_expiringValue.TryGetTarget(out value);

            if (value == null) // Expired/Wasn't set
               return false;
            else // Cache still valid
            {
               // Restore the cached value, which could be pointing to null
               m_cachedValue = value;

               // Conditionally reset the expiration timer
               if (m_resetTimerOnAccess)
                  m_timer.Change(m_expirationMs, Timeout.Infinite);
               return true;
            }
         }
      }

      /// <summary>
      /// Places a value into the cache
      /// </summary>
      /// <param name="value"></param>
      public void PutValue(T value)
      {
         lock (m_accessLock)
         {
            m_timer.Change(m_expirationMs, Timeout.Infinite);

            m_cachedValue = value;
            m_expiringValue = null;
         }
      }

      /// <summary>
      /// Resets the cache expiration
      /// </summary>
      /// <param name="timeout">
      /// If set to -2 (default), the default cache timeout (as specified in the constructor) is 
      /// used.  If set to any other value, that value is used as the cache expiration timer.  
      /// It is important to note that the timer may be automatically reset when the cached value
      /// is accessed or replaced, and that when that occurs, the default timeout will be used.
      /// 
      /// We use -1337 because -1 is used by Timeout.Infinite
      /// </param>
      public void ResetCacheExpiration(int timeout = -1337)
      {
         lock (m_accessLock)
         {
            if (timeout == -1337)
               timeout = m_expirationMs;
            m_timer.Change(timeout, Timeout.Infinite);
         }
      }

      /// <summary>
      /// Determines whether or not the cache has expired.  Note that the value of this may change
      /// after the method returns.
      /// </summary>
      /// <returns></returns>
      public bool IsExpired()
      {
         if (m_cachedValue != null) 
            return false;
         var capture = m_expiringValue;
         if (capture == null) return true;
         T value;
         return !capture.TryGetTarget(out value);
      }
   }
}
