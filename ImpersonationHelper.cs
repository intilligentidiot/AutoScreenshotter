using System;
using System.Runtime.InteropServices;
using System.Security.Principal;
using Microsoft.Win32.SafeHandles;

namespace AutoScreenshotter
{
    public class ImpersonationHelper
    {
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword,
            int dwLogonType, int dwLogonProvider, out IntPtr phToken);

        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr hObject);

        private const int LOGON32_LOGON_NEW_CREDENTIALS = 9;
        private const int LOGON32_PROVIDER_WINNT50 = 3;

        /// <summary>
        /// Logs in the user and runs the provided 'action' under their permissions.
        /// </summary>
        public static void ExecuteActionAsUser(string username, string domain, string password, Action action)
        {
            IntPtr tokenHandle = IntPtr.Zero;

            try
            {
                // 1. Attempt to login
                bool loggedIn = LogonUser(username, domain, password,
                    LOGON32_LOGON_NEW_CREDENTIALS,
                    LOGON32_PROVIDER_WINNT50,
                    out tokenHandle);

                if (!loggedIn)
                {
                    throw new Exception("Login failed for network user.");
                }

                // 2. Use the modern .NET 8 way to run the action
                // We create an Identity from the token, then use RunImpersonated
                using (WindowsIdentity identity = new WindowsIdentity(tokenHandle))
                {
                    WindowsIdentity.RunImpersonated(identity.AccessToken, () =>
                    {
                        // This code block runs as the network user
                        action();
                    });
                }
            }
            finally
            {
                // 3. Always clean up the raw handle
                if (tokenHandle != IntPtr.Zero)
                {
                    CloseHandle(tokenHandle);
                }
            }
        }
    }
}