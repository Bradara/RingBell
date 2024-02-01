using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RingBell
{
    internal class HIDRelay
    {
        [DllImport("hidapi.dll", SetLastError = true)]
        private static extern int hid_init();

        [DllImport("hidapi.dll", SetLastError = true)]
        private static extern int hid_exit();

        [DllImport("hidapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr hid_open(ushort vendor_id, ushort product_id, IntPtr serial_number);

        [DllImport("hidapi.dll", SetLastError = true)]
        private static extern void hid_close(IntPtr device);

        [DllImport("hidapi.dll", SetLastError = true)]
        private static extern int hid_write(IntPtr device, byte[] data, IntPtr length);

        // Replace these with the actual VID and PID of your HID relay device
        private static ushort targetVendorId = 0x5131;
        private static ushort targetProductId = 0x2007;

        public static void OpenRelay()
        {
            try
            {
                // Initialize hidapi
                if (hid_init() != 0)
                {
                    throw new Exception("Failed to initialize hidapi.");
                    return;
                }

                IntPtr device = hid_open(targetVendorId, targetProductId, IntPtr.Zero);

                if (device != IntPtr.Zero)
                {
                    try
                    {
                        // Define the command to turn on the 1-channel relay switch
                        byte[] command = { 0x00, 0xA0, 0x01, 0x01, 0xA2 };

                        // Send the command to the HID device
                        int result = hid_write(device, command, (IntPtr)command.Length);

                        if (result < 0)
                        {
                            throw new Exception("Failed to write to HID device.");
                        }
                    }
                    finally
                    {
                        // Close the HID device
                        hid_close(device);
                    }
                }
                else
                {
                    throw new Exception("HID relay device not found.");
                }

            }
            finally
            {
                // Exit hidapi
                hid_exit();
            }
            
        }

        public static void CloseRelay()
        {
            try
            {
                // Initialize hidapi
                if (hid_init() != 0)
                {
                    throw new Exception("Failed to initialize hidapi.");
                    return;
                }

                IntPtr device = hid_open(targetVendorId, targetProductId, IntPtr.Zero);

                if (device != IntPtr.Zero)
                {
                    try
                    {
                        // Define the command to turn off the 1-channel relay switch
                        byte[] command = { 0x00, 0xA0, 0x01, 0x00, 0xA1 };

                        // Send the command to the HID device
                        int result = hid_write(device, command, (IntPtr)command.Length);

                        if (result < 0)
                        {
                            throw new Exception("Failed to write to HID device.");
                        }
                    }
                    finally
                    {
                        // Close the HID device
                        hid_close(device);
                    }
                }
                else
                {
                    throw new Exception("HID relay device not found.");
                }

            }
            finally
            {
                // Exit hidapi
                hid_exit();
            }

        }

    }
}
