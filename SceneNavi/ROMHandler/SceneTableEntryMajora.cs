using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace SceneNavi.ROMHandler
{
	class SceneTableEntryMajora : ISceneTableEntry
	{
		[ReadOnly(true)]
		public ushort Number { get; set; }
		public ushort GetNumber() { return Number; }
		public void SetNumber(ushort number) { Number = number; }

		[Browsable(false)]
		readonly string dmaFilename;
		public string GetDMAFilename() { return dmaFilename; }

		[ReadOnly(true)]
		public string Name { get; private set; }
		public string GetName() { return Name; }

		[Browsable(false)]
		public int Offset { get; private set; }
		[Browsable(false)]
		public bool IsOffsetRelative { get; private set; }

		[Browsable(false)]
		readonly uint sceneStartAddress, sceneEndAddress;
		public uint GetSceneStartAddress() { return sceneStartAddress; }
		public uint GetSceneEndAddress() { return sceneEndAddress; }

		[DisplayName("Unknown 1")]
		public byte Unknown1 { get; set; }
		[DisplayName("Unknown 2")]
		public byte Unknown2 { get; set; }
		[DisplayName("Unknown 3")]
		public byte Unknown3 { get; set; }
		[DisplayName("Unknown 4")]
		public byte Unknown4 { get; set; }
		[DisplayName("Padding?")]
		public uint PresumedPadding { get; set; }

		public bool IsValid()
		{
			return (sceneStartAddress < ROM.Size) && (sceneEndAddress < ROM.Size) && ((sceneStartAddress & 0xF) == 0) && ((sceneEndAddress & 0xF) == 0) &&
				(sceneEndAddress > sceneStartAddress) && (PresumedPadding == 0);
		}

		public bool IsAllZero()
		{
			return (sceneStartAddress == 0) && (sceneEndAddress == 0) &&
				(Unknown1 == 0) && (Unknown2 == 0) && (Unknown3 == 0) && (Unknown4 == 0) &&
				(PresumedPadding == 0);
		}


		[Browsable(false)]
		readonly byte[] data;
		public byte[] GetData() { return data; }

		[Browsable(false)]
		List<HeaderLoader> sceneHeaders;
		public List<HeaderLoader> GetSceneHeaders() { return sceneHeaders; }

		[Browsable(false)]
		readonly bool inROM;
		public bool IsInROM() { return inROM; }

		[Browsable(false)]
		readonly bool isNameExternal;
		public bool IsNameExternal() { return isNameExternal; }

		[Browsable(false)]
		HeaderLoader currentSceneHeader;
		public HeaderLoader GetCurrentSceneHeader() { return currentSceneHeader; }
		public void SetCurrentSceneHeader(HeaderLoader header) { currentSceneHeader = header; }

		public HeaderCommands.Actors GetActiveTransitionData()
		{
			return currentSceneHeader == null ? null : currentSceneHeader.Commands.FirstOrDefault(x => x.Command == HeaderLoader.CommandTypeIDs.Transitions) as HeaderCommands.Actors;
		}

		public HeaderCommands.Actors GetActiveSpawnPointData()
		{
			return currentSceneHeader == null ? null : currentSceneHeader.Commands.FirstOrDefault(x => x.Command == HeaderLoader.CommandTypeIDs.Spawns) as HeaderCommands.Actors;
		}

		public HeaderCommands.SpecialObjects GetActiveSpecialObjs()
		{
			return currentSceneHeader == null ? null : currentSceneHeader.Commands.FirstOrDefault(x => x.Command == HeaderLoader.CommandTypeIDs.SpecialObjects) as HeaderCommands.SpecialObjects;
		}

		public HeaderCommands.Waypoints GetActiveWaypoints()
		{
			return currentSceneHeader == null ? null : currentSceneHeader.Commands.FirstOrDefault(x => x.Command == HeaderLoader.CommandTypeIDs.Waypoints) as HeaderCommands.Waypoints;
		}

		public HeaderCommands.Collision GetActiveCollision()
		{
			return currentSceneHeader == null ? null : currentSceneHeader.Commands.FirstOrDefault(x => x.Command == HeaderLoader.CommandTypeIDs.Collision) as HeaderCommands.Collision;
		}

		public HeaderCommands.SettingsSoundScene GetActiveSettingsSoundScene()
		{
			return currentSceneHeader == null ? null : currentSceneHeader.Commands.FirstOrDefault(x => x.Command == HeaderLoader.CommandTypeIDs.SettingsSoundScene) as HeaderCommands.SettingsSoundScene;
		}

		public HeaderCommands.EnvironmentSettings GetActiveEnvSettings()
		{
			return currentSceneHeader == null ? null : currentSceneHeader.Commands.FirstOrDefault(x => x.Command == HeaderLoader.CommandTypeIDs.EnvironmentSettings) as HeaderCommands.EnvironmentSettings;
		}

		ROMHandler ROM;

		public SceneTableEntryMajora(ROMHandler rom, string fn)
		{
			ROM = rom;
			inROM = false;

			Offset = -1;
			IsOffsetRelative = false;

			sceneStartAddress = sceneEndAddress = 0;

			Unknown1 = Unknown2 = Unknown3 = Unknown4 = 0;

			System.IO.FileStream fs = new System.IO.FileStream(fn, System.IO.FileMode.Open);
			data = new byte[fs.Length];
			fs.Read(data, 0, (int)fs.Length);
			fs.Close();

			Name = System.IO.Path.GetFileNameWithoutExtension(fn);
		}

		public SceneTableEntryMajora(ROMHandler rom, int ofs, bool isrel)
		{
			ROM = rom;
			inROM = true;

			Offset = ofs;
			IsOffsetRelative = isrel;

			sceneStartAddress = Endian.SwapUInt32(BitConverter.ToUInt32(IsOffsetRelative ? rom.CodeData : rom.Data, ofs));
			sceneEndAddress = Endian.SwapUInt32(BitConverter.ToUInt32(IsOffsetRelative ? rom.CodeData : rom.Data, ofs + 4));

			Unknown1 = (IsOffsetRelative ? rom.CodeData : rom.Data)[ofs + 8];
			Unknown2 = (IsOffsetRelative ? rom.CodeData : rom.Data)[ofs + 9];
			Unknown3 = (IsOffsetRelative ? rom.CodeData : rom.Data)[ofs + 10];
			Unknown4 = (IsOffsetRelative ? rom.CodeData : rom.Data)[ofs + 11];
			PresumedPadding = Endian.SwapUInt32(BitConverter.ToUInt32(IsOffsetRelative ? rom.CodeData : rom.Data, ofs + 12));

			if (IsValid() && !IsAllZero())
			{
				DMATableEntry dma = rom.Files.Find(x => x.PStart == sceneStartAddress);
				if (dma != null) dmaFilename = dma.Name;

				if ((Name = (ROM.XMLSceneNames.Names[sceneStartAddress] as string)) == null)
				{
					isNameExternal = false;

					if (dma != null)
						Name = dmaFilename;
					else
						Name = string.Format("S{0:X}_E{1:X}", sceneStartAddress, sceneEndAddress);
				}
				else
					isNameExternal = true;

				data = new byte[sceneEndAddress - sceneStartAddress];
				Array.Copy(ROM.Data, sceneStartAddress, data, 0, sceneEndAddress - sceneStartAddress);
			}
		}

		public void SaveTableEntry()
		{
			if (!inROM) throw new Exception("Trying to save scene table entry for external scene file");

			byte[] tmpbuf = null;

			tmpbuf = BitConverter.GetBytes(Endian.SwapUInt32(sceneStartAddress));
			Buffer.BlockCopy(tmpbuf, 0, (IsOffsetRelative ? ROM.CodeData : ROM.Data), Offset, tmpbuf.Length);

			tmpbuf = BitConverter.GetBytes(Endian.SwapUInt32(sceneEndAddress));
			Buffer.BlockCopy(tmpbuf, 0, (IsOffsetRelative ? ROM.CodeData : ROM.Data), Offset + 4, tmpbuf.Length);

			(IsOffsetRelative ? ROM.CodeData : ROM.Data)[Offset + 8] = Unknown1;
			(IsOffsetRelative ? ROM.CodeData : ROM.Data)[Offset + 9] = Unknown2;
			(IsOffsetRelative ? ROM.CodeData : ROM.Data)[Offset + 10] = Unknown3;
			(IsOffsetRelative ? ROM.CodeData : ROM.Data)[Offset + 11] = Unknown4;

			tmpbuf = BitConverter.GetBytes(Endian.SwapUInt32(PresumedPadding));
			Buffer.BlockCopy(tmpbuf, 0, (IsOffsetRelative ? ROM.CodeData : ROM.Data), Offset + 12, tmpbuf.Length);
		}

		public void ReadScene(HeaderCommands.Rooms forcerooms = null)
		{
			Program.Status.Message = string.Format("Reading scene '{0}'...", Name);

			ROM.SegmentMapping.Remove((byte)0x02);
			ROM.SegmentMapping.Add((byte)0x02, data);

			sceneHeaders = new List<HeaderLoader>();

			HeaderLoader newheader = null;
			HeaderCommands.Rooms rooms = null;
			HeaderCommands.Collision coll = null;

			if (data[0] == (byte)HeaderLoader.CommandTypeIDs.SettingsSoundScene || data[0] == (byte)HeaderLoader.CommandTypeIDs.Rooms ||
				BitConverter.ToUInt32(data, 0) == (byte)HeaderLoader.CommandTypeIDs.SubHeaders)
			{
				/* Get rooms & collision command from first header */
				newheader = new HeaderLoader(ROM, this, 0x02, 0, 0);

				/* If external rooms should be forced, overwrite command in header */
				if (forcerooms != null)
				{
					int roomidx = newheader.Commands.FindIndex(x => x.Command == HeaderLoader.CommandTypeIDs.Rooms);
					if (roomidx != -1) newheader.Commands[roomidx] = forcerooms;
				}

				rooms = newheader.Commands.FirstOrDefault(x => x.Command == HeaderLoader.CommandTypeIDs.Rooms) as HeaderCommands.Rooms;
				coll = newheader.Commands.FirstOrDefault(x => x.Command == HeaderLoader.CommandTypeIDs.Collision) as HeaderCommands.Collision;
				sceneHeaders.Add(newheader);

				if (BitConverter.ToUInt32((byte[])ROM.SegmentMapping[(byte)0x02], 0) == 0x18)
				{
					int hnum = 1;
					uint aofs = Endian.SwapUInt32(BitConverter.ToUInt32((byte[])ROM.SegmentMapping[(byte)0x02], 4));
					while (true)
					{
						uint rofs = Endian.SwapUInt32(BitConverter.ToUInt32((byte[])ROM.SegmentMapping[(byte)0x02], (int)(aofs & 0x00FFFFFF)));
						if (rofs != 0)
						{
							if ((rofs & 0x00FFFFFF) > ((byte[])ROM.SegmentMapping[(byte)0x02]).Length || (rofs >> 24) != 0x02) break;
							newheader = new HeaderLoader(ROM, this, 0x02, (int)(rofs & 0x00FFFFFF), hnum++);

							/* Get room command index... */
							int roomidx = newheader.Commands.FindIndex(x => x.Command == HeaderLoader.CommandTypeIDs.Rooms);

							/* If external rooms should be forced, overwrite command in header */
							if (roomidx != -1 && forcerooms != null) newheader.Commands[roomidx] = forcerooms;

							/* If rooms were found in first header, force using these! */
							if (roomidx != -1 && rooms != null) newheader.Commands[roomidx] = rooms;

							/* If collision was found in header, force */
							int collidx = newheader.Commands.FindIndex(x => x.Command == HeaderLoader.CommandTypeIDs.Collision);
							if (collidx != -1 && coll != null) newheader.Commands[collidx] = coll;

							sceneHeaders.Add(newheader);
						}
						aofs += 4;
					}
				}

				currentSceneHeader = sceneHeaders[0];
			}
		}
	}
}
