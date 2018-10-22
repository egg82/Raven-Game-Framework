using System;
using System.Runtime.InteropServices;

namespace Raven.Utils {
    public class ThreadUtil {
        // externs
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern ExecutionState SetThreadExecutionState(ExecutionState esFlags);

        // enums
        [Flags]
        private enum ExecutionState : uint {
            ES_SYSTEM_REQUIRED = 0x00000001,
            ES_DISPLAY_REQUIRED = 0x00000002,
            //ES_USER_PRESENT = 0x00000004,
            ES_AWAYMODE_REQUIRED = 0x00000040,
            ES_CONTINUOUS = 0x80000000
        }

        // vars

        // constructor
        private ThreadUtil() {

        }

        // public
        public static void PreventSystemSleep() {
            SetThreadExecutionState(ExecutionState.ES_CONTINUOUS | ExecutionState.ES_DISPLAY_REQUIRED | ExecutionState.ES_SYSTEM_REQUIRED);
        }

        // private

    }
}
