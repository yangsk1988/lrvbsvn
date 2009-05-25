#pragma once

#include "Archive.h"


using namespace System;
using namespace System::IO;
using namespace System::Collections::Generic;


namespace V3
{
	namespace IO
	{
		public ref class FileSystem sealed
		{
		private:
	        Dictionary<String^, Archive> stdPack;
			Dictionary<String^, ArchiveFactory> factories;

			List<String^> workingDirs;


		public:
			static const String^ dotDll = ".dll";

			static FileSystem^ singleton;
			static Object^ syncHelper = gcnew Object();

			static array<Char>^ DirSepCharArray = gcnew array<Char>(1) { Path::DirectorySeparatorChar };

		    property static FileSystem^ Instance
			{
				FileSystem^ get()
				{
					if (singleton == nullptr)
					{
						System::Threading::Monitor::Enter(syncHelper);
						if (singleton == nullptr)						
						{
							singleton = gcnew FileSystem();
						}
						System::Threading::Monitor::Exit(syncHelper);
					}
					return singleton;
				}
			}


			FileSystem(void);
		};
	}
}
