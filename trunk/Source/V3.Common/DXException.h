
#pragma once

#include "Result.h"

#define RECORD_SDX_EX(x, key, value) Result::Record<DXException^>( (x), (key), (value) )
#define RECORD_SDX(x) RECORD_SDX_EX(x, nullptr, nullptr)

namespace V3
{
	/// <summary>
	/// The base class for errors that occur in SlimDX.
	/// </summary>
	/// <unmanaged>None</unmanaged>
	[System::Serializable]
	public ref class DXException : public System::Exception
	{
	private:
		Result m_Result;
	
	protected:
		DXException( System::Runtime::Serialization::SerializationInfo^ info, System::Runtime::Serialization::StreamingContext context );	

	public:
		/// <summary>
		/// Gets the <see cref="Result">Result code</see> for the exception. This value indicates
		/// the specific type of failure that occured within SlimDX.
		/// </summary>
		property Result ResultCode
		{
			Result get();
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="SlimDXException"/> class.
		/// </summary>
		DXException();
		
		/// <summary>
		/// Initializes a new instance of the <see cref="SlimDXException"/> class.
		/// </summary>
		/// <param name="message">The message describing the exception.</param>
		DXException( System::String^ message );
		
		/// <summary>
		/// Initializes a new instance of the <see cref="SlimDXException"/> class.
		/// </summary>
		/// <param name="message">The message describing the exception.</param>
		/// <param name="innerException">The exception that caused this exception.</param>
		DXException( System::String^ message, System::Exception^ innerException );
		
		/// <summary>
		/// Initializes a new instance of the <see cref="SlimDXException"/> class.
		/// </summary>
		/// <param name="result">The result code that caused this exception.</param>
		DXException( Result result );
		
		/// <summary>
		/// When overridden in a derived class, sets the <see cref="System::Runtime::Serialization::SerializationInfo"/> with information about the exception.
		/// </summary>
		/// <param name="info">The <see cref="System::Runtime::Serialization::SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="System::Runtime::Serialization::StreamingContext"/> that contains contextual information about the source or destination.</param>
		[System::Security::Permissions::SecurityPermission(System::Security::Permissions::SecurityAction::LinkDemand, Flags = System::Security::Permissions::SecurityPermissionFlag::SerializationFormatter)]
		virtual void GetObjectData(System::Runtime::Serialization::SerializationInfo^ info, System::Runtime::Serialization::StreamingContext context) override;
	};
}
