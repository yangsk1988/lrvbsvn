#pragma once

using namespace System;
using namespace System::IO;

namespace V3
{
	namespace IO
	{
		public ref class FileBase abstract
		{
		protected:
			bool m_isInArchive;
			long m_fileSize;
			String^ m_fileName;
			String^ m_filePath;

			FileBase(String^ file, long size, bool isInArchive)
				: m_filePath(file), m_fileName(Path::GetFileName(file)), m_fileSize(size), m_isInArchive(isInArchive)
			{
			}

		public:		
			property bool IsInArchive
			{
				bool get() { return m_isInArchive; }
			}
			property long FileSize
			{
				long get() { return m_fileSize; }
			}
			property String^ FileName
			{
				String^ get() { return m_fileName; }
			}
			property String^ FilePath
			{
				String^ get() { return m_filePath; }
			}
		};
	}
}