// http://stackoverflow.com/questions/3282365/opening-a-named-pipe-in-low-integrity-level
// I have not bothered learning what black magic is actually happening here.

using System;
using System.ComponentModel;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace ItzWarty.Networking
{
   public static class LowIntegrityPipeFactory
   {
      [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
      private static extern SafePipeHandle CreateNamedPipe(
         [In] string pipeName,
         [In] uint openMode,
         [In] uint pipeMode,
         [In] uint maxInstances,
         [In] uint outBufferSize,
         [In] uint inBufferSize,
         [In] uint defaultTimeout,
         ref SECURITY_ATTRIBUTES securityAttributes);
         //[In] [Optional] SECURITY_ATTRIBUTES securityAttributes);


      [DllImport("Advapi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = false)]
      private static extern bool ConvertStringSecurityDescriptorToSecurityDescriptor(
          [In] string StringSecurityDescriptor,
          [In] uint StringSDRevision,
          [Out] out IntPtr SecurityDescriptor,
          [Out] out int SecurityDescriptorSize
      );

      [StructLayout(LayoutKind.Sequential)]
      public struct SECURITY_ATTRIBUTES
      {
         public uint nLength;
         public IntPtr lpSecurityDescriptor;
         public uint bInheritHandle;
      }

      private const string LOW_INTEGRITY_SSL_SACL = "S:(ML;;NW;;;LW)";

      public static SafePipeHandle CreateLowIntegrityNamedPipe(string pipeName)
      {
         // convert the security descriptor
         IntPtr securityDescriptorPtr = IntPtr.Zero;
         int securityDescriptorSize = 0;
         //bool result = ConvertStringSecurityDescriptorToSecurityDescriptor(
         //    LOW_INTEGRITY_SSL_SACL, 1, out securityDescriptorPtr, out securityDescriptorSize);
         bool result = ConvertStringSecurityDescriptorToSecurityDescriptor(
            CreateSddlForPipeSecurity(), 1, out securityDescriptorPtr,
            out securityDescriptorSize);
         if (!result)
            throw new Win32Exception(Marshal.GetLastWin32Error());

         SECURITY_ATTRIBUTES securityAttributes = new SECURITY_ATTRIBUTES();
         securityAttributes.nLength = (uint)Marshal.SizeOf(securityAttributes);
         securityAttributes.bInheritHandle = (uint)1;
         securityAttributes.lpSecurityDescriptor = securityDescriptorPtr;

         SafePipeHandle handle = CreateNamedPipe(@"\\.\pipe\" + pipeName,
             PipeDirection.InOut, 100, PipeTransmissionMode.Byte, PipeOptions.Asynchronous,
             0, 0, PipeAccessRights.ReadWrite, securityAttributes);
         if (handle.IsInvalid)
            throw new Win32Exception(Marshal.GetLastWin32Error());

         return handle;
      }

      private static SafePipeHandle CreateNamedPipe(string fullPipeName, PipeDirection direction,
          uint maxNumberOfServerInstances, PipeTransmissionMode transmissionMode, PipeOptions options,
          uint inBufferSize, uint outBufferSize, PipeAccessRights rights, SECURITY_ATTRIBUTES secAttrs)
      {
         uint openMode = (uint)direction | (uint)options;
         uint pipeMode = 0;
         if (maxNumberOfServerInstances == -1)
            maxNumberOfServerInstances = 0xff;

         SafePipeHandle handle = CreateNamedPipe(fullPipeName, openMode, pipeMode,
             maxNumberOfServerInstances, outBufferSize, inBufferSize, 0, ref secAttrs);
         if (handle.IsInvalid)
            throw new Win32Exception(Marshal.GetLastWin32Error());
         return handle;
      }

      private static string CreateSddlForPipeSecurity()
      {
         const string LOW_INTEGRITY_LABEL_SACL = "S:(ML;;NW;;;LW)";
         const string EVERYONE_CLIENT_ACE = "(A;;0x12019b;;;WD)";
         const string CALLER_ACE_TEMPLATE = "(A;;0x12019f;;;{0})";

         StringBuilder sb = new StringBuilder();
         sb.Append(LOW_INTEGRITY_LABEL_SACL);
         sb.Append("D:");
         sb.Append(EVERYONE_CLIENT_ACE);
         sb.AppendFormat(CALLER_ACE_TEMPLATE, WindowsIdentity.GetCurrent().Owner.Value);
         return sb.ToString();
      }
   }
}