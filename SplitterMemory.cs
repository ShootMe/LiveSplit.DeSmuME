using LiveSplit.Memory;
using System;
using System.Diagnostics;
namespace LiveSplit.DeSmuME {
	public partial class SplitterMemory {
		private static ProgramPointer RAM = new ProgramPointer(AutoDeref.None, 0);
		public Process Program { get; set; }
		public bool IsHooked { get; set; } = false;
		private DateTime lastHooked;

		public SplitterMemory() {
			lastHooked = DateTime.MinValue;
		}
		public string Pointer() {
			return RAM.GetPointer(Program).ToString("X");
		}
		public T Read<T>(uint address) where T : struct {
			return RAM.Read<T>(Program, (int)address);
		}
		public bool HookProcess() {
			IsHooked = Program != null && !Program.HasExited;
			if (!IsHooked && DateTime.Now > lastHooked.AddSeconds(1)) {
				lastHooked = DateTime.Now;
				Process[] processes = Process.GetProcesses();
				Program = null;
				for (int i = 0; i < processes.Length; i++) {
					Process process = processes[i];
					if (process.ProcessName.Equals("DeSmuME_0.9.11_x86", StringComparison.OrdinalIgnoreCase)) {
						Program = process;
						break;
					} else if (process.ProcessName.Equals("DeSmuME_0.9.11_x64", StringComparison.OrdinalIgnoreCase)) {
						Program = process;
						break;
					} else if (process.ProcessName.Equals("DeSmuME_0.9.9_x86", StringComparison.OrdinalIgnoreCase)) {
						Program = process;
						break;
					} else if (process.ProcessName.Equals("DeSmuME_0.9.9_x64", StringComparison.OrdinalIgnoreCase)) {
						Program = process;
						break;
					}
				}

				if (Program != null && !Program.HasExited) {
					MemoryReader.Update64Bit(Program);
					IsHooked = true;
				}
			}

			return IsHooked;
		}
		public void Dispose() {
			if (Program != null) {
				Program.Dispose();
			}
		}
	}
}