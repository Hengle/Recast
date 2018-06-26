namespace FrameWork.Utility
{
    /// <summary>
    /// Hierarch this class to make sure the class only have a instance
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Singleton<T> where T : Singleton<T>
    {
        private static T s_Instance;

        private static object helper_lock = new object();

        public static T GetInstance()
        {
            if(null == s_Instance)
            {
                lock (helper_lock)
                {
                    if(null == s_Instance)
                    {
                        s_Instance = System.Activator.CreateInstance<T>();
                    }
                }
            }

            return s_Instance;
        }
    }
}
