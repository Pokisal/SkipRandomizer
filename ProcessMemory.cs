using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

public class ProcessMemory
{
	private int idc;

	public Process Owner { get; private set; }

	[DllImport("Kernel32.dll")]
	private static extern bool ReadProcessMemory(IntPtr processHandle, IntPtr address, IntPtr bufferAddress, int bufferLength, out int BytesRead);

	[DllImport("Kernel32.dll")]
	private static extern bool WriteProcessMemory(IntPtr processHandle, IntPtr address, IntPtr bufferAddress, int bufferLength, out int BytesWritten);

	[DllImport("Kernel32.dll")]
	private static extern IntPtr VirtualAllocEx(IntPtr processHandle, IntPtr address, int allocationSize, int allocationType, int protectionType);

	[DllImport("Kernel32.dll")]
	private static extern bool VirtualFreeEx(IntPtr processHandle, IntPtr address, int freeSize, int freeType);

	[DllImport("Kernel32.dll")]
	private static extern bool VirtualProtectEx(IntPtr processHandle, IntPtr address, int protectSize, int newProtectionType, out int oldProtectionType);

	public ProcessMemory(Process owner)
	{
		Process.EnterDebugMode();
		Owner = owner;
	}

	public static IEnumerable<ProcessMemory> GetProcesses()
	{
		Process.EnterDebugMode();
		Process[] processes = Process.GetProcesses();
		foreach (Process owner in processes)
		{
			yield return new ProcessMemory(owner);
		}
	}

	public static IEnumerable<ProcessMemory> GetProcessesByName(string processName)
	{
		Process.EnterDebugMode();
		Process[] processesByName = Process.GetProcessesByName(processName);
		foreach (Process owner in processesByName)
		{
			yield return new ProcessMemory(owner);
		}
	}

	public static IEnumerable<ProcessMemory> GetProcessesByWindowName(string windowName)
	{
		Process.EnterDebugMode();
		Process[] processes = Process.GetProcesses();
		foreach (Process process in processes)
		{
			if (process.MainWindowTitle.ToLower() == windowName.ToLower())
			{
				yield return new ProcessMemory(process);
			}
		}
	}

	public static ProcessMemory GetCurrentProcess()
	{
		Process.EnterDebugMode();
		return new ProcessMemory(Process.GetCurrentProcess());
	}

	public static ProcessMemory GetProcessById(int processId)
	{
		Process.EnterDebugMode();
		return new ProcessMemory(Process.GetProcessById(processId));
	}

	public T[] ReadArray<T>(IntPtr address, int length)
	{
		int num = Marshal.SizeOf(typeof(T));
		T[] array = new T[length];
		GCHandle gCHandle = GCHandle.Alloc(array, GCHandleType.Pinned);
		if (!VirtualProtectEx(Owner.Handle, address, length, 64, out var oldProtectionType))
		{
			ThrowLastWin32Error();
		}
		if (!ReadProcessMemory(Owner.Handle, address, gCHandle.AddrOfPinnedObject(), length * num, out idc))
		{
			ThrowLastWin32Error();
		}
		if (!VirtualProtectEx(Owner.Handle, address, length, oldProtectionType, out oldProtectionType))
		{
			ThrowLastWin32Error();
		}
		gCHandle.Free();
		return array;
	}

	public void WriteArray<T>(IntPtr address, params T[] values)
	{
		int num = Marshal.SizeOf(typeof(T));
		GCHandle gCHandle = GCHandle.Alloc(values, GCHandleType.Pinned);
		if (!VirtualProtectEx(Owner.Handle, address, values.Length * num, 64, out var oldProtectionType))
		{
			ThrowLastWin32Error();
		}
		if (!WriteProcessMemory(Owner.Handle, address, gCHandle.AddrOfPinnedObject(), values.Length * num, out idc))
		{
			ThrowLastWin32Error();
		}
		if (!VirtualProtectEx(Owner.Handle, address, values.Length * num, oldProtectionType, out oldProtectionType))
		{
			ThrowLastWin32Error();
		}
		gCHandle.Free();
	}

	public T ReadValue<T>(IntPtr address)
	{
		return ReadArray<T>(address, 1)[0];
	}

	public void WriteValue<T>(IntPtr address, T value)
	{
		WriteArray<T>(address, value);
	}

	public unsafe string ReadStringA(IntPtr address, int length = 32)
	{
		fixed (byte* ptr = ReadArray<byte>(address, length))
		{
			return Marshal.PtrToStringAnsi((IntPtr)ptr);
		}
	}

	public unsafe string ReadStringW(IntPtr address, int length = 32)
	{
		fixed (byte* ptr = ReadArray<byte>(address, length * 2))
		{
			return Marshal.PtrToStringUni((IntPtr)ptr);
		}
	}

	public void WriteStringA(IntPtr address, string value)
	{
		WriteArray(address, Encoding.ASCII.GetBytes(value));
	}

	public void WriteStringW(IntPtr address, string value)
	{
		WriteArray(address, Encoding.Unicode.GetBytes(value));
	}

	public IntPtr Alloc(int allocationSize)
	{
		IntPtr intPtr = VirtualAllocEx(Owner.Handle, IntPtr.Zero, allocationSize, 12288, 64);
		if (intPtr == IntPtr.Zero)
		{
			ThrowLastWin32Error();
		}
		WriteArray(intPtr, new byte[allocationSize]);
		return intPtr;
	}

	public IntPtr AllocArray<T>(params T[] values)
	{
		int num = Marshal.SizeOf(typeof(T));
		IntPtr intPtr = Alloc(values.Length * num);
		WriteArray(intPtr, values);
		return intPtr;
	}

	public IntPtr AllocValue<T>(T value)
	{
		return AllocArray<T>(value);
	}

	public IntPtr AllocStringA(string value)
	{
		return AllocArray(Encoding.ASCII.GetBytes(value + "\0"));
	}

	public IntPtr AllocStringW(string value)
	{
		return AllocArray(Encoding.Unicode.GetBytes(value + "\0"));
	}

	public void Free(IntPtr address)
	{
		if (!VirtualFreeEx(Owner.Handle, address, 0, 32768))
		{
			ThrowLastWin32Error();
		}
	}

	private static void ThrowLastWin32Error()
	{
		throw new Exception($"Win32 Error Code: {Marshal.GetLastWin32Error():X8}");
	}
}
